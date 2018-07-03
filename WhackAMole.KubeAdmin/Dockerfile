FROM microsoft/aspnetcore:2.0.0-jessie
WORKDIR /app
EXPOSE 8080
COPY ./obj/Docker/publish .
ENTRYPOINT ["dotnet", "WhackAMole.KubeAdmin.dll", "--urls", "http://*:8080"]
