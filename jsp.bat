@Echo off
setlocal
call jsb.bat payload.js %tmp%\payload.js.tmp
copy %tmp%\payload.js.tmp payload.js
