FROM microsoft/aspnetcore:2.0.0-jessie
ARG source
WORKDIR /app
EXPOSE 8080
COPY ./obj/Docker/publish .
ENTRYPOINT ["dotnet", "WhackAMole.MoleCloud.dll", "--urls", "http://*:8080"]
