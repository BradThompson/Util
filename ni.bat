@Echo off
setlocal
if "%~1%" EQU "" (
    C:\Util\n.bat
)
if NOT EXIST %~1 (
    C:\Util\n.bat %~1
)
if "%~d1" EQU "C:" (
    C:\Util\n.bat %~1
)
idir %~1 > c:\tmp\idir.tmp
for /f "delims=" %%x in (c:\tmp\idir.tmp) do set File=%%x
@Echo C:\Util\n.bat %File%>c:\tmp\idirtmp.bat
type c:\tmp\idirtmp.bat
c:\tmp\idirtmp.bat
