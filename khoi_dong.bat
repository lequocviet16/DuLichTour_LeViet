@echo off
chcp 65001 > nul
echo ===================================================
echo        ĐANG KHỞI ĐỘNG HỆ THỐNG LEVIET TRAVEL
echo ===================================================

echo [1/3] Đang khởi động Backend REST API (Port 5000)...
start "LeViet Travel - Backend API" cmd /k "cd /d %~dp0backend && npm run dev"

timeout /t 3 /nobreak > nul

echo [2/3] Đang khởi động Trang Quản Trị Admin (Port 5173)...
start "LeViet Travel - Admin Portal" cmd /k "cd /d %~dp0frontend-admin && npm run dev"

echo [3/3] Đang khởi động Website Khách Hàng (Port 5174)...
start "LeViet Travel - Client Website" cmd /k "cd /d %~dp0frontend-client && npm run dev"

echo ===================================================
echo   Hệ thống đã khởi động trong 3 cửa sổ riêng biệt!
echo   - Backend API:     http://localhost:5000/api/health
echo   - Cổng Quản Trị:   http://localhost:5173
echo   - Website Khách:   http://localhost:5174
echo ===================================================
pause
