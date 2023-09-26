CREATE PROCEDURE [dbo].[usp_editCustomer]
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
  first_name = @var_FirstName,
  last_name = @var_LastName,
  email = @var_Email,
  msisdn = @var_MSISDN,
  gender = @var_Gender,
  birthdate = @var_Birthdate
  
  WHERE PK_customer_guid = @var_Guid;

  UPDATE tbl_addresses 
  SET 
  country = @var_Country,
  county = @var_County,
  town = @var_Town,
  zip_code = @var_ZIP,
  street = @var_Street,
  number = @var_Number
  
  WHERE FK_customer_guid = @var_Guid;
END


