SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ConfigFtpDropFolder](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Alias] [varchar](25) NULL,
	[FtpHost] [varchar](255) NOT NULL,
	[FtpUser] [varchar](50) NOT NULL,
	[FtpPassword] [varchar](50) NOT NULL,
	[FtpRemotePath] [varchar](50) NOT NULL,
	[Enabled] [bit] NOT NULL,
 CONSTRAINT [PK_ConfigFtpDropFolder] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RunLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StartTimeUtc] [datetime] NOT NULL,
	[EndTimeUtc] [datetime] NULL,
	[BatchSize] [int] NULL,
	[InitialCountBuiltInCatalog] [int] NULL,
	[InitialCounteCtbCatalog] [int] NULL,
	[NumFilesDropped] [int] NULL,
	[NumFilesIngested] [int] NULL,
 CONSTRAINT [PK_RunLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[IngestionLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RunLogId] [int] NULL,
	[DropFolderId] [int] NULL,
	[EpubFileName] [nvarchar](256) NOT NULL,
	[Isbn] [varchar](13) NOT NULL,
	[ArchivePath] [nvarchar](500) NULL,
	[FileSize] [bigint] NULL,
	[Ingested] [bit] NOT NULL,
	[IngestType] [varchar](50) NULL,
	[ACS4Guid] [nvarchar](50) NULL,
	[PackageHttpResponseCode] [int] NULL,
	[PackageResponse] [nvarchar](max) NULL,
	[PackageErrorCode] [nvarchar](50) NULL,
	[PackageErrorMessage] [nvarchar](256) NULL,
	[DistributionRightsAssigned] [bit] NULL,
	[DistributionRightsErrorCode] [nvarchar](50) NULL,
	[DistributionRightsErrorMessage] [nvarchar](256) NULL,
	[IsCorrupt] [bit] NULL,
	[OpfIdentifier] [nvarchar](50) NULL,
	[IsbnFromOpf] [varchar](13) NULL,
	[IsbnFromFileName] [varchar](13) NULL,
	[IsValidPreCheck] [bit] NULL,
	[PreCheckError] [nvarchar](255) NULL,
	[EnteredDateUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_LogPackageRequest] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DropFolderStats](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RunLogId] [int] NOT NULL,
	[DropFolderId] [int] NOT NULL,
	[EpubCount] [int] NOT NULL,
 CONSTRAINT [PK_DropFolderStats] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[IngestionLog] ADD  CONSTRAINT [DF_LogPackageRequest_EnteredDate]  DEFAULT (getdate()) FOR [EnteredDateUtc]
GO
ALTER TABLE [dbo].[DropFolderStats]  WITH CHECK ADD  CONSTRAINT [FK_DropFolderStats_ConfigFtpDropFolder] FOREIGN KEY([DropFolderId])
REFERENCES [dbo].[ConfigFtpDropFolder] ([Id])
GO
ALTER TABLE [dbo].[DropFolderStats] CHECK CONSTRAINT [FK_DropFolderStats_ConfigFtpDropFolder]
GO
ALTER TABLE [dbo].[DropFolderStats]  WITH CHECK ADD  CONSTRAINT [FK_DropFolderStats_RunLog] FOREIGN KEY([RunLogId])
REFERENCES [dbo].[RunLog] ([Id])
GO
ALTER TABLE [dbo].[DropFolderStats] CHECK CONSTRAINT [FK_DropFolderStats_RunLog]
GO
ALTER TABLE [dbo].[IngestionLog]  WITH CHECK ADD  CONSTRAINT [FK_IngestionLog_ConfigFtpDropFolder] FOREIGN KEY([DropFolderId])
REFERENCES [dbo].[ConfigFtpDropFolder] ([Id])
GO
ALTER TABLE [dbo].[IngestionLog] CHECK CONSTRAINT [FK_IngestionLog_ConfigFtpDropFolder]
GO
ALTER TABLE [dbo].[IngestionLog]  WITH CHECK ADD  CONSTRAINT [FK_IngestionLog_RunLog] FOREIGN KEY([RunLogId])
REFERENCES [dbo].[RunLog] ([Id])
GO
ALTER TABLE [dbo].[IngestionLog] CHECK CONSTRAINT [FK_IngestionLog_RunLog]
GO
