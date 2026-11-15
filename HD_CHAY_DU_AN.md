# HƯỚNG DẪN CHẠY DỰ ÁN LEVIET TRAVEL (.NET 8 & FRONTEND)

Dự án đã được cấu trúc lại hoàn toàn theo đúng mô hình chuẩn N-Tier (3 lớp mở rộng):
```text
LeViet_Travel/
├── API_Gateway/          # Dự án Gateway YARP (Port 5000)
├── BLL/                  # Tầng xử lý nghiệp vụ (Business Logic Layer)
├── DAL/                  # Tầng kết nối cơ sở dữ liệu (Data Access Layer - EF Core)
├── Models/               # Tầng thực thể (Entities, DTOs, Responses)
├── LeViet_Travel_API/    # Dự án Web API chính (Port 5001, Controllers, Swagger)
├── Frontend/             # Giao diện Web (HTML5, CSS3, JavaScript)
├── database/             # Script tạo database SQL Server
├── postman/              # Bộ test Postman API
├── BLL.cs                # File tham chiếu tầng BLL
├── LeViet_Travel_API.sln # File Solution chuẩn Visual Studio
├── CHAY_DU_AN.bat        # 1-Click chạy toàn bộ hệ thống
└── DUNG_DU_AN.bat        # 1-Click tắt hệ thống
```

---

## CÁCH 1: KHỞI ĐỘNG NHANH NHẤT (KHUYẾN NGHỊ - 1 CLICK)

1. Nhấp đúp chuột vào file: **`CHAY_DU_AN.bat`**
2. Hệ thống sẽ tự động khởi động:
   - Backend API trên cổng `http://localhost:5001`
   - API Gateway trên cổng `http://localhost:5000`
   - Tự động mở trình duyệt web lên màn hình tại: **`http://localhost:5000`**
3. Khi muốn tắt hệ thống: Nhấp đúp chuột vào file **`DUNG_DU_AN.bat`**.

---

## CÁCH 2: CHẠY BẰNG VISUAL STUDIO 2022

1. Nhấp đúp mở file **`LeViet_Travel_API.sln`** bằng Visual Studio 2022.
2. Tại cửa sổ **Solution Explorer**:
   - Nhấp chuột phải vào dự án **`LeViet_Travel_API`** ➔ Chọn **Set as Startup Project**.
   - Hoặc nhấp chuột phải vào Solution ➔ Chọn **Configure Startup Projects...** ➔ Chọn **Multiple startup projects** và chọn `Start` cho cả `LeViet_Travel_API` và `API_Gateway`.
3. Bấm nút **Start** (hoặc phím **F5** / **Ctrl + F5**) để chạy.
4. Mở trình duyệt truy cập:
   - Website: `http://localhost:5000` hoặc `http://localhost:5001`
   - Swagger API: `http://localhost:5001/swagger`

---

## CÁCH 3: MỞ TRỰC TIẾP FILE FRONTEND TRONG TRÌNH DUYỆT

1. Chạy Backend API trước bằng cách mở Terminal và gõ:
   ```powershell
   dotnet run --project LeViet_Travel_API
   ```
2. Vào thư mục **`Frontend/`**:
   - Nhấp đúp vào file **`index.html`** để xem giao diện Khách hàng.
   - Nhấp đúp vào file **`admin.html`** để xem giao diện Quản trị viên.
   *(File `api.js` đã được lập trình thông minh để tự động nhận diện và gọi API về backend mà không bị lỗi).*

---

## TÀI KHOẢN ĐĂNG NHẬP MẪU

| Vai trò | Email | Mật khẩu | Chức năng |
| :--- | :--- | :--- | :--- |
| **Quản trị viên (Admin)** | `admin@leviet.com` | `Admin@123` | Quản trị tour, duyệt đơn đặt tour, test quyền RBAC |
| **Khách hàng (Customer)** | `khachhang@gmail.com` | `Customer@123` | Tìm kiếm tour, đặt tour, xem mã VietQR thanh toán |

*(Hệ thống đã tích hợp cơ chế tự động Seed dữ liệu mẫu và tự động Fallback sang In-Memory Database nếu máy tính của bạn chưa bật SQL Server, đảm bảo chắc chắn không bị crash khi chạy).*
