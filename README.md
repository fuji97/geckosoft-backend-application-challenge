# Build and run with dotnet CLI
Project:
```
dotnet run --project GeckosoftBackend
```
Run test:
```
dotnet test
```
# Using Docker
Run project (SSL not working):
```
docker build -t geckosoft-backend -f GeckosoftBackend/Dockerfile .
docker run -p 8080:80 -p 4433:443 geckosoft-backend
```
Run tests:
```
docker build -t geckosoft-tests -f GeckosoftBackend.Tests/Dockerfile .
docker run geckosoft-tests
```
# OpenAPI (Swagger)
Swagger is accessible at the location /swagger/index.html (only on SSL)