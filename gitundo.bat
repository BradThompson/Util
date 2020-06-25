@Echo off
setlocal
if NOT EXIST "%~1" goto :Usage
@Echo on
call git reset HEAD "%~1"
call git checkout "%~1"
goto :EOF

:Usage
@Echo Specify a file.
@Echo.
@Echo Maybe a directory or wild card works, too. Give it a try.
goto :EOF

