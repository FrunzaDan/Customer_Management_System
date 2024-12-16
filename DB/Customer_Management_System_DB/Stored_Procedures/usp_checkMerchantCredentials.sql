CREATE PROCEDURE [dbo].[usp_checkMerchantCredentials]
    @var_MerchantID NVARCHAR(50),
    @var_MerchantPassword VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @currDate DATETIME = GETDATE();

    UPDATE tbl_merchants
    SET last_interaction = @currDate
    WHERE merchant_id = @var_MerchantID;

    DECLARE @hashedMerchantPassword BINARY(32) = HASHBYTES('SHA2_256', @var_MerchantPassword);

    IF EXISTS (
        SELECT 1
        FROM tbl_merchants
        WHERE merchant_id = @var_MerchantID AND merchant_password = @hashedMerchantPassword
    )
    BEGIN
        -- Return the merchant role if credentials are valid
        SELECT merchant_role
        FROM tbl_merchants
        WHERE merchant_id = @var_MerchantID;
    END
    ELSE
    BEGIN
        -- If credentials are invalid, return NULL
        SELECT NULL AS merchant_role;
    END
END
