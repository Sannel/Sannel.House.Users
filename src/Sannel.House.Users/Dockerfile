#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["src/Sannel.House.Users/Sannel.House.Users.csproj", "src/Sannel.House.Users/"]
COPY ["src/Migrations/Sannel.House.Users.Data.Sqlite/Sannel.House.Users.Data.Sqlite.csproj", "src/Migrations/Sannel.House.Users.Data.Sqlite/"]
COPY ["src/Sannel.House.Users.Data/Sannel.House.Users.Data.csproj", "src/Sannel.House.Users.Data/"]
COPY ["src/Migrations/Sannel.House.Users.Data.SqlServer/Sannel.House.Users.Data.SqlServer.csproj", "src/Migrations/Sannel.House.Users.Data.SqlServer/"]
RUN dotnet restore "src/Sannel.House.Users/Sannel.House.Users.csproj"
COPY . .
WORKDIR "/src/src/Sannel.House.Users"
RUN dotnet build "Sannel.House.Users.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Sannel.House.Users.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
RUN "mkdir c:\app\app_config"
RUN "mkdir c:\app\app_data"
ENTRYPOINT ["dotnet", "Sannel.House.Users.dll"]