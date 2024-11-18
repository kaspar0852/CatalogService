# Use the official .NET SDK as a build environment
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# Install dotnet-ef tool
RUN dotnet tool install -g dotnet-ef

# Make sure global tools are available in PATH
ENV PATH="$PATH:/root/.dotnet/tools"

# Copy and restore project dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the source code and publish the application
COPY . ./
RUN rm -f appsettings.json
RUN dotnet publish -c Release -o out

# Use the .NET ASP.NET Core runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build-env /app/out .

# Expose the application port
EXPOSE 80

# Set environment variables and define the entry point
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "CatalogService.dll"]