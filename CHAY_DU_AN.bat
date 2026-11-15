@echo off
chcp 65001 > nul
title LEVIET TRAVEL - KHOI DONG HE THONG

echo ====================================================================
echo        KHOI DONG DU AN LEVIET TRAVEL (ASP.NET CORE 8.0 N-TIER)
echo ====================================================================
echo.

echo [1/3] Kiem tra va giai phong tien trinh cu (neu co)...
for /f "tokens=5" %%a in ('netstat -aon ^| findstr :5000 ^| findstr LISTENING 2^>nul') do (
    echo - Dang tat tien trinh port 5000 (PID: %%a)...
    taskkill /F /PID %%a >nul 2>&1
)
taskkill /F /IM LeViet_Travel_API.exe >nul 2>&1
timeout /t 1 /nobreak > nul

echo.
echo [2/3] Dang khoi dong may chu Web & API tai http://localhost:5000...
start "LeViet_Travel_API - Cong 5000" cmd /k "dotnet run --project LeViet_Travel_API/LeViet_Travel_API.csproj --urls=http://localhost:5000"

timeout /t 3 /nobreak > nul

echo.
echo [3/3] He thong da khoi chay thanh cong!
echo ====================================================================
echo - Website Khach Hang:       http://localhost:5000
echo - Website Quan Tri (Admin): http://localhost:5000/admin/
echo - Tai Lieu Swagger API:     http://localhost:5000/swagger
echo ====================================================================
echo.
echo Dang mo trinh duyet...
start http://localhost:5000

echo.
echo Nhan phim bat ky de thoat cua so nay (Backend van tiep tuc chay).
echo Khi muon tat may chu, hay chay file DUNG_DU_AN.bat.
pause > nul
