#!/bin/sh

git pull
dotnet build -c Release

#cp -f ../configs/appsettings.json src/Nexinho/bin/Release/net5.0/

cd src/Nexinho/bin/Release/net5.0/

read -p "PRESS ENTER TO START"

dotnet Nexinho.dll

