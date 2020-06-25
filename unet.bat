@Echo off
setlocal
set File="%~1"
set TempFile=%TMP%\tt.txt
net use>%TempFile%
set Drive=%CD:~0,2%
call :GetMatch %Drive%
goto :EOF

:GetMatch
for /F "skip=6 tokens=2,3 delims= " %%c in (%TempFile%) do call :FindMatch %%c %%d
goto :EOF

:FindMatch
if /I "%Drive%" EQU "%1" call :NextStep %File% %2
goto :EOF

:NextStep
@Echo %~f1>%TempFile%
type %TempFile%
@Echo ================
set final=%2

for /F "tokens=1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20 delims=\" %%c in (%TempFile%) do call :ShowNet %%c %%d %%e %%f %%g %%h %%i %%j %%k %%l %%m %%n %%o %%p %%q %%r %%s %%t %%u %%v
goto :EOF

:ShowNet
if /I "%~1" EQU "" @Echo %final% & goto :EOF
if /I %1 EQU %Drive% shift & goto :ShowNet
set final=%final%\%1
shift
goto :ShowNet
