@echo off
chcp 65001 > nul
title LEVIET TRAVEL - KHOI DONG HE THONG

echo ====================================================================
echo        KHOI DONG DU AN LEVIET TRAVEL (ASP.NET CORE 8.0 N-TIER)
echo ====================================================================
echo.
echo Dang khoi dong may chu Web & API tai cong 5000...
echo.

start "LeViet_Travel_API - Cong 5000" cmd /k "dotnet run --project LeViet_Travel_API/LeViet_Travel_API.csproj --urls=http://localhost:5000"

timeout /t 3 /nobreak > nul

echo ====================================================================
echo                 HE THONG DA KHOI CHAY THANH CONG!
echo ====================================================================
echo - Website Khach Hang:       http://localhost:5000
echo - Website Quan Tri (Admin): http://localhost:5000/admin/
echo - Tai Lieu Swagger API:     http://localhost:5000/swagger
echo.
echo Dang mo trinh duyet...
start http://localhost:5000

echo.
echo Nhan phim bat ky de thoat cua so nay (Backend van tiep tuc chay).
echo Khi muon tat may chu, hay chay file DUNG_DU_AN.bat.
pause > nul
