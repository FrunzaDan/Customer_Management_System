CREATE PROCEDURE [dbo].[usp_editCustomer]
    @var_Guid NVARCHAR(50),
    @var_FirstName NVARCHAR(50) = NULL,
    @var_LastName NVARCHAR(50) = NULL,
    @var_Email NVARCHAR(50) = NULL,
    @var_MSISDN NVARCHAR(50) = NULL,
    @var_Gender NVARCHAR(50) = NULL,
    @var_Birthdate NVARCHAR(50) = NULL,
    @var_Country NVARCHAR(100) = NULL,
    @var_County NVARCHAR(100) = NULL,
    @var_Town NVARCHAR(50) = NULL,
    @var_ZIP NVARCHAR(50) = NULL,
    @var_Street NVARCHAR(100) = NULL,
    @var_Number NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if the customer exists
    IF EXISTS (
        SELECT 1
        FROM tbl_customers
        WHERE PK_customer_guid = @var_Guid
    )
    BEGIN
        DECLARE @currDate DATETIME = GETDATE();

        -- Update customer details
        UPDATE tbl_customers
        SET 
            interaction_Date = @currDate,
            first_name = ISNULL(@var_FirstName, first_name),
            last_name = ISNULL(@var_LastName, last_name),
            email = ISNULL(@var_Email, email),
            msisdn = ISNULL(@var_MSISDN, msisdn),
            gender = ISNULL(@var_Gender, gender),
            birthdate = ISNULL(@var_Birthdate, birthdate)
        WHERE PK_customer_guid = @var_Guid;

        -- Update address details
        UPDATE tbl_addresses
        SET 
            country = ISNULL(@var_Country, country),
            county = ISNULL(@var_County, county),
            town = ISNULL(@var_Town, town),
            zip_code = ISNULL(@var_ZIP, zip_code),
            street = ISNULL(@var_Street, street),
            number = ISNULL(@var_Number, number)
        WHERE FK_customer_guid = @var_Guid;

        -- Return the customer status
        SELECT customer_Status
        FROM tbl_customers
        WHERE PK_customer_guid = @var_Guid;
    END
END
