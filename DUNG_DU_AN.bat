@echo off
chcp 65001 > nul
title DUNG HE THONG LEVIET TRAVEL

echo ====================================================================
echo        DANG DUNG CAC DICH VU LEVIET TRAVEL
echo ====================================================================
echo.

for %%p in (5000 5001) do (
    for /f "tokens=5" %%a in ('netstat -aon ^| findstr :%%p ^| findstr LISTENING') do (
        echo Dang tat tien trinh port %%p (PID: %%a)...
        taskkill /F /PID %%a >nul 2>&1
    )
)
taskkill /F /IM LeViet_Travel_API.exe >nul 2>&1

echo.
echo ====================================================================
echo          DA DUNG TOAN BO HE THONG THANH CONG!
echo ====================================================================
timeout /t 2 > nul
