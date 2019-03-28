FROM microsoft/dotnet:2.2-aspnetcore-runtime        
WORKDIR /app
EXPOSE 80
COPY publish/ /app/
COPY app/ /app/
ENTRYPOINT ["dotnet", "AliasMailApi.dll"]