@echo off
setlocal
goto SKIP
if "%1" == "-?" (
    echo GitDiff - enables diffing of file lists, instead of having to serially
    echo diff files without being able to go back to a previous file.
    echo Command-line options are passed through to git diff.
    echo If GIT_FOLDER_DIFF is set, it is used to diff the file lists. Default is windff.
    goto :EOF
)

if "%GIT_DIFF_COPY_FILES%" == "" (
    rd /s /q %TEMP%\GitDiff
    mkdir %TEMP%\GitDiff
    mkdir %TEMP%\GitDiff\old
    mkdir %TEMP%\GitDiff\new

    REM This batch file will be called by git diff. This env var indicates whether it is
    REM being called directly, or inside git diff
    set GIT_DIFF_COPY_FILES=1

    set GIT_DIFF_OLD_FILES=%TEMP%\GitDiff\old
    set GIT_DIFF_NEW_FILES=%TEMP%\GitDiff\new

    set GIT_EXTERNAL_DIFF=%~dp0\GitDiff.bat
dir %GIT_EXTERNAL_DIFF%
pause
    echo Please wait and press q when you see "(END)" printed in reverse color...
    @Echo call git diff %*
REM    call git diff %*
pause
    if defined GIT_FOLDER_DIFF (
        REM This command using GIT_FOLDER_DIFF just does not work for some reason.
@Echo        %GIT_FOLDER_DIFF% %TEMP%\GitDiff\old %TEMP%\GitDiff\new
REM        %GIT_FOLDER_DIFF% %TEMP%\GitDiff\old %TEMP%\GitDiff\new
        goto :EOF
    )

    if exist "%ProgramFiles%\Beyond Compare 2\BC2.exe" (
        set GIT_FOLDER_DIFF="%ProgramFiles%\Beyond Compare 2\BC2.exe"
@Echo "%ProgramFiles%\Beyond Compare 2\BC2.exe" %TEMP%\GitDiff\old %TEMP%\GitDiff\new
REM        "%ProgramFiles%\Beyond Compare 2\BC2.exe" %TEMP%\GitDiff\old %TEMP%\GitDiff\new
        goto :EOF
    )

    @Echo "%ProgramFiles(x86)%\WinMerge\WinMergeU.exe" -r -e -dl "Base" -dr "Mine" %TEMP%\GitDiff\old %TEMP%\GitDiff\new
REM    "%ProgramFiles(x86)%\WinMerge\WinMergeU.exe" -r -e -dl "Base" -dr "Mine"  %TEMP%\GitDiff\old %TEMP%\GitDiff\new
    goto :EOF
)

@Echo GIT_DIFF_COPY_FILES: %GIT_DIFF_COPY_FILES%
:SKIP
REM diff is called by git with 7 parameters:
REM     path old-file old-hex old-mode new-file new-hex new-mode
@Echo copy %TEMP%\%~nx2 %GIT_DIFF_OLD_FILES%\%1
REM copy %TEMP%\%~nx2 %GIT_DIFF_OLD_FILES%\%1
@Echo copy %5 %GIT_DIFF_NEW_FILES%
REM copy %5 %GIT_DIFF_NEW_FILES%
