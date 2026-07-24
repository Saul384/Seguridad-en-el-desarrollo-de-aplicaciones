FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build-env
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine
WORKDIR /App

RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "VulnerableApp.dll"]
