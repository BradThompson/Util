@echo off
setlocal
set reviewIdentifier=%1

if "%reviewIdentifier%" NEQ "" (
	call \\codeflow\public\cf open -id %reviewIdentifier%
	exit /b 
)

call \\codeflow\public\cf open
exit /b 
