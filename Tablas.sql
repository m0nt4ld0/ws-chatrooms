SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Usuario]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Usuario](
	[Login] [char](20) NOT NULL,
	[Password] [char](20) NOT NULL,
	[Nombre] [varchar](128) NOT NULL,
	[FechaHoraUltimaConexion] [datetime] NULL,
	[Administrador] [bit] NOT NULL CONSTRAINT [DF_Usuario_Administrador]  DEFAULT ((0)),
	[Activo] [bit] NOT NULL CONSTRAINT [DF_Usuario_Activo]  DEFAULT ((1)),
 CONSTRAINT [PK_Usuario] PRIMARY KEY CLUSTERED 
(
	[Login] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Sala]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Sala](
	[IdSala] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](128) NOT NULL,
	[Activa] [bit] NOT NULL CONSTRAINT [DF_Sala_Activa]  DEFAULT ((1)),
 CONSTRAINT [PK_Sala] PRIMARY KEY CLUSTERED 
(
	[IdSala] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Mensaje]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Mensaje](
	[IdMensaje] [int] IDENTITY(1,1) NOT NULL,
	[Login] [char](20) NOT NULL,
	[IdSala] [int] NOT NULL,
	[Texto] [text] NOT NULL,
	[FechaHora] [datetime] NOT NULL CONSTRAINT [DF_Mensaje_FechaHora]  DEFAULT (getutcdate()),
 CONSTRAINT [PK_Mensaje] PRIMARY KEY CLUSTERED 
(
	[IdMensaje] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Mensaje_Sala1]') AND parent_object_id = OBJECT_ID(N'[dbo].[Mensaje]'))
ALTER TABLE [dbo].[Mensaje]  WITH CHECK ADD  CONSTRAINT [FK_Mensaje_Sala1] FOREIGN KEY([IdSala])
REFERENCES [dbo].[Sala] ([IdSala])
GO
ALTER TABLE [dbo].[Mensaje] CHECK CONSTRAINT [FK_Mensaje_Sala1]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Mensaje_Usuario]') AND parent_object_id = OBJECT_ID(N'[dbo].[Mensaje]'))
ALTER TABLE [dbo].[Mensaje]  WITH CHECK ADD  CONSTRAINT [FK_Mensaje_Usuario] FOREIGN KEY([Login])
REFERENCES [dbo].[Usuario] ([Login])
GO
ALTER TABLE [dbo].[Mensaje] CHECK CONSTRAINT [FK_Mensaje_Usuario]
