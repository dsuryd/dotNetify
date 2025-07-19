@echo off

rmdir .\local-nuget /s /q
xcopy C:\Users\dicky\.nuget\packages\dotnetify.core .\local-nuget\dotnetify.core\ /y /d /e
xcopy C:\Users\dicky\.nuget\packages\dotnetify.signalr .\local-nuget\dotnetify.signalr\ /y /d /e
xcopy C:\Users\dicky\.nuget\packages\dotnetify.observer .\local-nuget\dotnetify.observer\ /y /d /e
xcopy C:\Users\dicky\.nuget\packages\dotnetify.observer.client .\local-nuget\dotnetify.observer.client\ /y /d /e

if "%1"=="local" goto :local
goto :heroku

:local
echo --- Remove any existing image
docker rmi appserver -f

echo --- Build a new image
docker build -t appserver -f ./Dockerfile . --build-arg observerurl=http://host.docker.internal:9000

echo --- Remove build images
docker image prune -f --filter label=stage=build

echo --- Run a container on port 6100
docker run -it --rm -p6100:80 --name appserver_6100 appserver

goto :end

:heroku
rem heroku login
call heroku container:login
call heroku container:push web -a dotnetify-appserver 
call heroku container:release web -a dotnetify-appserver  

:end