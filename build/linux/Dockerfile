ARG tag=2.2
FROM mcr.microsoft.com/dotnet/core/runtime:${tag} AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8443
RUN useradd -m house && mkdir /app && chown -R house /app
USER house
COPY Sannel.House.Users/ .
ENTRYPOINT ["dotnet", "Sannel.House.Users.dll"]