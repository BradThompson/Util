@Echo off
setlocal
if /I '%1' == '' goto Usage
if /I '%1' == '/?' goto Usage
if /I '%1' == '-?' goto Usage
@Echo %SystemRoot%\system32\shutdown.exe /r /m \\%1 /t 30 /f
%SystemRoot%\system32\shutdown.exe /r /m \\%1 /t 30 /f
goto :EOF

:Usage
@Echo.
@Echo Usage: REMBOOT [MyComputerame]"
@Echo.
@Echo Reboots the computer within 10 seconds, forcing all applications to close.
@Echo For more information, use "REMBOOT --help"
@Echo.
@Echo The cmd line that will be used is %SystemRoot%\system32\shutdown /r /m \\(servername) /t 10 /f
@Echo.
@Echo Use %SystemRoot%\system32\shutdown /? for full usage.
@Echo.
goto :EOF
