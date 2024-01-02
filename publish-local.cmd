
dotnet restore RegexRename.sln

dotnet build RegexRename.sln --configuration Release --no-restore

dotnet test "test\RegexRename.Tests\RegexRename.Tests.csproj" --configuration Release --no-build

dotnet publish "src\RegexRename\RegexRename.csproj" --configuration Release --output "./publish" --no-build /p:EnvironmentName=local /p:DebugType=None /p:DebugSymbols=false

@ECHO Replacing 'appsettings.json' with 'appsettings.local.json'
@PUSHD "./publish"

@DEL /q appsettings.json
@REN appsettings.local.json appsettings.json

@POPD
