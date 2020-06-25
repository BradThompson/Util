@Echo off
Setlocal
@if '%1' == '' Goto Usage
@if '%1' == '-?' Goto Usage
@if '%1' == '/?' Goto Usage

set WithFiles=*
if /I '%1' EQU '/w' goto :NewNormal
if /I '%1' EQU '-w' goto :NewNormal

:Normal
REM NOTE! Findstr does not work with Unicode files.
REM You can use Find.exe, but it has a different set of parameters
REM Todo - Write my own Findstr someday...
@Echo findstr /L /I /N /P %1 %2 %3 %4 %5 %WithFiles%
findstr /L /I /N /P %1 %2 %3 %4 %5 %WithFiles%
@Goto Done

:NewNormal
shift
set WithFiles=%1
shift
goto :Normal

:Usage
@Echo.
@Echo fs [/w WithFile*] /Additional /Switches String
@Echo.
@Echo This is a wrapper for the FINDSTR.EXE program available in the
@Echo resource kit. It puts the frequently used switches on the line.
@Echo.
@Echo findstr /L /I /N /P (your parameters here) (WithFiles)
@Echo.
@Echo The default set of files is * (the equivalent of /w *)
@Echo Use the /w switch to limit the search. For example: /w *csproj
@Echo.
@Goto Done

:Done
endlocal
