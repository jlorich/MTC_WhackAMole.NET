FROM microsoft/dotnet:2.1.1-aspnetcore-runtime-stretch-slim
WORKDIR /app
EXPOSE 8080
COPY ./obj/Docker/publish .
ENTRYPOINT ["dotnet", "WhackAMole.KubeAdmin.dll", "--urls", "http://*:8080"]
