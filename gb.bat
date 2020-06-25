@Echo off
setlocal

if /I "%~1" EQU "" goto :Usage
if /I "%~2" NEQ "" goto :Usage
set Branch=%~1
git fetch origin %Branch%:%Branch%
if ERRORLEVEL == 1 goto :ErrorsOccured
git branch --set-upstream-to=origin/%Branch% %Branch%
if ERRORLEVEL == 1 goto :ErrorsOccured
goto :EOF

:ErrorsOccured
@Echo off
@Echo.
@Echo An error was reported. Aborting.
@Echo.
goto :EOF

:Usage
@Echo off
@Echo.
@Echo Usage:
@Echo Fetches branch from origin branch to local using the same branch name.
@Echo Sets the upstream tracking for this branch on origin.
@Echo gb branchName
@Echo.
@Echo git error checking is done at each step and will abort the process.
@Echo.
goto :EOF
