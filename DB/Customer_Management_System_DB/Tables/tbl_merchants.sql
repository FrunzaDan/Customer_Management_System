CREATE TABLE [dbo].[tbl_merchants] (
    [merchant_id]      NVARCHAR (50) NOT NULL,
    [merchant_password]    BINARY(32) NULL,
    [merchant_password_salt] BINARY(16) NULL,
    [merchant_role]      INT           NULL,
    [last_interaction] DATETIME      NULL,
    PRIMARY KEY (merchant_id)
);
