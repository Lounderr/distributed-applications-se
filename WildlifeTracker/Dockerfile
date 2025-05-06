# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
COPY *.csproj ./
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o out

COPY https-dev.pfx /https/https-dev.pfx

ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/https-dev.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=Test123!

EXPOSE 80
EXPOSE 443

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "WildlifeTracker.dll"]
