[xml]$xml = Get-Content .\WonderLab\WonderLab.csproj  
  
$versionNode = $xml.Project.SelectSingleNode("PropertyGroup/Version")  
$runtimeNode = $xml.Project.SelectSingleNode("PropertyGroup/TargetFramework")  

$version = $versionNode."#text"
$runtime = $runtimeNode."#text"
Write-Output "Version: $version"
Write-Output "Runtime: $runtime"

function build_win {
    param (
        [String]$Arch
    )
    
    Write-Output "build WonderLab.$version.$Arch.zip"

    $base_dir="./WonderLab.Desktop/bin/Release/$runtime/publish/$Arch"

    dotnet publish WonderLab.Desktop -p:PublishProfile=$Arch

    $exe_path="$base_dir/WonderLab.Desktop.exe"
    $zip_path="$base_dir/WonderLab.$version.$Arch.zip"
    Compress-Archive -Path $exe_path -DestinationPath $zip_path
    Write-Output "WonderLab.$version.$Arch.zip build done!"
}

build_win -Arch "win-x64"
build_win -Arch "win-arm64"