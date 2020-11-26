@echo off
if "%1"=="local" goto :local
goto :heroku

:local
echo -- Build client app
npm run build --prefix ./DevApp

echo --- Remove any existing image
docker rmi dotnetify -f

echo --- Build a new image
docker build -t dotnetify -f ./Dockerfile . --build-arg aspnetenv=Production

echo --- Remove build images
docker image prune -f --filter label=stage=build
rd __tmp__ /q /s

echo --- Run a container on port 8080
docker run -it --rm -p:8080:80 --name dotnetify_8080 dotnetify

goto :end

:heroku
rem heroku login
call heroku container:login
call heroku container:push web -a dotnetify 
call heroku container:release web -a dotnetify 

:end