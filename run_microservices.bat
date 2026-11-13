@echo off
chcp 65001 > nul
echo ====================================================================
echo        KHỞI ĐỘNG HỆ THỐNG LEVIET TRAVEL ASP.NET 8.0 MICROSERVICES
echo ====================================================================
echo.

echo [1/4] Đang khởi động AuthService (Cổng 5001)...
start "AuthService - Cổng 5001" cmd /k "dotnet run --project src/Services/AuthService/LeVietTravel.AuthService/LeVietTravel.AuthService.csproj --urls=http://localhost:5001"

timeout /t 3 /nobreak > nul

echo [2/4] Đang khởi động TourService (Cổng 5002)...
start "TourService - Cổng 5002" cmd /k "dotnet run --project src/Services/TourService/LeVietTravel.TourService/LeVietTravel.TourService.csproj --urls=http://localhost:5002"

timeout /t 2 /nobreak > nul

echo [3/4] Đang khởi động BookingService (Cổng 5003)...
start "BookingService - Cổng 5003" cmd /k "dotnet run --project src/Services/BookingService/LeVietTravel.BookingService/LeVietTravel.BookingService.csproj --urls=http://localhost:5003"

timeout /t 2 /nobreak > nul

echo [4/4] Đang khởi động API Gateway & Web Frontend (Cổng 5000)...
start "ApiGateway - Cổng 5000" cmd /k "dotnet run --project src/Gateway/LeVietTravel.ApiGateway/LeVietTravel.ApiGateway.csproj --urls=http://localhost:5000"

timeout /t 3 /nobreak > nul

echo.
echo ====================================================================
echo                 TẤT CẢ MICROSERVICES ĐÃ KHỞI CHẠY!
echo ====================================================================
echo - Giao diện Web Khách hàng (HTML/CSS/JS): http://localhost:5000
echo - Giao diện Quản trị viên (Admin):        http://localhost:5000/admin.html
echo - Swagger Auth Service:                   http://localhost:5001/swagger
echo - Swagger Tour Service:                   http://localhost:5002/swagger
echo - Swagger Booking Service:                http://localhost:5003/swagger
echo.
echo Tự động mở trình duyệt...
start http://localhost:5000
pause
