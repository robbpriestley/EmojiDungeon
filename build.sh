echo "*** BEGIN TYPESCRIPT TRANSPILE ***"
tsc -p ./TypeScript/tsconfig.json --outDir ./wwwroot/js/
echo "*** END TYPESCRIPT TRANSPILE ***"
echo "*** BEGIN DOTNET BUILD ***"
dotnet build DungeonGenerator.csproj
echo "*** END DOTNET BUILD ***"