ARG DOTNET_VER=8.0

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VER} AS base
EXPOSE 8080
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VER} AS build
WORKDIR /src
COPY BookPricesJob.sln .
COPY BookPricesJob.API/ BookPricesJob.API/
COPY BookPricesJob.Common/ BookPricesJob.Common/
COPY BookPricesJob.Data/ BookPricesJob.Data/
COPY BookPricesJob.Application/ BookPricesJob.Application/
WORKDIR /src/BookPricesJob.API/
RUN dotnet restore

WORKDIR /src/BookPricesJob.API
RUN dotnet build BookPricesJob.API.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish BookPricesJob.API.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookPricesJob.API.dll"]
