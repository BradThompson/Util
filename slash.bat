@Echo off
setlocal
set slash=\
set begin=
set end=
if /I "%~1" EQU "" goto :Usage
set Work=%1
if /I "%~2" NEQ "" (
    set Work=%~2
    if /I "%~1" EQU "b" (
        set slash=/
        goto :Good
    )
    if /I "%~1" NEQ "f" goto :Usage
)
:Good
set TempFile=%TMP%\slash.txt
@Echo %Work%>%TempFile%
type %TempFile%
@Echo ================
set final=
set ws=
if "%slash%" EQU "/" (
    set newslash=\
) else (
    set newslash=/
)
if "%slash%" EQU "/" (
    @Echo Converting %slash% to %newslash%

    if "%Work:~0,1%" EQU "\" set final=/
    if "%Work:~-1%" EQU "\" set end=/

    for /F "tokens=1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20 delims=/" %%c in (%TempFile%) do call :Show %%c %%d %%e %%f %%g %%h %%i %%j %%k %%l %%m %%n %%o %%p %%q %%r %%s %%t %%u %%v
) else (
    @Echo Converting %slash% to %newslash% here

    if "%Work:~0,1%" EQU "/" set final=\
    if "%Work:~-1%" EQU "/" set end=\

    for /F "tokens=1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20 delims=\" %%c in (%TempFile%) do call :Show %%c %%d %%e %%f %%g %%h %%i %%j %%k %%l %%m %%n %%o %%p %%q %%r %%s %%t %%u %%v
)
goto :EOF

:Show
if /I "%~1" EQU "" @Echo %final%%end% & goto :EOF
set final=%final%%ws%%1
set ws=%newslash%
shift
goto :Show

:Usage
@Echo Usage:
@Echo     slash \..\..\..\
@Echo returns /../../../
@Echo or
@Echo     slash f \..\..\..\
@Echo returns /../../../
@Echo or
@Echo Usage: slash b /../../../
@Echo returns \..\..\..\
goto :EOF
