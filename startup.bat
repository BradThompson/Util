@echo off
pushd
set Util=%~dp0

call :CheckOnGoBat
call :CheckAndFixPath
if ERRORLEVEL == 1 goto :Done
doskey /INSERT
set copycmd=/y
set dircmd=/ogn
set BBUNPACKDEFCMD=-w
Title DOS
if /I "%~1" EQU "white" (
    Color f0
    goto :NextStep
)
if /I "%~1" EQU "blue" (
    Color 1f
    goto :NextStep
)
if /I "%~1" EQU "red" (
    Color 4f
    goto :NextStep
)
if /I "%~1" EQU "green" (
    Color 0a
    goto :NextStep
)
if /I "%~1" EQU "purple" (
    Color 5f
    goto :NextStep
)
REM Default to white on black
Color 0f

:NextStep
set Tmp=C:\Tmp
if EXIST %Tmp% goto :TmpExists
md %Tmp% 1>NUL 2>NUL
if NOT EXIST %Tmp% (
    @Echo Could not create %Tmp% directory.
    goto :SomethingIsWrong
)
:TmpExists
set Temp=C:\Temp
if EXIST %Temp% goto :Done
md %Temp% 1>NUL 2>NUL
if NOT EXIST %Temp% (
    @Echo Could not create %Temp% directory.
    goto :SomethingIsWrong
)
@Echo.
@Echo Good color choices are: 1f 0f 4f 2f f0 09 0c 1e
@Echo.
goto :Done

:CheckAndFixPath
if NOT EXIST %Util%Startup.bat (
    %Util%Startup.bat is missing.
    goto :SomethingIsWrong
)
if NOT EXIST %Util%AddPath.bat (
    %Util%AddPath.bat is missing.
    goto :SomethingIsWrong
)
call %Util%AddPath.bat %Util% 1>NUL 2>NUL
if EXIST C:\EdmondsCC\bin\tools.bat call %Util%AddPath.bat C:\EdmondsCC\bin 1>NUL 2>NUL
REM If Git is installed, use the latest and greatest toolset.
if NOT EXIST "C:\Program Files\Git\usr\bin" (
    goto :AddGitUsrBin
) else (
    call %Util%AddPath.bat "C:\Program Files\Git\usr\bin" 1>NUL 2>NUL
)
exit /B 0

REM Alternatively, use the saved tools. We should update these occasionaly.
:AddGitUsrBin
if NOT EXIST %Util%GitUsrBin (
    @Echo Could not find Git tools. Ignoring Git.
) else (
    call %Util%AddPath.bat %Util%GitUsrBin 1>NUL 2>NUL
)
REM If Git isn't available at all. It's not the end of the world. Just ignore.
exit /B 0

:SomethingIsWrong
@Echo off
@Echo.
@Echo Something is wrong. Cannot continue.
@Echo.
exit /B 1

:Done
if /I "%COMPUTERNAME%" EQU "1FP0VF2" (
    @Echo Cleaning up net connections on 1FP0VF2
    net use R: /D 1>NUL 2>NUL
    net use U: /D 1>NUL 2>NUL
    if EXIST W:\DoNotWantThis.txt net use W: /D 1>NUL 2>NUL
)

for %%c in ( Rolaids, Tums, Odie, Lalinea, Casper2, BetaSql12, BetaStaging12, BetaCF, BetaDotNet ) do call :dd %%c
goto :ReallyDone

:dd
if /I "%COMPUTERNAME%" EQU "%1" (
    @Echo Cleaning up net connections on %1
    net use U: /D 1>NUL 2>NUL
    if EXIST W:\DoNotWantThis.txt net use W: /D 1>NUL 2>NUL
    if NOT EXIST R:\GitRoot.txt (
        @Echo Connecting R: to 1FP0VF2 to be able to use Util directory.
        net use R: /D 1>NUL 2>NUL
        net use R: \\1FP0VF2\C$\EdmondsCC 1>NUL 2>NUL
    )
)
goto :EOF

:CheckOnGoBat
CALL :GetFilesize RemoteFileSize \\1FP0VF2\C$\Util\go.bat
CALL :GetFilesize LocalFileSize go.bat
@Echo Check Local
if NOT DEFINED LocalFileSize exit /b 0
if "%LocalFileSize%" EQU 0 exit /b 0
if NOT DEFINED RemoteFileSize exit /b 0
if "%RemoteFileSize%" EQU 0 exit /b 0
if %RemoteFilesize% EQU %LocalFilesize% exit /b 0
@Echo Go.bat is different. Copying...
Copy %Util%\go.bat go.bat 1>NUL 2>NUL
exit /b 0

:GetFilesize
set %1=%~z2
goto :EOF

:ReallyDone
set Util=
Set LocalFileSize=
Set RemoteFileSize=
popd
if EXIST C:\EdmondsCC\Tools\Batch\GetGitRoot.bat call C:\EdmondsCC\Tools\Batch\GetGitRoot.bat
cd /D %USERPROFILE%
