REM https://github.com/NuGet/Home/issues/6978
@echo off
pushd %~dp0
set HTTP_PROXY=
set HTTPS_PROXY=
dotnet nuget locals -c all
dotnet restore