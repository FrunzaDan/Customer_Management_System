IF NOT EXISTS ( SELECT 1 FROM tbl_merchants WHERE merchant_id = 'TestMerchantID')
    BEGIN
      DECLARE @hashedMerchantPassword BINARY(32);
      SET @hashedMerchantPassword = HASHBYTES('SHA2_256', 'Merchant123')
      DECLARE @currDate DATETIME;
      SET @currDate = GETDATE();
      INSERT INTO dbo.tbl_merchants(merchant_id, merchant_password, merchant_role, last_interaction)
        VALUES('TestMerchantID', @hashedMerchantPassword, '1801', @currDate)
    END

