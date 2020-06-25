@Echo off
setlocal
if NOT EXIST \\1FP0VF2\C$\EdmondsCC\Tools\ListFiles\bin\Debug\ListFiles.exe goto :BuildIt
@Echo ListFiles %*
\\1FP0VF2\C$\EdmondsCC\Tools\ListFiles\bin\Debug\ListFiles.exe %*
goto :EOF

:BuildIt
@Echo Build c:\EdmondsCC\Tools\ListFiles\bin\Debug\ListFiles.exe
@Echo Then try again.
goto :EOF
