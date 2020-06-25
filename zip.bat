@Echo off
setlocal
@Echo Example adds:
@Echo zip a My.zip *.sql
@Echo zip a My.zip File1.txt File2.txt
set EXE="C:\Program Files\7-Zip\7z.exe"
if NOT EXIST %EXE% goto :InstallSevenZip
@Echo %EXE% %*
%EXE% %*
goto :EOF

:InstallSevenZip
@Echo.
@Echo Install Seven Zip: https://www.7-zip.org/
@Echo.
goto :EOF
