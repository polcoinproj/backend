# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY ./ ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/out .

# Generate HS256 key for JWT
RUN mkdir -p ./store
RUN dd if=/dev/urandom of=./store/jwtHS256.key count=64 bs=1

ENTRYPOINT ["dotnet", "backend.dll"]