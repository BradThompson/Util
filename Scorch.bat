@Echo off
@Echo.
if /I "%~1" EQU "q" goto OkGo
@Echo Calls:  git clean -xfd
REM - x = Don't use ignore when deleting.
REM - f = Force
REM - d = Deletes untracked directories
@Echo Add -n or --dry-run if you want to do a dry run 
@Echo Add any other parameters such as more excludes.
@Echo.
@Echo Deletes all non-git files in the repository at %CD%!!!!
choice /M "Are you sure you want to continue"?
if ERRORLEVEL == 2 goto :Dont
if ERRORLEVEL == 1 goto :OkGo
exit /b 1

:OkGo
@Echo git clean -xfd %*
git clean -xfd %*
exit /b 0

:Dont
@Echo OK, not scorching
exit /b 1
