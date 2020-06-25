@Echo off
setlocal
if "%~1" EQU "" goto :Usage
sqlcmd -d master -Q "select substring(name, 1, 40) from sys.databases" -E -S %1
goto :EOF

:Usage
@Echo.
@Echo Usage: testsql ServerName
@Echo.
goto :EOF
