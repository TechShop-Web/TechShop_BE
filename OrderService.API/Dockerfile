FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["OrderService.API/OrderService.API.csproj", "OrderService.API/"]
COPY ["OrderService.Service/OrderService.Service.csproj", "OrderService.Service/"]
COPY ["OrderService.Repository/OrderService.Repository.csproj", "OrderService.Repository/"]
COPY ["SharedProtos/SharedProtos.csproj", "SharedProtos/"]
COPY ["OrderService.API/appsettings.Docker.json", "OrderService.API/"]


RUN dotnet restore "OrderService.API/OrderService.API.csproj"
COPY . .
WORKDIR "/src/OrderService.API"
RUN dotnet build "OrderService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.API.dll"]