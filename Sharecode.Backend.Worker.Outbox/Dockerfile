# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Sharecode.Backend.Worker.Outbox/Sharecode.Backend.Worker.Outbox.csproj", "Sharecode.Backend.Worker.Outbox/"]
RUN dotnet restore "Sharecode.Backend.Worker.Outbox/Sharecode.Backend.Worker.Outbox.csproj"
COPY . .
WORKDIR "/src/Sharecode.Backend.Worker.Outbox"
RUN dotnet build "Sharecode.Backend.Worker.Outbox.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Sharecode.Backend.Worker.Outbox.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Copy the email templates
COPY email-templates /app/email-templates/

ENTRYPOINT ["dotnet", "Sharecode.Backend.Worker.Outbox.dll"]
