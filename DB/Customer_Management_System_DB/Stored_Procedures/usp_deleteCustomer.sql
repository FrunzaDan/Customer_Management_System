CREATE PROCEDURE [dbo].[usp_deleteCustomer]
    @var_Guid NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @result INT;
    DECLARE @message NVARCHAR(255);

    IF NOT EXISTS (
        SELECT 1
        FROM tbl_customers
        WHERE PK_customer_guid = @var_Guid
    )
    BEGIN
        SET @result = 404;
        SET @message = 'Customer not found.';
    END
    ELSE IF NOT EXISTS (
        SELECT 1
        FROM tbl_customers
        WHERE PK_customer_guid = @var_Guid AND customer_Status = 1903
    )
    BEGIN
        SET @result = 409;
        SET @message = 'Customer must be deactivated before it can be deleted.';
    END
    ELSE
    BEGIN
        DELETE FROM tbl_addresses
        WHERE FK_customer_guid = @var_Guid;

        DELETE FROM tbl_customers
        WHERE PK_customer_guid = @var_Guid;

        IF NOT EXISTS (
            SELECT 1
            FROM tbl_customers
            WHERE PK_customer_guid = @var_Guid
        )
        BEGIN
            SET @result = 0;
            SET @message = 'Customer deleted successfully.';
        END
        ELSE
        BEGIN
            SET @result = 409;
            SET @message = 'Failed to delete customer. Deletion may not have been successful.';
        END
    END

    SELECT @result AS result, @message AS message;
END
