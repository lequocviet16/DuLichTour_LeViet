-- ====================================================================
-- Script khởi tạo cơ sở dữ liệu Microsoft SQL Server cho LeViet Travel
-- Kiến trúc: ASP.NET Core 8.0 Microservices (EF Core Compatible)
-- ====================================================================

-- 1. TẠO CÁC CƠ SỞ DỮ LIỆU MICROSERVICES HOẶC CHẠY CHUNG MỘT DATABASE
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'LeVietTravel_AuthDb')
    CREATE DATABASE LeVietTravel_AuthDb;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'LeVietTravel_TourDb')
    CREATE DATABASE LeVietTravel_TourDb;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'LeVietTravel_BookingDb')
    CREATE DATABASE LeVietTravel_BookingDb;
GO

-- ====================================================================
-- 2. DỮ LIỆU AUTH SERVICE (LeVietTravel_AuthDb)
-- ====================================================================
USE LeVietTravel_AuthDb;
GO

IF OBJECT_ID('RefreshTokens', 'U') IS NOT NULL DROP TABLE RefreshTokens;
IF OBJECT_ID('Users', 'U') IS NOT NULL DROP TABLE Users;
GO

CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    PhoneNumber NVARCHAR(20) NULL,
    Role NVARCHAR(20) NOT NULL DEFAULT 'CUSTOMER',
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

CREATE TABLE RefreshTokens (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    Token NVARCHAR(256) NOT NULL UNIQUE,
    ExpiresAt DATETIME2 NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    RevokedAt DATETIME2 NULL,
    ReplacedByToken NVARCHAR(256) NULL,
    CONSTRAINT FK_RefreshTokens_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
GO

-- Nạp tài khoản mẫu:
-- 1. Admin: admin@leviet.com / Mật khẩu: Admin@123
-- 2. Customer: an@example.com / Mật khẩu: Password123
INSERT INTO Users (FullName, Email, PasswordHash, PhoneNumber, Role, IsActive, CreatedAt)
VALUES 
(N'Quản trị viên LeViet', 'admin@leviet.com', '$2a$11$eU10gqRovW5F4Z8tT3tWwupW32N9rP.z39bM2w7eL/tN/f/5k2hye', '0988888888', 'ADMIN', 1, GETUTCDATE()),
(N'Nguyễn Văn An', 'an@example.com', '$2a$11$NvyJvJ/zEflm7D1432fS3.O8F7c6g8h0zE8L7i0jGk9M1n2O3P4Qe', '0901234567', 'CUSTOMER', 1, GETUTCDATE());
GO

-- ====================================================================
-- 3. DỮ LIỆU TOUR SERVICE (LeVietTravel_TourDb)
-- ====================================================================
USE LeVietTravel_TourDb;
GO

IF OBJECT_ID('TourDepartures', 'U') IS NOT NULL DROP TABLE TourDepartures;
IF OBJECT_ID('Tours', 'U') IS NOT NULL DROP TABLE Tours;
IF OBJECT_ID('Categories', 'U') IS NOT NULL DROP TABLE Categories;
GO

CREATE TABLE Categories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Slug NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL
);
GO

CREATE TABLE Tours (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Slug NVARCHAR(250) NOT NULL,
    Code NVARCHAR(50) NOT NULL,
    Summary NVARCHAR(500) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    DiscountPrice DECIMAL(18,2) NULL,
    DurationDays INT NOT NULL DEFAULT 3,
    DurationNights INT NOT NULL DEFAULT 2,
    DestinationName NVARCHAR(150) NOT NULL,
    ImageUrl NVARCHAR(500) NOT NULL,
    MaxCapacity INT NOT NULL DEFAULT 30,
    Status NVARCHAR(20) NOT NULL DEFAULT 'ACTIVE',
    CategoryId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Tours_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);
GO

CREATE TABLE TourDepartures (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TourId INT NOT NULL,
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2 NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    TotalSeats INT NOT NULL DEFAULT 30,
    BookedSeats INT NOT NULL DEFAULT 0,
    Status NVARCHAR(20) NOT NULL DEFAULT 'AVAILABLE',
    CONSTRAINT FK_TourDepartures_Tours FOREIGN KEY (TourId) REFERENCES Tours(Id) ON DELETE CASCADE
);
GO

-- Nạp danh mục mẫu
INSERT INTO Categories (Name, Slug, Description)
VALUES
(N'Du lịch Biển Đảo', 'du-lich-bien-dao', N'Khám phá các bãi biển đẹp nhất Việt Nam'),
(N'Du lịch Vùng Cao', 'du-lich-vung-cao', N'Trải nghiệm núi rừng Tây Bắc và Tây Nguyên'),
(N'Du lịch Di Sản & Văn Hóa', 'du-lich-di-san', N'Hành trình di sản văn hóa miền Trung');
GO

-- Nạp tour mẫu
INSERT INTO Tours (Title, Slug, Code, Summary, Description, Price, DiscountPrice, DurationDays, DurationNights, DestinationName, ImageUrl, MaxCapacity, Status, CategoryId)
VALUES
(N'Tour Khám Phá Vịnh Hạ Long 3N2Đ - Du Thuyền 5 Sao', 'tour-kham-pha-vinh-ha-long-3n2d', 'HL-3N2D', N'Thưởng ngoạn kỳ quan thiên nhiên thế giới trên du thuyền đẳng cấp sang trọng.', N'Hành trình 3 ngày 2 đêm tham quan Ti Tốp, Hang Sửng Sốt.', 3850000, 3450000, 3, 2, N'Hạ Long, Quảng Ninh', 'https://images.unsplash.com/photo-1528127269322-539801943592?auto=format&fit=crop&w=800&q=80', 30, 'ACTIVE', 1),
(N'Tour Đà Nẵng - Bà Nà Hills - Cầu Vàng - Hội An 4N3Đ', 'tour-da-nang-ba-na-hills-hoi-an-4n3d', 'DN-4N3D', N'Hành trình di sản miền Trung hấp dẫn với Cầu Vàng và phố cổ lung linh.', N'Khám phá Bà Nà Hills, Cầu Vàng và thả đèn hoa đăng sông Hoài.', 4690000, 4190000, 4, 3, N'Đà Nẵng - Hội An', 'https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?auto=format&fit=crop&w=800&q=80', 35, 'ACTIVE', 3),
(N'Tour Nghỉ Dưỡng Phú Quốc - Cáp Treo Hòn Thơm 3N2Đ', 'tour-phu-quoc-sunset-town-hon-thom-3n2d', 'PQ-3N2D', N'Thiên đường biển đảo ngọc, chiêm ngưỡng hoàng hôn rực rỡ nhất Việt Nam.', N'Cáp treo vượt biển dài nhất thế giới, Grand World.', 5200000, 4750000, 3, 2, N'Phú Quốc, Kiên Giang', 'https://images.unsplash.com/photo-1540555700478-4be289fbecef?auto=format&fit=crop&w=800&q=80', 25, 'ACTIVE', 1),
(N'Tour Sapa - Fansipan Huyền Thoại - Bản Cát Cát 3N2Đ', 'tour-sapa-fansipan-ban-cat-cat-3n2d', 'SP-3N2D', N'Chinh phục nóc nhà Đông Dương và hòa mình vào sắc màu văn hóa Tây Bắc.', N'Chinh phục đỉnh Fansipan 3.143m ngắm biển mây ngút ngàn.', 3190000, 2890000, 3, 2, N'Sa Pa, Lào Cai', 'https://images.unsplash.com/photo-1506744038136-46273834b3fb?auto=format&fit=crop&w=800&q=80', 30, 'ACTIVE', 2);
GO

-- ====================================================================
-- 4. DỮ LIỆU BOOKING SERVICE (LeVietTravel_BookingDb)
-- ====================================================================
USE LeVietTravel_BookingDb;
GO

IF OBJECT_ID('Bookings', 'U') IS NOT NULL DROP TABLE Bookings;
GO

CREATE TABLE Bookings (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    BookingCode NVARCHAR(50) NOT NULL UNIQUE,
    UserId INT NOT NULL,
    ContactName NVARCHAR(100) NOT NULL,
    ContactEmail NVARCHAR(150) NOT NULL,
    ContactPhone NVARCHAR(20) NOT NULL,
    TourId INT NOT NULL,
    TourTitle NVARCHAR(200) NOT NULL,
    DepartureId INT NOT NULL,
    StartDate DATETIME2 NOT NULL,
    AdultsCount INT NOT NULL DEFAULT 1,
    ChildrenCount INT NOT NULL DEFAULT 0,
    TotalPrice DECIMAL(18,2) NOT NULL,
    DiscountAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    FinalAmount DECIMAL(18,2) NOT NULL,
    Status NVARCHAR(30) NOT NULL DEFAULT 'PENDING',
    VietQrUrl NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

INSERT INTO Bookings (BookingCode, UserId, ContactName, ContactEmail, ContactPhone, TourId, TourTitle, DepartureId, StartDate, AdultsCount, ChildrenCount, TotalPrice, DiscountAmount, FinalAmount, Status, VietQrUrl, CreatedAt)
VALUES
('BK-202611-001', 2, N'Nguyễn Văn An', 'an@example.com', '0901234567', 1, N'Tour Khám Phá Vịnh Hạ Long 3N2Đ - Du Thuyền 5 Sao', 1, DATEADD(day, 7, GETUTCDATE()), 2, 0, 6900000, 0, 6900000, 'CONFIRMED', 'https://img.vietqr.io/image/MB-0988888888-compact2.png?amount=6900000&addInfo=BK202611001', DATEADD(day, -1, GETUTCDATE()));
GO
