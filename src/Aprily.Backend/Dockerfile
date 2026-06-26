FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/Aprily.Backend/Aprily.Backend.csproj src/Aprily.Backend/
RUN dotnet restore src/Aprily.Backend/Aprily.Backend.csproj
COPY src/Aprily.Backend/ src/Aprily.Backend/
RUN dotnet publish src/Aprily.Backend/Aprily.Backend.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

FROM runtime AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Aprily.Backend.dll"]
