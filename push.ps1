#!/usr/local/bin/pwsh
param(
)

$version = $env:BUILD_BUILDNUMBER;

if($null -eq $version -or $version -eq "")
{
	$version = (Get-Date -Format yyMMdd) + "-local";
}

if($IsLinux -eq $true -or $IsMacOS -eq $true)
{
	$uname = uname -m
	if($uname -eq "x86_64" -or $uname -eq "i386")
	{
		$uname = "x86";
	}
	if($uname -eq "armv7l" -or $uname -eq "unknown") # for now assume unknown is arm
	{
		$uname = "arm";
	}

	$env:SANNEL_ARCH="linux-$uname"
	$env:SANNEL_VERSION=Get-Date -format yyMM.dd
	return docker-compose -f docker-compose.yml -f docker-compose.unix.yml push $target
}
else
{
	$env:SANNEL_ARCH="win"
	$env:SANNEL_VERSION=Get-Date -format yyMM.dd
	return docker-compose -f docker-compose.yml -f docker-compose.windows.yml push $target
}
