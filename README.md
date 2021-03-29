# ElectronNet-BlazorServer
Made an Electron Application that pulls data from the API and list the data with using Blazor front-end

# Database
I used a docker image that contains SQL-Server Northwind Database.

## How to pull and run the Norhtwind Database in a docker container.
docker run -d --name nw -p 1444:1433 kcornwall/sqlnorthwind
Make sure to clean you docker container, images, and volume

=> look for current containers
docker ps -a
=> delete containers
docker rm <container-id>

=> look for current images
docker images
=> delete images
docker rmi <image-id>

=> look for volumes
docker volume ls
=> delete volume
docker volume rm <volume-name>

## You can run it either as a Web Application or as an Electron Application

Web App

dotnet build
dotnet run

Electron App

electronize init
electronize start

## If you want to install the application as a Windows Desktop application

Run the following command:

electronize build /target win /PublishReadyToRun false

Go to folder bin -> Desktop -> exe

Run the exe file and it will download the application in your windows desktop
