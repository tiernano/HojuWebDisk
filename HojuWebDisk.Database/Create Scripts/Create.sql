 SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[up_Files_list]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[up_Files_list] (
	@ID int = NULL,
	@ParentID int = NULL,
	@FileName varchar(255) = NULL,
	@ContentType varchar(255) = NULL,
	@FileDataSize bigint = NULL,
	@update_date_stamp datetime = NULL,
	@update_user_stamp varchar(40) = NULL,
	@create_date_stamp datetime = NULL,
	@create_user_stamp varchar(40) = NULL,
	@update_seq_stamp int = NULL
	,@page_size int = null,
	@page_num int = null,
	@sort_col varchar(120) = NULL,
	@sort_seq varchar(4) = NULL,
	@count_only_flag char(1) = ''N''
) AS

	DECLARE
		@sql_statement nvarchar(4000),
		@paramlist nvarchar(4000),
		@rvs_sort_seq varchar(4),
		@sbound varchar(10),
		@spage_size varchar(10),
		@count int,
		@spage_num varchar(10)
	

	BEGIN

	SET @sort_seq = ISNULL(@sort_seq,'''')
	SET @count_only_flag = ISNULL(@count_only_flag,''N'')
	SET NOCOUNT ON
	
	IF @count_only_flag = ''N''
	BEGIN
		IF (@page_size IS NOT NULL AND @page_num IS NULL) OR (@page_size IS NULL AND @page_num IS NOT NULL)
			RAISERROR(50003,16,1,''page_size or page_num'',''Must specify both page_size and page_num'')
		IF @page_size <> 0 AND @page_size IS NOT NULL
		BEGIN
			SET @spage_size = CONVERT(VARCHAR(10),@page_size)
			SET @spage_num = CONVERT(VARCHAR(10),@page_num)
			IF @page_size < 1
				RAISERROR(50003,16,1,''@page_size'',''@spage_size'')
			IF @page_num < 1
				RAISERROR(50003,16,1,''@page_num'',''@spage_num'')
			IF @sort_col IS NULL
			BEGIN
				SET @sort_col = 1
				SET @sort_seq = ''asc''
			END
			IF @sort_seq = ''asc''
			BEGIN
				SET @rvs_sort_seq = ''desc''
				SET @sbound = CONVERT(varchar(10),(@page_size * @page_num))
			END
			ELSE
			BEGIN
				SET @rvs_sort_seq = ''asc''
				SET @sbound = CONVERT(varchar(10),(@page_size * @page_num))
			END
		END	

		SET @sql_statement = ''SELECT 		[ID],
		[ParentID],
		[FileName],
		[ContentType],
		[FileDataSize],
		[update_date_stamp],
		[update_user_stamp],
		[create_date_stamp],
		[create_user_stamp],
		[update_seq_stamp]
	FROM ''
		IF @page_size <> 0 AND @page_size IS NOT NULL
		BEGIN
			SET @sql_statement = @sql_statement + ''(select top '' + @spage_size + '' * from (select top '' + @sbound + '' * from ''
		END
		SET @sql_statement = @sql_statement + ''[Files] WHERE 1 = 1''
	END
	ELSE
	BEGIN
		SET @sql_statement = ''SELECT count(*) as count FROM [Files] WHERE 1 = 1''
	END
	IF @ID IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [ID] = @ID''
	IF @ParentID IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [ParentID] = @ParentID''
	IF @FileName IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [FileName] LIKE @FileName''
	IF @ContentType IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [ContentType] LIKE @ContentType''
	IF @FileDataSize IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [FileDataSize] = @FileDataSize''
	IF @update_date_stamp IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [update_date_stamp] = @update_date_stamp''
	IF @update_user_stamp IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [update_user_stamp] LIKE @update_user_stamp''
	IF @create_date_stamp IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [create_date_stamp] = @create_date_stamp''
	IF @create_user_stamp IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [create_user_stamp] LIKE @create_user_stamp''
	IF @update_seq_stamp IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [update_seq_stamp] = @update_seq_stamp''


	IF @sort_col IS NOT NULL
		SET @sql_statement = @sql_statement + '' order by '' + @sort_col + '' '' + @sort_seq
		
	IF @page_size <> 0 and @page_size IS NOT NULL
	BEGIN
		SET @sql_statement = @sql_statement + '' ) a order by '' + @sort_col + '' '' + @rvs_sort_seq + '') b order by '' + @sort_col + '' '' + @sort_seq
	END
	
	SET @paramlist = ''@ID int,@ParentID int,@FileName varchar(255),@ContentType varchar(255),@FileDataSize bigint,@update_date_stamp datetime,@update_user_stamp varchar(40),@create_date_stamp datetime,@create_user_stamp varchar(40),@update_seq_stamp int''
	
	EXEC sp_executesql @sql_statement,@paramlist,@ID,@ParentID,@FileName,@ContentType,@FileDataSize,@update_date_stamp,@update_user_stamp,@create_date_stamp,@create_user_stamp,@update_seq_stamp
	
	SET NOCOUNT OFF

	RETURN @@ROWCOUNT
END


' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[up_Folders_list]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[up_Folders_list] (
	@ID int = NULL,
	@ParentID int = NULL,
	@FolderName varchar(255) = NULL,
	@update_date_stamp datetime = NULL,
	@update_user_stamp varchar(40) = NULL,
	@create_date_stamp datetime = NULL,
	@create_user_stamp varchar(40) = NULL,
	@update_seq_stamp int = NULL
	,@page_size int = null,
	@page_num int = null,
	@sort_col varchar(120) = NULL,
	@sort_seq varchar(4) = NULL,
	@count_only_flag char(1) = ''N''
) AS

	DECLARE
		@sql_statement nvarchar(4000),
		@paramlist nvarchar(4000),
		@rvs_sort_seq varchar(4),
		@sbound varchar(10),
		@spage_size varchar(10),
		@count int,
		@spage_num varchar(10)
	

	BEGIN

	SET @sort_seq = ISNULL(@sort_seq,'''')
	SET @count_only_flag = ISNULL(@count_only_flag,''N'')
	SET NOCOUNT ON
	
	IF @count_only_flag = ''N''
	BEGIN
		IF (@page_size IS NOT NULL AND @page_num IS NULL) OR (@page_size IS NULL AND @page_num IS NOT NULL)
			RAISERROR(50003,16,1,''page_size or page_num'',''Must specify both page_size and page_num'')
		IF @page_size <> 0 AND @page_size IS NOT NULL
		BEGIN
			SET @spage_size = CONVERT(VARCHAR(10),@page_size)
			SET @spage_num = CONVERT(VARCHAR(10),@page_num)
			IF @page_size < 1
				RAISERROR(50003,16,1,''@page_size'',''@spage_size'')
			IF @page_num < 1
				RAISERROR(50003,16,1,''@page_num'',''@spage_num'')
			IF @sort_col IS NULL
			BEGIN
				SET @sort_col = 1
				SET @sort_seq = ''asc''
			END
			IF @sort_seq = ''asc''
			BEGIN
				SET @rvs_sort_seq = ''desc''
				SET @sbound = CONVERT(varchar(10),(@page_size * @page_num))
			END
			ELSE
			BEGIN
				SET @rvs_sort_seq = ''asc''
				SET @sbound = CONVERT(varchar(10),(@page_size * @page_num))
			END
		END	

		SET @sql_statement = ''SELECT 		[ID],
		[ParentID],
		[FolderName],
		[update_date_stamp],
		[update_user_stamp],
		[create_date_stamp],
		[create_user_stamp],
		[update_seq_stamp]
	FROM ''
		IF @page_size <> 0 AND @page_size IS NOT NULL
		BEGIN
			SET @sql_statement = @sql_statement + ''(select top '' + @spage_size + '' * from (select top '' + @sbound + '' * from ''
		END
		SET @sql_statement = @sql_statement + ''[Folders] WHERE 1 = 1''
	END
	ELSE
	BEGIN
		SET @sql_statement = ''SELECT count(*) as count FROM [Folders] WHERE 1 = 1''
	END
	IF @ID IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [ID] = @ID''
	IF @ParentID IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [ParentID] = @ParentID''
	IF @FolderName IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [FolderName] LIKE @FolderName''
	IF @update_date_stamp IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [update_date_stamp] = @update_date_stamp''
	IF @update_user_stamp IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [update_user_stamp] LIKE @update_user_stamp''
	IF @create_date_stamp IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [create_date_stamp] = @create_date_stamp''
	IF @create_user_stamp IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [create_user_stamp] LIKE @create_user_stamp''
	IF @update_seq_stamp IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [update_seq_stamp] = @update_seq_stamp''


	IF @sort_col IS NOT NULL
		SET @sql_statement = @sql_statement + '' order by '' + @sort_col + '' '' + @sort_seq
		
	IF @page_size <> 0 and @page_size IS NOT NULL
	BEGIN
		SET @sql_statement = @sql_statement + '' ) a order by '' + @sort_col + '' '' + @rvs_sort_seq + '') b order by '' + @sort_col + '' '' + @sort_seq
	END
	
	SET @paramlist = ''@ID int,@ParentID int,@FolderName varchar(255),@update_date_stamp datetime,@update_user_stamp varchar(40),@create_date_stamp datetime,@create_user_stamp varchar(40),@update_seq_stamp int''
	
	EXEC sp_executesql @sql_statement,@paramlist,@ID,@ParentID,@FolderName,@update_date_stamp,@update_user_stamp,@create_date_stamp,@create_user_stamp,@update_seq_stamp
	
	SET NOCOUNT OFF

	RETURN @@ROWCOUNT
END

' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[up_Locks_Tokens_list]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[up_Locks_Tokens_list] (
	@ID int = NULL,
	@LockID int = NULL,
	@Token varchar(255) = NULL,
	@update_date_stamp datetime = NULL,
	@update_user_stamp varchar(40) = NULL,
	@create_date_stamp datetime = NULL,
	@create_user_stamp varchar(40) = NULL,
	@update_seq_stamp int = NULL
	,@page_size int = null,
	@page_num int = null,
	@sort_col varchar(120) = NULL,
	@sort_seq varchar(4) = NULL,
	@count_only_flag char(1) = ''N''
) AS

	DECLARE
		@sql_statement nvarchar(4000),
		@paramlist nvarchar(4000),
		@rvs_sort_seq varchar(4),
		@sbound varchar(10),
		@spage_size varchar(10),
		@count int,
		@spage_num varchar(10)
	

	BEGIN

	SET @sort_seq = ISNULL(@sort_seq,'''')
	SET @count_only_flag = ISNULL(@count_only_flag,''N'')
	SET NOCOUNT ON
	
	IF @count_only_flag = ''N''
	BEGIN
		IF (@page_size IS NOT NULL AND @page_num IS NULL) OR (@page_size IS NULL AND @page_num IS NOT NULL)
			RAISERROR(50003,16,1,''page_size or page_num'',''Must specify both page_size and page_num'')
		IF @page_size <> 0 AND @page_size IS NOT NULL
		BEGIN
			SET @spage_size = CONVERT(VARCHAR(10),@page_size)
			SET @spage_num = CONVERT(VARCHAR(10),@page_num)
			IF @page_size < 1
				RAISERROR(50003,16,1,''@page_size'',''@spage_size'')
			IF @page_num < 1
				RAISERROR(50003,16,1,''@page_num'',''@spage_num'')
			IF @sort_col IS NULL
			BEGIN
				SET @sort_col = 1
				SET @sort_seq = ''asc''
			END
			IF @sort_seq = ''asc''
			BEGIN
				SET @rvs_sort_seq = ''desc''
				SET @sbound = CONVERT(varchar(10),(@page_size * @page_num))
			END
			ELSE
			BEGIN
				SET @rvs_sort_seq = ''asc''
				SET @sbound = CONVERT(varchar(10),(@page_size * @page_num))
			END
		END	

		SET @sql_statement = ''SELECT 		[ID],
		[LockID],
		[Token],
		[update_date_stamp],
		[update_user_stamp],
		[create_date_stamp],
		[create_user_stamp],
		[update_seq_stamp]
	FROM ''
		IF @page_size <> 0 AND @page_size IS NOT NULL
		BEGIN
			SET @sql_statement = @sql_statement + ''(select top '' + @spage_size + '' * from (select top '' + @sbound + '' * from ''
		END
		SET @sql_statement = @sql_statement + ''[Locks_Tokens] WHERE 1 = 1''
	END
	ELSE
	BEGIN
		SET @sql_statement = ''SELECT count(*) as count FROM [Locks_Tokens] WHERE 1 = 1''
	END
	IF @ID IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [ID] = @ID''
	IF @LockID IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [LockID] = @LockID''
	IF @Token IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [Token] LIKE @Token''
	IF @update_date_stamp IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [update_date_stamp] = @update_date_stamp''
	IF @update_user_stamp IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [update_user_stamp] LIKE @update_user_stamp''
	IF @create_date_stamp IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [create_date_stamp] = @create_date_stamp''
	IF @create_user_stamp IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [create_user_stamp] LIKE @create_user_stamp''
	IF @update_seq_stamp IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [update_seq_stamp] = @update_seq_stamp''


	IF @sort_col IS NOT NULL
		SET @sql_statement = @sql_statement + '' order by '' + @sort_col + '' '' + @sort_seq
		
	IF @page_size <> 0 and @page_size IS NOT NULL
	BEGIN
		SET @sql_statement = @sql_statement + '' ) a order by '' + @sort_col + '' '' + @rvs_sort_seq + '') b order by '' + @sort_col + '' '' + @sort_seq
	END
	
	SET @paramlist = ''@ID int,@LockID int,@Token varchar(255),@update_date_stamp datetime,@update_user_stamp varchar(40),@create_date_stamp datetime,@create_user_stamp varchar(40),@update_seq_stamp int''
	
	EXEC sp_executesql @sql_statement,@paramlist,@ID,@LockID,@Token,@update_date_stamp,@update_user_stamp,@create_date_stamp,@create_user_stamp,@update_seq_stamp
	
	SET NOCOUNT OFF

	RETURN @@ROWCOUNT
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Locks_Tokens]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Locks_Tokens](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[LockID] [int] NOT NULL,
	[Token] [varchar](255) NOT NULL,
	[update_date_stamp] [datetime] NOT NULL,
	[update_user_stamp] [varchar](40) NOT NULL,
	[create_date_stamp] [datetime] NOT NULL,
	[create_user_stamp] [varchar](40) NOT NULL,
	[update_seq_stamp] [int] NOT NULL,
 CONSTRAINT [PK_Locks_Tokens] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[up_Locks_list]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[up_Locks_list] (
	@ID int = NULL,
	@ResID int = NULL,
	@LockType int = NULL,
	@ResType int = NULL,
	@LockScope int = NULL,
	@LockDepth int = NULL,
	@LockOwner varchar(255) = NULL,
	@LockOwnerType int = NULL,
	@Timeout int = NULL,
	@update_date_stamp datetime = NULL,
	@update_user_stamp varchar(40) = NULL,
	@create_date_stamp datetime = NULL,
	@create_user_stamp varchar(40) = NULL,
	@update_seq_stamp int = NULL
	,@page_size int = null,
	@page_num int = null,
	@sort_col varchar(120) = NULL,
	@sort_seq varchar(4) = NULL,
	@count_only_flag char(1) = ''N''
) AS

	DECLARE
		@sql_statement nvarchar(4000),
		@paramlist nvarchar(4000),
		@rvs_sort_seq varchar(4),
		@sbound varchar(10),
		@spage_size varchar(10),
		@count int,
		@spage_num varchar(10)
	

	BEGIN

	SET @sort_seq = ISNULL(@sort_seq,'''')
	SET @count_only_flag = ISNULL(@count_only_flag,''N'')
	SET NOCOUNT ON
	
	IF @count_only_flag = ''N''
	BEGIN
		IF (@page_size IS NOT NULL AND @page_num IS NULL) OR (@page_size IS NULL AND @page_num IS NOT NULL)
			RAISERROR(50003,16,1,''page_size or page_num'',''Must specify both page_size and page_num'')
		IF @page_size <> 0 AND @page_size IS NOT NULL
		BEGIN
			SET @spage_size = CONVERT(VARCHAR(10),@page_size)
			SET @spage_num = CONVERT(VARCHAR(10),@page_num)
			IF @page_size < 1
				RAISERROR(50003,16,1,''@page_size'',''@spage_size'')
			IF @page_num < 1
				RAISERROR(50003,16,1,''@page_num'',''@spage_num'')
			IF @sort_col IS NULL
			BEGIN
				SET @sort_col = 1
				SET @sort_seq = ''asc''
			END
			IF @sort_seq = ''asc''
			BEGIN
				SET @rvs_sort_seq = ''desc''
				SET @sbound = CONVERT(varchar(10),(@page_size * @page_num))
			END
			ELSE
			BEGIN
				SET @rvs_sort_seq = ''asc''
				SET @sbound = CONVERT(varchar(10),(@page_size * @page_num))
			END
		END	

		SET @sql_statement = ''SELECT 		[ID],
		[ResID],
		[LockType],
		[ResType],
		[LockScope],
		[LockDepth],
		[LockOwner],
		[LockOwnerType],
		[Timeout],
		[update_date_stamp],
		[update_user_stamp],
		[create_date_stamp],
		[create_user_stamp],
		[update_seq_stamp]
	FROM ''
		IF @page_size <> 0 AND @page_size IS NOT NULL
		BEGIN
			SET @sql_statement = @sql_statement + ''(select top '' + @spage_size + '' * from (select top '' + @sbound + '' * from ''
		END
		SET @sql_statement = @sql_statement + ''[Locks] WHERE 1 = 1''
	END
	ELSE
	BEGIN
		SET @sql_statement = ''SELECT count(*) as count FROM [Locks] WHERE 1 = 1''
	END
	IF @ID IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [ID] = @ID''
	IF @ResID IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [ResID] = @ResID''
	IF @LockType IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [LockType] = @LockType''
	IF @ResType IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [ResType] = @ResType''
	IF @LockScope IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [LockScope] = @LockScope''
	IF @LockDepth IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [LockDepth] = @LockDepth''
	IF @LockOwner IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [LockOwner] LIKE @LockOwner''
	IF @LockOwnerType IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [LockOwnerType] = @LockOwnerType''
	IF @Timeout IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [Timeout] = @Timeout''
	IF @update_date_stamp IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [update_date_stamp] = @update_date_stamp''
	IF @update_user_stamp IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [update_user_stamp] LIKE @update_user_stamp''
	IF @create_date_stamp IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [create_date_stamp] = @create_date_stamp''
	IF @create_user_stamp IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [create_user_stamp] LIKE @create_user_stamp''
	IF @update_seq_stamp IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [update_seq_stamp] = @update_seq_stamp''


	IF @sort_col IS NOT NULL
		SET @sql_statement = @sql_statement + '' order by '' + @sort_col + '' '' + @sort_seq
		
	IF @page_size <> 0 and @page_size IS NOT NULL
	BEGIN
		SET @sql_statement = @sql_statement + '' ) a order by '' + @sort_col + '' '' + @rvs_sort_seq + '') b order by '' + @sort_col + '' '' + @sort_seq
	END
	
	SET @paramlist = ''@ID int,@ResID int,@LockType int,@ResType int,@LockScope int,@LockDepth int,@LockOwner varchar(255),@LockOwnerType int,@Timeout int,@update_date_stamp datetime,@update_user_stamp varchar(40),@create_date_stamp datetime,@create_user_stamp varchar(40),@update_seq_stamp int''
	
	EXEC sp_executesql @sql_statement,@paramlist,@ID,@ResID,@LockType,@ResType,@LockScope,@LockDepth,@LockOwner,@LockOwnerType,@Timeout,@update_date_stamp,@update_user_stamp,@create_date_stamp,@create_user_stamp,@update_seq_stamp
	
	SET NOCOUNT OFF

	RETURN @@ROWCOUNT
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Locks]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Locks](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ResID] [int] NOT NULL,
	[LockType] [int] NOT NULL,
	[ResType] [int] NOT NULL,
	[LockScope] [int] NOT NULL,
	[LockDepth] [int] NOT NULL,
	[LockOwner] [varchar](255) NOT NULL,
	[LockOwnerType] [int] NOT NULL,
	[Timeout] [int] NOT NULL,
	[update_date_stamp] [datetime] NOT NULL,
	[update_user_stamp] [varchar](40) NOT NULL,
	[create_date_stamp] [datetime] NOT NULL,
	[create_user_stamp] [varchar](40) NOT NULL,
	[update_seq_stamp] [int] NOT NULL,
 CONSTRAINT [PK_Locks] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Access]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Access](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FolderID] [int] NOT NULL,
	[UID] [varchar](255) NULL,
	[Access] [int] NOT NULL,
	[update_date_stamp] [datetime] NOT NULL,
	[update_user_stamp] [varchar](40) NOT NULL,
	[create_date_stamp] [datetime] NOT NULL,
	[create_user_stamp] [varchar](40) NOT NULL,
	[update_seq_stamp] [int] NOT NULL,
 CONSTRAINT [PK_Access] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[up_Access_list]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[up_Access_list] (
	@ID int = NULL,
	@FolderID int = NULL,
	@UID varchar(255) = NULL,
	@Access int = NULL,
	@update_date_stamp datetime = NULL,
	@update_user_stamp varchar(40) = NULL,
	@create_date_stamp datetime = NULL,
	@create_user_stamp varchar(40) = NULL,
	@update_seq_stamp int = NULL
	,@page_size int = null,
	@page_num int = null,
	@sort_col varchar(120) = NULL,
	@sort_seq varchar(4) = NULL,
	@count_only_flag char(1) = ''N''
) AS

	DECLARE
		@sql_statement nvarchar(4000),
		@paramlist nvarchar(4000),
		@rvs_sort_seq varchar(4),
		@sbound varchar(10),
		@spage_size varchar(10),
		@count int,
		@spage_num varchar(10)
	

	BEGIN

	SET @sort_seq = ISNULL(@sort_seq,'''')
	SET @count_only_flag = ISNULL(@count_only_flag,''N'')
	SET NOCOUNT ON
	
	IF @count_only_flag = ''N''
	BEGIN
		IF (@page_size IS NOT NULL AND @page_num IS NULL) OR (@page_size IS NULL AND @page_num IS NOT NULL)
			RAISERROR(50003,16,1,''page_size or page_num'',''Must specify both page_size and page_num'')
		IF @page_size <> 0 AND @page_size IS NOT NULL
		BEGIN
			SET @spage_size = CONVERT(VARCHAR(10),@page_size)
			SET @spage_num = CONVERT(VARCHAR(10),@page_num)
			IF @page_size < 1
				RAISERROR(50003,16,1,''@page_size'',''@spage_size'')
			IF @page_num < 1
				RAISERROR(50003,16,1,''@page_num'',''@spage_num'')
			IF @sort_col IS NULL
			BEGIN
				SET @sort_col = 1
				SET @sort_seq = ''asc''
			END
			IF @sort_seq = ''asc''
			BEGIN
				SET @rvs_sort_seq = ''desc''
				SET @sbound = CONVERT(varchar(10),(@page_size * @page_num))
			END
			ELSE
			BEGIN
				SET @rvs_sort_seq = ''asc''
				SET @sbound = CONVERT(varchar(10),(@page_size * @page_num))
			END
		END	

		SET @sql_statement = ''SELECT 		[ID],
		[FolderID],
		[UID],
		[Access],
		[update_date_stamp],
		[update_user_stamp],
		[create_date_stamp],
		[create_user_stamp],
		[update_seq_stamp]
	FROM ''
		IF @page_size <> 0 AND @page_size IS NOT NULL
		BEGIN
			SET @sql_statement = @sql_statement + ''(select top '' + @spage_size + '' * from (select top '' + @sbound + '' * from ''
		END
		SET @sql_statement = @sql_statement + ''[Access] WHERE 1 = 1''
	END
	ELSE
	BEGIN
		SET @sql_statement = ''SELECT count(*) as count FROM [Access] WHERE 1 = 1''
	END
	IF @ID IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [ID] = @ID''
	IF @FolderID IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [FolderID] = @FolderID''
	IF @UID IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [UID] LIKE @UID''
	IF @Access IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [Access] = @Access''
	IF @update_date_stamp IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [update_date_stamp] = @update_date_stamp''
	IF @update_user_stamp IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [update_user_stamp] LIKE @update_user_stamp''
	IF @create_date_stamp IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [create_date_stamp] = @create_date_stamp''
	IF @create_user_stamp IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [create_user_stamp] LIKE @create_user_stamp''
	IF @update_seq_stamp IS NOT NULL 
		SET @sql_statement = @sql_statement + '' AND [update_seq_stamp] = @update_seq_stamp''


	IF @sort_col IS NOT NULL
		SET @sql_statement = @sql_statement + '' order by '' + @sort_col + '' '' + @sort_seq
		
	IF @page_size <> 0 and @page_size IS NOT NULL
	BEGIN
		SET @sql_statement = @sql_statement + '' ) a order by '' + @sort_col + '' '' + @rvs_sort_seq + '') b order by '' + @sort_col + '' '' + @sort_seq
	END
	
	SET @paramlist = ''@ID int,@FolderID int,@UID varchar(255),@Access int,@update_date_stamp datetime,@update_user_stamp varchar(40),@create_date_stamp datetime,@create_user_stamp varchar(40),@update_seq_stamp int''
	
	EXEC sp_executesql @sql_statement,@paramlist,@ID,@FolderID,@UID,@Access,@update_date_stamp,@update_user_stamp,@create_date_stamp,@create_user_stamp,@update_seq_stamp
	
	SET NOCOUNT OFF

	RETURN @@ROWCOUNT
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Files]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Files](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ParentID] [int] NOT NULL,
	[FileName] [varchar](255) NULL,
	[ContentType] [varchar](255) NULL,
	[FileData] [image] NULL,
	[FileDataSize] [bigint] NULL,
	[update_date_stamp] [datetime] NOT NULL,
	[update_user_stamp] [varchar](40) NOT NULL,
	[create_date_stamp] [datetime] NOT NULL,
	[create_user_stamp] [varchar](40) NOT NULL,
	[update_seq_stamp] [int] NOT NULL,
 CONSTRAINT [PK_Files] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Folders]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Folders](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ParentID] [int] NOT NULL,
	[FolderName] [varchar](255) NOT NULL,
	[update_date_stamp] [datetime] NOT NULL,
	[update_user_stamp] [varchar](40) NOT NULL,
	[create_date_stamp] [datetime] NOT NULL,
	[create_user_stamp] [varchar](40) NOT NULL,
	[update_seq_stamp] [int] NOT NULL,
 CONSTRAINT [PK_Folders] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[up_Locks_Tokens_get]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[up_Locks_Tokens_get]
(
	@ID int
)
AS
BEGIN

	SET NOCOUNT ON
	
	SELECT
		[ID],
		[LockID],
		[Token],
		[update_date_stamp],
		[update_user_stamp],
		[create_date_stamp],
		[create_user_stamp],
		[update_seq_stamp]
	FROM [Locks_Tokens]
	WHERE
		([ID] = @ID)

	SET NOCOUNT OFF

	RETURN @@ROWCOUNT
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[up_Locks_Tokens_upd]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[up_Locks_Tokens_upd]
(
	@ID int,
	@LockID int,
	@Token varchar(255),
	@update_date_stamp datetime = NULL output,
	@update_user_stamp varchar(40) = NULL output,
	@create_date_stamp datetime = NULL output,
	@create_user_stamp varchar(40) = NULL output,
	@update_seq_stamp int = NULL output
)
AS
BEGIN

	DECLARE

	@keyvalues varchar(400),
	@updrowcount int,
	@keyrowcount int,
	@new_update_date_stamp datetime,
	@new_update_user_stamp varchar(40),
	@new_create_date_stamp datetime,
	@new_create_user_stamp varchar(40),
	@new_update_seq_stamp int
		
	SET NOCOUNT ON
	
	SET @new_update_date_stamp = getdate()
	SET @new_update_user_stamp = @update_user_stamp
	IF @update_seq_stamp = 2147483647 OR @update_seq_stamp IS NULL
		SET @new_update_seq_stamp = 1
	ELSE
		SET @new_update_seq_stamp = @update_seq_stamp + 1

	UPDATE [Locks_Tokens]
	SET
		[LockID] = @LockID,
		[Token] = @Token,
		[update_date_stamp] = @new_update_date_stamp,
		[update_user_stamp] = @new_update_user_stamp,
		[update_seq_stamp] = @new_update_seq_stamp
	WHERE
		[ID] = @ID
	AND 
		([update_seq_stamp] = @update_seq_stamp or @update_seq_stamp IS NULL)

	SET @updrowcount = @@ROWCOUNT

	IF @updrowcount = 0 BEGIN
		SET @keyvalues = ''ID:'' + convert(varchar,@ID)
		
		SET @keyrowcount = (SELECT count(*)
							FROM Locks_Tokens
							WHERE 		[ID] = @ID
)
						
		IF @keyrowcount = 0
			-- Record already gone
			RAISERROR(50001,16,1,@keyvalues,''Locks_Tokens'')
		ELSE
			-- Record already updated
			RAISERROR(50002,16,1,@keyvalues,''Locks_Tokens'')
		RETURN
	END
	
	SET @update_date_stamp = @new_update_date_stamp
	SET @update_user_stamp = @new_update_user_stamp
	SET @update_seq_stamp = @new_update_seq_stamp

	SET NOCOUNT OFF

	RETURN @updrowcount
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[up_Locks_Tokens_add]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[up_Locks_Tokens_add]
(
	@ID int = NULL output,
	@LockID int,
	@Token varchar(255),
	@update_date_stamp datetime = NULL output,
	@update_user_stamp varchar(40) = NULL output,
	@create_date_stamp datetime = NULL output,
	@create_user_stamp varchar(40) = NULL output,
	@update_seq_stamp int = NULL output
)
AS
BEGIN

	DECLARE 
	
	@Err int
	,@new_update_date_stamp datetime
	,@new_update_user_stamp varchar(40)
	,@new_create_date_stamp datetime
	,@new_create_user_stamp varchar(40)
	,@new_update_seq_stamp int
	
	SET NOCOUNT ON
	
	SET @new_update_date_stamp = getdate()
	SET @new_update_user_stamp = @update_user_stamp
	SET @new_create_date_stamp = @new_update_date_stamp
	SET @new_create_user_stamp = @new_update_user_stamp
	SET @new_update_seq_stamp = 1

	INSERT
	INTO [Locks_Tokens]
	(
		[LockID],
		[Token],
		[update_date_stamp],
		[update_user_stamp],
		[create_date_stamp],
		[create_user_stamp],
		[update_seq_stamp]
	)
	VALUES
	(
		@LockID,
		@Token,
		@new_update_date_stamp,
		@new_update_user_stamp,
		@new_create_date_stamp,
		@new_create_user_stamp,
		@new_update_seq_stamp
	)

	SET @ID = SCOPE_IDENTITY()
	SET @update_date_stamp = @new_update_date_stamp
	SET @update_user_stamp = @new_update_user_stamp
	SET @create_date_stamp = @new_create_date_stamp
	SET @create_user_stamp = @new_create_user_stamp
	SET @update_seq_stamp = @new_update_seq_stamp


	SET NOCOUNT OFF

	RETURN @@ROWCOUNT
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[up_Locks_Tokens_del]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[up_Locks_Tokens_del]
(
	@LockID int
	
)
AS
BEGIN

	DECLARE
	
		@keyvalues varchar(400),
		@delrowcount int,
		@keyrowcount int
	
	SET NOCOUNT ON

	DELETE
	FROM [Locks_Tokens]
	WHERE
		[LockID] = @LockID
	

	SET @delrowcount = @@ROWCOUNT
	
	

	RETURN @delrowcount
END


' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[up_Locks_get]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[up_Locks_get]
(
	@ID int
)
AS
BEGIN

	SET NOCOUNT ON
	
	SELECT
		[ID],
		[ResID],
		[LockType],
		[ResType],
		[LockScope],
		[LockDepth],
		[LockOwner],
		[LockOwnerType],
		[Timeout],
		[update_date_stamp],
		[update_user_stamp],
		[create_date_stamp],
		[create_user_stamp],
		[update_seq_stamp]
	FROM [Locks]
	WHERE
		([ID] = @ID)

	SET NOCOUNT OFF

	RETURN @@ROWCOUNT
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[up_Locks_add]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[up_Locks_add]
(
	@ID int = NULL output,
	@ResID int,
	@LockType int,
	@ResType int,
	@LockScope int,
	@LockDepth int,
	@LockOwner varchar(255),
	@LockOwnerType int,
	@Timeout int,
	@update_date_stamp datetime = NULL output,
	@update_user_stamp varchar(40) = NULL output,
	@create_date_stamp datetime = NULL output,
	@create_user_stamp varchar(40) = NULL output,
	@update_seq_stamp int = NULL output
)
AS
BEGIN

	DECLARE 
	
	@Err int
	,@new_update_date_stamp datetime
	,@new_update_user_stamp varchar(40)
	,@new_create_date_stamp datetime
	,@new_create_user_stamp varchar(40)
	,@new_update_seq_stamp int
	
	SET NOCOUNT ON
	
	SET @new_update_date_stamp = getdate()
	SET @new_update_user_stamp = @update_user_stamp
	SET @new_create_date_stamp = @new_update_date_stamp
	SET @new_create_user_stamp = @new_update_user_stamp
	SET @new_update_seq_stamp = 1

	INSERT
	INTO [Locks]
	(
		[ResID],
		[LockType],
		[ResType],
		[LockScope],
		[LockDepth],
		[LockOwner],
		[LockOwnerType],
		[Timeout],
		[update_date_stamp],
		[update_user_stamp],
		[create_date_stamp],
		[create_user_stamp],
		[update_seq_stamp]
	)
	VALUES
	(
		@ResID,
		@LockType,
		@ResType,
		@LockScope,
		@LockDepth,
		@LockOwner,
		@LockOwnerType,
		@Timeout,
		@new_update_date_stamp,
		@new_update_user_stamp,
		@new_create_date_stamp,
		@new_create_user_stamp,
		@new_update_seq_stamp
	)

	SET @ID = SCOPE_IDENTITY()
	SET @update_date_stamp = @new_update_date_stamp
	SET @update_user_stamp = @new_update_user_stamp
	SET @create_date_stamp = @new_create_date_stamp
	SET @create_user_stamp = @new_create_user_stamp
	SET @update_seq_stamp = @new_update_seq_stamp


	SET NOCOUNT OFF

	RETURN @@ROWCOUNT
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[up_Locks_del]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[up_Locks_del]
(
	@ID int
	
)
AS
BEGIN

	DECLARE
	
		@keyvalues varchar(400),
		@delrowcount int,
		@keyrowcount int
	
	SET NOCOUNT ON

	DELETE
	FROM [Locks]
	WHERE
		[ID] = @ID
	

	SET @delrowcount = @@ROWCOUNT
	
	IF @delrowcount = 0 BEGIN
		SET @keyvalues = ''ID:'' + convert(varchar,@ID)
		
		SET @keyrowcount = (SELECT count(*)
							FROM Locks
							WHERE 		[ID] = @ID)
						
		IF @keyrowcount = 0
			-- Record already gone
			RAISERROR(50001,16,1,@keyvalues,''Locks'')
		ELSE
			-- Record already updated
			RAISERROR(50002,16,1,@keyvalues,''Locks'')
		RETURN
	END
		
	SET NOCOUNT OFF

	RETURN @delrowcount
END

' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[up_Locks_upd]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[up_Locks_upd]
(
	@ID int,
	@ResID int,
	@LockType int,
	@ResType int,
	@LockScope int,
	@LockDepth int,
	@LockOwner varchar(255),
	@LockOwnerType int,
	@Timeout int,
	@update_date_stamp datetime = NULL output,
	@update_user_stamp varchar(40) = NULL output,
	@create_date_stamp datetime = NULL output,
	@create_user_stamp varchar(40) = NULL output,
	@update_seq_stamp int = NULL output
)
AS
BEGIN

	DECLARE

	@keyvalues varchar(400),
	@updrowcount int,
	@keyrowcount int,
	@new_update_date_stamp datetime,
	@new_update_user_stamp varchar(40),
	@new_create_date_stamp datetime,
	@new_create_user_stamp varchar(40),
	@new_update_seq_stamp int
		
	SET NOCOUNT ON
	
	SET @new_update_date_stamp = getdate()
	SET @new_update_user_stamp = @update_user_stamp
	IF @update_seq_stamp = 2147483647 OR @update_seq_stamp IS NULL
		SET @new_update_seq_stamp = 1
	ELSE
		SET @new_update_seq_stamp = @update_seq_stamp + 1

	UPDATE [Locks]
	SET
		[ResID] = @ResID,
		[LockType] = @LockType,
		[ResType] = @ResType,
		[LockScope] = @LockScope,
		[LockDepth] = @LockDepth,
		[LockOwner] = @LockOwner,
		[LockOwnerType] = @LockOwnerType,
		[Timeout] = @Timeout,
		[update_date_stamp] = @new_update_date_stamp,
		[update_user_stamp] = @new_update_user_stamp,
		[update_seq_stamp] = @new_update_seq_stamp
	WHERE
		[ID] = @ID
	AND 
		([update_seq_stamp] = @update_seq_stamp or @update_seq_stamp IS NULL)

	SET @updrowcount = @@ROWCOUNT

	IF @updrowcount = 0 BEGIN
		SET @keyvalues = ''ID:'' + convert(varchar,@ID)
		
		SET @keyrowcount = (SELECT count(*)
							FROM Locks
							WHERE 		[ID] = @ID
)
						
		IF @keyrowcount = 0
			-- Record already gone
			RAISERROR(50001,16,1,@keyvalues,''Locks'')
		ELSE
			-- Record already updated
			RAISERROR(50002,16,1,@keyvalues,''Locks'')
		RETURN
	END
	
	SET @update_date_stamp = @new_update_date_stamp
	SET @update_user_stamp = @new_update_user_stamp
	SET @update_seq_stamp = @new_update_seq_stamp

	SET NOCOUNT OFF

	RETURN @updrowcount
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[up_Access_get]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[up_Access_get]
(
	@ID int
)
AS
BEGIN

	SET NOCOUNT ON
	
	SELECT
		[ID],
		[FolderID],
		[UID],
		[Access],
		[update_date_stamp],
		[update_user_stamp],
		[create_date_stamp],
		[create_user_stamp],
		[update_seq_stamp]
	FROM [Access]
	WHERE
		([ID] = @ID)

	SET NOCOUNT OFF

	RETURN @@ROWCOUNT
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[up_Access_upd]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[up_Access_upd]
(
	@ID int,
	@FolderID int,
	@UID varchar(255) = NULL,
	@Access int,
	@update_date_stamp datetime = NULL output,
	@update_user_stamp varchar(40) = NULL output,
	@create_date_stamp datetime = NULL output,
	@create_user_stamp varchar(40) = NULL output,
	@update_seq_stamp int = NULL output
)
AS
BEGIN

	DECLARE

	@keyvalues varchar(400),
	@updrowcount int,
	@keyrowcount int,
	@new_update_date_stamp datetime,
	@new_update_user_stamp varchar(40),
	@new_create_date_stamp datetime,
	@new_create_user_stamp varchar(40),
	@new_update_seq_stamp int
		
	SET NOCOUNT ON
	
	SET @new_update_date_stamp = getdate()
	SET @new_update_user_stamp = @update_user_stamp
	IF @update_seq_stamp = 2147483647 OR @update_seq_stamp IS NULL
		SET @new_update_seq_stamp = 1
	ELSE
		SET @new_update_seq_stamp = @update_seq_stamp + 1

	UPDATE [Access]
	SET
		[FolderID] = @FolderID,
		[UID] = @UID,
		[Access] = @Access,
		[update_date_stamp] = @new_update_date_stamp,
		[update_user_stamp] = @new_update_user_stamp,
		[update_seq_stamp] = @new_update_seq_stamp
	WHERE
		[ID] = @ID
	AND 
		([update_seq_stamp] = @update_seq_stamp or @update_seq_stamp IS NULL)

	SET @updrowcount = @@ROWCOUNT

	IF @updrowcount = 0 BEGIN
		SET @keyvalues = ''ID:'' + convert(varchar,@ID)
		
		SET @keyrowcount = (SELECT count(*)
							FROM Access
							WHERE 		[ID] = @ID
)
						
		IF @keyrowcount = 0
			-- Record already gone
			RAISERROR(50001,16,1,@keyvalues,''Access'')
		ELSE
			-- Record already updated
			RAISERROR(50002,16,1,@keyvalues,''Access'')
		RETURN
	END
	
	SET @update_date_stamp = @new_update_date_stamp
	SET @update_user_stamp = @new_update_user_stamp
	SET @update_seq_stamp = @new_update_seq_stamp

	SET NOCOUNT OFF

	RETURN @updrowcount
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[up_Access_add]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[up_Access_add]
(
	@ID int = NULL output,
	@FolderID int,
	@UID varchar(255) = NULL,
	@Access int,
	@update_date_stamp datetime = NULL output,
	@update_user_stamp varchar(40) = NULL output,
	@create_date_stamp datetime = NULL output,
	@create_user_stamp varchar(40) = NULL output,
	@update_seq_stamp int = NULL output
)
AS
BEGIN

	DECLARE 
	
	@Err int
	,@new_update_date_stamp datetime
	,@new_update_user_stamp varchar(40)
	,@new_create_date_stamp datetime
	,@new_create_user_stamp varchar(40)
	,@new_update_seq_stamp int
	
	SET NOCOUNT ON
	
	SET @new_update_date_stamp = getdate()
	SET @new_update_user_stamp = @update_user_stamp
	SET @new_create_date_stamp = @new_update_date_stamp
	SET @new_create_user_stamp = @new_update_user_stamp
	SET @new_update_seq_stamp = 1

	INSERT
	INTO [Access]
	(
		[FolderID],
		[UID],
		[Access],
		[update_date_stamp],
		[update_user_stamp],
		[create_date_stamp],
		[create_user_stamp],
		[update_seq_stamp]
	)
	VALUES
	(
		@FolderID,
		@UID,
		@Access,
		@new_update_date_stamp,
		@new_update_user_stamp,
		@new_create_date_stamp,
		@new_create_user_stamp,
		@new_update_seq_stamp
	)

	SET @ID = SCOPE_IDENTITY()
	SET @update_date_stamp = @new_update_date_stamp
	SET @update_user_stamp = @new_update_user_stamp
	SET @create_date_stamp = @new_create_date_stamp
	SET @create_user_stamp = @new_create_user_stamp
	SET @update_seq_stamp = @new_update_seq_stamp


	SET NOCOUNT OFF

	RETURN @@ROWCOUNT
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[up_Access_del]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[up_Access_del]
(
	@ID int,
	@update_seq_stamp int = NULL
)
AS
BEGIN

	DECLARE
	
		@keyvalues varchar(400),
		@delrowcount int,
		@keyrowcount int
	
	SET NOCOUNT ON

	DELETE
	FROM [Access]
	WHERE
		[ID] = @ID
	AND 
		([update_seq_stamp] = @update_seq_stamp or @update_seq_stamp IS NULL)

	SET @delrowcount = @@ROWCOUNT
	
	IF @delrowcount = 0 BEGIN
		SET @keyvalues = ''ID:'' + convert(varchar,@ID)
		
		SET @keyrowcount = (SELECT count(*)
							FROM Access
							WHERE 		[ID] = @ID)
						
		IF @keyrowcount = 0
			-- Record already gone
			RAISERROR(50001,16,1,@keyvalues,''Access'')
		ELSE
			-- Record already updated
			RAISERROR(50002,16,1,@keyvalues,''Access'')
		RETURN
	END
		
	SET NOCOUNT OFF

	RETURN @delrowcount
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[up_Access_del_bulk]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[up_Access_del_bulk]
(
	@FolderID int
)
AS
BEGIN

	DECLARE
	
		@keyvalues varchar(400),
		@delrowcount int,
		@keyrowcount int
	
	SET NOCOUNT ON

	DELETE
	FROM [Access]
	WHERE
		[FolderID] = @FolderID
	
	SET @delrowcount = @@ROWCOUNT
	
	IF @delrowcount = 0 BEGIN
		SET @keyvalues = ''ID:'' + convert(varchar,@FolderID)
		
		SET @keyrowcount = (SELECT count(*)
							FROM Access
							WHERE 		[FolderID] = @FolderID)
						
		IF @keyrowcount = 0
			-- Record already gone
			RAISERROR(50001,16,1,@keyvalues,''Access'')
		ELSE
			-- Record already updated
			RAISERROR(50002,16,1,@keyvalues,''Access'')
		RETURN
	END
		
	SET NOCOUNT OFF

	RETURN @delrowcount
END

' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[up_Files_get_ByPath]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[up_Files_get_ByPath]
(
	@FileName varchar(255),
	@ParentID int
)
AS
BEGIN

	SET NOCOUNT ON
	
	SELECT
		[ID],
		[ParentID],
		[FileName],
		[ContentType],
		[FileData],
		[FileDataSize],
		[update_date_stamp],
		[update_user_stamp],
		[create_date_stamp],
		[create_user_stamp],
		[update_seq_stamp]
	FROM [Files]
	WHERE
		([FileName] = @FileName and [ParentID] = @ParentID)

	SET NOCOUNT OFF

	RETURN @@ROWCOUNT
END


' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[up_Files_get_ByPath_Att]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[up_Files_get_ByPath_Att]
(
	@FileName varchar(255),
	@ParentID int
)
AS
BEGIN

	SET NOCOUNT ON
	
	SELECT
		[ID],
		[ParentID],
		[FileName],
		[ContentType],
		[FileDataSize],
		[update_date_stamp],
		[update_user_stamp],
		[create_date_stamp],
		[create_user_stamp],
		[update_seq_stamp]
	FROM [Files]
	WHERE
		([FileName] = @FileName and [ParentID] = @ParentID)

	SET NOCOUNT OFF

	RETURN @@ROWCOUNT
END



' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[up_Files_get]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[up_Files_get]
(
	@ID int
)
AS
BEGIN

	SET NOCOUNT ON
	
	SELECT
		[ID],
		[ParentID],
		[FileName],
		[ContentType],
		[FileData],
		[FileDataSize],
		[update_date_stamp],
		[update_user_stamp],
		[create_date_stamp],
		[create_user_stamp],
		[update_seq_stamp]
	FROM [Files]
	WHERE
		([ID] = @ID)

	SET NOCOUNT OFF

	RETURN @@ROWCOUNT
END

' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[up_Files_upd]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[up_Files_upd]
(
	@ID int,
	@ParentID int,
	@FileName varchar(255) = NULL,
	@ContentType varchar(255) = NULL,
	@FileData image = NULL,
	@FileDataSize bigint = NULL,
	@update_date_stamp datetime = NULL output,
	@update_user_stamp varchar(40) = NULL output,
	@create_date_stamp datetime = NULL output,
	@create_user_stamp varchar(40) = NULL output,
	@update_seq_stamp int = NULL output
)
AS
BEGIN

	DECLARE

	@keyvalues varchar(400),
	@updrowcount int,
	@keyrowcount int,
	@new_update_date_stamp datetime,
	@new_update_user_stamp varchar(40),
	@new_create_date_stamp datetime,
	@new_create_user_stamp varchar(40),
	@new_update_seq_stamp int
		
	SET NOCOUNT ON
	
	SET @new_update_date_stamp = getdate()
	SET @new_update_user_stamp = @update_user_stamp
	IF @update_seq_stamp = 2147483647 OR @update_seq_stamp IS NULL
		SET @new_update_seq_stamp = 1
	ELSE
		SET @new_update_seq_stamp = @update_seq_stamp + 1

	UPDATE [Files]
	SET
		[ParentID] = @ParentID,
		[FileName] = @FileName,
		[ContentType] = @ContentType,
		[FileData] = @FileData,
		[FileDataSize] = @FileDataSize,
		[update_date_stamp] = @new_update_date_stamp,
		[update_user_stamp] = @new_update_user_stamp,
		[update_seq_stamp] = @new_update_seq_stamp
	WHERE
		[ID] = @ID
	AND 
		([update_seq_stamp] = @update_seq_stamp or @update_seq_stamp IS NULL)

	SET @updrowcount = @@ROWCOUNT

	IF @updrowcount = 0 BEGIN
		SET @keyvalues = ''ID:'' + convert(varchar,@ID)
		
		SET @keyrowcount = (SELECT count(*)
							FROM Files
							WHERE 		[ID] = @ID
)
						
		IF @keyrowcount = 0
			-- Record already gone
			RAISERROR(50001,16,1,@keyvalues,''Files'')
		ELSE
			-- Record already updated
			RAISERROR(50002,16,1,@keyvalues,''Files'')
		RETURN
	END
	
	SET @update_date_stamp = @new_update_date_stamp
	SET @update_user_stamp = @new_update_user_stamp
	SET @update_seq_stamp = @new_update_seq_stamp

	SET NOCOUNT OFF

	RETURN @updrowcount
END

' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[up_Files_add]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[up_Files_add]
(
	@ID int = NULL output,
	@ParentID int,
	@FileName varchar(255) = NULL,
	@ContentType varchar(255) = NULL,
	@FileData image = NULL,
	@FileDataSize bigint = NULL,
	@update_date_stamp datetime = NULL output,
	@update_user_stamp varchar(40) = NULL output,
	@create_date_stamp datetime = NULL output,
	@create_user_stamp varchar(40) = NULL output,
	@update_seq_stamp int = NULL output
)
AS
BEGIN

	DECLARE 
	
	@Err int
	,@new_update_date_stamp datetime
	,@new_update_user_stamp varchar(40)
	,@new_create_date_stamp datetime
	,@new_create_user_stamp varchar(40)
	,@new_update_seq_stamp int
	
	SET NOCOUNT ON
	
	SET @new_update_date_stamp = getdate()
	SET @new_update_user_stamp = @update_user_stamp
	SET @new_create_date_stamp = @new_update_date_stamp
	SET @new_create_user_stamp = @new_update_user_stamp
	SET @new_update_seq_stamp = 1

	INSERT
	INTO [Files]
	(
		[ParentID],
		[FileName],
		[ContentType],
		[FileData],
		[FileDataSize],
		[update_date_stamp],
		[update_user_stamp],
		[create_date_stamp],
		[create_user_stamp],
		[update_seq_stamp]
	)
	VALUES
	(
		@ParentID,
		@FileName,
		@ContentType,
		@FileData,
		@FileDataSize,
		@new_update_date_stamp,
		@new_update_user_stamp,
		@new_create_date_stamp,
		@new_create_user_stamp,
		@new_update_seq_stamp
	)

	SET @ID = SCOPE_IDENTITY()
	SET @update_date_stamp = @new_update_date_stamp
	SET @update_user_stamp = @new_update_user_stamp
	SET @create_date_stamp = @new_create_date_stamp
	SET @create_user_stamp = @new_create_user_stamp
	SET @update_seq_stamp = @new_update_seq_stamp


	SET NOCOUNT OFF

	RETURN @@ROWCOUNT
END

' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[up_Files_del]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[up_Files_del]
(
	@ID int
	
)
AS
BEGIN

	DECLARE
	
		@keyvalues varchar(400),
		@delrowcount int,
		@keyrowcount int
	
	SET NOCOUNT ON

	DELETE
	FROM [Files]
	WHERE
		[ID] = @ID
	

	SET @delrowcount = @@ROWCOUNT
	
	IF @delrowcount = 0 BEGIN
		SET @keyvalues = ''ID:'' + convert(varchar,@ID)
		
		SET @keyrowcount = (SELECT count(*)
							FROM Files
							WHERE 		[ID] = @ID)
						
		IF @keyrowcount = 0
			-- Record already gone
			RAISERROR(50001,16,1,@keyvalues,''Files'')
		ELSE
			-- Record already updated
			RAISERROR(50002,16,1,@keyvalues,''Files'')
		RETURN
	END
		
	SET NOCOUNT OFF

	RETURN @delrowcount
END

' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[up_Folders_get]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[up_Folders_get]
(
	@ID int
)
AS
BEGIN

	SET NOCOUNT ON
	
	SELECT
		[ID],
		[ParentID],
		[FolderName],
		[update_date_stamp],
		[update_user_stamp],
		[create_date_stamp],
		[create_user_stamp],
		[update_seq_stamp]
	FROM [Folders]
	WHERE
		([ID] = @ID)

	SET NOCOUNT OFF

	RETURN @@ROWCOUNT
END

' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[up_Folders_upd]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[up_Folders_upd]
(
	@ID int,
	@ParentID int,
	@FolderName varchar(255),
	@update_date_stamp datetime = NULL output,
	@update_user_stamp varchar(40) = NULL output,
	@create_date_stamp datetime = NULL output,
	@create_user_stamp varchar(40) = NULL output,
	@update_seq_stamp int = NULL output
)
AS
BEGIN

	DECLARE

	@keyvalues varchar(400),
	@updrowcount int,
	@keyrowcount int,
	@new_update_date_stamp datetime,
	@new_update_user_stamp varchar(40),
	@new_create_date_stamp datetime,
	@new_create_user_stamp varchar(40),
	@new_update_seq_stamp int
		
	SET NOCOUNT ON
	
	SET @new_update_date_stamp = getdate()
	SET @new_update_user_stamp = @update_user_stamp
	IF @update_seq_stamp = 2147483647 OR @update_seq_stamp IS NULL
		SET @new_update_seq_stamp = 1
	ELSE
		SET @new_update_seq_stamp = @update_seq_stamp + 1

	UPDATE [Folders]
	SET
		[ParentID] = @ParentID,
		[FolderName] = @FolderName,
		[update_date_stamp] = @new_update_date_stamp,
		[update_user_stamp] = @new_update_user_stamp,
		[update_seq_stamp] = @new_update_seq_stamp
	WHERE
		[ID] = @ID
	AND 
		([update_seq_stamp] = @update_seq_stamp or @update_seq_stamp IS NULL)

	SET @updrowcount = @@ROWCOUNT

	IF @updrowcount = 0 BEGIN
		SET @keyvalues = ''ID:'' + convert(varchar,@ID)
		
		SET @keyrowcount = (SELECT count(*)
							FROM Folders
							WHERE 		[ID] = @ID
)
						
		IF @keyrowcount = 0
			-- Record already gone
			RAISERROR(50001,16,1,@keyvalues,''Folders'')
		ELSE
			-- Record already updated
			RAISERROR(50002,16,1,@keyvalues,''Folders'')
		RETURN
	END
	
	SET @update_date_stamp = @new_update_date_stamp
	SET @update_user_stamp = @new_update_user_stamp
	SET @update_seq_stamp = @new_update_seq_stamp

	SET NOCOUNT OFF

	RETURN @updrowcount
END

' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[up_Folders_add]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[up_Folders_add]
(
	@ID int = NULL output,
	@ParentID int,
	@FolderName varchar(255),
	@update_date_stamp datetime = NULL output,
	@update_user_stamp varchar(40) = NULL output,
	@create_date_stamp datetime = NULL output,
	@create_user_stamp varchar(40) = NULL output,
	@update_seq_stamp int = NULL output
)
AS
BEGIN

	DECLARE 
	
	@Err int
	,@new_update_date_stamp datetime
	,@new_update_user_stamp varchar(40)
	,@new_create_date_stamp datetime
	,@new_create_user_stamp varchar(40)
	,@new_update_seq_stamp int
	
	SET NOCOUNT ON
	
	SET @new_update_date_stamp = getdate()
	SET @new_update_user_stamp = @update_user_stamp
	SET @new_create_date_stamp = @new_update_date_stamp
	SET @new_create_user_stamp = @new_update_user_stamp
	SET @new_update_seq_stamp = 1

	INSERT
	INTO [Folders]
	(
		[ParentID],
		[FolderName],
		[update_date_stamp],
		[update_user_stamp],
		[create_date_stamp],
		[create_user_stamp],
		[update_seq_stamp]
	)
	VALUES
	(
		@ParentID,
		@FolderName,
		@new_update_date_stamp,
		@new_update_user_stamp,
		@new_create_date_stamp,
		@new_create_user_stamp,
		@new_update_seq_stamp
	)

	SET @ID = SCOPE_IDENTITY()
	SET @update_date_stamp = @new_update_date_stamp
	SET @update_user_stamp = @new_update_user_stamp
	SET @create_date_stamp = @new_create_date_stamp
	SET @create_user_stamp = @new_create_user_stamp
	SET @update_seq_stamp = @new_update_seq_stamp


	SET NOCOUNT OFF

	RETURN @@ROWCOUNT
END

' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[up_Folders_del]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[up_Folders_del]
(
	@ID int
)	
AS
BEGIN

	DECLARE
	
		@keyvalues varchar(400),
		@delrowcount int,
		@keyrowcount int
	
	SET NOCOUNT ON

	DELETE
	FROM [Folders]
	WHERE
		[ID] = @ID
	

	SET @delrowcount = @@ROWCOUNT
	
	IF @delrowcount = 0 BEGIN
		SET @keyvalues = ''ID:'' + convert(varchar,@ID)
		
		SET @keyrowcount = (SELECT count(*)
							FROM Folders
							WHERE 		[ID] = @ID)
						
		IF @keyrowcount = 0
			-- Record already gone
			RAISERROR(50001,16,1,@keyvalues,''Folders'')
		ELSE
			-- Record already updated
			RAISERROR(50002,16,1,@keyvalues,''Folders'')
		RETURN
	END
		
	SET NOCOUNT OFF

	RETURN @delrowcount
END

' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[up_Folders_get_ByPath]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[up_Folders_get_ByPath]
(
	@FolderName varchar(255),
	@ParentID int
)
AS
BEGIN

	SET NOCOUNT ON
	
	SELECT
		[ID],
		[ParentID],
		[FolderName],
		[update_date_stamp],
		[update_user_stamp],
		[create_date_stamp],
		[create_user_stamp],
		[update_seq_stamp]
	FROM [Folders]
	WHERE
		([FolderName] = @FolderName and [ParentID] = @ParentID)

	SET NOCOUNT OFF

	RETURN @@ROWCOUNT
END


' 
END
GO


DECLARE	@return_value int,
		@ID int,
		@update_date_stamp datetime,
		@update_user_stamp varchar(40),
		@create_date_stamp datetime,
		@create_user_stamp varchar(40),
		@update_seq_stamp int

SET @update_date_stamp = getdate()

EXEC	@return_value = [dbo].[up_Folders_add]
		@ID = @ID OUTPUT,
		@ParentID=0,
		@FolderName=N'',
		@update_user_stamp = N'Build',
		@create_date_stamp = @create_date_stamp OUTPUT,
		@create_user_stamp = @create_user_stamp OUTPUT,
		@update_seq_stamp = @update_seq_stamp OUTPUT

SELECT	@ID as N'@ID',
		@update_date_stamp as N'@update_date_stamp',
		@update_user_stamp as N'@update_user_stamp',
		@create_date_stamp as N'@create_date_stamp',
		@create_user_stamp as N'@create_user_stamp',
		@update_seq_stamp as N'@update_seq_stamp'

SELECT	'Return Value' = @return_value

GO