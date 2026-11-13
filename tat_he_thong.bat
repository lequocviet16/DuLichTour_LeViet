@echo off
chcp 65001 > nul
echo ===================================================
echo         ĐANG DỪNG HỆ THỐNG LEVIET TRAVEL
echo ===================================================

powershell -NoProfile -Command ^
  "$ports = 5000, 5173, 5174; ^
   foreach ($p in $ports) { ^
     $conn = Get-NetTCPConnection -LocalPort $p -State Listen -ErrorAction SilentlyContinue; ^
     if ($conn) { ^
       $pids = $conn | Select-Object -ExpandProperty OwningProcess -Unique; ^
       foreach ($id in $pids) { ^
         Write-Host \"Dang dung tien trinh PID $id tren port $p...\"; ^
         Stop-Process -Id $id -Force -ErrorAction SilentlyContinue; ^
       } ^
     } else { ^
       Write-Host \"Port $p da duoc giai phong.\"; ^
     } ^
   }"

echo ===================================================
echo   Đã giải phóng các cổng 5000, 5173, 5174!
echo ===================================================
pause
