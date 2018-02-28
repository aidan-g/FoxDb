﻿CREATE TABLE [Test001](
    [Id] INTEGER CONSTRAINT [sqlite_master_PK_Test001] PRIMARY KEY NOT NULL,
    [Field1] text NOT NULL,
    [Field2] text NOT NULL,
    [Field3] text NOT NULL,
	[Field4] INTEGER NULL,
	[Field5] numeric(53,0) NULL 
);

CREATE TABLE [Test002](
    [Id] INTEGER CONSTRAINT [sqlite_master_PK_Test002] PRIMARY KEY NOT NULL,
	[Test003_Id] INTEGER NULL,
	[Test004_Id] INTEGER NULL,
    [Name] text NOT NULL
);

CREATE TABLE [Test002_Test004](
    [Id] INTEGER CONSTRAINT [sqlite_master_PK_Test002_Test004] PRIMARY KEY NOT NULL,
    [Test002_Id] INTEGER NOT NULL,
	[Test004_Id] INTEGER NOT NULL
);

CREATE TABLE [Test003](
    [Id] INTEGER CONSTRAINT [sqlite_master_PK_Test003] PRIMARY KEY NOT NULL,
	[Test002_Id] INTEGER NULL,
    [Name] text NOT NULL
);

CREATE TABLE [Test004](
    [Id] INTEGER CONSTRAINT [sqlite_master_PK_Test004] PRIMARY KEY NOT NULL,
	[Test002_Id] INTEGER NULL,
    [Name] text NOT NULL
);