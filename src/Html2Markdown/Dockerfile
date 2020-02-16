FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env

WORKDIR /source

COPY *.csproj ./

RUN dotnet restore

COPY . .
RUN dotnet build