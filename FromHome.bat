@Echo off
Setlocal
if /I "%~1" NEQ "" goto :Custom
set ServerAndDrive=\\1FP0VF2\C$
goto :Default

:Custom
set ServerAndDrive=%1
goto :Default

:Default
@Echo net use * %ServerAndDrive% /user:R230_admin\bthompson_admin
net use * %ServerAndDrive% /user:R230_admin\bthompson_admin
goto :EOF

:Usage
@Echo Usage:
@Echo 1FP0VF2 [Server and drive]
@Echo Example: 1FP0VF2 \\Rolaids\E$
goto :EOF
