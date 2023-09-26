CREATE PROCEDURE [dbo].[usp_deleteCustomer]
  @var_Guid NVARCHAR(50)
AS
SET NOCOUNT ON
IF EXISTS ( SELECT 1
FROM tbl_customers
WHERE PK_customer_guid = @var_Guid)
BEGIN
  DELETE FROM tbl_addresses
    WHERE  FK_customer_guid = @var_Guid
END
BEGIN
  DELETE FROM tbl_customers
    WHERE  PK_customer_guid = @var_Guid

  SELECT tbl_customers.PK_customer_guid
  FROM tbl_customers
  WHERE tbl_customers.PK_customer_guid = @var_Guid

END