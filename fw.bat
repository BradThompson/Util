@Echo off
if "%~1" EQU "" (
    @Echo Fixwhite.exe
    Fixwhite.exe
) else (
    @Echo Fixwhite.exe -t -i -e "%~1"
    Fixwhite.exe -t -i -e "%~1"
)
REM Check out unix2dos.exe 
