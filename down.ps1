


if($IsLinux -eq $true)
{
	$env:SANNEL_ARCH=(uname -p)
	docker-compose -f docker-compose.yml -f docker-compose.unix.yml down
}
else
{
	$env:SANNEL_ARCH="win"
	docker-compose -f docker-compose.yml -f docker-compose.windows.yml down
}