@Echo off
setlocal
set IsGood=
if /I "%~1" EQU "c" (
    @Echo npm run private-build-client
    set IsGood=true
    npm run private-build-client
)
if /I "%~1" EQU "s" (
    @Echo npm run private-build-server
    set IsGood=true
    npm run private-build-server
)
if /I "%~1" EQU "b" (
    @Echo npm run private-build -both client and server tests-
    set IsGood=true
    npm run private-build
)
if NOT DEFINED IsGood (
  @Echo Invalid parameter. Specify c, s or b.
  pause
)
@Echo Sleeping 30 seconds....
sleep 30
exit

From: Pravesh Gupta (HCL America Inc) 
Sent: Wednesday, June 1, 2016 10:27 AM
Subject: Optimized Private Builds

1.	Build - Only Client projects – 
New Build definition to use – ‘MagicGlass-Client’
To run client build from cmd line – ‘npm run private-build-client’

Build time –  ~36 mins

2.	Build - Only Server projects – 
New Build definition to use - ‘MagicGlass-Server
To run server build from cmd line – ‘npm run private-build-server’

Build time - ~33 mins

3.	Build - Both Client and Server build (Existing build system)– 
Build definition – ‘MagicGlass’
To run build from cmd line – ‘npm run private-build’

Build time - ~58 mins

