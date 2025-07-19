FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
LABEL stage=build
WORKDIR /src
COPY . .

RUN dotnet restore -s https://api.nuget.org/v3/index.json -s ./local-nuget
RUN dotnet publish AppServer.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:3.1-buster-slim
EXPOSE 443
WORKDIR /app
COPY --from=build /app .
ARG aspnetenv=Production
ARG observerurl=https://dotnetify-observer.herokuapp.com

ENV OBSERVER_URL ${observerurl}
ENV ASPNETCORE_ENVIRONMENT ${aspnetenv}
CMD ASPNETCORE_URLS=http://*:$PORT dotnet AppServer.dll
