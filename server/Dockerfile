FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -o /app/published-app

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/published-app .
EXPOSE 8080
ENV TZ America/Vancouver
ENTRYPOINT ["dotnet", "AdvSol.dll"]