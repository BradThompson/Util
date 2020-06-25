@Echo off
setlocal

REM net use o: \\rolaids\E$
REM CD /D \\rolaids\E$\_Exports\Rave\History
REM dir /b /od
dir /b /od \\rolaids\E$\_Exports\Rave\History > %TEMP%\LatestHistory.tmp
tail -5 < %TEMP%\LatestHistory.tmp > %TEMP%\LatestHistory2.tmp
type %TEMP%\LatestHistory2.tmp
for /F "skip=2" %%c in ( %TEMP%\LatestHistory2.tmp ) do call :GetHistory %%c
goto :EOF

:GetHistory
@Echo -------------------
dir \\rolaids\E$\_Exports\Rave\History\%1 > 
@Echo -------------------
goto :EOF
:GetHistory
@Echo -------------------
@Echo %1
REM dir \\rolaids\E$\_Exports\Rave\History\%1
@Echo -------------------
goto :EOF
