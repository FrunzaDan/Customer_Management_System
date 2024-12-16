CREATE PROCEDURE [dbo].[usp_deleteCustomer]
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
        -- Delete related address records
        DELETE FROM tbl_addresses
        WHERE FK_customer_guid = @var_Guid;

        -- Delete the customer record
        DELETE FROM tbl_customers
        WHERE PK_customer_guid = @var_Guid;

        -- Verify deletion by checking if the customer record still exists
        SELECT PK_customer_guid
        FROM tbl_customers
        WHERE PK_customer_guid = @var_Guid;
    END
END
