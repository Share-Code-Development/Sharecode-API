#!/bin/bash

cd ..

# Check if dotnet is installed and is version 8
dotnet_version=$(dotnet --version 2>&1)
if [[ $dotnet_version == *"8"* ]]; then
    # Run the migrations script
    dotnet ef database update --startup-project Sharecode.Backend.Api --project Sharecode.Backend.Infrastructure
else
    # Throw an error and exit
    echo "Error: dotnet version 8 is required."
    exit 1
fi
