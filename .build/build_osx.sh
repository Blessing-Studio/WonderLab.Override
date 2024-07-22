#!/bin/bash

file_content=$(cat ./WonderLab/WonderLab.csproj)
branch=$(echo "$file_content" | sed -n 's/.*<Branch>\([^<]*\)<\/Branch>.*/\1/p')  
version=$(echo "$file_content" | sed -n 's/.*<Version>\([^<]*\)<\/Version>.*/\1/p')  
runtime=$(echo "$file_content" | sed -n 's/.*<TargetFramework>\([^<]*\)<\/TargetFramework>.*/\1/p') 

echo "Branch: $branch"
echo "Version: $version"
echo "RunTime: $runtime"

# WonderLab.Desktop.2.0.0.osx-x64.zip
# bin\Release\net8.0\publish\osx-arm64\
build_osx() {
    echo "build WonderLab.$branch.$version.$1.app.zip"

    base_dir="./WonderLab.Desktop/bin/Release/$runtime/publish/$1" 
    base_app_dir="$base_dir/WonderLab.app/Contents"
    zip_name="WonderLab.$branch.$version.$1.app.zip"

    dotnet publish ./WonderLab.Desktop -p:PublishProfile=$1

    mkdir $base_dir/WonderLab.app
    mkdir $base_app_dir

    resources_dir=$base_app_dir/Resources

    mkdir $resources_dir

    cp ./WonderLab.Desktop/Info.plist $base_app_dir
    cp ./WonderLab.Desktop/wonderlab.icns $resources_dir

    files=("libAvaloniaNative.dylib"
    "libHarfBuzzSharp.dylib"
    "libSkiaSharp.dylib"
    "WonderLab.Desktop.pdb"
    "WonderLab.Desktop")

    app_dir="$base_app_dir/MacOS"
    mkdir $app_dir

    for line in ${files[@]}
    do
        cp $base_dir/$line \
            $app_dir/$line
    done

    chmod a+x $app_dir/WonderLab.Desktop

    zip -r $zip_name $base_dir/WonderLab.app
    echo "$zip_name build done!"
}

build_osx osx-x64
build_osx osx-arm64