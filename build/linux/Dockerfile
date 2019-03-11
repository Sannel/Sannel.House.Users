ARG tag=2.2
FROM mcr.microsoft.com/dotnet/core/runtime:${tag} AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8443
COPY passwd /etc/
USER house
COPY Sannel.House.Users/ .
RUN /bin/chown -R house /app
ENTRYPOINT ["dotnet", "Sannel.House.Users.dll"]