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
docker rmi proxyserver -f

echo --- Build a new image
docker build -t proxyserver -f ./Dockerfile . 

echo --- Remove build images
docker image prune -f --filter label=stage=build --build-arg observerurl=http://host.docker.internal:9000 appserverurl=http://host.docker.internal:6100

echo --- Run a container on port 5100
docker run -it --rm -p5100:80 --name proxyserver_5100 proxyserver

goto :end

:heroku
rem heroku login
call heroku container:login
call heroku container:push web -a dotnetify-proxy1 
call heroku container:release web -a dotnetify-proxy1  

call heroku container:push web -a dotnetify-proxy2 
call heroku container:release web -a dotnetify-proxy2  
:end