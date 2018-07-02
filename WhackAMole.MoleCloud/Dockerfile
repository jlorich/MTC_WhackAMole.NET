FROM microsoft/aspnetcore:2.0.0-jessie
ARG pod_color=7FC9FF
ENV POD_COLOR=$pod_color
WORKDIR /app
EXPOSE 8080
COPY ./obj/Docker/publish .
ENTRYPOINT ["dotnet", "WhackAMole.MoleCloud.dll", "--urls", "http://*:8080"]
