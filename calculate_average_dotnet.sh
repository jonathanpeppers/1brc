#!/bin/sh

dotnet build -c Release ./dotnet-src/BillionRowChallenge/BillionRowChallenge.csproj
time dotnet ./dotnet-src/BillionRowChallenge/bin/Release/net8.0/BillionRowChallenge.dll