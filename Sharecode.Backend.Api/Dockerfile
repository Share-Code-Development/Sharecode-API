# Use the ASP.NET Core runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the SDK image to build the solution
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the main API csproj file and restore its dependencies
COPY ["Sharecode.Backend.Api/Sharecode.Backend.Api.csproj", "Sharecode.Backend.Api/"]
# Copy and restore dependencies of all projects in the solution
COPY ["Sharecode.Backend.Application/Sharecode.Backend.Application.csproj", "Sharecode.Backend.Application/"]
COPY ["Sharecode.Backend.Domain/Sharecode.Backend.Domain.csproj", "Sharecode.Backend.Domain/"]
COPY ["Sharecode.Backend.Infrastructure/Sharecode.Backend.Infrastructure.csproj", "Sharecode.Backend.Infrastructure/"]
COPY ["Sharecode.Backend.Presentation/Sharecode.Backend.Presentation.csproj", "Sharecode.Backend.Presentation/"]
COPY ["Sharecode.Backend.Utilities/Sharecode.Backend.Utilities.csproj", "Sharecode.Backend.Utilities/"]

# Restore the entire solution
RUN dotnet restore "Sharecode.Backend.Api/Sharecode.Backend.Api.csproj"

# Copy the entire solution to the build context
COPY . .

# Set the working directory to the main API project
WORKDIR "/src/Sharecode.Backend.Api"

# Build the API project
RUN dotnet build "Sharecode.Backend.Api.csproj" -c Release -o /app/build

# Publish the API project
FROM build AS publish
RUN dotnet publish "Sharecode.Backend.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Sharecode.Backend.Api.dll"]
