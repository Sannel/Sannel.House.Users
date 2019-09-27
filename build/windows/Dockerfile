ARG tag=3.0
FROM mcr.microsoft.com/dotnet/core/aspnet:${tag} AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8443

COPY Sannel.House.Users/ .
USER ContainerUser
ENTRYPOINT ["dotnet", "Sannel.House.Users.dll"]