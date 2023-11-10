#!/bin/bash

cd ..

# Check for optional parameter -d or -delete
deleteMigrations=false
if [ "$1" == "-d" ] || [ "$1" == "-delete" ]; then
    deleteMigrations=true
fi

# Change directory to 'Sharecode.Backend.Infrastructure'
cd Sharecode.Backend.Infrastructure

# Check if the directory exists
if [ ! -d . ]; then
    # Throw an error and exit if the directory doesn't exist
    echo "Error: 'Sharecode.Backend.Infrastructure' directory not found."
    exit 1
fi

# Check if dotnet 8 is installed
dotnet_version=$(dotnet --version 2>&1)
if [[ $dotnet_version != *"8"* ]]; then
    # Throw an error and exit if dotnet 8 is not installed
    echo "Error: dotnet version 8 is required."
    exit 1
fi

# Run the 'dotnet ef remove' command or delete 'Migrations' folder
if [ "$deleteMigrations" = true ]; then
    echo "Deleting 'Migrations' folder..."
    rm -rf Migrations
else
    dotnet ef remove
fi
