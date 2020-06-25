@Echo off
setlocal
if /I "%~1" EQU "" goto :Usage
if /I "%~1" EQU "/?" goto :Usage
if /I "%~1" EQU "-?" goto :Usage
if NOT EXIST "%~1" goto :FailedOne
if EXIST "%~1\" goto :GoodToAsk
goto :FailedTwo

:GoodToAsk
@Echo.
@Echo ------------------------------------------------------------
@Echo This is very dangerous. It will delete the entire directory!
@Echo ------------------------------------------------------------
set ReallyDo=
@Echo.
SET /P ReallyDo=Are you absolutely sure you want to delete the directory "%~f1"? Enter YES (caps required) to approve.
if "%ReallyDo%" NEQ "YES" goto :Whew
if EXIST c:\DeleteMe\ (
    rd /q /s c:\DeleteMe\
    if ERRORLELVEL == 1 goto :Failed
)
md c:\DeleteMe\
if ERRORLELVEL == 1 goto :Failed
robocopy c:\DeleteMe\ "%~f1" /mir
if ERRORLELVEL == 1 goto :Failed
rd c:\DeleteMe\
rd "%~f1"
goto :EOF

:Whew
@Echo Good choice!
goto :EOF

:FailedOne
@Echo The directory "%~1" does not exist.
goto :EOF

:FailedTwo
@Echo This only works on directories. "%~1" is a file.
goto :EOF

:Usage
@Echo.
@Echo Usage: RoboDelete MyDirToDelete
@Echo.
@Echo Used to handle directories that are to long. Typically directories that are
@Echo created with repeated "Application Data". (Stupid Windows bug)
@Echo Robocopy has the ability to handle directory paths up to 2000 characters.
@Echo Cmd and File explorer do not.
@Echo Uses Robocopy to "Mirror" an empty directory over a full one. The mirroring
@Echo process makes the direcories exactly the same. Since the "source" is empty,
@Echo the contents of the "destination" (the specified directory) are deleted.
@Echo Example: robocopy c:\DeleteMe C:\Directory\To\Delete /mir
goto :EOF
