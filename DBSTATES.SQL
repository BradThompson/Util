set nocount on

declare @status         int
declare @DatabaseName   sysname
declare @p              varchar(255)
select @DatabaseName = 'LogShippingFSS1'

declare curWork CURSOR LOCAL READ_ONLY for
select name, status from master.dbo.sysdatabases
where name not in ('MSDB', 'master', 'model', 'Northwind', 'pubs', 'tempdb')

open curWork
fetch next from curWork into @DatabaseName, @Status
while @@fetch_status = 0
BEGIN
    select @p = @DatabaseName + ' has status: ' + convert(varchar(33), @status) 
    print @p
    if @status & 1 = 1
        print '    1 = autoclose; set with sp_dboption.'
    if @status & 2 = 2
        print '    2 = UNKNOWN STATUS'
    if @status & 4 = 4
        print '    4 = select into/bulkcopy; set with sp_dboption.'
    if @status & 8 = 8
        print '    8 = trunc. log on chkpt; set with sp_dboption.'
    if @status & 16 = 16
        print '    16 = torn page detection, set with sp_dboption.'
    if @status & 32 = 32
        print '    32 = loading.'
    if @status & 64 = 64
        print '    64 = pre recovery.'
    if @status & 128 = 128
        print '    128 = recovering.'
    if @status & 256 = 256
        print '    256 = not recovered.'
    if @status & 512 = 512
        print '    512 = offline; set with sp_dboption.'
    if @status & 1024 = 1024
        print '    1024 = read only; set with sp_dboption.'
    if @status & 2048 = 2048
        print '    2048 = dbo use only; set with sp_dboption.'
    if @status & 4096 = 4096
        print '    4096 = single user; set with sp_dboption.'
    if @status & 8092 = 8092
        print '    8092 = UNKNOWN STATUS'
    if @status & 16384 = 16384
        print '    16384 = UNKNOWN STATUS'
    if @status & 32768 = 32768
        print '    32768 = emergency mode.'
    if @status & 65536 = 65536
        print '    65536 = UNKNOWN STATUS'
    if @status & 131072 = 131072
        print '    131072 = UNKNOWN STATUS'
    if @status & 262144 = 262144
        print '    262144 = UNKNOWN STATUS'
    if @status & 524288 = 524288
        print '    524288 = UNKNOWN STATUS'
    if @status & 1048576 = 1048576
        print '    1048576 = UNKNOWN STATUS'
    if @status & 2097152 = 2097152
        print '    2097152 = Standby state.'
    if @status & 4194304 = 4194304
        print '    4194304 = autoshrink.'
    if @status & 8388608 = 8388608
        print '    8388608 = UNKNOWN STATUS'
    if @status & 16777216 = 16777216
        print '    16777216 = UNKNOWN STATUS'
    if @status & 33554432 = 33554432
        print '    33554432 = UNKNOWN STATUS'
    if @status & 67108864 = 67108864
        print '    67108864 = UNKNOWN STATUS'
    if @status & 134217728 = 134217728
        print '    134217728 = UNKNOWN STATUS'
    if @status & 268435456 = 268435456
        print '    268435456 = UNKNOWN STATUS'
    if @status & 536870912 = 536870912
        print '    536870912 = UNKNOWN STATUS'
    if @status & 1073741824 = 1073741824
        print '    1073741824 = cleanly shutdown.'
    fetch next from curWork into @DatabaseName, @Status
END

close curWork
deallocate curWork
go

