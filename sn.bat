@Echo off
setlocal
@Echo This calls sn.exe located in the NETFX directory.
@Echo.
set exe="C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\x64\sn.exe"
if NOT EXIST %exe% goto :SNMissing
%exe% %*
goto :EOF

:SNMissing
@Echo The sn.exe file is missing. Could not find it at:
@Echo %exe%
goto :EOF
