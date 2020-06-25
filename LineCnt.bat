@Echo off
setlocal
if "%~1" EQU "" goto Usage
if "%~2" NEQ "" goto Usage
if NOT EXIST "%~1" goto MissingFile
set LineCount=0
call :CountEm %1
@Echo LineCount: %LineCount%
exit /b%LineCount%
goto :EOF

:CountEm
FOR /F %%V IN (%~1) DO call :AddOne
goto :EOF

:AddOne
set /A LineCount=%LineCount% + 1
goto :EOF

:MissingFile
@Echo off
@Echo.
@Echo The file %~1 does not exist.
@Echo.
exit /b-1

:Usage
@Echo off
@Echo.
@Echo Usage: %0 Myfile.ext
@Echo.
exit /b-1
