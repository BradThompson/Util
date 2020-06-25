use DbaAdmin
go

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[uspTestParameters]') AND type in (N'P', N'PC'))
BEGIN
    EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[uspTestParameters] AS' 
END
go

ALTER Proc [dbo].[uspTestParameters]
    @RequiredParameter int,
    @OptionalParameter varchar(32) = null
AS
BEGIN
    if @OptionalParameter IS NOT NULL and @OptionalParameter = 'OneResult'
    BEGIN
        select @RequiredParameter AS [RequiredParameter], @OptionalParameter AS [OptionalParameter], 42
    END
    ELSE
    BEGIN
        Declare @NumberOfRows int
        Declare @ReturnedVarChar varchar(255)
        select @NumberOfRows = 1
        Declare @Test TABLE
        (
            ReturnedInt     int         NOT NULL,
            ReturnedVarChar varchar(32) NULL
        )
        WHILE (@NumberOfRows <= @RequiredParameter)
        BEGIN
            insert @Test (ReturnedInt, ReturnedVarChar) VALUES (@NumberOfRows, @ReturnedVarChar)
            if @ReturnedVarChar IS NULL
            BEGIN
                set @ReturnedVarChar = 'NOT NULL';
            END
            ELSE
            BEGIN
                set @ReturnedVarChar = 'Nothing much';
            END
            set @NumberOfRows = @NumberOfRows + 1;
        END
        select * from @Test
    END
END
go

exec uspTestParameters 0
exec uspTestParameters 1
exec uspTestParameters 2
exec uspTestParameters 3, 'Ignore'
exec uspTestParameters 3, 'OneResult'
