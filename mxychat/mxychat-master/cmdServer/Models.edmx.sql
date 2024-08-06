
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 04/16/2024 19:13:49
-- Generated from EDMX file: D:\C#Project\MXY_Chat\cmdServer\Models.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [QQ];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------


-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'UserSet'
CREATE TABLE [dbo].[UserSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(10)  NOT NULL,
    [Signal] nvarchar(100)  NOT NULL,
    [Number] nvarchar(15)  NOT NULL,
    [Password] nvarchar(20)  NOT NULL,
    [AddTime] datetime  NOT NULL
);
GO

-- Creating table 'FriendRelateSet'
CREATE TABLE [dbo].[FriendRelateSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FromUserId] int  NOT NULL,
    [ToUserId] int  NOT NULL,
    [IsPass] bit  NOT NULL,
    [AddTime] datetime  NOT NULL
);
GO

-- Creating table 'MessageSet'
CREATE TABLE [dbo].[MessageSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FromUserId] int  NOT NULL,
    [ToUserId] int  NOT NULL,
    [Content] nvarchar(300)  NOT NULL,
    [AddTime] datetime  NOT NULL,
    [IsRead] bit  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'UserSet'
ALTER TABLE [dbo].[UserSet]
ADD CONSTRAINT [PK_UserSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'FriendRelateSet'
ALTER TABLE [dbo].[FriendRelateSet]
ADD CONSTRAINT [PK_FriendRelateSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'MessageSet'
ALTER TABLE [dbo].[MessageSet]
ADD CONSTRAINT [PK_MessageSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------