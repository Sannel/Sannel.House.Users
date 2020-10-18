ARG tag=5.0
ARG sdk_tag=5.0
FROM mcr.microsoft.com/dotnet/aspnet:${tag} AS base
WORKDIR /app
EXPOSE 8181
EXPOSE 8443

FROM mcr.microsoft.com/dotnet/sdk:${sdk_tag} AS build
WORKDIR /src
COPY [".nuget/nuget.config", ".nuget/nuget.config"]
COPY ["Directory.Build.props", "./"]
COPY ["src/Sannel.House.Users/Sannel.House.Users.csproj", "src/Sannel.House.Users/"]
COPY ["src/Sannel.House.Users.Data/Sannel.House.Users.Data.csproj", "src/Sannel.House.Users.Data/"]
COPY ["src/Migrations/Sannel.House.Users.Data.Sqlite/Sannel.House.Users.Data.Sqlite.csproj", "src/Migrations/Sannel.House.Users.Data.Sqlite/"]
COPY ["src/Migrations/Sannel.House.Users.Data.SqlServer/Sannel.House.Users.Data.SqlServer.csproj", "src/Migrations/Sannel.House.Users.Data.SqlServer/"]
COPY ["src/Migrations/Sannel.House.Users.Data.PostgreSQL/Sannel.House.Users.Data.PostgreSQL.csproj", "src/Migrations/Sannel.House.Users.Data.PostgreSQL/"]
RUN dotnet restore --configfile .nuget/nuget.config "src/Sannel.House.Users/Sannel.House.Users.csproj"
COPY . .
WORKDIR "/src/src/Sannel.House.Users"
RUN dotnet build "Sannel.House.Users.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Sannel.House.Users.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY --from=build /src/src/Sannel.House.Users/app_config app_config
COPY --from=build /src/src/Sannel.House.Users/app_data app_data
ENTRYPOINT ["dotnet", "Sannel.House.Users.dll"]