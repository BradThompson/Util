@Echo off
setlocal EnableDelayedExpansion
call :Initialize
if "%~1" EQU "/?" goto :Usage   &REM Dos Batch will use this for its own usage, so we have to use it first.
call :ProcessCommandLine %*
if ERRORLEVEL 0 (

REM Put your code here.

    call :ListFlags
	goto :EOF
)
@Echo ERRORLEVEL: %ERRORLEVEL%
@Echo ========================================================================
goto :Usage

:ListFlags
set ListCount=1
:ListFlagsContinued
set Work=!%ListCount%Name!
if "!%Work%Value!" NEQ "" @Echo %Work% = !%Work%Value!
if %ListCount% EQU %SwitchCount% (
    if DEFINED Value @Echo Value: %Value%
    goto :EOF
)
set /A ListCount=%ListCount% + 1
goto :ListFlagsContinued

:Initialize
REM Types are:
REM     Flag  - doesn't have a value such as        "MyBatch /?"
REM     Value - has a value in the next parameter:  "MyBatch /F 999"
REM Switches must be a single character.
set SwitchCount=0
call :InitSwitch ? 		Flag    "-?     - Works with -?. The check for /? is explicit"
call :InitSwitch F 		Flag    "-F     - Follow up on testing"
call :InitSwitch FOut 	Flag    "-FOut  - A variation on Follow up"
call :InitSwitch Win    Value   "-Win   - Its a win/win scenario"
call :InitSwitch Test   Value   "-Test  - Of course we need testing"
call :InitSwitch V      Flag    "-V     - Verbose, of course"
call :InitSwitch -help  Flag    "--help - More help!"
set Value=
set TempFile=%Tmp%\ExampleSwitch.tmp
goto :EOF

:InitSwitch
set /A SwitchCount=%SwitchCount% + 1
set Usage%SwitchCount%=%~3
set %1Value=
set %1Type=%2
set %SwitchCount%Name=%1
goto :EOF

:Usage
@Echo.
@Echo ------------ Usage ------------
@Echo.
set UsageCount=1
:UsageContinued
@Echo !Usage%UsageCount%!
if %UsageCount% EQU %SwitchCount% (
    @Echo Free form value is also allowed.
    @Echo.
    @Echo Switches are either - or /
    @Echo Examples:
    @Echo %0 /f /Test -v "My value"
    @Echo %0 --help
    @Echo %0 Something
    @Echo.
    goto :EOF
)
set /A UsageCount=%UsageCount% + 1
goto :UsageContinued

REM ===================== Parameter processing below ====================

:ProcessCommandLine
Call :FindSwitch "%~1" "%~2"
if ERRORLEVEL 2 shift & shift & goto :ProcessCommandLine
if ERRORLEVEL 1 shift & goto :ProcessCommandLine
if ERRORLEVEL 0 exit /b 0
exit /b -1

:FindSwitch
call :IsSwitch "%~1"
if ERRORLEVEL 2 goto :FoundSwitch
if ERRORLEVEL 1 goto :NotASwitch
if ERRORLEVEL 0 exit /b 0
exit /b -1

:FoundSwitch
if /I "!%SwitchName%Type!" EQU "" (
    @Echo Not a valid switch
    exit /b -1
)
if /I "!%SwitchName%Type!" EQU "FLAG" (
    if /I "!%SwitchName%Value!" NEQ "" @Echo Switch was previously set & exit /b -1
	set %SwitchName%Value=True
	exit /b 1
)
if /I "!%SwitchName%Type!" EQU "VALUE" (
    if /I "!%SwitchName%Value!" NEQ "" @Echo Switch was previously set & exit /b -1
	set %SwitchName%Value=%~2
	exit /b 2
)
exit /b 1

:NotASwitch
if DEFINED Value @Echo Value cannot be used twice & exit /b -1
set Value=%~1
exit /b 1

:IsSwitch
set SwitchName=
if /I "%~1" EQU "" exit /b 0
set Work=%~1
if /I "%Work:~0,1%" EQU "/" goto :CouldBeASwitch
if /I "%Work:~0,1%" EQU "-" goto :CouldBeASwitch
exit /b 1
:CouldBeASwitch
if /I "%Work:~1,1%" EQU "" exit /b -1
set SwitchName=%Work:~1%
exit /b 2
