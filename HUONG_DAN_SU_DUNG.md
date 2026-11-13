# TÀI LIỆU HƯỚNG DẪN VẬN HÀNH & SỬ DỤNG HỆ THỐNG LEVIET TRAVEL

Dự án **LeViet Travel** là hệ thống quản lý và bán tour du lịch trực tuyến gồm 3 phân hệ hoạt động độc lập kết nối với cơ sở dữ liệu MySQL:
1. **Backend REST API** (Node.js/Express/Sequelize) – Cổng dịch vụ trung tâm: `http://localhost:5000`
2. **Cổng Quản Trị - Admin** (React 19/Vite/Ant Design) – Dành cho ban quản trị và điều hành tour: `http://localhost:5173`
3. **Website Khách Hàng - Client** (React 19/Vite/Tailwind CSS) – Dành cho du khách tìm kiếm, đặt chỗ và thanh toán: `http://localhost:5174`

---

## MỤC LỤC
1. [Cách khởi động website](#1-cách-khởi-động-website)
2. [Tài khoản đăng nhập mẫu](#2-tài-khoản-đăng-nhập-mẫu)
3. [Hướng dẫn sử dụng các chức năng](#3-hướng-dẫn-sử-dụng-các-chức-năng)
   - [3.1. Dành cho Khách hàng (Website Client)](#31-dành-cho-khách-hàng-website-client)
   - [3.2. Dành cho Quản trị viên (Trang Quản trị Admin)](#32-dành-cho-quản-trị-viên-trang-quản-trị-admin)
4. [Cách sao lưu và phục hồi Cơ sở dữ liệu (Database)](#4-cách-sao-lưu-và-phục-hồi-cơ-sở-dữ-liệu-database)
5. [Cách tắt và mở lại hệ thống](#5-cách-tắt-và-mở-lại-hệ-thống)
6. [Xử lý sự cố thường gặp (Troubleshooting)](#6-xử-lý-sự-cố-thường-gặp-troubleshooting)

---

## 1. CÁCH KHỞI ĐỘNG WEBSITE

### 1.1. Yêu cầu môi trường tiên quyết
Trước khi khởi động, máy tính cần cài đặt sẵn:
- **Node.js**: Phiên bản 20.x hoặc 22.x LTS (khuyến nghị). Kiểm tra: `node -v`
- **npm**: Đi kèm Node.js. Kiểm tra: `npm -v`
- **MySQL Server**: Phiên bản 8.0 trở lên đang chạy cổng mặc định `3306`.

### 1.2. Kiểm tra cấu hình biến môi trường (`.env`)
Đảm bảo các file cấu hình đã sẵn sàng:
- **Backend** (`backend/.env`):
  ```dotenv
  PORT=5000
  NODE_ENV=development
  DB_HOST=localhost
  DB_PORT=3306
  DB_NAME=leviet_travel_db
  DB_USER=root
  DB_PASSWORD=MẬT_KHẨU_MYSQL_CỦA_BẠN
  JWT_SECRET=leviet_travel_super_secret_jwt_key_2026_at_least_32_chars
  ```
- **Frontend Client** (`frontend-client/.env`):
  ```dotenv
  VITE_API_BASE_URL=http://localhost:5000/api
  ```
- **Frontend Admin** (`frontend-admin/.env`): Mặc định trỏ về `http://localhost:5000/api`.

### 1.3. Khởi động hệ thống (Hằng ngày)
Vì dự án gồm 3 ứng dụng riêng biệt, bạn mở **3 cửa sổ Terminal (PowerShell)** tương ứng:

#### 🔹 Terminal 1: Khởi động Backend REST API
```powershell
cd D:\kienthuc\LeViet_Travel\backend
npm run dev
```
> Khi màn hình hiển thị: `Server running on port 5000` và kết nối database thành công, backend đã sẵn sàng.  
> Kiểm tra nhanh qua trình duyệt: `http://localhost:5000/api/health` trả về `{"status":"ok"}`.

#### 🔹 Terminal 2: Khởi động Cổng Quản Trị (Admin)
```powershell
cd D:\kienthuc\LeViet_Travel\frontend-admin
npm run dev
```
> Trình duyệt mở tại: **`http://localhost:5173`**

#### 🔹 Terminal 3: Khởi động Website Khách Hàng (Client)
```powershell
cd D:\kienthuc\LeViet_Travel\frontend-client
npm run dev
```
> Trình duyệt mở tại: **`http://localhost:5174`**

---

## 2. TÀI KHOẢN ĐĂNG NHẬP MẪU

Hệ thống đã chuẩn bị sẵn tài khoản demo phục vụ kiểm thử và trải nghiệm đầy đủ quyền hạn:

| Vai trò | Cổng đăng nhập | Email | Mật khẩu | Mục đích sử dụng |
| :--- | :--- | :--- | :--- | :--- |
| **Quản trị viên (Admin)** | `http://localhost:5173/login` | `admin@leviet.com` | `Admin@123` | Toàn quyền quản trị hệ thống, duyệt đơn, tạo tour, quản lý tài nguyên, xuất báo cáo. |
| **Khách hàng mẫu (Customer)** | `http://localhost:5174/login` | `khachhang@gmail.com` | `Customer@123` | Trải nghiệm đặt tour, thanh toán VietQR, áp mã voucher, quản lý đơn đặt, nhận vé QR, đánh giá tour. |
| **Khách hàng thử nghiệm khác** | `http://localhost:5174/login` | `testuser@gmail.com` | `Customer@123` | Thử nghiệm đặt tour độc lập với tài khoản chính. |

> **Lưu ý:** Du khách mới có thể bấm **Đăng ký** tài khoản trực tiếp tại website khách hàng (`http://localhost:5174/register`).

---

## 3. HƯỚNG DẪN SỬ DỤNG CÁC CHỨC NĂNG

### 3.1. Dành cho Khách hàng (Website Client - `http://localhost:5174`)

#### 1. Khám phá và Tìm kiếm Tour
- **Trang chủ (`/`)**: Xem các chương trình khuyến mãi banner, danh mục du lịch (Biển đảo, Núi rừng, Di sản văn hóa...), tour ưu đãi nổi bật, điểm đến yêu thích và voucher giảm giá công khai.
- **Trang Danh sách Tour (`/tours`)**:
  - Tìm kiếm theo từ khóa tên tour hoặc địa danh.
  - Sử dụng bộ lọc đa năng: Điểm đến, khoảng giá tiền, thời lượng chuyến đi (số ngày, số đêm), danh mục.
  - Sắp xếp theo giá tăng dần, giảm dần hoặc mức độ phổ biến.

#### 2. Xem Chi tiết Tour & Lưu yêu thích
- **Trang Chi tiết Tour (`/tours/:id`)**:
  - Xem thông tin tổng quan, thư viện hình ảnh và video hành trình thực tế.
  - Đọc lịch trình chi tiết theo từng ngày (Day-by-Day Itinerary).
  - Kiểm tra các chính sách hoàn hủy, bảo hiểm, ghi chú đồ dùng cá nhân.
  - Xem các đánh giá & phản hồi từ khách đi trước.
- **Tour Đã Lưu (`/saved-tours`)**: Bấm vào biểu tượng trái tim để lưu tour quan tâm hoặc thêm vào bảng so sánh giữa các chuyến đi.

#### 3. Quy trình Đặt Tour & Nhập Mã Giảm Giá
1. Tại trang chi tiết tour, chọn **Ngày khởi hành** còn mở.
2. Nhấn nút **Đặt ngay** để chuyển đến trang Đặt vé (`/booking/:tourId`).
3. Điền thông tin Người liên hệ (Họ tên, Số điện thoại, Email, Địa chỉ).
4. Chọn số lượng hành khách theo nhóm:
   - Người lớn (bắt buộc ít nhất 1 người).
   - Trẻ em, Em bé, Người cao tuổi (áp dụng tỷ lệ giá ưu đãi riêng theo từng nhóm).
   - Đăng ký phụ thu phòng đơn (nếu có nhu cầu ở riêng).
5. Nhập danh sách họ tên và năm sinh hành khách.
6. **Nhập mã giảm giá (Voucher)**: Nhập mã (ví dụ `LEVITWELCOME`), hệ thống tự động kiểm tra điều kiện và trừ trực tiếp vào tổng tiền.
7. Bấm **Xác nhận đặt tour**: Backend tự động khóa chỗ, sinh mã đơn `BK...` với trạng thái `PENDING_PAYMENT` và cấp 30 phút giữ chỗ.

#### 4. Thanh toán chuyển khoản VietQR
- Hệ thống tự động sinh mã VietQR chuẩn NAPAS chứa sẵn số tài khoản công ty, số tiền chính xác và nội dung chuyển khoản định danh đơn hàng (ví dụ: `LEVIET BK123456`).
- Khách hàng mở App ngân hàng quét mã QR để chuyển khoản.
- Sau khi chuyển tiền, khách có thể tải lên ảnh chụp biên lai/chứng từ giao dịch để nhân viên đối soát duyệt nhanh.

#### 5. Quản lý Đơn đặt & Nhận Vé QR (`/my-bookings`)
- **Theo dõi tiến độ đơn hàng**:
  - `PENDING_PAYMENT`: Đang giữ chỗ, chờ xác nhận thanh toán.
  - `CONFIRMED`: Đã thanh toán thành công, giữ chỗ chính thức.
  - `COMPLETED`: Chuyến đi đã hoàn thành xuất sắc.
  - `CANCELLED`: Đã hủy đơn.
- **Vé QR Check-in**: Khi đơn chuyển sang `CONFIRMED`, xuất hiện mã QR vé điện tử. Khách đưa mã này cho hướng dẫn viên quét lúc lên xe/khởi hành.
- **Thao tác hậu mãi**: Tự bấm hủy đơn khi chưa thanh toán, hoặc gửi phiếu yêu cầu đổi ngày khởi hành / xin hủy hoàn tiền.
- **Đánh giá tour**: Đánh giá 1-5 sao và để lại nhận xét sau khi đơn được xác nhận hoặc sau chuyến đi.

---

### 3.2. Dành cho Quản trị viên (Trang Quản trị Admin - `http://localhost:5173`)

#### 1. Bảng điều khiển Tổng quan (Dashboard - `/`)
- Báo cáo tổng doanh thu thực tế, số lượng đơn đặt, số khách hàng mới, số tour đang hoạt động.
- Biểu đồ biến động doanh thu theo tuần/tháng và cơ cấu trạng thái booking.

#### 2. Quản lý Danh mục & Sản phẩm Tour (`/catalog`)
- **Quản lý Danh mục**: Thêm mới danh mục du lịch, cập nhật mô tả, trạng thái hiển thị.
- **Quản lý Tour**:
  - Thêm tour mới, sửa đổi thông tin, thiết lập mã tour (`Tour Code`), thời gian đi.
  - Đăng tải bộ sưu tập hình ảnh, cập nhật link video YouTube giới thiệu tour.
  - Biên tập lịch trình chi tiết từng ngày (ngày 1, ngày 2... kèm địa điểm tham quan).

#### 3. Điều hành Lịch Khởi Hành (`/departures`)
- Mở đợt khởi hành cho từng tour: Chọn ngày bắt đầu, ngày kết thúc.
- Cài đặt giá vé người lớn, giá trẻ em và sức chứa tối đa (`capacity`).
- Phân công tài nguyên: Khách sạn đối tác, Phương tiện vận chuyển, Hướng dẫn viên phụ trách đoàn.

#### 4. Quản lý Đơn đặt tour - Booking Work Queue (`/bookings`)
- Tra cứu danh sách đơn theo mã booking, số điện thoại khách hàng, trạng thái xử lý.
- Xem danh sách hành khách chi tiết của từng đơn.
- Ghi chú nội bộ cho nhân viên kinh doanh / điều hành.
- Xác nhận đơn (`CONFIRMED`), hoàn tất chuyến đi (`COMPLETED`), hoặc hủy đơn.
- **Check-in khách**: Nhập hoặc quét mã QR trên vé khách hàng để xác nhận có mặt tại điểm tập kết.

#### 5. Quản lý Thanh toán & Đối soát (`/payments`)
- Kiểm tra danh sách giao dịch VietQR đang chờ duyệt.
- Mở xem ảnh biên lai khách đã tải lên.
- Nhấn **Duyệt thành công** (`SUCCESS`) để hệ thống tự động xác nhận đơn booking và phát hành vé QR; hoặc **Từ chối** (`FAILED`) kèm lý do rõ ràng.
- **Đối soát tự động (Import)**: Tải lên file Excel/CSV sao kê ngân hàng, hệ thống tự động khớp mã booking trong nội dung chuyển khoản để duyệt hàng loạt.
- **Ghi nhận hoàn tiền**: Cập nhật trạng thái `REFUNDED` sau khi kế toán đã chuyển khoản trả tiền thật cho khách ngoài ngân hàng.

#### 6. Quản lý Tài nguyên & Đối tác (`/resources`)
- **Điểm đến**: Thêm các tỉnh thành, địa danh du lịch.
- **Khách sạn**: Danh bạ khách sạn liên kết, hạng sao, địa chỉ, người liên hệ.
- **Phương tiện**: Quản lý xe 16-29-45 chỗ, biển số xe, tài xế.
- **Hướng dẫn viên**: Hồ sơ HDV, số điện thoại, ngôn ngữ.
- **Mã giảm giá (Vouchers)**: Tạo mã khuyến mại, kiểu giảm (% hoặc số tiền VNĐ), giá trị đơn tối thiểu, số lượt dùng tối đa, thời hạn hiệu lực.

#### 7. Quản lý Khách hàng (`/customers`)
- Xem danh sách khách hàng đã đăng ký trong hệ thống và lịch sử mua tour.
- Khóa hoặc kích hoạt lại tài khoản khách hàng khi cần.

#### 8. Kiểm duyệt Đánh giá (`/reviews`)
- Xem và kiểm duyệt nhận xét, số sao du khách đánh giá các tour.
- Ẩn hoặc xóa các đánh giá có ngôn từ tiêu cực hoặc sai sự thật.

#### 9. Báo cáo & Xuất file Excel (`/reports`)
- Xuất file Excel toàn bộ dữ liệu đơn đặt hàng phục vụ lưu trữ kế toán.
- Xuất danh sách hành khách đi tour (Passenger Manifest) theo từng đợt khởi hành cho hướng dẫn viên.

---

## 4. CÁCH SAO LƯU VÀ PHỤC HỒI CƠ SỞ DỮ LIỆU (DATABASE)

Dự án sử dụng cơ sở dữ liệu MySQL tên là `leviet_travel_db`. File backup mẫu hiện tại trong project là: `database_backup.sql`.

### Cách 1: Sử dụng dòng lệnh PowerShell (Nhanh và chuẩn nhất)

#### 🔹 1. Sao lưu (Backup - Export)
Mở PowerShell tại thư mục gốc dự án (`D:\kienthuc\LeViet_Travel`):
```powershell
mysqldump -u root -p --single-transaction --routines --triggers leviet_travel_db > database_backup.sql
```
*Hệ thống sẽ hỏi mật khẩu root MySQL của bạn. Sau khi gõ đúng và Enter, toàn bộ cấu trúc và dữ liệu sẽ được ghi vào file `database_backup.sql`.*

> **Mẹo sao lưu theo thời gian:**
> ```powershell
> mysqldump -u root -p --single-transaction --routines --triggers leviet_travel_db > "backup_$(Get-Date -Format 'yyyyMMdd_HHmmss').sql"
> ```

#### 🔹 2. Phục hồi (Restore - Import)
Khi cần khôi phục lại dữ liệu từ file backup:
```powershell
# Bước 1: Đảm bảo database tồn tại
mysql -u root -p -e "CREATE DATABASE IF NOT EXISTS leviet_travel_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;"

# Bước 2: Nạp dữ liệu từ file backup vào database
mysql -u root -p leviet_travel_db < database_backup.sql
```

---

### Cách 2: Sử dụng giao diện MySQL Workbench

#### 🔹 1. Sao lưu (Export)
1. Mở **MySQL Workbench** và kết nối vào máy chủ MySQL local.
2. Tại cột bên trái **Navigator**, chọn tab **Administration** -> chọn **Data Export**.
3. Tại khung **Tables to Export**, tích chọn database `leviet_travel_db`.
4. Tại mục **Export Options**:
   - Chọn **Export to Self-Contained File**.
   - Bấm nút `...` và chỉ định nơi lưu file (ví dụ: `D:\kienthuc\LeViet_Travel\database_backup.sql`).
   - Tích chọn ô **Include Create Schema**.
5. Bấm nút **Start Export** ở góc phải dưới.

#### 🔹 2. Phục hồi (Import)
1. Trong MySQL Workbench, chọn **Administration** -> chọn **Data Import/Restore**.
2. Chọn **Import from Self-Contained File** và trỏ đến file `database_backup.sql`.
3. Tại mục **Default Target Schema**, chọn `leviet_travel_db` (hoặc nhấn New để tạo DB mới).
4. Bấm nút **Start Import** để khôi phục dữ liệu.

---

## 5. CÁCH TẮT VÀ MỞ LẠI HỆ THỐNG

### 5.1. Cách tắt hệ thống an toàn (Shutdown)

#### Bước 1: Dừng các tiến trình ứng dụng
Vào từng cửa sổ Terminal đang chạy (`backend`, `frontend-admin`, `frontend-client`):
- Nhấn tổ hợp phím **`Ctrl + C`**.
- Nếu terminal hiện thông báo `Terminate batch job (Y/N)?`, nhập `Y` rồi nhấn **Enter**.

#### Bước 2: Kiểm tra và giải phóng cổng (Nghi ngờ ứng dụng chạy ngầm)
Nếu bạn lỡ tắt cửa sổ terminal nhưng tiến trình Node.js vẫn chạy ngầm chiếm cổng, chạy lệnh PowerShell sau:
```powershell
# Tìm các tiến trình đang chiếm cổng 5000, 5173, 5174
Get-NetTCPConnection -LocalPort 5000, 5173, 5174 -State Listen -ErrorAction SilentlyContinue | Select-Object LocalPort, OwningProcess

# Tắt tiến trình theo ID (PID) tìm được:
Stop-Process -Id <PID> -Force
```

#### Bước 3: Tắt dịch vụ MySQL (Không bắt buộc, chỉ khi muốn tắt hẳn máy chủ DB)
```powershell
Stop-Service MySQL80
```

---

### 5.2. Cách mở lại hệ thống (Restart / Start)

Mỗi khi muốn tiếp tục làm việc hoặc trình diễn dự án, thực hiện đúng 3 bước theo thứ tự sau:

#### 🟢 Bước 1: Đảm bảo MySQL đang chạy
Kiểm tra dịch vụ MySQL trên Windows:
```powershell
Get-Service MySQL80
```
Nếu dịch vụ chưa chạy, khởi động bằng lệnh:
```powershell
Start-Service MySQL80
```

#### 🟢 Bước 2: Khởi động 3 dịch vụ theo thứ tự

1. **Khởi động Backend (Ưu tiên số 1):**
   ```powershell
   cd D:\kienthuc\LeViet_Travel\backend
   npm run dev
   ```
2. **Khởi động Frontend Quản trị (Admin):**
   ```powershell
   cd D:\kienthuc\LeViet_Travel\frontend-admin
   npm run dev
   ```
3. **Khởi động Website Khách hàng (Client):**
   ```powershell
   cd D:\kienthuc\LeViet_Travel\frontend-client
   npm run dev
   ```

#### 🟢 Bước 3: Truy cập kiểm tra trên trình duyệt
- Kiểm tra kết nối Backend: `http://localhost:5000/api/health`
- Mở Website Khách hàng: `http://localhost:5174`
- Mở Cổng Quản trị Admin: `http://localhost:5173`

---

## 6. XỬ LÝ SỰ CỐ THƯỜNG GẶP (TROUBLESHOOTING)

1. **Lỗi `Port 5174 is already in use` hoặc `Port 5000 is already in use`:**
   - Nguyên nhân: Terminal cũ chưa được tắt hẳn.
   - Khắc phục: Xem mục [Kiểm tra và giải phóng cổng](#bước-2-kiểm-tra-và-giải-phóng-cổng-nghi-ngờ-ứng-dụng-chạy-ngầm) ở trên để giải phóng cổng.

2. **Lỗi `Access denied for user 'root'@'localhost'`:**
   - Nguyên nhân: Sai mật khẩu MySQL trong file `backend/.env`.
   - Khắc phục: Mở file `backend/.env` và cập nhật lại dòng `DB_PASSWORD=...` cho đúng với mật khẩu MySQL trên máy bạn.

3. **Lỗi `Unknown database 'leviet_travel_db'`:**
   - Nguyên nhân: Chưa tạo hoặc chưa import cơ sở dữ liệu.
   - Khắc phục: Thực hiện lệnh phục hồi database như hướng dẫn tại [Mục 4](#4-cách-sao-lưu-và-phục-hồi-cơ-sở-dữ-liệu-database).

4. **Trang web không tải được dữ liệu tour / báo lỗi mạng:**
   - Đảm bảo Backend (Terminal 1) đang chạy và truy cập được vào `http://localhost:5000/api/health`.
   - Kiểm tra file `frontend-client/.env` đã có `VITE_API_BASE_URL=http://localhost:5000/api` chưa.
