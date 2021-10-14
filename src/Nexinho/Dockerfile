# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.csproj .
RUN dotnet restore -r linux-arm

# copy and publish app and libraries
COPY . .
RUN dotnet publish -c release -o /app -r linux-arm --self-contained false --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:3.1-bullseye-slim-arm32v7
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Nexinho.dll"]