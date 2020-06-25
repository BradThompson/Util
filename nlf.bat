@Echo off
@REM This file is used by Git in order to use Notepad for commit messages.
@REM It converts the commit message to use CRLF before opening the comment file.
setlocal
if NOT EXIST %~f1 (
    @Echo Something is wrong. This doesn't exist: %~f1
        goto :EOF
)
call c:\util\FixWhite.exe -i %~f1
cmd /C "Notepad.exe %~f1"
copy %~f1 c:\tmp\%~nx1
