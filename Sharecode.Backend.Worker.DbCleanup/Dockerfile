# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Sharecode.Backend.Worker.DbCleanup/Sharecode.Backend.Worker.DbCleanup.csproj", "Sharecode.Backend.Worker.DbCleanup/"]
RUN dotnet restore "Sharecode.Backend.Worker.DbCleanup/Sharecode.Backend.Worker.DbCleanup.csproj"
COPY . .
WORKDIR "/src/Sharecode.Backend.Worker.DbCleanup"
RUN dotnet build "Sharecode.Backend.Worker.DbCleanup.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Sharecode.Backend.Worker.DbCleanup.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Sharecode.Backend.Worker.DbCleanup.dll"]
