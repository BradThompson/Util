@Echo off
setlocal
if '%1' == '' goto Usage
osql -S %1 -d master -w 4096 -n -E -i %~d0%~p0DBStates.sql
goto :EOF

:Usage
Echo.
Echo Usage: DBStatus ServerName
Echo.
