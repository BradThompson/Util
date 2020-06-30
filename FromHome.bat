@Echo off
Setlocal
set ServerAndDrive=\\1FP0VF2\C$
set Password=%1
@Echo net use * %ServerAndDrive% /user:R230_admin\bthompson_admin
net use * %ServerAndDrive% /user:R230_admin\bthompson_admin %Password%
goto :EOF

:Usage
@Echo Usage:
@Echo 1FP0VF2 OptionalPassword
@Echo Example: 1FP0VF2 \\Rolaids\E$
goto :EOF
