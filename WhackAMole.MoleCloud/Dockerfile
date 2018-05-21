FROM microsoft/aspnetcore:2.0.0-jessie
ARG source
WORKDIR /app
EXPOSE 8080
COPY ./bin/Debug/netcoreapp2.0/publish .
ENTRYPOINT ["dotnet", "WhackAMole.MoleCloud.dll", "--urls", "http://*:8080"]
