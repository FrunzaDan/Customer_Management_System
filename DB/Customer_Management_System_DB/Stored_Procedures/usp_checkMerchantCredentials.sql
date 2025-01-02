CREATE PROCEDURE [dbo].[usp_checkMerchantCredentials]
    @var_MerchantID NVARCHAR(50),
    @var_MerchantPassword VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @currDate DATETIME = GETDATE();
    DECLARE @result INT;
    DECLARE @message NVARCHAR(255);

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
        SELECT 
            0 AS result, -- Success
            'Credentials validated successfully.' AS message,
            merchant_role
        FROM tbl_merchants
        WHERE merchant_id = @var_MerchantID;
    END
    ELSE
    BEGIN
        SET @result = 4001; -- Invalid credentials
        SET @message = 'Invalid Merchant ID or Password.';
        SELECT @result AS result, @message AS message, NULL AS merchant_role;
    END
END;
