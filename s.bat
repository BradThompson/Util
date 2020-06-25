@Echo off
setlocal
set TmpFile=%Tmp%\GitStatus.tmp
set Verbose=

:loop
if /I "%~1" EQU "v" (
    set Verbose=True
    shift
    goto :loop
)
if /I "%~1" EQU "r" goto :DoRemote
if /I "%~1" EQU "ro" goto :DoRemoteOrdered
if /I "%~1" EQU "s" goto :DoStash
if /I "%~1" EQU "n" goto :DoGitStatus
if /I "%~1" NEQ "" goto :Usage

:DoMyStatus
@Echo ---------- Branches ----------
set BranchName=
call git branch > %TmpFile%
for /F "tokens=1,2" %%c in (%TmpFile%) do call :GetCurrentBranch %%c %%d
call git bov > %TmpFile%
for /F "tokens=1,2,3,4" %%c in (%TmpFile%) do call :PrintStatus %%c %%d %%e %%f
@Echo ---------- Status ----------
call git status
goto :EOF

:PrintStatus
if DEFINED Verbose (
    if "%4" EQU "%BranchName%" (
        @EchoColor A0 %1 %2 %3       %4
    ) else (
        @Echo %1 %2 %3       %4
    )
) else (
    if "%4" EQU "%BranchName%" (
        @EchoColor A0 %4
    ) else (
        @Echo %4
    )
)
goto :EOF

:GetCurrentBranch
if /I "%1" EQU "*" (
    set BranchName=%2
)
goto :EOF

:DoGitStatus
setlocal
@Echo Local branches
git branch
@Echo Status
git status
goto :EOF

:DoRemote
@Echo.
@Echo Remote Search
@Echo.
if "%~2" EQU "*" (
    @Echo List of all remote branches
    git branch -r  | sed "s/  origin\///"
    goto :EOF
)
if "%~2" EQU "" (
  set RemoteSearch=brad
    @Echo Filtered list of remote branches using the default string "brad"
) else (
  set RemoteSearch=%2
    @Echo Filtered list of remote branches using the string "%2"
)
git fetch 1> nul 2>nul
@Echo.
if DEFINED Verbose (
    git brol | grep %RemoteSearch%
) else (
    git branch -r | grep %RemoteSearch% | sed "s/  origin\///"
)
goto :EOF

:DoRemoteOrdered
@Echo Filtered and Ordered list of remote branches.
REM Depends on the git alias brol
git fetch 1>nul 2>nul
git brol
goto :EOF

:DoStash
@Echo Local stashes
git stash list
goto :EOF

:Usage
@Echo off
@Echo.
@Echo No parameters gives list of local branches and the status of the current branch in date of last access order
@Echo n    --- Does normal git status (git branch, then git status)
@Echo ro   --- List all remote branches with ordered time stamps filtered by second parameter
@Echo r    --- Filtered list of remote branches. Filter is "second parameter" or asterisk *
@Echo s    --- List of all local stashes
@Echo v    --- Verbose list (includes date time)
@Echo.
@Echo The f parameter requires a bug number.
@Echo.
@Echo Anything else brings up this usage.
@Echo.
goto :EOF

REM Here is the contents of the .gitconfig at this time:
[user]
	name = Brad Thompson
	email = brad.thompson@email.edcc.edu
[core]
	editor = nlf.bat
	autocrlf = true
	excludesfile = d:\\am\\.gitignore_global
    
[alias]
	a = config --global --list
	alias = config --global --list
	b = branch
	bl = branch --list
    bol = !git for-each-ref --sort='authordate:iso8601' --format='%(refname)' refs/heads | sed -e 's-refs/heads/--'
    bov = !git for-each-ref --sort='authordate:iso8601' --format='%(authordate:iso8601)%09%(refname)' refs/heads | sed -e 's-refs/heads/--'
    brol = !git for-each-ref --sort='authordate:iso8601' --format=' %(authordate:iso8601)%09%(refname)' refs/remotes/origin | sed -e 's-refs/remotes/origin/--'
	co = checkout
	com = checkout master
	co6 = checkout 1.6-dev
  co7 = checkout 1.7-dev
	hist = log --pretty=format:'%h %ad | %s%d [%an]' --graph --date=short
    ignore = "!f() { echo Look at .git/info/exclude or c:/am/.gitignore_global; }; f"
	last = log -1 --pretty=format:"%ai - %h - %s"
	lastn =  "!f() { git log -n$1 --pretty=format:\"%ai - %h - %an - %s\"; }; f"
	lol = log --oneline --graph --decorate
	pom = pull --rebase origin master
	s = !git branch && git status
    undo = "!f() { echo Use git checkout filename_to_undo for unstaged files; }; f"
	p = pull -q

[diff-SAVE]
    tool = kdiff3
[difftool-SAVE "kdiff3"]
    path = C:/KDiff3/kdiff3.exe

[merge-SAVE]
	tool = kdiff3
[mergetool-SAVE "kdiff3"]
    path = C:/KDiff3/kdiff3.exe
    keepBackup = false
    trustExitCode = false

[diff]
    tool = vsdiffmerge
[difftool]
      prompt = false
[difftool "vsdiffmerge"]
      cmd = '"C:/Program Files (x86)/Microsoft Visual Studio 12.0/Common7/IDE/vsdiffmerge.exe"' "$LOCAL" "$REMOTE" //t
      keepbackup = false
      trustexitcode = true
      
[merge]
	tool = vsdiffmerge
[mergetool "vsdiffmerge"]
	cmd = \"c:/Program Files (x86)/Microsoft Visual Studio 12.0/Common7/IDE/vsdiffmerge.exe\" \"$REMOTE\" \"$LOCAL\" \"$BASE\" \"$MERGED\" //m
[mergetool]
	prompt = false

[gui]
    
[push]
	default = simple
    
[commit]
	template = d:\\AM\\.gitmessage.txt
    
[credential]
	helper = manager
[color "diff"]
	meta = blue black bold
[gui]
	recentrepo = D:/Ambient2
