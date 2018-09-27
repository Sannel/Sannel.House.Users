

if($IsLinux -eq $true)
{
	$uname = uname -p
	$env:SANNEL_ARCH="linux-$uname"
	docker-compose -f docker-compose.yml -f docker-compose.unix.yml build
}
else
{
	$env:SANNEL_ARCH="win"
	docker-compose -f docker-compose.yml -f docker-compose.windows.yml build
}