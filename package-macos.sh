#!/bin/bash

ROOT_DIR="$(pwd)"
PROJECT_DIR="$(pwd)/src/ProjectFrenzy"
# echo $PROJECT_DIR/bin/Release


echo "Preparation"
mkdir -p ./publish/app
rm -rv $PROJECT_DIR/bin/Release
rm -rv $PROJECT_DIR/obj/Release

echo "Publishing"
dotnet publish -c Release --self-contained -r osx-x64 -o $ROOT_DIR/dist/osx/x64 -v q --nologo $PROJECT_DIR
mv -v $ROOT_DIR/dist/osx/x64/Obfuscator_Output/*.dll $ROOT_DIR/dist/osx/x64
rm -rv $ROOT_DIR/dist/osx/x64/Obfuscator_Output

echo "Obfuscating"
cd $ROOT_DIR/dist/osx/x64 && $ROOT_DIR/resources/Obfuscar.ConsoleUI obfuscar.xml

echo "Bundling app"
cd $PROJECT_DIR && dotnet msbuild -t:BundleApp -p:RuntimeIdentifier=osx-x64 -property:Configuration=Release

echo "Moving packages app to final dir"
mv $PROJECT_DIR/bin/Release/netcoreapp3.1/osx-x64/publish/ProjectFrenzy.app $ROOT_DIR/publish/app

echo "Copying obfuscated assemblies"
cp -v $ROOT_DIR/dist/osx/x64/ProjectFrenzy.dll $ROOT_DIR/publish/app/ProjectFrenzy.app/Contents/MacOS

# hdiutil create -srcfolder "${source}" -volname "${title}" -fs HFS+
#       -fsargs "-c c=64,a=16,e=16" -format UDRW -size ${size}k pack.temp.dmg


# cd ./src/ProjectFrenzy && dotnet msbuild -t:BundleApp -p:RuntimeIdentifier=osx-x64 -property:Configuration=Release