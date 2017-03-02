FROM microsoft/dotnet:latest

# This technique speeds up the build by decoupling the restore from any basic code changes.
# http://blogs.msmvps.com/theproblemsolver/2016/03/01/turbocharging-docker-build/
# Fixed bug. If "COPY . ./app/" like example gave, it incorrecly creates additional subdirectory.

COPY ./LevelGenerator.csproj ./LevelGenerator/
WORKDIR ./LevelGenerator/
RUN dotnet restore
COPY . /LevelGenerator/
RUN dotnet build

EXPOSE 5000/tcp
ENTRYPOINT ["dotnet", "run"]