ARG DOTNET_VER=9.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VER} AS buildbase

WORKDIR /src
COPY BookPricesJob.sln .
COPY BookPricesJob.API/ BookPricesJob.API/
COPY BookPricesJob.Common/ BookPricesJob.Common/
COPY BookPricesJob.Data/ BookPricesJob.Data/
COPY BookPricesJob.Application/ BookPricesJob.Application/
WORKDIR /src/BookPricesJob.API/
RUN dotnet restore

FROM buildbase AS migrations
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
WORKDIR /src/BookPricesJob.API/
ENTRYPOINT ["dotnet-ef", "database", "update", "--project", "/src/BookPricesJob.Data", "--startup-project", "/src/BookPricesJob.API", "--context", "DatabaseContextMysql"]
