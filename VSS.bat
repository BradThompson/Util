@Echo off
setlocal

set ProjectFileCount=0
set VSProject=
set DevenvDirectory=C:\Program Files (x86)\Microsoft Visual Studio\2017\SQL\Common7\IDE
set Devenv=%DevenvDirectory%\devenv.exe

if NOT EXIST "%Devenv%" goto :NotHere

:FoundDevEnv
if /I "%~1" EQU "/setup" (
    "%Devenv%" /setup
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
call AddPath /I0 "%DevenvDirectory%" > NUL
@Echo %Devenv% "%VSProject%"
START /B devenv.exe "%VSProject%"
call AddPath /R "%DevenvDirectory%" > NUL
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
@Echo The SQL server version of devenv.exe is missing. It is not in this location:
@Echo.
@Echo %Devenv%
@Echo.
goto :EOF
