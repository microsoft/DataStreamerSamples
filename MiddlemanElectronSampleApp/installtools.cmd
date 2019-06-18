set REPO="http://github.com/mjfusa/samples"

:checkPrivileges 
NET FILE 1>NUL 2>NUL
if '%errorlevel%' == '0' ( goto continue 
) else ( powershell "saps -filepath %0 -verb runas" >nul 2>&1)
exit /b

:continue
cd %~dp0
powershell Set-ExecutionPolicy bypass -force
powershell Invoke-Expression ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))
set "PATH=%PATH%;%ALLUSERSPROFILE%\chocolatey\bin;c:\program files\nodejs;C:\Program Files\Git\cmd"
for %%a in (git.install, nodejs, 7zip.install, googlechrome, netfx-4.6.1-devpack) do choco install %%a -y
cmd /c npm install --global --production windows-build-tools@4.0.0 -y
choco install windows-sdk-10-version-1809-all -y

:eds
md \EDS
cd \EDS
cmd.exe /c git clone %REPO%
cd samples\electron\getstarted\electron
cmd /c npm install --global --production windows-build-tools
cmd /c npm config set msvs_version 2015 --global
cmd /c npm config set python %userprofile%\.windows-build-tools\python27\python.exe
cmd /c PowerShell [Environment]::SetEnvironmentVariable('VCTargetsPath', 'C:\Program Files (x86)\MSBuild\Microsoft.Cpp\v4.0\V140', 'User')
SET VCTargetsPath=C:\Program Files (x86)\MSBuild\Microsoft.Cpp\v4.0\V140
copy "c:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\VC\Tools\MSVC\14.16.27023\lib\x86\store\references\platform.winmd" "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\"   
copy "C:\Program Files (x86)\Windows Kits\10\UnionMetadata\10.0.17763.0\windows.winmd" "C:\Program Files (x86)\Windows Kits\10\UnionMetadata"
cmd.exe /c npm install
cmd.exe /c npm install electron-builder
cmd.exe /c .\node_modules\\.bin\electron-builder -w
rem choco install visualstudio2019community --package-parameters "--add Microsoft.VisualStudio.Workload.ManagedDesktop --add Microsoft.VisualStudio.Workload.NativeDesktop --add Microsoft.VisualStudio.Workload.NetCoreTools --add Microsoft.VisualStudio.Workload.Universal  --includeRecommended --includeOptional --passive --locale en-US" -y

:end
