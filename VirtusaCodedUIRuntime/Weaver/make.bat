
pushd "%TargetAssemblyDir%"
set TargetAssemblyDir=%CD%\
popd

set TargetAssemblyPath=%TargetAssemblyDir%%TargetAssembly%%TargetAssemblyExt%
set TargetAssemblyName=%TargetAssembly%%TargetAssemblyExt%

cd "%AspectDotNetDir%"
weaver -aspects %1 -in "%TargetAssemblyPath%" -out "%TargetAssemblyName%"
if %ERRORLEVEL% == 0 (
    move /Y "%TargetAssemblyName%" "%TargetAssemblyPath%"
    move /Y "%TargetAssembly%.pdb" "%TargetAssemblyDir%%TargetAssembly%.pdb"
) 

start taskkill /F /IM mspdbsrv.exe

::cmd /c "exit /b 0"