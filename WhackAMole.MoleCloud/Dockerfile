FROM microsoft/aspnetcore:2.0.0-jessie
ARG source
WORKDIR /app
EXPOSE 80
COPY ${source:-bin/Release/netcoreapp2.0/publish} .
ENTRYPOINT ["dotnet", "WhackAMole.MoleCloud.dll"]
