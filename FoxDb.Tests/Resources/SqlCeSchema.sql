CREATE TABLE [Test001](
    [Id] INTEGER CONSTRAINT [sqlite_master_PK_Test001] PRIMARY KEY NOT NULL,
    [Field1] nvarchar(50) NOT NULL,
    [Field2] nvarchar(50) NOT NULL,
    [Field3] nvarchar(50) NOT NULL,
	[Field4] INTEGER NULL,
	[Field5] float NULL 
);

GO

CREATE TABLE [Test002](
    [Id] INTEGER CONSTRAINT [sqlite_master_PK_Test002] PRIMARY KEY NOT NULL,
	[Test003_Id] INTEGER NULL,
	[Test004_Id] INTEGER NULL,
    [Name] nvarchar(50) NOT NULL
);

GO

CREATE TABLE [Test002_Test004](
    [Id] INTEGER CONSTRAINT [sqlite_master_PK_Test002_Test004] PRIMARY KEY NOT NULL,
    [Test002_Id] INTEGER NOT NULL,
	[Test004_Id] INTEGER NOT NULL
);

GO

CREATE TABLE [Test003](
    [Id] INTEGER CONSTRAINT [sqlite_master_PK_Test003] PRIMARY KEY NOT NULL,
	[Test002_Id] INTEGER NULL,
    [Name] nvarchar(50) NOT NULL
);

GO

CREATE TABLE [Test004](
    [Id] INTEGER CONSTRAINT [sqlite_master_PK_Test004] PRIMARY KEY NOT NULL,
	[Test002_Id] INTEGER NULL,
    [Name] nvarchar(50) NOT NULL
);

GO