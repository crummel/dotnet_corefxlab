@for /r %%a in (src\*.xproj) do @move %%~fa %%~dpna.xproj.old > NUL
call init-tools.cmd
for /r %%a in (System*.csproj) do (
  msbuild %%a /fl "/flp:v=diag;logfile=%%~n.log"
  if errorlevel 1 (
    echo %%a failed
    exit /b
  )
)
@for /r %%a in (*.xproj.old) do @move %%~fa %%~dpna > NUL
