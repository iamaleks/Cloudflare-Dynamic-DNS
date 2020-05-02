#!/bin/bash

declare -a runtimes=("win-x64" "win-x86" "win-arm" "win-arm64" "win7-x64" "win7-x86" "win81-x64" "win81-x86" "win81-arm" "win10-x64" "win10-x86" "win10-arm" "win10-arm64" "linux-x64" "linux-musl-x64" "linux-arm" "rhel-x64" "rhel.6-x64" "osx-x64" "osx.10.10-x64" "osx.10.11-x64" "osx.10.12-x64" "osx.10.13-x64" "osx.10.14-x64")

for i in "${runtimes[@]}"
do
   echo "Bulding for runtime: $i"
   dotnet publish -r $i -c Release
done