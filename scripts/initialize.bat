@echo off
echo Initializing environment...

if exist ..\.env (
    echo .env already exists
    exit /b 0
)

copy ..\.env.dev ..\.env

echo .env created from .env.dev