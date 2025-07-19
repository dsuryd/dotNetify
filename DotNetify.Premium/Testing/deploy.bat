@echo off
if "%1"=="local" goto :local
goto :heroku

:local
echo --- Remove any existing image
docker rmi test-server -f

echo --- Build a new image
docker build -t test-server -f ./Dockerfile . --build-arg aspnetenv=Production

echo --- Remove build images
docker image prune -f --filter label=stage=build
rd __tmp__ /q /s

echo --- Run a container on port 8080
docker run -it --rm -p:8080:80 --name testserver_8080 test-server

goto :end

:heroku
rem heroku login
call heroku container:login
call heroku container:push web -a dotnetify-test-server
call heroku container:release web -a dotnetify-test-server

:end