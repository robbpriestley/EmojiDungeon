### HOW TO USE ###
# 1) "cd /Users/wizard/DigitalWizardry/Projects/DungeonGenerator/"
# 2) "git tag" (determine next highest tag)
# 3) Call this script with next highest tag as arg "./build.sh v1.1.003"
# 4) If no new tag is needed, call with "latest" ex: "./build.sh latest"

VERSION=$1

echo "*** DOCKER BUILD BEGIN ***"

cd /Users/wizard/DigitalWizardry/Projects/DungeonGenerator/

echo "*** TAGGING & VERSIONING ***"

if [ $VERSION != "latest" ]; then
	git tag $VERSION
	git push --tags
else
	VERSION=$(git for-each-ref refs/tags --sort=-taggerdate --format='%(refname:short)' --count=1)
fi

git for-each-ref refs/tags --sort=-taggerdate --format='%(refname:short)' --count=1 > wwwroot/version.txt
echo $ASPNETCORE_ENVIRONMENT >> wwwroot/version.txt

echo "*** SET DOCKER IMAGE & REPOSITORY ***"

IMAGE=digitalwizardry/dungeongenerator
#REPO=284127806438.dkr.ecr.us-east-1.amazonaws.com/shoutexchange/service:$VERSION

echo "*** DOCKER BUILD ***"

docker build -t $IMAGE .

#echo "*** DOCKER TAG ***"

#docker tag $IMAGE:latest $REPO

#echo "*** AWS LOGIN ***"

#eval $(aws ecr get-login --region us-east-1)

#echo "*** DOCKER PUSH ***"

#docker push $REPO
#echo $REPO

echo "*** DOCKER BUILD END ***"