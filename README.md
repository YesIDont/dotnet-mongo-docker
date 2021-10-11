# ASP.NET + MongoDB tutorial

### Instructions worth noting
dotnet --version
dotnet new webapi -o Catalog
Data transfer objects
dotnet add package MongoDB.Driver
docker run -d --rm --name mongo -p 27017:27017 -v mongodbdata:/data/db mongo
docker exec -it [mongo|id] bash
docker ps
docker stop [docker id]
user-secrets init
user-secrets set MongoDbSettings:Password Pass#word1

#### Run mongo in docker container
docker run -d --rm --name mongo -p 27017:27017 -v mongodbdata:/data/db -e MONGO_INITDB_ROOT_USERNAME=mongoadmin -e MONGO_INITDB_ROOT_PASSWORD=Pass#word1 --network=itemsCatalog mongo

#### Build api into docker with seetings from Dockerfile
docker build -t catalog:v1 .
docker images
docker network create itemsCatalog
docker network ls (to see running docker networks)
docker login/docker logout

#### Run api in docker
docker run -it --rm -p 8080:80 -e MongoDbSettings:Host=mongo -e MongoDbSettings:Password=Pass#word1 --network=itemsCatalog catalog:v1

#### Publish docker image on hub.docker.com
Once you have an account:
- use docker cli to login
- you can retag current tag with ```docker tag catalog:v1 mateuszfryc/catalog:v1```
- ```docker push mateuszfryc/catalog:v1```

### Read more about:
- mongo
- docker
- C# async

### Notes
- docker pluging for vs code -> ```Add docker files``` and follow instructions
- dockerhub - registry of images provided by vendors that want their software to be used with docker, e.g. linux, other oses, database engines, programming environments etc.
- docker is well optimised, uses layered caching,
has efficient resource usage, starts quick, works in isolation, runs enywhere, is scalable
- 307 code - redirection
- Port for localhost that works on windows: appsettings.json : MongoDbSettings : Port = 192.168.99.100