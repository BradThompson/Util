@echo off
if "%TEMP%" == "" goto ErrorTemp
if /I "%~1" EQU "" goto :Usage
if /I "%~1" EQU "New" goto :NewestDirectory
if /I "%~1" EQU "Browse" goto :Browse
Set IsSpecific=
for %%c in (Apollo Tums Rolaids Odie BetaSql12 BetaStaging12 Lalinea Casper Transfer PDrive RDrive Blackboard Edcap RSO SqlBackup DataPump TestLogs EdPass BetaDotNet BetaCF PTums PRolaids PLalinea PCasper Rave) do call :CheckSpecific "%~1" %%c
if DEFINED IsSpecific (
    Set IsSpecific=
    goto :Specific
)
if /I "%~1" EQU "?" goto :Usage
if /I "%~1" EQU "/?" goto :Usage
if /I "%~1" EQU "-?" goto :Usage

if exist %TEMP%\tobat.bat del %TEMP%\tobat.bat
%~dp0\toexe.exe %1 %2 %3 %4 %5 %6 %7 %8
goto :CheckForToBat

:CheckSpecific
if /I "%~1" EQU "%~2" Set IsSpecific=Yes
goto :EOF

:NewestDirectory
setlocal
set Last=
dir /b /od /ad > %TEMP%\toNewest.tmp
for /F "tokens=1,2,3,4,5" %%c in (%TEMP%\toNewest.tmp) do call :NewestWork %%c %%d %%e %%f %%g
Echo cd "%Last%"
if DEFINED Last Echo cd "%Last%" > %TEMP%\tobat.bat
endlocal
goto :CheckForToBat

:NewestWork
REM Only works with one sequential space.
if "%~1" EQU "" goto :EOF
if "%~5" NEQ "" goto Five
if "%~4" NEQ "" goto Four
if "%~3" NEQ "" goto Three
if "%~2" NEQ "" goto Two
set Last=%~1
goto :EOF
:Two
set Last=%1 %2
goto :EOF
:Three
set Last=%1 %2 %3
goto :EOF
:Four
set Last=%1 %2 %3 %4
goto :EOF
:Five
set Last=%1 %2 %3 %4 %5
goto :EOF

:Browse
if /I "%~2" EQU "" (
    @Echo.
    @Echo For Browse, specify the name of the server as the second parameter:
    @Echo Example: to.bat Browse Casper2
    @Echo.
    exit /b 1
)
Set Server=%~2
SET FileLength=
for %%c in ( c d e f g h i j k l m n o p q r s t u v w x y z ) do (
    net use \\%Server%\%%c$ 1> NUL 2> C:\Tmp\ToBrowse.tmp
    call :GetFileLength C:\Tmp\ToBrowse.tmp %%c
    net use \\%Server%\%%c$ /d 1>NUL 2>&1
)
del C:\Tmp\ToBrowse.tmp
exit /b 0

:GetFileLength
SET FileLength=%~z1
if "%FileLength%" EQU "0" (
    @Echo %2$ - Good
) else (
    @Echo %2$ - Bad
)
goto :EOF

:Specific
C:
SETLOCAL
Set Server=%~1

Set Share=E$
Set Persist=
if /I "%Server%" EQU "RDrive" (
    net use R: /DELETE 1>NUL 2>NUL
    @Echo net use R: \\edcc.ctc.edu\shares\RDrive
    net use R: \\edcc.ctc.edu\shares\RDrive
    if ERRORLEVEL == 1 (
        exit /b 1
    )
    @Echo CD /D R:\Ads\Brad > %TEMP%\tobat.bat
    ENDLOCAL
    goto :CheckForToBat
)
if /I "%Server%" EQU "edcap" (
    Set Server=edcc.ctc.edu\shares
    Set Share=EDCAP$
)
if /I "%Server%" EQU "RSO" (
    Set Server=edcc.ctc.edu\shares
    Set Share=RSO$
)
if /I "%Server%" EQU "SqlBackup" (
    Set Server=edcc.ctc.edu\shares
    Set Share=sql$
)
if /I "%Server%" EQU "DataPump" (
    Set Server=datapump.edcc.ctc.edu\starfishdata\test
    Set Share=ODSdata
)
if /I "%Server%" EQU "TestLogs" (
    Set Server=datapump.edcc.edu
    Set Share=testlogs
)
if /I "%Server%" EQU "EdPass" (
    Set Server=transfer01
    Set Share=blackboard
)
if /I "%Server%" EQU "Apollo" (
    if /I "%COMPUTERNAME%" NEQ "Odie" (
        @Echo.
        @Echo Apollo can only be accessed using Remote to Odie
        @Echo.
        goto :EOF
    )
)
if /I "%Server%" EQU "Transfer" (
    Set Server=transfer01
    Set Share=Transfer
)
if /I "%Server%" EQU "Rave" (
    Set Server=Rolaids
    Set Share=E$\_Exports
    dir \\Rolaids\e$\_Exports\rave-people.csv
)
if /I "%Server%" EQU "PDrive" (
    Set LocalDriveLetter=P:
    Set Server=Transfer01
    Set Share=Transfer
    @Echo ---------------- Details ----------------
    @Echo P:\5.0 Departments Other\HR\FT Faculty Contract Template.docx
    @Echo \\Transfer01\Transfer\5.0 Departments Other\HR\FT Faculty Contract Template.docx
    @Echo \\Transfer01\Transfer\5.0 Departments Other\HR\HR_FacultyContracts\HR_ContractAdministrator_Current.accdb
    @Echo ---------------- Details ----------------
    @Echo.
)
if /I "%Server%" EQU "Lalinea" set Share=C$
if /I "%Server%" EQU "BetaDotNet" set Share=C$\inetpub\wwwroot
if /I "%Server%" EQU "BetaCF" set Share=E$\rootwww\cfusion\wwwroot
if /I "%Server%" EQU "Casper" Set Server=Casper2&set Share=E$\rootwww\ColdFusion10\cfusion\wwwroot
if /I "%Server%" EQU "PCasper" Set Server=POC-Casper2&set Share=E$\rootwww\ColdFusion10\cfusion\wwwroot
if /I "%Server%" EQU "PRolaids" Set Server=POC-Rolaids
if /I "%Server%" EQU "PTums" Set Server=POC-Tums
if /I "%Server%" EQU "PLalinea" Set Server=POC-Lalinea
if /I "%Server%" EQU "BlackBoard" (
    set Server=Tums
    set Share=Blackboard
)
if /I "%~2" NEQ "" Set Share=%~2
if /I "%LocalDriveLetter%" NEQ "" goto :GotLocal
Set LocalDriveLetter=*
if /I "%~3" NEQ "" Set LocalDriveLetter=%~3
:GotLocal
@Echo net use %LocalDriveLetter% \\%Server%\%Share%
net use %LocalDriveLetter% \\%Server%\%Share%
if ERRORLEVEL == 1 goto :ConnectFailed

ENDLOCAL
goto :CheckForToBat

:CheckForToBat
if NOT exist %TEMP%\tobat.bat exit /b 1
@Echo.>>%TEMP%\tobat.bat
@Echo Title %%CD%%>>%TEMP%\tobat.bat
call %TEMP%\tobat.bat
exit /b 0

:ErrorTemp
@Echo.
@Echo ERROR: TEMP environment variable must be set to a valid directory.
exit /b1

:ConnectFailed
@Echo.
@Echo ERROR: Could not connect drive.
exit /b1

:Usage
@Echo.
@Echo Usage:
@Echo.
@Echo Uses the TwoExe.EXE utility. If you want specific help for TwoExe, use TwoExe /? or TwoExe /??
@Echo This has been extended to also do the following:
@Echo.
@Echo New - Finds the newest directory in the current directory and CDs to it.
@Echo.
@Echo Browse Server  - Finds available remote drives on server
@Echo.
@Echo If you use any of the following, usage is slightly different:
@Echo.
@Echo     BetaCF    - \\BetaCF\E$\rootwww\cfusion\wwwroot
@Echo     BetaDotNet- \\BetaDotNet\C$\inetpub\wwwroot
@Echo     BetaSql12 - \\BetaSql12\E$
@Echo     Blackboard- \\Tums\Blackboard
@Echo     Casper    - \\Casper2\E$\rootwww\ColdFusion10\cfusion\wwwroot
@Echo     DataPump  - \\datapump.edcc.ctc.edu\starfishdata\test\ODSdata
@Echo     EdCap     - \\edcc.ctc.edu\shares\EDCAP$
@Echo     EdPass    - \\transfer01\blackboard
@Echo     Lalinea   - \\Lalinea\C$
@Echo     Odie      - \\Odie\E$
@Echo     PCasper   - \\POC-Casper2\E$\rootwww\ColdFusion10\cfusion\wwwroot
@Echo     PDrive    - \\Transfer01\P$
@Echo     PLalinea  - \\POC-Lalinea\C$
@Echo     PRolaids  - \\POC-Rolaids\E$
@Echo     PTums     - \\POC-Tums\E$
@Echo     Rave      - \\Rolaids\e$\_Exports
@Echo     RDrive    - \\edcc.ctc.edu\shares\RDrive
@Echo     Rolaids   - \\Rolaids\E$
@Echo     RSO       - \\edcc.ctc.edu\shares\RSO$
@Echo     SqlBackup - \\edcc.ctc.edu\shares\sql$
@Echo     TestLogs  - \\datapump.edcc.edu\testlogs
@Echo     Transfer  - \\Transfer01\Transfer
@Echo     Tums      - \\Tums\E$
@Echo.
@Echo Specific Usage: TO.BAT ServerName [E$]
@Echo Examples:
@Echo   "To.Bat Casper" would connect * \\Casper2\C$
@Echo   "To.Bat Rolaids C$" would connect * \\Rolaids\C$
@Echo   "To.Bat Odie E$ X:" would connect X: \\Odie\E$
@Echo   "To.Bat Browse Rolaids" Shows list of available admin shares
@Echo   "To.Bat New" From current location move to the newest child directory
@Echo   "To.Bat BetaCF C$" Uses C$ instead of E$\rootwww\cfusion\wwwroot
exit /b 1
