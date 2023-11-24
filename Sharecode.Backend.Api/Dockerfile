# Use the official Microsoft .NET runtime image as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the .NET SDK image to build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the main csproj and restore as distinct layers
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

# Copy everything else and build
COPY . .
WORKDIR "/src/Sharecode.Backend.Api"
RUN dotnet build "Sharecode.Backend.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sharecode.Backend.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sharecode.Backend.Api.dll"]
