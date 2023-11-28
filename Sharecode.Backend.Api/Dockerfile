FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Sharecode.Backend.Api/Sharecode.Backend.Api.csproj", "Sharecode.Backend.Api/"]
# ...

# Run restore
RUN dotnet restore "Sharecode.Backend.Api/Sharecode.Backend.Api.csproj"
COPY . .
WORKDIR "/src/Sharecode.Backend.Api"
RUN dotnet build "Sharecode.Backend.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sharecode.Backend.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Copy the email templates
COPY email-templates /app/email-templates/

ENTRYPOINT ["dotnet", "Sharecode.Backend.Api.dll"]
