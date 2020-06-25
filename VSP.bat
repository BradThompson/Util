@Echo off
setlocal

set ProjectFileCount=0
set VSProject=
set Devenv=
where devenv.exe 1>NUL 2>NUL
if ERRORLEVEL == 1 goto :Check2019
@Echo Something is wrong. The path should not have Devenv.exe in it.
goto :EOF

:Check2019
set PATH=%PATH%;C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\Common7\IDE
where devenv.exe 1>NUL 2>NUL
if ERRORLEVEL == 1 goto :Check2017
@Echo Using Visual Studio 2019 Professional
goto :FoundDevEnv

:Check2017
set PATH=%PATH%;C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\IDE
where devenv.exe 1>NUL 2>NUL
if ERRORLEVEL == 1 goto :Check2019Community
@Echo Using Visual Studio 2017 Professional
goto :FoundDevEnv

:Check2019Community
set PATH=%PATH%;C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE
where devenv.exe
if ERRORLEVEL == 1 goto :Check2017Community
@Echo Using Visual Studio 2019 Community
goto :FoundDevEnv

:Check2017Community
set PATH=%PATH%;C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE
where devenv.exe
if ERRORLEVEL == 1 goto :NotHere
@Echo Using Visual Studio 2017 Community
goto :FoundDevEnv

:FoundDevEnv
if /I "%~1" EQU "/setup" (
    Devenv.exe /setup
    goto :EOF
)

if "%~1" == "" goto FindIt
set VSProject=%~1
goto GotIt

:FindIt
if exist *.ssmssln goto ssmssln
if exist *.sln goto sln
if exist src\*.sln cd src & goto sln
if exist source\*.sln cd source & goto sln
if exist *.csproj goto CSProj
if exist *.vbproj goto VBProj
if exist *.wixproj goto WixProj
if exist *.vcproj goto VCProj
if exist *.contentproj goto ContentProj
@Echo No Visual Studio project files found.
goto :EOF

:VCProj
for %%c in (*.vcproj) do call :SetAndCheck "%%c" & if ERRORLEVEL == 1 goto :EOF
goto GotIt

:CSProj
for %%c in (*.csproj) do call :SetAndCheck "%%c" & if ERRORLEVEL == 1 goto :EOF
goto GotIt

:VBProj
for %%c in (*.vbproj) do call :SetAndCheck "%%c" & if ERRORLEVEL == 1 goto :EOF
goto GotIt

:ContentProj
for %%c in (*.contentproj) do call :SetAndCheck "%%c" & if ERRORLEVEL == 1 goto :EOF
goto GotIt

:WixProj
for %%c in (*.wixproj) do call :SetAndCheck "%%c" & if ERRORLEVEL == 1 goto :EOF
goto GotIt

:sln
for %%c in (*.sln) do call :SetAndCheck "%%c" & if ERRORLEVEL == 1 goto :EOF
goto GotIt

:ssmssln
for %%c in (*.ssmssln) do call :SetAndCheck "%%c" & if ERRORLEVEL == 1 goto :EOF
goto GotIt

:GotIt
@Echo Here we go...
@Echo Devenv.exe "%VSProject%"
START /B devenv.exe "%VSProject%"
goto :EOF

:SetAndCheck
set /A ProjectFileCount=%ProjectFileCount% + 1
if %ProjectFileCount% GTR 1 (
    @Echo.
    @Echo There is more than one project file!
    @Echo.
    exit /b1
)
set VSProject=%~1
exit /b0

:NotHere
@Echo off
@Echo.
@Echo The Visual Studio devenv.exe is missing.
@Echo.
goto :EOF
