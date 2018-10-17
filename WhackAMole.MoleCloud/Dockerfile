FROM microsoft/dotnet:2.1.1-aspnetcore-runtime-stretch-slim
ARG pod_color=7FC9FF
ENV POD_COLOR=$pod_color
WORKDIR /app
EXPOSE 8080
COPY ./bin/Debug/netcoreapp2.1/publish/ .
ENTRYPOINT ["dotnet", "WhackAMole.MoleCloud.dll", "--urls", "http://*:8080"]
