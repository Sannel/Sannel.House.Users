#!/bin/pwsh

if($IsLinux -eq $true)
{
	docker-compose -f docker-compose.yml -f docker-compose.unix.yml down
}
else
{
	docker-compose -f docker-compose.yml -f docker-compose.windows.yml down
}