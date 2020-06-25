@Echo off
setlocal

set KillingDataflow=
if "%1" EQU "/f" goto :OkGo
choice /M "This kills all Visual Studio instances and service command prompts. Have you saved your files"
if ERRORLEVEL == 2 goto :Dont
if ERRORLEVEL == 1 goto :OkGo
goto :EOF

:OkGo
set TmpFile=%Tmp%\KillVS.tmp
tlist -c > %TmpFile%
c:\util\ReplaceInFiles.exe /f:%TmpFile% /s:"\\" /r:" " /go 1>NUL 2>NUL
for /F "tokens=1,2,3,4,5" %%c in (%TmpFile%) do call :dd %%c %%d %%e %%f %%g
if DEFINED KillingDataflow call killvs /f
goto :EOF

:dd
if "%2" EQU "devenv.exe" (
	@Echo PID: %1, process name: %2
    kill -f %1
)
if "%2" EQU "node.exe" goto :GotNode
:AfterNode
if "%2" EQU "cmd.exe" goto :GotCmd
:AfterCmd
goto :EOF

:GotNode
if "%3" EQU "Dataflow" (
    @Echo PID: %1, process name: %3
    kill -f %1
)
goto :AfterNode

:GotCmd
if "%5" EQU "IdentityService" (
    @Echo Have to kill iisexpress before we can kill the cmd window.
    for /F "tokens=1,2" %%x in (%TmpFile%) do call :KillIIExpress %%x %%y
    @Echo PID: %1, process name: %5
    kill -f %1
)
if "%3" EQU "Dataflow" (
    set KillingDataflow=true
    @Echo PID: %1, process name: %3
    kill -f %1
)
goto :AfterCmd

:KillIIExpress
if "%2" EQU "iisexpress.exe" (
    @Echo PID: %1, process name: %2
    kill -f %1
)
goto :EOF

:Dont
@Echo OK, not killing
goto :EOF
