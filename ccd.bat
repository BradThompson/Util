@Echo off
if /I "%~1" EQU "/?" goto :Usage
if NOT EXIST %~dp0CCDExe.exe (
    @Echo The file %~dp0CCDExe.exe does not exist.
    goto :EOF
)
set TempBatch=%Temp%\ccdbat.bat
if EXIST %TempBatch% del %TempBatch%
call %~dp0CCDExe.exe %*
if EXIST %TempBatch% call %TempBatch%
set TempBatch=
goto :EOF

:Usage
@Echo off
@Echo Enhancement of the CD command.
@Echo.
@Echo ccd regular\style\path     - Dos path
@Echo ccd unix/style/path        - Unix paths
@Echo ccd this/is/a\filename.txt - Finds parent directory of file
@Echo ccd this/PartialFileNa     - Finds parent directory
@Echo Special characters OK and no need for quotes even for single spaces:
@Echo ccd \Program Files (x86)\Microsoft Office
@Echo Changing drives is automatic. From C:\Users, you can use ccd d:\Mine
@Echo.
goto :EOF
