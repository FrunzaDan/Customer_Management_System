CREATE TABLE [dbo].[tbl_customers]
(
    [PK_customer_guid] NVARCHAR (50) NOT NULL,
    [first_name] NVARCHAR (50) NULL,
    [last_name] NVARCHAR (50) NULL,
    [email] NVARCHAR (50) NULL,
    [msisdn] NVARCHAR (50) NULL,
    [gender] INT NULL,
    [birthdate] NVARCHAR (50) NULL,
    [customer_Status] INT NULL,
    [creation_Date] NVARCHAR (50) NULL,
    [interaction_Date] NVARCHAR (50) NULL,
    PRIMARY KEY (PK_customer_guid)
);
