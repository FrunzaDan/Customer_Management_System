CREATE PROCEDURE [dbo].[usp_getCustomer]
    @var_SearchVariable NVARCHAR(50),
    @var_SearchOption NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        c.PK_customer_guid, 
        c.first_name, 
        c.last_name, 
        c.email, 
        c.msisdn,
        c.gender, 
        c.birthdate,
        c.customer_Status,
        a.country, 
        a.county, 
        a.town, 
        a.zip_code, 
        a.street, 
        a.number
    FROM 
        tbl_customers AS c
    INNER JOIN 
        tbl_addresses AS a 
        ON c.PK_customer_guid = a.FK_customer_guid
    WHERE 
        (@var_SearchOption = 1 AND c.PK_customer_guid = @var_SearchVariable) OR
        (@var_SearchOption = 2 AND c.msisdn = @var_SearchVariable) OR
        (@var_SearchOption = 3 AND c.email = @var_SearchVariable);
END
