FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ProductService.API/ProductService.API.csproj", "ProductService.API/"]
COPY ["ProductService.Repository/ProductService.Repository.csproj", "ProductService.Repository/"]
COPY ["ProductService.Service/ProductService.Service.csproj", "ProductService.Service/"]
COPY ["SharedProtos/SharedProtos.csproj", "SharedProtos/"]
RUN dotnet restore "ProductService.API/ProductService.API.csproj"
COPY . .
WORKDIR "/src/ProductService.API"
RUN dotnet build "ProductService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductService.API.dll"]