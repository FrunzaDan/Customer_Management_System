CREATE PROCEDURE [dbo].[usp_getCustomers]
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
        ON c.PK_customer_guid = a.FK_customer_guid;
END
