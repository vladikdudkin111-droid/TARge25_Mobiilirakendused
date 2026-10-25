@echo off
setlocal
pushd "%~dp0"
dotnet build "Heliaed\Heliaed.csproj" -f net10.0-windows10.0.19041.0 -p:TargetFrameworks=net10.0-windows10.0.19041.0 -p:RuntimeIdentifier=win-x64 -v:minimal
if errorlevel 1 (
  echo Build failed. See README_RU.md for .NET 10 and MAUI setup.
  pause
  popd
  exit /b 1
)
start "" "%~dp0Heliaed\bin\Debug\net10.0-windows10.0.19041.0\win-x64\Heliaed.exe"
popd
endlocal
