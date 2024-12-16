CREATE PROCEDURE [dbo].[usp_deactivateCustomer]
    @var_Guid NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if the customer exists
    IF EXISTS (
        SELECT 1
        FROM tbl_customers
        WHERE PK_customer_guid = @var_Guid
    )
    BEGIN
        DECLARE @currDate DATETIME = GETDATE();

        -- Update the customer's status to deactivated
        UPDATE tbl_customers
        SET 
            interaction_Date = @currDate,
            customer_Status = 1903
        WHERE PK_customer_guid = @var_Guid;

        -- Return the updated customer status
        SELECT customer_Status
        FROM tbl_customers
        WHERE PK_customer_guid = @var_Guid;
    END
END
