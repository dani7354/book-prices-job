# book-prices-job
![.NET test (xunit)](https://github.com/dani7354/book-prices-job/actions/workflows/10-test.yml/badge.svg)
![Docker Image Build and Push](https://github.com/dani7354/book-prices-job/actions/workflows/20-build-docker-image.yml/badge.svg)

## Docker

### Local development
To build the images and start docker locally for testing, run the command:\
`docker compose -f compose.yaml compose.dev.yaml up --build`

### Swarm
`docker swarm init`\
`docker stack deploy -c compose.yaml -c compose.prod.yaml bookprices_jobapi` \
"bookprices_jobapi" is the name chosen for the stack and is used as prefix in the service names.

You can check if all the services are running properly by running: \
`docker service ls`

It is possible to connect more nodes to the swarm and scale the services by creating multiple instances (containers) of each service.

`docker service scale api=2` \
"api" refers to the name of the service, which is found by listing the running services.

## Testing
Change to test directory:\
`cd BookPricesJob.Test/`

Run all:\
`dotnet test`

See all tests available:\
`dotnet test --list-tests`

Run filtered:\
`dotnet test  --filter "FullyQualifiedName~BookPricesJob.Test.IntegrationTest"`
