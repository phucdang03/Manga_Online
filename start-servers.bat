@echo off
echo Starting MangaOnline Services...
echo.

echo Starting Service.MangaOnline API Server (Port 5098)...
start "API Server" cmd /c "cd /d \"d:\.NET\Manga\MangaOnline\Service.MangaOnline\" && dotnet run"

echo Waiting 5 seconds for API server to start...
timeout /t 5 /nobreak >nul

echo Starting Client.Manager Web Server (Port 5030)...
start "Web Client" cmd /c "cd /d \"d:\.NET\Manga\MangaOnline\Client.Manager\" && dotnet run"

echo.
echo Both servers are starting...
echo - API Server: http://localhost:5098/swagger
echo - Web Client: http://localhost:5030
echo.
echo Press any key to close this window...
pause >nul