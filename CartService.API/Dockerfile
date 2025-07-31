FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CartService.API/CartService.API.csproj", "CartService.API/"]
COPY ["CartService.Repository/CartService.Repository.csproj", "CartService.Repository/"]
COPY ["CartService.Service/CartService.Service.csproj", "CartService.Service/"]
COPY ["SharedProtos/SharedProtos.csproj", "SharedProtos/"]
RUN dotnet restore "CartService.API/CartService.API.csproj"
COPY . .
WORKDIR "/src/CartService.API"
RUN dotnet build "CartService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CartService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CartService.API.dll"]