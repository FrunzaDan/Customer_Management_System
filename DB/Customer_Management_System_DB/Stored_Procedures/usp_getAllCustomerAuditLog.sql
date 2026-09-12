CREATE PROCEDURE [dbo].[usp_getAllCustomerAuditLog]
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    -- LEFT JOIN, not INNER: tbl_customer_audit_log has no FK to tbl_customers
    -- (a deleted customer's history must survive the delete — see
    -- tbl_customer_audit_log.sql), so first_name/last_name come back NULL for
    -- a customer that no longer exists rather than dropping that row.
    SELECT
        l.audit_id,
        l.customer_guid,
        c.first_name,
        c.last_name,
        l.merchant_id,
        l.action,
        l.details,
        l.action_Date,
        COUNT(*) OVER() AS total_count
    FROM
        dbo.tbl_customer_audit_log AS l
    LEFT JOIN
        dbo.tbl_customers AS c
        ON c.PK_customer_guid = l.customer_guid
    ORDER BY
        l.action_Date DESC, l.audit_id DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
