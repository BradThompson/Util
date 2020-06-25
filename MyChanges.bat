@Echo off
setlocal
set Work1=%tmp%\MyChanges.txt
set Work2=%tmp%\MyChange.txt
set Final=%tmp%\Final.txt
@Echo All Source Depot changes>%Final%
@Echo.>>%Final%

call sd changes -u REDMOND\v-brth > %Work1%
FOR /F "tokens=2" %%c IN (%Work1%) DO call :DoIt %%c
call notepad.exe %Final%
goto :EOF

:DoIt
call sd change -o %1 > %Work2%
type %Work2% >> %Final%
@Echo. >> %Final%
@Echo ============================================================================================================= >> %Final%
@Echo. >> %Final%
goto :EOF