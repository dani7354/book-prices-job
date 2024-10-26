FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
EXPOSE 8080
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY BookPricesJob.sln .
COPY BookPricesJob.API/BookPricesJob.API.csproj BookPricesJob.API/
COPY BookPricesJob.Common/BookPricesJob.Common.csproj BookPricesJob.Common/
COPY BookPricesJob.Data/BookPricesJob.Data.csproj BookPricesJob.Data/
COPY BookPricesJob.Application/BookPricesJob.Application.csproj BookPricesJob.Application/
WORKDIR /src/BookPricesJob.API/
RUN dotnet restore

WORKDIR /src
COPY . .
WORKDIR /src/BookPricesJob.API
RUN dotnet build BookPricesJob.API.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish BookPricesJob.API.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookPricesJob.API.dll"]
