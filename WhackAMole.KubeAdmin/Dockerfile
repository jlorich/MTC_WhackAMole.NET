FROM microsoft/aspnetcore:2.0.0-jessie
ARG source
WORKDIR /app
EXPOSE 80
COPY ./obj/Docker/publish .
ENTRYPOINT ["dotnet", "WhackAMole.KubeAdmin.dll", "--urls", "http://*:80"]
