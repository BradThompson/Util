@Echo off
@Echo https://github.com/einars/js-beautify (Click on Zip to get files)
setlocal
if "%~1" EQU "" goto Usage
if "%~2" EQU "" goto Usage
if "%~2" EQU "%~1" goto Usage
if not exist "%~1" goto Missing
if exist "%~2" (
    @Echo.
    @Echo The file: %~2 already exists. Press any key to overwrite. Otherwise press ^C
    @Echo.
    pause
)
REM Assumes beautifier was installed using: npm -g install js-beautify
js-beautify %1 > %2
goto :EOF

:Missing
@Echo.
@Echo The file %1 does not exist. Aborting.
@Echo.
goto :EOF

:Usage
@Echo.
@Echo Usage:
@Echo jsb JavaScriptFileToBeautify.js BeautifiedFile.js
@Echo File 1 and File 2 cannot be the same file.
@Echo.
goto :EOF
