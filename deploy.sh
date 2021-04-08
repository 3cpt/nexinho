#!/bin/sh

git pull
dotnet build -c Release

cp -f ../configs/appsettings.json src/Nexinho

dotnet src/Nexinho/bin/Release/net5.0/Nexinho.dll

