FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Sharecode.Backend.Worker.Outbox/Sharecode.Backend.Worker.Outbox.csproj", "Sharecode.Backend.Worker.Outbox/"]
# ...

# Run restore
RUN dotnet restore "Sharecode.Backend.Worker.Outbox/Sharecode.Backend.Worker.Outbox.csproj"
COPY . .
WORKDIR "/src/Sharecode.Backend.Worker.Outbox"
RUN dotnet build "Sharecode.Backend.Worker.Outbox.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sharecode.Backend.Worker.Outbox.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Copy the email templates
COPY email-templates /app/email-templates/

ENTRYPOINT ["dotnet", "Sharecode.Backend.Worker.Outbox.dll"]
