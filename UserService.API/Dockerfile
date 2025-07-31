FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["UserService.API/UserService.API.csproj", "UserService.API/"]
COPY ["UserService.Repository/UserService.Repository.csproj", "UserService.Repository/"]
COPY ["UserService.Service/UserService.Service.csproj", "UserService.Service/"]
COPY ["SharedProtos/SharedProtos.csproj", "SharedProtos/"]
RUN dotnet restore "UserService.API/UserService.API.csproj"
COPY . .
WORKDIR "/src/UserService.API"
RUN dotnet build "UserService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserService.API.dll"]
