#!/bin/pwsh

if($IsLinux -eq $true)
{
	$uname = uname -p
	$env:SANNEL_ARCH="linux-$uname"
	$env:SANNEL_VERSION=Get-Date -format yyMM.dd
	return docker-compose -f docker-compose.yml -f docker-compose.unix.yml build
}
else
{
	$env:SANNEL_ARCH="win"
	$env:SANNEL_VERSION=Get-Date -format yyMM.dd
	return docker-compose -f docker-compose.yml -f docker-compose.windows.yml build
}