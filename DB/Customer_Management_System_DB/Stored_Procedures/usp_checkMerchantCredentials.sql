CREATE PROCEDURE [dbo].[usp_checkMerchantCredentials]
  @var_MerchantID NVARCHAR(50),
  @var_MerchantPassword VARCHAR(100)

AS
BEGIN
  SET NOCOUNT ON

  DECLARE @currDate DATETIME;
  SET @currDate = GETDATE();

  UPDATE tbl_merchants SET last_interaction = @currDate WHERE merchant_id = @var_MerchantID;

  DECLARE @hashedMerchantPassword BINARY(32);
  SET @hashedMerchantPassword = HASHBYTES('SHA2_256', @var_MerchantPassword)

  IF EXISTS ( SELECT merchant_id
  FROM tbl_merchants
  WHERE merchant_id = @var_MerchantID AND merchant_password = @hashedMerchantPassword)
SELECT merchant_role
  FROM tbl_merchants
  WHERE merchant_id = @var_MerchantID
END