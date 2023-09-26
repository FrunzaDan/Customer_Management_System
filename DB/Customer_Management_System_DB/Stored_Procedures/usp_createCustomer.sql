CREATE PROCEDURE [dbo].[usp_createCustomer]
  @var_Guid NVARCHAR(50),
  @var_FirstName NVARCHAR(50),
  @var_LastName NVARCHAR(50),
  @var_Email NVARCHAR(50),
  @var_MSISDN NVARCHAR(50),
  @var_Gender NVARCHAR(50),
  @var_Birthdate NVARCHAR(50),

  @var_Country NVARCHAR(100),
  @var_County NVARCHAR(100),
  @var_Town NVARCHAR(50),
  @var_ZIP NVARCHAR(50),
  @var_Street NVARCHAR(100),
  @var_Number NVARCHAR(50)

AS
BEGIN
  SET NOCOUNT ON

  DECLARE @returnValue INTEGER;

  DECLARE @dateTimeString NVARCHAR(255);
  DECLARE @thedate DATETIME = GETDATE();

  IF EXISTS ( SELECT 1
  FROM tbl_customers
  WHERE msisdn = @var_MSISDN)
    BEGIN
    SET @returnValue = 4001
  END

  ELSE IF EXISTS ( SELECT 1
  FROM tbl_customers
  WHERE email = @var_Email)
    BEGIN
    SET @returnValue = 4002
  END

  ELSE
    BEGIN
    SET @dateTimeString = CONVERT( NVARCHAR(255), @thedate, 120 );
    INSERT INTO dbo.tbl_customers
      (PK_customer_guid, first_name, last_name, email, msisdn, gender, birthdate, customer_Status, creation_Date, interaction_Date)
    VALUES(@var_Guid, @var_FirstName, @var_LastName, @var_Email, @var_MSISDN, @var_Gender, @var_Birthdate, 1901, @dateTimeString, @dateTimeString)
    INSERT INTO dbo.tbl_addresses
      (FK_customer_guid, country, county, town, zip_code, street, number)
    VALUES(@var_Guid, @var_Country, @var_County, @var_Town, @var_ZIP, @var_Street, @var_Number)
    SET @returnValue = 200
  END
  SELECT 'ReturnValue' = @returnValue
END
