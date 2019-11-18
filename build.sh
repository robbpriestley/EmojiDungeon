echo "*** BEGIN TYPESCRIPT TRANSPILE ***"
tsc -p ./wwwroot/js/tsconfig.json --outDir ./wwwroot/js/
echo "*** END TYPESCRIPT TRANSPILE ***"
echo "*** BEGIN DOTNET BUILD ***"
dotnet build EmojiDungeon.csproj
echo "*** END DOTNET BUILD ***"