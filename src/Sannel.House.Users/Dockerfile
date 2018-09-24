#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see http://aka.ms/containercompat 

FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Sannel.House.Users/Sannel.House.Users.csproj Sannel.House.Users/
COPY Sannel.House.Users.Data/Sannel.House.Users.Data.csproj Sannel.House.Users.Data/
COPY Sannel.House.Users.Data.SqlServer/Sannel.House.Users.Data.SqlServer.csproj Sannel.House.Users.Data.SqlServer/
COPY Sannel.House.Users.Data.Sqlite/Sannel.House.Users.Data.Sqlite.csproj Sannel.House.Users.Data.Sqlite/
RUN dotnet restore Sannel.House.Users/Sannel.House.Users.csproj
COPY . .
WORKDIR /src/Sannel.House.Users
RUN dotnet build Sannel.House.Users.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Sannel.House.Users.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
RUN "mkdir c:\app\App_Config"
RUN "mkdir c:\app\App_Data"
ENTRYPOINT ["dotnet", "Sannel.House.Users.dll"]
