@Echo off
setlocal ENABLEEXTENSIONS

set Util=%~dp0
set NotepadExe="C:\Program Files (x86)\Notepad++\notepad++.exe"
if NOT EXIST %NotepadExe% (
    set NotepadExe=C:\Windows\system32\notepad.exe
)
if "%~1" == "" goto :Resume
if /I "%~1" == "Notes" goto :DoNotes
if /I "%~1" == "AwsNotes" goto :DoAwsNotes
if /I "%~1" == "GitNotes" goto :DoGitNotes
if /I "%~1" == "SqlNotes" goto :DoSqlNotes
call :GetFileToEdit "%~1"
if ERRORLEVEL == 1 exit /b1

:EditTheFile
@Echo Editing the file "%FileToEdit%"
start /b %ComSpec% /c "%NotepadExe% "%FileToEdit%""
goto :Finished

:Resume
if NOT DEFINED FileToEdit (
    if EXIST "%Util%\Notes.txt" call n.bat %Util%\Notes.txt
    if EXIST "%Util%\GitNotes.txt" call n.bat "%Util%\GitNotes.txt"
    exit /b0
)

call :GetFileToEdit "%FileToEdit%"
if ERRORLEVEL == 1 exit /b1
@Echo off
@Echo.
@Echo Resume editing file "%FileToEdit%"
@Echo.
goto :EditTheFile

:GetFileToEdit
set Work=%~a1
if "%Work:~0,1%" EQU "d" (
    @Echo Cannot edit a directory
    exit /b 1
)
if NOT EXIST "%~f1" (
    call :ShouldWeCreate "%~f1"
    if ERRORLEVEL == 1 exit /b1
)
set FileToEdit=%~f1
exit /b0

:ShouldWeCreate
    set /P ShouldWeCreate=The file %1 does not exist. Create? (y if yes)
    if /I "%ShouldWeCreate%" EQU "y" goto YesCreate
    exit /b1
:YesCreate
@Echo.>%1
exit /b0

:DoNotes
n.bat C:\EdmondsCC\Notes\Notes.txt

:DoAwsNotes
n.bat C:\EdmondsCC\AWS\AWSNotes.txt

:DoGitNotes
n.bat %Util%\GitNotes.txt

:DoSqlNotes
n.bat C:\EdmondsCC\Sql\SqlNotes.txt

:ErrorOccured
@Echo off
@Echo.
@Echo An error occured. Cannot continue.
@Echo.
type %Tmp%\StdOutAndErr.txt
@Echo.
exit /b1

:Usage
@Echo off
@Echo.
@Echo Usage:
@Echo     n [filename]
@Echo.
@Echo The first time "n MyFile.txt" is called, the filename will be persisted in
@Echo the environment variable "FileToEdit".
@Echo If no file is specified, the "FileToEdit" variable will be used to determine
@Echo what file to edit.
@Echo If Notepad++ does not exist, then Notepad.exe will be used.
@Echo.
goto :EOF

:Finished
@Echo set FileToEdit=%FileToEdit%>%Tmp%\FileToEdit.bat
endlocal
call %Tmp%\FileToEdit.bat
