FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY BookPricesJob.API/BookPricesJob.API.csproj BookPricesJob.API/
COPY BookPricesJob.Common/BookPricesJob.Common.csproj BookPricesJob.Common/
COPY BookPricesJob.Data/BookPricesJob.Data.csproj BookPricesJob.Data/
COPY BookPricesJob.Application/BookPricesJob.Application.csproj BookPricesJob.Application/

RUN dotnet restore BookPricesJob.API/BookPricesJob.API.csproj
COPY . .
WORKDIR "/src/."
RUN dotnet build BookPricesJob.API/BookPricesJob.API.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish BookPricesJob.API/BookPricesJob.API.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookPricesJob.API.dll"]
