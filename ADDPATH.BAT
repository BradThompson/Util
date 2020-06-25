@Echo off
if /I "%~1" EQU "/?" goto :Usage
if NOT EXIST %~dp0AddPathExe.exe (
    @Echo The file %~dp0AddPathExe.exe does not exist.
    goto :EOF
)
set TempBatch=%Temp%\AddPathExe.bat
if EXIST %TempBatch% del %TempBatch%
@Echo %~dp0AddPathExe.exe %*
call %~dp0AddPathExe.exe %*
if EXIST %TempBatch% call %TempBatch%
set TempBatch=
goto :EOF

:Usage
@Echo off
@Echo Utility for adding or subtracting from the PATH variable.
@Echo.
@Echo AddPath C:\Utility		- Adds C:\Utility to the end of the current PATH
@Echo AddPath /R C:\Utility		- Removes C:\Utility from PATH
@Echo AddPath /I## C:\Utility	- Inserts at specified location. 0 (Zero) would prepend.
@Echo AddPath /S C:\Utility		- Skips check to make sure the C:\Utility directory exists.
@Echo.
@Echo Limited testing is performed.
@Echo Changes only affect the current DOS instance.
@Echo.
goto :EOF
