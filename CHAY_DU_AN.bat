@echo off
chcp 65001 > nul
title LEVIET TRAVEL - KHOI DONG HE THONG

echo ====================================================================
echo        KHOI DONG DU AN LEVIET TRAVEL (ASP.NET CORE 8.0 N-TIER)
echo ====================================================================
echo.
echo [1/2] Dang khoi dong Backend API (LeViet_Travel_API :5001)...
start "LeViet_Travel_API - Cong 5001" cmd /k "dotnet run --project LeViet_Travel_API/LeViet_Travel_API.csproj --urls=http://localhost:5001"

timeout /t 3 /nobreak > nul

echo [2/2] Dang khoi dong API Gateway & Web Frontend (:5000)...
start "API_Gateway - Cong 5000" cmd /k "dotnet run --project API_Gateway/API_Gateway.csproj --urls=http://localhost:5000"

timeout /t 3 /nobreak > nul

echo.
echo ====================================================================
echo                 HE THONG DA KHOI CHAY THANH CONG!
echo ====================================================================
echo - Website Khach Hang:       http://localhost:5000
echo - Website Quan Tri (Admin): http://localhost:5000/admin.html
echo - Tai Lieu Swagger API:     http://localhost:5001/swagger
echo.
echo Dang mo trinh duyet...
start http://localhost:5000

echo.
echo Nhan phim bat ky hoac chay file DUNG_DU_AN.bat de dong he thong.
pause > nul
