CREATE PROCEDURE [dbo].[usp_checkMerchantCredentials]
    @var_MerchantID NVARCHAR(50),
    @var_MerchantPassword VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @currDate DATETIME = GETDATE();

    -- Update last interaction timestamp
    UPDATE tbl_merchants
    SET last_interaction = @currDate
    WHERE merchant_id = @var_MerchantID;

    -- Hash the provided password
    DECLARE @hashedMerchantPassword BINARY(32) = HASHBYTES('SHA2_256', @var_MerchantPassword);

    -- Check credentials and return merchant role if valid
    IF EXISTS (
        SELECT 1
        FROM tbl_merchants
        WHERE merchant_id = @var_MerchantID AND merchant_password = @hashedMerchantPassword
    )
    BEGIN
        SELECT merchant_role
        FROM tbl_merchants
        WHERE merchant_id = @var_MerchantID;
    END
END
