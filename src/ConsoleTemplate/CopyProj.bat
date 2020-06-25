@Echo off
setlocal
if "%~1" EQU "" (
    @Echo.
    @Echo Specify the name of an existing app. Must be in this parent folder.[No spaces]
    goto :Usage
)
set NameOfExistingApp=%~1
if NOT EXIST %NameOfExistingApp% (
    @Echo.
    @Echo %NameOfExistingApp% does not have a directory
    goto :Usage
)
CD %NameOfExistingApp%
Call :DoWeHaveFiles
if ERRORLEVEL == 1 goto :Usage
set NameOfExistingAppDirectory=%CD%
CD ..

if "%~2" EQU "" goto :GetGuids

set NameOfNewApp=%~2
if EXIST %NameOfNewApp% (
    @Echo.
    @Echo %NameOfNewApp% directory must not exist. Cannot continue
    goto :Usage
)
if "%~3" EQU "" (
    @Echo.
    @Echo Specify the ExistingProjectUid
    goto :Usage
)
set ExistingProjectUid=%~3

if "%~4" EQU "" (
    @Echo.
    @Echo Specify the ExistingSlnGuid
    goto :Usage
)
set ExistingSlnGuid=%~4

if "%~1" EQU "%~2" (
    @Echo Idiot
    goto :SomethingIsWrong
)

:GoodToGo
MD %NameOfNewApp%
CD %NameOfNewApp%
@Echo CD: %CD%
set AreWeGood=
:Again
set /P AreWeGood=Are we good (Y/N) ?
if /I "%AreWeGood%" EQU "N" goto :EOF
if /I "%AreWeGood%" EQU "NO" goto :EOF
if /I "%AreWeGood%" EQU "Y" goto :Good
if /I "%AreWeGood%" EQU "YES" goto :Good
@Echo Enter Y or N
goto :Again

:Good
set NameOfNewAppDir=%CD%
@Echo NameOfNewAppDir: %NameOfNewAppDir%
ROBOCOPY %NameOfExistingAppDirectory% %NameOfNewAppDir% * /s /e /v
rd /q /s bin 1>NUL 2>NUL
rd /q /s obj 1>NUL 2>NUL
rd /q /s .vs 1>NUL 2>NUL
ReplaceInFiles.exe /s:%NameOfExistingApp% /r:%NameOfNewApp% /x /f:*.cs /go 1>NUL 2>NUL
ReplaceInFiles.exe /s:%NameOfExistingApp% /r:%NameOfNewApp% /x /f:*.dtsx /go 1>NUL 2>NUL
ReplaceInFiles.exe /s:%NameOfExistingApp% /r:%NameOfNewApp% /x /f:*.params /go 1>NUL 2>NUL
ReplaceInFiles.exe /s:%NameOfExistingApp% /r:%NameOfNewApp% /x /f:*proj /go 1>NUL 2>NUL
ReplaceInFiles.exe /s:%NameOfExistingApp% /r:%NameOfNewApp% /x /f:*.sln /go 1>NUL 2>NUL
ReplaceInFiles.exe /s:%NameOfExistingApp% /r:%NameOfNewApp% /f:Readme.md /go 1>NUL 2>NUL
ren %NameOfExistingApp%.* %NameOfNewApp%.*

uuidgen -c > %TMP%\uuidgen.tmp
for /F %%c in (%TMP%\uuidgen.tmp) do call :ReplaceGuid %ExistingProjectUid% %%c
uuidgen -c > %TMP%\uuidgen.tmp
for /F %%c in (%TMP%\uuidgen.tmp) do call :ReplaceGuid %ExistingSlnGuid% %%c
endlocal
CD %NameOfNewAppDir%
goto :EOF

:ReplaceGuid
ReplaceInFiles.exe /s:"%1" /r:"%2" /x /f:*.csproj /go 1>NUL 2>NUL
ReplaceInFiles.exe /s:"%1" /r:"%2" /x /f:*.sln /go 1>NUL 2>NUL
ReplaceInFiles.exe /s:"%1" /r:"%2" /x /f:*.cs /go 1>NUL 2>NUL
goto :EOF

:GetGuids
CD %NameOfExistingApp%
@Echo.
@Echo.
call findstr /L /I /N /P ProjectGuid *
call findstr /L /I /N /P SolutionGuid *
goto :EOF

:DoWeHaveFiles
if NOT EXIST *proj goto :MissingFile
if NOT EXIST *.sln goto :MissingFile
where ReplaceInFiles.exe 1>NUL 2>NUL
if ERRORLEVEL == 1 goto :MissingFile
where uuidgen.exe 1>NUL 2>NUL
if ERRORLEVEL == 1 goto :MissingFile
exit /b 0

:MissingFile 
@Echo One of the following files is missing:
@Echo    *proj
@Echo    *.sln
@Echo    ReplaceInFiles.exe
@Echo    uuidgen.exe
@Echo Cannot continue
exit /b 1

:Usage
@Echo.
@Echo Usage:
@Echo Copies an existing project, then handles name changes and guid changes.
@Echo Usage: CopyProj NameOfExistingApp    -- This will give your the ProjectGuid and SolutionGuild values
@Echo Usage: CopyProj NameOfExistingApp NameOfNewApp ExistingProjectUid ExistingSlnGuid
@Echo If the directory already exists an error is returned.
goto :EOF

:SomethingIsWrong
@Echo Whoa is me
@Echo Whoa is me
@Echo Whoa is me
@Echo Whoa is me
@Echo Whoa is me
goto :EOF
