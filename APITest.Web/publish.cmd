@echo off
echo ��ʼ����
pause
dotnet restore

echo ѡ����뵽�ĸ�����ϵͳ
echo 0------all
echo 1------win7-x64
echo 2------win10-x64
echo 3------centos.7-x64
echo 4------osx.10.12-x64
echo 5------osx.10.11-x64
echo 6------win7-x86

set /p input=����ϵͳ:

if %input%==1 (
	dotnet publish -r win7-x64 -c release
)
if %input%==2 (
	dotnet publish -r win10-x64 -c release
)
if %input%==3 (
	dotnet publish -r centos.7-x64 -c release
) 
if %input%==4 (
	dotnet publish -r osx.10.12-x64 -c release
) 
if %input%==5 (
	dotnet publish -r osx.10.11-x64 -c release
) 
if %input%==6 (
	dotnet publish -r win7-x86 -c release
)
if %input%==99 (
	dotnet publish -r osx.10.12-x64 -c debug
) 
if %input%==0 (
	dotnet publish -r win7-x64 -c release
	dotnet publish -r win10-x64 -c release
	dotnet publish -r centos.7-x64 -c release
	dotnet publish -r osx.10.12-x64 -c release
	dotnet publish -r osx.10.11-x64 -c release
) 


echo ��������˳�
pause