FROM microsoft/dotnet:2.2-runtime-alpine
EXPOSE 80
ENV APP_CONFIGURATION=Debug
ENV ASPNETCORE_ENVIRONMENT=Development
COPY publish/ /app/
COPY app/ /app/

WORKDIR /app
ENTRYPOINT ["dotnet", "AliasMailApi.dll"]