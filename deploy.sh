#!/bin/sh

git pull
dotnet build -c Release
dotnet src/Nexinho/bin/Release/net5.0/Nexinho.dll