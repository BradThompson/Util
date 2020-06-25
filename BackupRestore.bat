@Echo off
setlocal
if NOT EXIST \\1FP0VF2\C$\EdmondsCC\Tools\BackupRestore\bin\Debug\BackupRestore.exe goto :BuildIt
@Echo BackupRestore %*
\\1FP0VF2\C$\EdmondsCC\Tools\BackupRestore\bin\Debug\BackupRestore.exe %*
goto :EOF

:BuildIt
@Echo Build c:\EdmondsCC\Tools\BackupRestore\bin\Debug\BackupRestore.exe
@Echo Then try again.
goto :EOF
