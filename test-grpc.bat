@echo off
echo Starting gRPC Server...
start "gRPC Server" /D "grpc-project" dotnet run

echo Waiting for server to start...
timeout /t 5 /nobreak >nul

echo Starting gRPC Client...
cd grpc-client
dotnet run

echo.
echo Test completed. Press any key to exit...
pause >nul