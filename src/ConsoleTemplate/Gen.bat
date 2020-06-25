@Echo off
setlocal
@Echo Generates a new console application.
@Echo Usage: Gen NameOfApp
@Echo Creates a console application solution in the name supplied.
@Echo If the directory already exists an error is returned.
if "%~1" EQU "" (
    @Echo Specify an app name [No spaces]
    goto :EOF
)
set NameOfApp=%~1
if EXIST %NameOfApp% (
    @Echo There is already a file or directory with the name %NameOfApp%. Cannot continue.
    exit /b 1
)
Call :AreWeGood %~dp0
if ERRORLEVEL == 1 goto :EOF

@Echo CD: %CD%
MD %NameOfApp%
CD %NameOfApp%
set NameOfAppDir=%CD%
@Echo NameOfAppDir: %NameOfAppDir%
ROBOCOPY %~dp0 %NameOfAppDir% * /s /e /v
if NOT Exist Gen.bat goto :SomethingIsWrong
del GEN.BAT
if Exist CopyProj.bat del CopyProj.bat

ReplaceInFiles.exe /s:ConsoleTemplate /r:%NameOfApp% /f:ConsoleTemplate* /go 1>nul 2>nul
ReplaceInFiles.exe /s:ConsoleTemplate /r:%NameOfApp% /f:Readme.md /go 1>nul 2>nul
ren ConsoleTemplate.* %NameOfApp%.*
pushd Properties
ReplaceInFiles.exe /s:ConsoleTemplate /r:%NameOfApp% /f:AssemblyInfo.cs /go 1>nul 2>nul
popd
@Echo CD: %CD%
uuidgen -c > c:\tmp\uuidgen.tmp
for /F %%c in (c:\tmp\uuidgen.tmp) do call :ReplaceProjectGuid %%c
uuidgen -c > c:\tmp\uuidgen.tmp
for /F %%c in (c:\tmp\uuidgen.tmp) do call :ReplaceSolutionGuid %%c
@Echo NameOfAppDir %NameOfAppDir%
Echo CD %NameOfAppDir%>%Tmp%\GenTmp.bat
endlocal
%Tmp%\GenTmp.bat
goto :EOF

:ReplaceProjectGuid
ReplaceInFiles.exe /s:"46C9181F-B70A-4F94-A3F4-BEA764AACCC7" /r:"%1" /x /f:*.csproj /go 1>NUL 2>NUL
ReplaceInFiles.exe /s:"46C9181F-B70A-4F94-A3F4-BEA764AACCC7" /r:"%1" /x /f:*.sln /go 1>NUL 2>NUL
ReplaceInFiles.exe /s:"46C9181F-B70A-4F94-A3F4-BEA764AACCC7" /r:"%1" /x /f:*.cs /go 1>NUL 2>NUL
goto :EOF

:ReplaceSolutionGuid
ReplaceInFiles.exe /s:"47631FE2-EB8C-4BB9-B035-DB97CDEB5D5B" /r:"%1" /x /f:*.sln /go 1>NUL 2>NUL
goto :EOF

:AreWeGood
if NOT EXIST %1\ConsoleTemplate.cs goto :MissingFile
if NOT EXIST %1\ConsoleTemplate.csproj goto :MissingFile
if NOT EXIST %1\ConsoleTemplate.sln goto :MissingFile
if NOT EXIST %1\ReadMe.MD goto :MissingFile
if NOT EXIST %1\Properties\AssemblyInfo.cs goto :MissingFile
where ReplaceInFiles.exe 1>NUL 2>NUL
if ERRORLEVEL == 1 goto :MissingFile
exit /b 0

:MissingFile 
@Echo One of the following files is missing:
@Echo    %1ConsoleTemplate.cs
@Echo    %1ConsoleTemplate.csproj
@Echo    %1ConsoleTemplate.sln
@Echo    %1ReadMe.MD
@Echo    %1Properties\AssemblyInfo.cs
@Echo    ReplaceInFiles.exe
@Echo Cannot continue
exit /b 1

:SomethingIsWrong
@Echo Whoa is me
@Echo Whoa is me
@Echo Whoa is me
@Echo Whoa is me
@Echo Whoa is me
goto :EOF
