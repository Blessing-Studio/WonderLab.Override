#!/bin/bash

file_content=$(cat ./WonderLab/WonderLab.csproj)
branch=$(echo "$file_content" | grep -oPm1 "(?<=<Branch>)[^<]+")
version=$(echo "$file_content" | grep -oPm1 "(?<=<Version>)[^<]+")

echo "Version: $version"
echo "Branch: $branch"

build_osx() {
    echo "build WonderLab-"
}