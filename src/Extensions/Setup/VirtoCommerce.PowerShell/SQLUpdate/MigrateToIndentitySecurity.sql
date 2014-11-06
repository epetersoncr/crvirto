/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 2014-11-04 18:40:30 ******/
SET ANSI_NULLS ON
GO


SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.AspNetUserRoles', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUserRoles]
GO
IF OBJECT_ID('dbo.AspNetUserLogins', 'U') IS NOT NULL
  DROP TABLE [dbo].[AspNetUserLogins]
GO
IF OBJECT_ID('dbo.AspNetUserClaims', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUserClaims]
GO
IF OBJECT_ID('dbo.AspNetRoles', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetRoles]
GO
IF OBJECT_ID('dbo.AspNetUsers', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUsers]
GO

CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](128) NOT NULL,
	[Email] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEndDateUtc] [datetime] NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[UserName] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO

/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 2014-11-04 18:45:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO

ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId]
GO

/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 2014-11-04 18:45:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO

/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 2014-11-04 18:46:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO

ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId]
GO

/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 2014-11-04 18:46:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](128) NOT NULL,
	[RoleId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO

ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId]
GO

ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId]
GO

INSERT [__MigrationHistory] ([MigrationId], [ContextKey], [Model], [ProductVersion]) VALUES (N'201411042133537_InitialCreate', N'VirtoCommerce.Web.Client.Security.Identity.Data.ApplicationDbContext', 0x1F8B0800000000000400DD5CDB6EDC36107D2FD07F10F4D416EECA9726488DDD16EEDA6E8DC617649DB46F0157E2AE8948942251AE8DA25FD6877E527FA143DDC58B2EBBDA8B8B00C14A24CF0C8787E47034F4BF7FFF33FEF1C9738D471C46C4A713F36874681A98DABE43E87262C66CF1ED1BF3C71FBEFC627CE1784FC687BCDE09AF072D6934311F180B4E2D2BB21FB087A29147ECD08FFC051BD9BE6721C7B78E0F0FBFB78E8E2C0C10266019C6F85D4C19F170F2008F539FDA38603172AF7D07BB51F61E4A6609AA71833C1C05C8C613F30309993FF53D0F87361EFD86E7A3A94B3065A319B6E390B0E7D195038FFCC73962C834CE5C8240CD197617A68128F51962D089D3F7119EB1D0A7CB59002F907BFF1C60A8B7406E84B3CE9D96D5BBF6F3F098F7D32A1BE650761C31DFEB0978749219CE129BAF647EB3302C98F6223112EF7562DE8999DBED9DEF82014481A75337E49527E67521E22C0A6E302B0D9E425E8600F7871F7E1A55110F8CCEED0E0AA21D8F0EF9BF03631ABB2C0EF184E29885C83D30EEE2B94BEC5FF1F3BDFF09D3C9C9D17C71F2E6D56BE49CBCFE0E9FBCAAF614FA0AF56A2FE0D55DE8073804DDF0A2E8BF6958F57696D8B0685669935A05B80473C634AED1D35B4C97EC0166D3F11BD3B8244FD8C9DF64E47A4F094C3168C4C2181E6F62D745731717E556A34CFE7F83D4E357AF07917A831EC932197A413E4C9C30328D77D84D4AA30712A4D3AB36DE1FB36A97A1EFF1E73ABFD2D28F333F86590C9DF1B555EE51B8C4ACAEDDD82AC9DB89D21C6A785AE7A8FB4F6DAEA94C6F6555DEA15566422E62DBB321D777B3723B33EE2C0860F0126A718B3411AEC74E966C8B23011BB82422A40025C98E464727DD5846A1F7FFE745F3C243C41D60D5EC2005BC9905093D5CF4F2271F388A686F9DEF5014C1A2E1FC82A28706D5E1E700AAE7A49B31E4051B9776F7E0537C137B733E45B6276BB0A1B9FFC3BF4436F3C30BCA5BAD8DF7D6B73FF931BBA00E38ADF83DB37340FE784FBCEE0083A87366DB388A2E81CCD899FAE0ACE78057949D1CF786E3ABD5AEBD96A98B88A7765B8475F5635EB5745DD43524F745534DE5C234A9FAD65F12DA4DD5BCAA5ED5B446ABAA59B5BEAA72B06E9A6635F58A26155AF54C6B0DE614262334BC5798C0EEBF5BB8DEE6AD5B0B2A669CC10A897FC61487B08C397788311CD27204BAAC1BBB701692E1E34237BE3725923E20371E5AD44AB3215904869F0D09ECFECF86444D78FD481CEE9574382BE59501BE537DF531AC7DCE099A6D7B3AD4BAB96DE1DB590374D3E52C8A7C9B24B3401125CB621C75FDC18733DA031E696FC4A009740C884EF896076FA06FA648AA5B7A8E5DCCB07166A751C4298A6CE4C866840E393D14CB7754856265F0A4AEDC37924C603A0E7923C40F4111CC5442993C2D08B54980DC562B092D3B6E61BCEF850CB1E41C07987281AD96E8225C1D2BE10A1472844169B3D0D8AA30AE99881AAF5537E66D2E6C39EE5208632B9C6CF19D35BCCCFCB78D10B3D9625B2067B349BA28A08DFBED82A0D959A52B01C483CBBE1154383169089AB9545B2168DD623B2068DD242F8EA0E911B5EBF80BE7D57DA367FDA0BCFD6DBDD15C3BE066CD1E7B46CDD4F784360C5AE050A6E7F99C17E227A6389C819ED9F92CCA5C5D91221C7C86593D6453FABB4A3FD46A061149D4045812AD0534FB6228014913AA8772792CAF51BBCC8BE8019BC7DD1A61B3B55F80AD7040C6AE7E39AD54D47F5F15C9D9E9F451F4AC608344F24E87850A8E8210E2E255EF7807A3E8E2B2B261BAF8C27DBCE14AC7B2C16830508BE7AA3152DE99C1AD9453B3DD4A2A87AC8F4BB6969504F74963A5BC33835B29E368BB91144E410FB7602D13D5B7F081265B1EE928769BA26C6CA5D956D98BB1A549CB1A5FA32020745949D3CADE18B334476BFAEDAC7F7E9297625876A448532AB42D24313F444B2C948268D0F4928411E3C95F73C4E33C53C793AA29F756CDF29F8BAC6E9FF220E6FB405E9BFF4E5BF4CC58AB6FC4B2A79209B884EE7BDCDD4962EC0A72A89B1B3CA70EB9285484F5A7BE1B7B54EF7DE95BA71FF7AAEDD33732C2D812F497BC2BC994920F5C1F974EA326CF986D8C60E1F9AC3E8A7A08DD58E47E6B753474BEAC1E250F6D555174E1AE9D8DAACE051A6E2445D7B3FF40B6226C664666F92E5580EC554F8C4ACA84045629EB8E5ACF6AA962D64BBA230AA92B5548A1A88796D504959A92D58295F0341655D7E82E414E49A9A2CBA5DD9115C929556845F10AD80A9DC5B2EEA88AFC952AB0A2B83B7699CC22AEB07BBCE7690F449BD9F4D203F57ABB9E066333CBE5309B66256FA00A5479DD132BCB0C90C0B2F77B4935EDA97233544B832CEB514D83A15FB36A9FE3EB4B56630E811EB3F68DBDB62D34E518E8F1FA117AA3B4914E9C6295427A71F2144E98E3ECB4D77E3B483AFEA5554C233723B804CF11C35E4AA4D96737655A59E11A51B2C0114BF34ACCE3C3A363E10ED1FEDCE7B1A2C87115A765DDA59EFA986D21458C3EA2D07E40A19CB0B1C69D9712548A855F51073F4DCC3F9356A7495885FF4A5E1F1857D17B4A3EC750701FC6D8F84B4E401DE60E40F3D96D4F6F6C74B7EAD5EF1FD3A607C66D0833E6D438146CB9CA08D7EF71F4D2266DBA86362BDFEE78B913AA761F42892A4C88D5AF3FCC091BE4EA43AEE5571E7AFABAAF6ACAEB0D6B212AAE300C8537880975571456C1D25E4F70E09125D713FA75567D5D6115D5B4571508ED0F265E54E8BE0CE52D77B8D5280E4CDB5892123BB7267AAF95F5B9EBBD49CA075F6BA2CB39DF3DE0D6C8EB5E81192F2C257AB0DD5191F13C18F62EA9BDF134E77DC96C2E734E769BD0BCCD1CE686AF4DFFABD4E53D48B653240FED3E4179DB5CD30579F73CCBB35F1AF29E912D4B29DB7DB2F1B6C9A60BF3EE39D97AA514EF19D776B57FEE98699DB7D09D2708CBB94E9A8F35AA58705B02701A388713FEDC0712A41E657A6F539D71A6135692452BB0ACA217AA4F7513054B1347922BD56816DBAFAFD986DFD8D9AC4EB3584D826893EC6CFD6F949DD56996AD49BBDC45EAB232F151954EDEB28E35E556BDA454E55A4F5A32E3DB7CD6C62FEF2F29337910A3D4668FE61BF1CB49441EC424434E9D1E89C7F2E75ED83B2B7F3212F6EF882C4B08FE072429B66BBB6651E78A2EFC7CF31634CAAB08119A6BCC90035BEA59C8C802D90C8A798C39B9789EC4EDF8978E3976AEE86DCC82984197B137776B012FEE0434C94FB2ABEB3A8F6F83E46FA80CD1055093F0D8FC2DFD2926AE53E87DA988096920B877914574F958321ED95D3E1748373EED089499AF708AEEB117B80016DDD2197AC4ABE806F47B8B97C87E2E23803A90F681A89B7D7C4ED032445E946194EDE11138EC784F3FFC0704B9AF5A39550000, N'6.1.1-30610')
GO

INSERT INTO [AspNetUsers]
(Id, UserName, EmailConfirmed, PhoneNumberConfirmed, TwoFactorEnabled, PasswordHash, SecurityStamp, AccessFailedCount, LockoutEndDateUtc, LockoutEnabled) 
SELECT UserId, a.UserName, IsConfirmed, 0, 0, Password, PasswordSalt, PasswordFailuresSinceLastSuccess, LastPasswordFailureDate, 
CASE WHEN a.RegisterType = 2 THEN 1 ELSE 0 END
FROM [webpages_Membership] m INNER JOIN [Account] a on a.AccountId = m.UserId

INSERT INTO [AspNetUserLogins](UserId, LoginProvider, ProviderKey)
SELECT UserId, Provider, ProviderUserId
FROM webpages_OAuthMembership
GO

IF OBJECT_ID('dbo.webpages_OAuthMembership', 'U') IS NOT NULL
    DROP TABLE [dbo].[webpages_OAuthMembership]
GO

IF OBJECT_ID('dbo.webpages_UsersInRoles', 'U') IS NOT NULL
    DROP TABLE [dbo].[webpages_UsersInRoles]
GO
IF OBJECT_ID('dbo.webpages_Roles', 'U') IS NOT NULL
    DROP TABLE [dbo].[webpages_Roles]
GO

IF OBJECT_ID('dbo.webpages_Membership', 'U') IS NOT NULL
    DROP TABLE [dbo].[webpages_Membership]
GO

