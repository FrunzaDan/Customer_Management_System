CREATE PROCEDURE [dbo].[usp_getCustomer]
  @var_SearchVariable NVARCHAR(50),
  @var_SearchOption NVARCHAR(50)

AS
SET NOCOUNT ON
BEGIN
  IF @var_SearchOption = 1
    BEGIN
    SELECT tbl_customers.PK_customer_guid, tbl_customers.first_name, tbl_customers.last_name, tbl_customers.email, tbl_customers.msisdn,
      tbl_customers.gender, tbl_customers.birthdate,
      tbl_customers.customer_Status,
      tbl_addresses.country, tbl_addresses.county, tbl_addresses.town, tbl_addresses.zip_code, tbl_addresses.street, tbl_addresses.number
    FROM tbl_customers
      INNER JOIN tbl_addresses ON tbl_customers.PK_customer_guid = tbl_addresses.FK_customer_guid
    WHERE tbl_customers.PK_customer_guid = @var_SearchVariable
  END
  IF @var_SearchOption = 2
    BEGIN
    SELECT tbl_customers.PK_customer_guid, tbl_customers.first_name, tbl_customers.last_name, tbl_customers.email, tbl_customers.msisdn,
      tbl_customers.gender, tbl_customers.birthdate,
      tbl_customers.customer_Status,
      tbl_addresses.country, tbl_addresses.county, tbl_addresses.town, tbl_addresses.zip_code, tbl_addresses.street, tbl_addresses.number
    FROM tbl_customers
      INNER JOIN tbl_addresses ON tbl_customers.PK_customer_guid = tbl_addresses.FK_customer_guid
    WHERE tbl_customers.msisdn = @var_SearchVariable
  END
  IF @var_SearchOption = 3
    BEGIN
    SELECT tbl_customers.PK_customer_guid, tbl_customers.first_name, tbl_customers.last_name, tbl_customers.email, tbl_customers.msisdn,
      tbl_customers.gender, tbl_customers.birthdate,
      tbl_customers.customer_Status,
      tbl_addresses.country, tbl_addresses.county, tbl_addresses.town, tbl_addresses.zip_code, tbl_addresses.street, tbl_addresses.number
    FROM tbl_customers
      INNER JOIN tbl_addresses ON tbl_customers.PK_customer_guid = tbl_addresses.FK_customer_guid
    WHERE tbl_customers.email = @var_SearchVariable
  END
END
