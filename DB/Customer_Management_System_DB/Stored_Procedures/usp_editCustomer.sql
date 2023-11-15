CREATE PROCEDURE [dbo].[usp_editCustomer]
  @var_Guid NVARCHAR(50),
  @var_FirstName NVARCHAR(50) = null,
  @var_LastName NVARCHAR(50) = null,
  @var_Email NVARCHAR(50) = null,
  @var_MSISDN NVARCHAR(50) = null,
  @var_Gender NVARCHAR(50) = null,
  @var_Birthdate NVARCHAR(50) = null,

  @var_Country NVARCHAR(100) = null,
  @var_County NVARCHAR(100) = null,
  @var_Town NVARCHAR(50) = null,
  @var_ZIP NVARCHAR(50) = null,
  @var_Street NVARCHAR(100) = null,
  @var_Number NVARCHAR(50) = null
AS
SET NOCOUNT ON
IF EXISTS ( SELECT 1
  FROM tbl_customers
  WHERE PK_customer_guid = @var_Guid)
BEGIN
  DECLARE @currDate DATETIME;
  SET @currDate = GETDATE();
  UPDATE tbl_customers 
  SET 
  interaction_Date = @currDate,
  first_name = isNull(@var_FirstName, first_name),
  last_name = isNull(@var_LastName, last_name),
  email = isNull(@var_Email, email),
  msisdn = isNull(@var_MSISDN, msisdn),
  gender = isNull(@var_Gender, gender),
  birthdate = isNull(@var_Birthdate, birthdate)
  
  WHERE PK_customer_guid = @var_Guid;

  UPDATE tbl_addresses 
  SET 
  country = isNull(@var_Country, country),
  county = isNull(@var_County, county),
  town = isNull(@var_Town, town),
  zip_code = isNull(@var_ZIP, zip_code),
  street = isNull(@var_Street, street),
  number = isNull(@var_Number, number)
  
  WHERE FK_customer_guid = @var_Guid;

  SELECT tbl_customers.customer_Status
  FROM tbl_customers
  WHERE tbl_customers.PK_customer_guid = @var_Guid
END


