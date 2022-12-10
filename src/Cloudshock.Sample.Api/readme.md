# Todo API

## Getting Started

1. dotnet dev-certs https --trust

## Local Run (within Cloudshock.Sample.Api folder)

```bash
dotnet run
open https://localhost:7011/swagger/index.html
```
### Run As Production

```bash
dotnet run --environment Production
```
## Docker Build Web Only (within Cloudshock.Sample.Api folder)

```bash
docker build -t Cloudshock.Sample.Api .
docker run -d -p 8080:80 --name myapp Cloudshock.Sample.Api
docker rm myapp  
```
## Docker Compose (root levelcd)

```bash
docker-compose build
docker-compose up
```
