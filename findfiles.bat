@Echo off
setlocal

set Edit=
if "%~1" EQU "" goto :Usage
if "%~2" NEQ "/edit" goto :Usage
set Edit=Edit This

FOR /F "usebackq" %%c IN (`dir /s /b %1`) DO call :dd %%c
goto :eof

:dd
if DEFINED Edit (
    call n.bat %1
    pause
) else (
    @Echo %1
)
goto :eof

:Usage
@Echo.
@Echo Usage: FindFiles MyFile* [/edit]
@Echo.
@Echo It searches in all child directories and returns the fully qualified name of the file.
@Echo If /edit is used, the file is opened with n.bat and a pause is inserted between each edited file.
@Echo.
goto :eof
