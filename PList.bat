@Echo off
setlocal
set Full=
set Quiet=
set ProcessID=
set WorkFile=%Tmp%\SortedTaskList.tmp
set WorkFileTwo=%Tmp%\SortedTaskListTwo.tmp
if /I "%~1" == "/?" goto Usage
if /I "%~1" == "-?" goto Usage
if /I "%~1" == "?" goto Usage
:Loop
if /I "%~1" == "/f" (
    shift
    set Full=True
    goto Loop
)
if /I "%~1" == "-f" (
    shift
    set Full=True
    goto Loop
)
if /I "%~1" == "/i" (
    shift
    set ProcessID=True
    goto Loop
)
if /I "%~1" == "-i" (
    shift
    set ProcessID=True
    goto Loop
)
if /I "%~1" == "/q" (
    shift
    set Quiet=True
    goto Loop
)
if /I "%~1" == "-q" (
    shift
    set Quiet=True
    goto Loop
)
call :GetTaskList
if "%~1" == "" goto ShowAll

if NOT DEFINED Quiet (
    @Echo.
    @Echo Looking for a process with "%~1" in the name.
    @Echo.
)
call findstr /L /I "%~1" %WorkFile%
if ERRORLEVEL 1 goto NotFound
exit /b0

:NotFound
if NOT DEFINED Quiet (
    @Echo.
    @Echo Could not find "%~1"
    @Echo.
)
exit /b1

:GetTaskList
where tlist.exe 1>NUL 2>NUL
if ERRORLEVEL 1 goto MissingTList
tlist > %WorkFile%
del %WorkFileTwo% 1>NUL 2>NUL
if DEFINED ProcessID (
    copy %WorkFile% %WorkFileTwo% 1>NUL 2>NUL
    call sort < %WorkFileTwo% > %WorkFile%
    goto :EOF
)
if DEFINED Full (
    copy %WorkFile% %WorkFileTwo% 1>NUL 2>NUL
    call sort /+6 < %WorkFileTwo% > %WorkFile%
) else (
    FOR /F "tokens=1* delims= " %%i in (%WorkFile%) do @echo %%j >> %WorkFileTwo%
    call sort < %WorkFileTwo% > %WorkFile%
)
goto :EOF

:ShowAll
@Echo.
@Echo All installed products in alphabetical order:
@Echo.
type %WorkFile%
@Echo.
goto :EOF

:MissingTList
@Echo.
@Echo The Tlist.exe program is not available.
@Echo.
exit /b2

:Usage
@Echo off
@Echo.
@Echo si [/f] [/q] [StringToSearchFor]
@Echo.
@Echo Uses MsiConfig to find installed programs. If Msiconfig is not available
@Echo an error will be returned.
@Echo.
@Echo If no search string is supplied, shows all installed programs in alphabetical
@Echo order. Otherwise, displays only those string that match the search criteria.
@Echo.
@Echo If /f is specified, shows the GUID of the installed packages.
@Echo If /q is specified, only lists the installed product.
@Echo.
@Echo ERRORLEVEL is honored. 0 for found. 1 for not found, 2 for error on the program.
@Echo.
goto :EOF

