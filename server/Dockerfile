FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish "StrDss.Api/StrDss.Api.csproj" -c Release -o /app/published-app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/published-app .
EXPOSE 8080
ENV TZ America/Vancouver
ENTRYPOINT ["dotnet", "StrDss.Api.dll"]