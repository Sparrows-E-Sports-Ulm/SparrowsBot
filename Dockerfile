FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /App
COPY src ./
RUN dotnet restore
RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/runtime:7.0
WORKDIR /App
COPY --from=build-env /App/Bot/bin/Release/net7.0 ./Bot 
ENTRYPOINT ["dotnet", "Bot/Bot.dll"]