#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM microsoft/dotnet:2.2-aspnetcore-runtime-stretch-arm32v7 AS base
WORKDIR /app
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk-stretch-arm32v7 AS build
WORKDIR /src
COPY ["src/Sannel.House.Users/Sannel.House.Users.csproj", "src/Sannel.House.Users/"]
COPY ["src/Migrations/Sannel.House.Users.Data.Sqlite/Sannel.House.Users.Data.Sqlite.csproj", "src/Migrations/Sannel.House.Users.Data.Sqlite/"]
COPY ["src/Sannel.House.Users.Data/Sannel.House.Users.Data.csproj", "src/Sannel.House.Users.Data/"]
COPY ["src/Migrations/Sannel.House.Users.Data.SqlServer/Sannel.House.Users.Data.SqlServer.csproj", "src/Migrations/Sannel.House.Users.Data.SqlServer/"]
COPY ["src/Migrations/Sannel.House.Users.Data.MySql/Sannel.House.Users.Data.MySql.csproj", "src/Migrations/Sannel.House.Users.Data.MySql/"]
COPY ["src/Migrations/Sannel.House.Users.Data.PostgreSQL/Sannel.House.Users.Data.PostgreSQL.csproj", "src/Migrations/Sannel.House.Users.Data.PostgreSQL/"]
RUN dotnet restore "src/Sannel.House.Users/Sannel.House.Users.csproj"
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