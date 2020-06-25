@Echo off
setlocal

if NOT EXIST C:\UTIL\EchoColor.exe (
    @Echo Util directory not found.
    @Echo Cannot continue
    goto :EOF
)
If NOT EXIST F:\HomeMadeBackup\Exists.txt (
    @Echo Backup drive is not connected.
    @Echo Cannot continue
    goto :EOF
)

if NOT EXIST F:\HomeMadeBackup\BackupLog md F:\HomeMadeBackup\BackupLog
for /F %%c in ('echocolor /ddate') do set DateTime=%%c
if NOT EXIST "F:\HomeMadeBackup\BackupLog\%DateTime%" md "F:\HomeMadeBackup\BackupLog\%DateTime%"
set LogDir=F:\HomeMadeBackup\BackupLog\%DateTime%
set Log="%LogDir%\Backup.log"
set ErrorLog="%LogDir%\BackupError.log"
EchoColor /datetime >> %Log%

if NOT EXIST "F:\HomeMadeBackup\BRAD_2015\Util" md "F:\HomeMadeBackup\BRAD_2015\Util"
if NOT EXIST "F:\HomeMadeBackup\BRAD_2015" md "F:\HomeMadeBackup\BRAD_2015"

@Echo Robocopy util copy log at "%LogDir%\BradsUtil.log"
robocopy "C:\Util" "F:\HomeMadeBackup\BRAD_2015\Util" * /s /e /v /r:1 /w:1 /LOG:"%LogDir%\BradsUtil.log"
@Echo Robocopy user brad log at "%LogDir%\BradsCDrive.log"
robocopy "C:\Users\Brad" "F:\HomeMadeBackup\BRAD_2015" * /s /e /v /r:1 /w:1 /LOG:"%LogDir%\BradsCDrive.log" /XD AppData Cookies NTUSER.DAT* "Local Settings\Application Data" TMP TEMP
@Echo Finished with Brad's local disk to F-Drive

@Echocolor ------------- using \\theresa-pc\users - /datetime >> %ErrorLog%
if NOT EXIST "\\theresa-pc\users\bradt.THERESA-PC\Verification.txt" (
    @Echo The network drive could not be verified. \\theresa-pc\users\bradt.THERESA-PC\Verification.txt does not exist.
    @Echo The network drive could not be verified. \\theresa-pc\users\bradt.THERESA-PC\Verification.txt does not exist >> %ErrorLog%
    @Echo The network drive could not be verified. \\theresa-pc\users\bradt.THERESA-PC\Verification.txt does not exist >> %Log%
    goto :Done
)
if NOT EXIST "\\theresa-pc\users\Theresa Thompson\Pictures\Tashi.jpg" (
    @Echo The network drive could not be verified. "\\theresa-pc\users\Theresa Thompson\Pictures\Tashi.jpg" does not exist.
    @Echo The network drive could not be verified. "\\theresa-pc\users\Theresa Thompson\Pictures\Tashi.jpg" does not exist >> %ErrorLog%
    @Echo The network drive could not be verified. "\\theresa-pc\users\Theresa Thompson\Pictures\Tashi.jpg" does not exist >> %Log%
    goto :Done
)

robocopy "C:\Users\Brad" "\\theresa-pc\users\bradt.THERESA-PC" * /s /e /v /r:1 /w:1 /LOG:"%LogDir%\BradsCDrive_to_THERESA-PC.log" /XD Cookies AppData "Local Settings" "Application Data" "OneDrive" "OneDrive.save"
robocopy "C:\Users\Brad\OneDrive" "\\theresa-pc\users\bradt.THERESA-PC\OneDrive.save" * /s /e /v /r:1 /w:1 /LOG:"%LogDir%\BradsOneDrive_to_THERESA-PC.log"
robocopy "\\theresa-pc\users\Theresa Thompson" "F:\HomeMadeBackup\THERESA-PC" * /s /e /v /r:1 /w:1 /LOG:"%LogDir%\From_THERESA-PC.log" /XD Cookies

goto :Done

:Done
@Echo --------------------------------------------------------------- >> %ErrorLog%
@Echo --------------------------------------------------------------- >> %Log%
goto :EOF
