FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY FlightBoard.Api/*.csproj ./FlightBoard.Api/
RUN dotnet restore FlightBoard.Api/FlightBoard.Api.csproj

COPY . .
WORKDIR /src/FlightBoard.Api
RUN dotnet publish FlightBoard.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "FlightBoard.Api.dll"]
