@Echo off
setlocal
md George 1>nul 2>nul
md Ringo 1>nul 2>nul
md Paul 1>nul 2>nul
md Someone 1>nul 2>nul
md AtTwo 1>nul 2>nul
md AtThree 1>nul 2>nul
md AtFour 1>nul 2>nul
md AtFive 1>nul 2>nul
md AtSix 1>nul 2>nul

set Path=George;Ringo;Paul;Someone
@Echo Original Path: %Path%
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /R George
type c:\temp\AddPathExe.bat
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /R Ringo
type c:\temp\AddPathExe.bat
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /R Paul
type c:\temp\AddPathExe.bat
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /R Someone
type c:\temp\AddPathExe.bat

@Echo.
set Path=Original
@Echo Original Path: %Path%
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe George
type c:\temp\AddPathExe.bat
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe Ringo
type c:\temp\AddPathExe.bat
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe Paul
type c:\temp\AddPathExe.bat
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe Someone
type c:\temp\AddPathExe.bat

@Echo.
set Path=Insert;Testing;2;3;4
@Echo Original Path: %Path%
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /I0 George
type c:\temp\AddPathExe.bat
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /I999 Ringo
type c:\temp\AddPathExe.bat
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /I1 Paul
type c:\temp\AddPathExe.bat
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /I2 AtTwo
type c:\temp\AddPathExe.bat
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /I3 AtThree
type c:\temp\AddPathExe.bat
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /I4 AtFour
type c:\temp\AddPathExe.bat
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /I5 AtFive
type c:\temp\AddPathExe.bat
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /I6 AtSix
type c:\temp\AddPathExe.bat

rd George 1>nul 2>nul
rd Ringo 1>nul 2>nul
rd Paul 1>nul 2>nul
rd Someone 1>nul 2>nul
rd AtTwo 1>nul 2>nul
rd AtThree 1>nul 2>nul
rd AtFour 1>nul 2>nul
rd AtFive 1>nul 2>nul
rd AtSix 1>nul 2>nul

@Echo.
set Path=Insert;Testing;Here;and;now;and;then
@Echo Original Path: %Path%
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /I0 Zero /S
type c:\temp\AddPathExe.bat
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /I999 TheEnd /S
type c:\temp\AddPathExe.bat
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /I1 One /S
type c:\temp\AddPathExe.bat
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /I2 Two /S
type c:\temp\AddPathExe.bat
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /I3 Three /S
type c:\temp\AddPathExe.bat
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /I4 Four /S
type c:\temp\AddPathExe.bat
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /I5 Five /S
type c:\temp\AddPathExe.bat
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /I6 Six /S
type c:\temp\AddPathExe.bat
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /I7 Seven /S
type c:\temp\AddPathExe.bat
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /I7 and /S
type c:\temp\AddPathExe.bat

@Echo.
set Path=Insert;Testing;Here;and;now;and;then
@Echo Original Path: %Path%
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /R /S and
type c:\temp\AddPathExe.bat
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /S newone /I0
type c:\temp\AddPathExe.bat
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /S and /I0
type c:\temp\AddPathExe.bat
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /S and
type c:\temp\AddPathExe.bat

@Echo.
set Path=
@Echo NO PATH
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /S OneAndOnly
type c:\temp\AddPathExe.bat
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /S One;And;Only
type c:\temp\AddPathExe.bat
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /S "Testing123"
type c:\temp\AddPathExe.bat

@Echo.
@Echo BAD PATHS
set Path=;
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /S OneAndOnly
type c:\temp\AddPathExe.bat
set Path=;;;
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /S "Testing123"
type c:\temp\AddPathExe.bat
set Path=   what   ;   shouldbe   ;   done
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /S "Testing123"
type c:\temp\AddPathExe.bat
set Path=   what   ;   shouldbe   ;   done
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe -i "Testing123"
type c:\temp\AddPathExe.bat

@Echo.
@Echo Real path!
endlocal 
setlocal
@Echo SET Path=%PATH% > c:\temp\AddPathExeOriginal.bat
c:\Util\src\AddPathExe\bin\Debug\AddPathExe.exe /S "Testing123"
copy c:\temp\AddPathExe.bat c:\temp\AddPathExeAfterAdd.bat
@Echo Windiff c:\temp\AddPathExeOriginal.bat c:\temp\AddPathExeAfterAdd.bat
endlocal 
