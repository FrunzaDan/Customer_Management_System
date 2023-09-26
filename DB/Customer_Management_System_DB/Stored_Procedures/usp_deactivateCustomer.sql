CREATE PROCEDURE [dbo].[usp_deactivateCustomer]
  @var_Guid NVARCHAR(50)
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
  customer_Status = 1903
  
  WHERE PK_customer_guid = @var_Guid;

  SELECT tbl_customers.customer_Status
  FROM tbl_customers
  WHERE tbl_customers.PK_customer_guid = @var_Guid
END


