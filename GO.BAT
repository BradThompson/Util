@Echo off
set Color=Blue
if /I "%COMPUTERNAME%" EQU "BetaSql12" set Color=Green
if /I "%COMPUTERNAME%" EQU "BetaStaging12" set Color=Green
if /I "%COMPUTERNAME%" EQU "BetaDotNet" set Color=Green
if /I "%COMPUTERNAME%" EQU "BetaCF" set Color=Green
if /I "%COMPUTERNAME%" EQU "Casper2" set Color=Red
if /I "%COMPUTERNAME%" EQU "Rolaids" set Color=Red
if /I "%COMPUTERNAME%" EQU "TUMS" set Color=Red
if /I "%COMPUTERNAME%" EQU "Odie" set Color=Red
if /I "%COMPUTERNAME%" EQU "Apollo" set Color=Red
if /I "%COMPUTERNAME%" EQU "Lalinea" set Color=Red
if /I "%COMPUTERNAME%" EQU "POC-Rolaids" set Color=Purple
if /I "%COMPUTERNAME%" EQU "POC-TUMS" set Color=Purple
if /I "%COMPUTERNAME%" EQU "POC-Lalinea" set Color=Purple
if /I "%COMPUTERNAME%" EQU "POC-Casper2" set Color=Purple
if /I "%COMPUTERNAME%" NEQ "1FP0VF2" SET PROMPT=$C%ComputerName%$F $P$G
if EXIST \\1FP0VF2\c$\Util\Startup.bat \\1FP0VF2\c$\Util\Startup.bat %Color%
