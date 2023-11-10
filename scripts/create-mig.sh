#!/bin/bash

cd ..

# Check for optional parameter and set it to initial-migration if not provided
migrationName="${1:-initial-migration}"

# Check if dotnet is installed and is version 8
dotnet_version=$(dotnet --version 2>&1)
if [[ $dotnet_version == *"8"* ]]; then
    # Run the migrations script
    dotnet ef migrations add "$migrationName" --startup-project Sharecode.Backend.Api --project Sharecode.Backend.Infrastructure
else
    # Throw an error and exit if dotnet 7 is not installed
    echo "Error: dotnet version 8 is required."
    exit 1
fi
