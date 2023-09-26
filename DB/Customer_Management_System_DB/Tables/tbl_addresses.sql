CREATE TABLE [dbo].[tbl_addresses] (
    [FK_customer_guid] NVARCHAR (50)  NOT NULL,
    [country]          NVARCHAR (100) NULL,
    [county]           NVARCHAR (100) NULL,
    [zip_code]         NVARCHAR (50) NULL,
    [town]             NVARCHAR (50) NULL,
    [street]           NVARCHAR (100) NULL,
    [number]           NVARCHAR (50) NULL,
    FOREIGN KEY (FK_customer_guid) REFERENCES tbl_customers(PK_customer_guid)
    ON UPDATE CASCADE
);
