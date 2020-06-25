@Echo off
@Echo.
if "%1" == "" goto Usage
if EXIST %Tmp%\Error.txt (
    del %Tmp%\Error.txt 1>NUL 2>NUL
)
C:
if /i "%~1" == "all" goto :ByeAll
call :DropConnection "%~1"
goto :EOF

:DropConnection
set Wrk=%~1
if /I "%Wrk:~1,1%" == "\" goto NetAddr
if /I "%Wrk:~1,1%" == ":" goto ColonAlready
Goto NoColon

:NoColon
@Echo Dropping %~1:
net use %~1: /d 1>%Tmp%\Error.txt 2>&1
goto :EOF

:ColonAlready
@Echo Dropping %~1
net use %~1 /d 1>%Tmp%\Error.txt 2>&1
goto :EOF

:NetAddr
@Echo :NetAddr
@Echo Dropping %1
net use %1 /d 1>%Tmp%\Error.txt 2>&1
goto :EOF

:Usage
@Echo off
@Echo.
@Echo Usage:
@Echo   bye (drive letter such as y)
@Echo Does a net use (drive letter): /d
@Echo or
@Echo   bye all
@Echo Drops all drive letter connections.
@Echo.
@Echo or
@Echo   bye f:
@Echo Drops f:
@Echo or
@Echo   bye \\Server\Share
@Echo Drops \\Server\Share
@Echo.
goto :EOF

:ByeAll
for %%c in (d e f g h i j k l m n o p q r s t u v w x y z) do call :DropConnection %%c
net use
goto :EOF
