@echo off
setlocal
"C:\Program Files\Microsoft SQL Server\110\COM\tablediff.exe" -sourceserver 1FP0VF2 -destinationserver 1FP0VF2 -sourcedatabase DeployUnitTest -destinationdatabase DeployUnitTest -sourcetable BcpTestTable -destinationtable BcpTestTableEmpty -c -et DiffError -dt
if ERRORLEVEL 10 goto :10
if ERRORLEVEL 9 goto :9
if ERRORLEVEL 8 goto :8
if ERRORLEVEL 7 goto :7
if ERRORLEVEL 6 goto :6
if ERRORLEVEL 5 goto :5
if ERRORLEVEL 4 goto :4
if ERRORLEVEL 3 goto :3
if ERRORLEVEL 2 goto :2
if ERRORLEVEL 1 goto :1
if ERRORLEVEL 0 goto :0
@Echo nothing
goto :EOF

:10
echo ERRORLEVEL: 10
echo ERRORLEVEL: %ERRORLEVEL%
goto :EOF

:9
echo ERRORLEVEL: 9
echo ERRORLEVEL: %ERRORLEVEL%
goto :EOF

:8
echo ERRORLEVEL: 8
echo ERRORLEVEL: %ERRORLEVEL%
goto :EOF

:7
echo ERRORLEVEL: 7
echo ERRORLEVEL: %ERRORLEVEL%
goto :EOF

:6
echo ERRORLEVEL: 6
echo ERRORLEVEL: %ERRORLEVEL%
goto :EOF

:5
echo ERRORLEVEL: 5
echo ERRORLEVEL: %ERRORLEVEL%
goto :EOF

:4
echo ERRORLEVEL: 4
echo ERRORLEVEL: %ERRORLEVEL%
goto :EOF

:3
echo ERRORLEVEL: 3
echo ERRORLEVEL: %ERRORLEVEL%
goto :EOF

:2
echo ERRORLEVEL: 2
echo ERRORLEVEL: %ERRORLEVEL%
goto :EOF

:1
echo ERRORLEVEL: 1
echo ERRORLEVEL: %ERRORLEVEL%
goto :EOF

:0
echo ERRORLEVEL: 0
echo ERRORLEVEL: %ERRORLEVEL%
goto :EOF

