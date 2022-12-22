#!/bin/bash -e

if [[ -z "${PKG_VER}" ]]; then
    echo "PKG_VER env var not present. Aborting"
    exit
fi

dotnet build -c Release Src/LevinSharp.sln -p:PackageVersion=${PKG_VER}
dotnet nuget push Bin/Binaries/AngryWasp.LevinSharp.${PKG_VER}.nupkg --source https://api.nuget.org/v3/index.json