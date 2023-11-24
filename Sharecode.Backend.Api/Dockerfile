FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Sharecode.Backend.Api/Sharecode.Backend.Api.csproj", "Sharecode.Backend.Api/"]
# Copy other project references (adjust paths as necessary)
COPY ["Sharecode.Backend.Api/Sharecode.Backend.Application.csproj", "Sharecode.Backend.Application/"]
COPY ["Sharecode.Backend.Api/Sharecode.Backend.Domain.csproj", "Sharecode.Backend.Domain/"]
COPY ["Sharecode.Backend.Api/Sharecode.Backend.Infrastructure.csproj", "Sharecode.Backend.Infrastructure/"]
COPY ["Sharecode.Backend.Api/Sharecode.Backend.Presentation.csproj", "Sharecode.Backend.Presentation/"]
COPY ["Sharecode.Backend.Api/Sharecode.Backend.Utilities.csproj", "Sharecode.Backend.Utilities/"]
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
ENTRYPOINT ["dotnet", "Sharecode.Backend.Api.dll"]
