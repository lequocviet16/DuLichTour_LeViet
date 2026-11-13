@echo off
chcp 65001 > nul
echo ====================================================================
echo        DỪNG TẤT CẢ CÁC TIẾN TRÌNH LEVIET TRAVEL MICROSERVICES
echo ====================================================================

for %%p in (5000 5001 5002 5003) do (
    for /f "tokens=5" %%a in ('netstat -aon ^| findstr ":%%p" ^| findstr "LISTENING"') do (
        echo Dang tat tien trinh dang chay o cong %%p (PID: %%a)...
        taskkill /F /PID %%a >nul 2>&1
    )
)

echo Hoan tat dung he thong.
pause
