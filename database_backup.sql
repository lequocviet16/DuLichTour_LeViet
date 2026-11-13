CREATE DATABASE  IF NOT EXISTS `leviet_travel_db` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `leviet_travel_db`;
-- MySQL dump 10.13  Distrib 8.0.45, for Win64 (x86_64)
--
-- Host: localhost    Database: leviet_travel_db
-- ------------------------------------------------------
-- Server version	8.0.45

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `booking_activities`
--

DROP TABLE IF EXISTS `booking_activities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `booking_activities` (
  `id` int NOT NULL AUTO_INCREMENT,
  `booking_id` int NOT NULL,
  `actor_id` int DEFAULT NULL,
  `event_type` varchar(40) COLLATE utf8mb4_unicode_ci NOT NULL,
  `from_status` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `to_status` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `note` varchar(1000) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `metadata` json DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `fk_booking_activities_actor` (`actor_id`),
  KEY `idx_booking_activities_booking_created` (`booking_id`,`created_at`),
  CONSTRAINT `fk_booking_activities_actor` FOREIGN KEY (`actor_id`) REFERENCES `users` (`id`) ON DELETE SET NULL,
  CONSTRAINT `fk_booking_activities_booking` FOREIGN KEY (`booking_id`) REFERENCES `bookings` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=119 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `booking_activities`
--

LOCK TABLES `booking_activities` WRITE;
/*!40000 ALTER TABLE `booking_activities` DISABLE KEYS */;
INSERT INTO `booking_activities` VALUES (13,29,NULL,'PAYMENT_EXPIRED','PENDING_PAYMENT','CANCELLED','Hệ thống tự động hoàn ghế và lượt voucher do quá hạn thanh toán.',NULL,'2026-10-14 01:22:53','2026-10-14 01:22:53'),(14,30,NULL,'PAYMENT_EXPIRED','PENDING_PAYMENT','CANCELLED','Hệ thống tự động hoàn ghế và lượt voucher do quá hạn thanh toán.',NULL,'2026-10-14 01:22:54','2026-10-14 01:22:54'),(15,31,NULL,'PAYMENT_EXPIRED','PENDING_PAYMENT','CANCELLED','Hệ thống tự động hoàn ghế và lượt voucher do quá hạn thanh toán.',NULL,'2026-10-14 01:22:54','2026-10-14 01:22:54'),(16,34,NULL,'PAYMENT_EXPIRED','PENDING_PAYMENT','CANCELLED','Hệ thống tự động hoàn ghế và lượt voucher do quá hạn thanh toán.',NULL,'2026-10-14 01:22:54','2026-10-14 01:22:54');
/*!40000 ALTER TABLE `booking_activities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `booking_change_requests`
--

DROP TABLE IF EXISTS `booking_change_requests`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `booking_change_requests` (
  `id` int NOT NULL AUTO_INCREMENT,
  `booking_id` int NOT NULL,
  `user_id` int NOT NULL,
  `request_type` enum('CANCEL','RESCHEDULE','REFUND') COLLATE utf8mb4_unicode_ci NOT NULL,
  `requested_departure_id` int DEFAULT NULL,
  `reason` varchar(1000) COLLATE utf8mb4_unicode_ci NOT NULL,
  `status` enum('PENDING','APPROVED','REJECTED') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'PENDING',
  `admin_note` varchar(1000) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `reviewed_by` int DEFAULT NULL,
  `reviewed_at` datetime DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `fk_change_requests_user` (`user_id`),
  KEY `fk_change_requests_departure` (`requested_departure_id`),
  KEY `fk_change_requests_reviewer` (`reviewed_by`),
  KEY `idx_change_requests_booking` (`booking_id`),
  KEY `idx_change_requests_status_created` (`status`,`created_at`),
  CONSTRAINT `fk_change_requests_booking` FOREIGN KEY (`booking_id`) REFERENCES `bookings` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_change_requests_departure` FOREIGN KEY (`requested_departure_id`) REFERENCES `tour_departures` (`id`) ON DELETE SET NULL,
  CONSTRAINT `fk_change_requests_reviewer` FOREIGN KEY (`reviewed_by`) REFERENCES `users` (`id`) ON DELETE SET NULL,
  CONSTRAINT `fk_change_requests_user` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `booking_change_requests`
--

LOCK TABLES `booking_change_requests` WRITE;
/*!40000 ALTER TABLE `booking_change_requests` DISABLE KEYS */;
/*!40000 ALTER TABLE `booking_change_requests` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `booking_participants`
--

DROP TABLE IF EXISTS `booking_participants`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `booking_participants` (
  `id` int NOT NULL AUTO_INCREMENT,
  `booking_id` int NOT NULL,
  `full_name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `gender` enum('MALE','FEMALE','OTHER') COLLATE utf8mb4_unicode_ci NOT NULL,
  `date_of_birth` date NOT NULL,
  `passenger_type` enum('ADULT','CHILD','INFANT','SENIOR') COLLATE utf8mb4_unicode_ci NOT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_bp_booking` (`booking_id`),
  CONSTRAINT `fk_bp_booking` FOREIGN KEY (`booking_id`) REFERENCES `bookings` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=152 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `booking_participants`
--

LOCK TABLES `booking_participants` WRITE;
/*!40000 ALTER TABLE `booking_participants` DISABLE KEYS */;
INSERT INTO `booking_participants` VALUES (48,22,'Nguyễn Minh Anh','MALE','1987-06-15','ADULT','2026-10-13 07:30:39','2026-10-13 07:30:39'),(49,22,'Khách người lớn 2','FEMALE','1988-06-15','ADULT','2026-10-13 07:30:39','2026-10-13 07:30:39'),(50,22,'Khách trẻ em 1','FEMALE','2016-09-20','CHILD','2026-10-13 07:30:39','2026-10-13 07:30:39'),(51,23,'Trần Hoàng Nam','MALE','1987-06-15','ADULT','2026-10-13 07:30:39','2026-10-13 07:30:39'),(52,23,'Khách người lớn 2','FEMALE','1988-06-15','ADULT','2026-10-13 07:30:39','2026-10-13 07:30:39'),(53,24,'Lê Thu Trang','MALE','1987-06-15','ADULT','2026-10-13 07:30:39','2026-10-13 07:30:39'),(54,24,'Khách trẻ em 1','FEMALE','2016-09-20','CHILD','2026-10-13 07:30:39','2026-10-13 07:30:39'),(55,25,'Trần Hoàng Nam','MALE','1987-06-15','ADULT','2026-10-13 07:30:39','2026-10-13 07:30:39'),(56,25,'Khách người lớn 2','FEMALE','1988-06-15','ADULT','2026-10-13 07:30:39','2026-10-13 07:30:39'),(57,25,'Khách người lớn 3','MALE','1989-06-15','ADULT','2026-10-13 07:30:39','2026-10-13 07:30:39'),(58,26,'Nguyễn Minh Anh','MALE','1987-06-15','ADULT','2026-10-13 07:30:39','2026-10-13 07:30:39'),(59,26,'Khách người lớn 2','FEMALE','1988-06-15','ADULT','2026-10-13 07:30:39','2026-10-13 07:30:39'),(60,26,'Khách trẻ em 1','FEMALE','2016-09-20','CHILD','2026-10-13 07:30:39','2026-10-13 07:30:39'),(61,26,'Khách trẻ em 2','MALE','2017-09-20','CHILD','2026-10-13 07:30:39','2026-10-13 07:30:39'),(62,27,'Lê Thu Trang','MALE','1987-06-15','ADULT','2026-10-13 07:30:39','2026-10-13 07:30:39'),(63,27,'Khách người lớn 2','FEMALE','1988-06-15','ADULT','2026-10-13 07:30:39','2026-10-13 07:30:39'),(64,27,'Khách trẻ em 1','FEMALE','2016-09-20','CHILD','2026-10-13 07:30:39','2026-10-13 07:30:39'),(65,28,'Nguyễn Minh Anh','MALE','1987-06-15','ADULT','2026-10-13 07:30:39','2026-10-13 07:30:39'),(66,29,'Trần Hoàng Nam','MALE','1987-06-15','ADULT','2026-10-13 07:30:39','2026-10-13 07:30:39'),(67,29,'Khách người lớn 2','FEMALE','1988-06-15','ADULT','2026-10-13 07:30:39','2026-10-13 07:30:39'),(68,29,'Khách trẻ em 1','FEMALE','2016-09-20','CHILD','2026-10-13 07:30:39','2026-10-13 07:30:39'),(69,30,'Lê Thu Trang','MALE','1987-06-15','ADULT','2026-10-13 07:30:39','2026-10-13 07:30:39'),(70,30,'Khách người lớn 2','FEMALE','1988-06-15','ADULT','2026-10-13 07:30:39','2026-10-13 07:30:39'),(71,31,'Nguyễn Minh Anh','MALE','1987-06-15','ADULT','2026-10-13 07:30:39','2026-10-13 07:30:39'),(72,31,'Khách trẻ em 1','FEMALE','2016-09-20','CHILD','2026-10-13 07:30:39','2026-10-13 07:30:39'),(77,34,'Dương tiến anh','MALE','2005-02-03','ADULT','2026-10-13 07:50:11','2026-10-13 07:50:11'),(78,34,'Dương tiến anh','MALE','2005-02-02','CHILD','2026-10-13 07:50:11','2026-10-13 07:50:11'),(81,36,'Dương Tiến Anh','MALE','2005-02-03','ADULT','2026-10-13 09:34:14','2026-10-13 09:34:14');
/*!40000 ALTER TABLE `booking_participants` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bookings`
--

DROP TABLE IF EXISTS `bookings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bookings` (
  `id` int NOT NULL AUTO_INCREMENT,
  `booking_code` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `user_id` int NOT NULL,
  `departure_id` int NOT NULL,
  `voucher_id` int DEFAULT NULL,
  `num_adults` int NOT NULL DEFAULT '1',
  `num_children` int NOT NULL DEFAULT '0',
  `num_infants` int NOT NULL DEFAULT '0',
  `num_seniors` int NOT NULL DEFAULT '0',
  `single_room_count` int NOT NULL DEFAULT '0',
  `total_amount` decimal(12,2) NOT NULL,
  `discount_amount` decimal(12,2) NOT NULL DEFAULT '0.00',
  `final_amount` decimal(12,2) NOT NULL,
  `deposit_amount` decimal(12,2) NOT NULL DEFAULT '0.00',
  `booking_date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `payment_due_at` datetime DEFAULT NULL,
  `expired_at` datetime DEFAULT NULL,
  `status` enum('PENDING_PAYMENT','CONFIRMED','CANCELLED','COMPLETED') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'PENDING_PAYMENT',
  `cancellation_reason` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `notes` text COLLATE utf8mb4_unicode_ci,
  `pricing_snapshot` json DEFAULT NULL,
  `policy_snapshot` json DEFAULT NULL,
  `check_in_code` varchar(64) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `checked_in_at` datetime DEFAULT NULL,
  `checked_in_by` int DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `booking_code` (`booking_code`),
  UNIQUE KEY `uq_bookings_check_in_code` (`check_in_code`),
  KEY `fk_bookings_voucher` (`voucher_id`),
  KEY `idx_bookings_code` (`booking_code`),
  KEY `idx_bookings_user` (`user_id`),
  KEY `idx_bookings_departure` (`departure_id`),
  KEY `idx_bookings_status` (`status`),
  KEY `idx_bookings_user_date` (`user_id`,`booking_date`),
  KEY `idx_bookings_status_date` (`status`,`booking_date`),
  KEY `idx_bookings_payment_expiry` (`status`,`payment_due_at`),
  CONSTRAINT `fk_bookings_departure` FOREIGN KEY (`departure_id`) REFERENCES `tour_departures` (`id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_bookings_user` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_bookings_voucher` FOREIGN KEY (`voucher_id`) REFERENCES `vouchers` (`id`) ON DELETE SET NULL,
  CONSTRAINT `chk_booking_amounts` CHECK (((`total_amount` >= 0) and (`discount_amount` >= 0) and (`final_amount` >= 0) and (`final_amount` = (`total_amount` - `discount_amount`)))),
  CONSTRAINT `chk_booking_passengers` CHECK (((`num_adults` > 0) and (`num_children` >= 0)))
) ENGINE=InnoDB AUTO_INCREMENT=90 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bookings`
--

LOCK TABLES `bookings` WRITE;
/*!40000 ALTER TABLE `bookings` DISABLE KEYS */;
INSERT INTO `bookings` VALUES (22,'DEMO-BOOKING-001',7,7,NULL,2,1,0,0,0,19470000.00,0.00,19470000.00,0.00,'2026-01-05 09:15:00',NULL,NULL,'COMPLETED',NULL,'Dữ liệu mẫu cho dashboard và quy trình duyệt thanh toán.',NULL,NULL,NULL,NULL,NULL,'2026-10-13 07:30:39','2026-10-13 07:56:11'),(23,'DEMO-BOOKING-002',8,15,NULL,2,0,0,0,0,17980000.00,0.00,17980000.00,0.00,'2026-02-06 10:15:00',NULL,NULL,'COMPLETED',NULL,'Dữ liệu mẫu cho dashboard và quy trình duyệt thanh toán.',NULL,NULL,NULL,NULL,NULL,'2026-10-13 07:30:39','2026-10-13 07:56:11'),(24,'DEMO-BOOKING-003',9,21,NULL,1,1,0,0,0,15180000.00,0.00,15180000.00,0.00,'2026-04-07 11:15:00',NULL,NULL,'COMPLETED',NULL,'Dữ liệu mẫu cho dashboard và quy trình duyệt thanh toán.',NULL,NULL,NULL,NULL,NULL,'2026-10-13 07:30:39','2026-10-13 07:56:11'),(25,'DEMO-BOOKING-004',8,7,NULL,3,0,0,0,0,22470000.00,0.00,22470000.00,0.00,'2026-05-08 12:15:00',NULL,NULL,'CONFIRMED',NULL,'Dữ liệu mẫu cho dashboard và quy trình duyệt thanh toán.',NULL,NULL,NULL,NULL,NULL,'2026-10-13 07:30:39','2026-10-13 07:56:11'),(26,'DEMO-BOOKING-005',7,18,NULL,2,2,0,0,0,15960000.00,0.00,15960000.00,0.00,'2026-06-09 13:15:00',NULL,NULL,'CONFIRMED',NULL,'Dữ liệu mẫu cho dashboard và quy trình duyệt thanh toán.',NULL,NULL,NULL,NULL,NULL,'2026-10-13 07:30:39','2026-10-13 07:56:11'),(27,'DEMO-BOOKING-006',9,21,NULL,2,1,0,0,0,24670000.00,0.00,24670000.00,0.00,'2026-07-10 14:15:00',NULL,NULL,'CONFIRMED',NULL,'Dữ liệu mẫu cho dashboard và quy trình duyệt thanh toán.',NULL,NULL,NULL,NULL,NULL,'2026-10-13 07:30:39','2026-10-13 07:56:11'),(28,'DEMO-BOOKING-007',7,15,NULL,1,0,0,0,0,8990000.00,0.00,8990000.00,0.00,'2026-09-11 15:15:00',NULL,NULL,'CANCELLED',NULL,'Dữ liệu mẫu cho dashboard và quy trình duyệt thanh toán.',NULL,NULL,NULL,NULL,NULL,'2026-10-13 07:30:39','2026-10-13 07:56:11'),(29,'DEMO-BOOKING-008',8,7,NULL,2,1,0,0,0,19470000.00,0.00,19470000.00,0.00,'2026-10-12 16:15:00','2026-10-12 16:45:00','2026-10-14 01:22:53','CANCELLED','Đơn tự động hủy do quá hạn thanh toán.','Dữ liệu mẫu cho dashboard và quy trình duyệt thanh toán.',NULL,NULL,NULL,NULL,NULL,'2026-10-13 07:30:39','2026-10-14 01:22:53'),(30,'DEMO-BOOKING-009',9,18,NULL,2,0,0,0,0,9980000.00,0.00,9980000.00,0.00,'2026-10-12 16:15:00','2026-10-12 16:45:00','2026-10-14 01:22:54','CANCELLED','Đơn tự động hủy do quá hạn thanh toán.','Dữ liệu mẫu cho dashboard và quy trình duyệt thanh toán.',NULL,NULL,NULL,NULL,NULL,'2026-10-13 07:30:39','2026-10-14 01:22:54'),(31,'DEMO-BOOKING-010',7,21,NULL,1,1,0,0,0,15180000.00,0.00,15180000.00,0.00,'2026-10-12 16:15:00','2026-10-12 16:45:00','2026-10-14 01:22:54','CANCELLED','Đơn tự động hủy do quá hạn thanh toán.','Dữ liệu mẫu cho dashboard và quy trình duyệt thanh toán.',NULL,NULL,NULL,NULL,NULL,'2026-10-13 07:30:39','2026-10-14 01:22:54'),(34,'LVT-20261013-F14C0677',2,105,NULL,1,1,0,0,0,8780000.00,0.00,8780000.00,0.00,'2026-10-13 07:50:11','2026-10-13 08:20:11','2026-10-14 01:22:54','CANCELLED','Đơn tự động hủy do quá hạn thanh toán.',NULL,NULL,NULL,NULL,NULL,NULL,'2026-10-13 07:50:11','2026-10-14 01:22:54'),(36,'LVT-20261013-08333AED',2,105,NULL,1,0,0,0,0,5490000.00,0.00,5490000.00,0.00,'2026-10-13 09:34:14',NULL,NULL,'CONFIRMED',NULL,NULL,NULL,NULL,'LVT-CHECKIN-3D0744BD9A4525178646FC88B8396CE4',NULL,NULL,'2026-10-13 09:34:14','2026-10-14 01:26:16');
/*!40000 ALTER TABLE `bookings` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `categories`
--

DROP TABLE IF EXISTS `categories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `categories` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `description` text COLLATE utf8mb4_unicode_ci,
  `image_url` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `deleted_at` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `name` (`name`)
) ENGINE=InnoDB AUTO_INCREMENT=68 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `categories`
--

LOCK TABLES `categories` WRITE;
/*!40000 ALTER TABLE `categories` DISABLE KEYS */;
INSERT INTO `categories` VALUES (1,'Tour Biển Đảo','Kỳ nghỉ biển, khám phá đảo và các trải nghiệm ven bờ tiêu biểu của Việt Nam.','https://images.unsplash.com/photo-1526139334526-f591a54b477c?auto=format&fit=crop&w=1600&q=82','2026-09-28 18:32:23','2026-10-12 00:41:05',NULL),(2,'Tour Di Sản Miền Trung','Hành trình kết nối các đô thị và di sản văn hóa đặc sắc tại miền Trung.','https://images.unsplash.com/flagged/photo-1549874613-8ea0adee1a82?auto=format&fit=crop&w=1600&q=82','2026-09-28 18:32:23','2026-10-12 00:41:05',NULL),(3,'Tour Khám Phá Tây Bắc','Cảnh quan núi cao, thung lũng ruộng bậc thang và văn hóa cộng đồng vùng Tây Bắc.','https://images.unsplash.com/photo-1570366583862-f91883984fde?auto=format&fit=crop&w=1600&q=82','2026-09-28 18:32:23','2026-10-12 00:41:05',NULL),(4,'Tour Nghỉ Dưỡng Sinh Thái','Những hành trình thiên nhiên có nhịp độ thư thả, ưu tiên cảnh quan và trải nghiệm bản địa.','https://images.unsplash.com/photo-1531737212413-667205e1cda7?auto=format&fit=crop&w=1600&q=82','2026-09-28 18:32:23','2026-10-12 00:41:05',NULL),(15,'Tour Văn Hóa - Ẩm Thực','Khám phá lịch sử đô thị, đời sống địa phương và các món ăn đặc trưng vùng miền.','https://images.unsplash.com/photo-1509030450996-dd1a26dda07a?auto=format&fit=crop&w=1600&q=82','2026-10-12 00:41:05','2026-10-12 00:41:05',NULL),(16,'Tour Miền Sông Nước','Trải nghiệm chợ nổi, vườn cây và nếp sống ven sông của Đồng bằng sông Cửu Long.','https://images.unsplash.com/photo-1543411789-1a67a2ac05c6?auto=format&fit=crop&w=1600&q=82','2026-10-12 00:41:05','2026-10-12 00:41:05',NULL);
/*!40000 ALTER TABLE `categories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `destinations`
--

DROP TABLE IF EXISTS `destinations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `destinations` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `region` enum('BAC','TRUNG','NAM') COLLATE utf8mb4_unicode_ci NOT NULL,
  `description` text COLLATE utf8mb4_unicode_ci,
  `image_url` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `deleted_at` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_destinations_region` (`region`)
) ENGINE=InnoDB AUTO_INCREMENT=86 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `destinations`
--

LOCK TABLES `destinations` WRITE;
/*!40000 ALTER TABLE `destinations` DISABLE KEYS */;
INSERT INTO `destinations` VALUES (1,'Đà Nẵng','TRUNG','Thành phố biển miền Trung với bán đảo Sơn Trà, bãi biển Mỹ Khê, Ngũ Hành Sơn và sông Hàn.','https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?auto=format&fit=crop&w=1600&q=82','2026-09-28 18:32:23','2026-10-12 00:41:05',NULL),(2,'Hội An','TRUNG','Đô thị cổ bên sông Hoài, nổi bật với kiến trúc nhà cổ, hội quán, làng nghề và ẩm thực Quảng Nam.','https://images.unsplash.com/photo-1563354860-799d15199ac3?auto=format&fit=crop&w=1600&q=82','2026-09-28 18:32:23','2026-10-12 00:41:05',NULL),(3,'Phú Quốc','NAM','Đảo nghỉ dưỡng ở phía Nam với bãi biển, rừng quốc gia, làng chài, hồ tiêu và nghề làm nước mắm.','https://images.unsplash.com/photo-1526139334526-f591a54b477c?auto=format&fit=crop&w=1600&q=82','2026-09-28 18:32:23','2026-10-12 00:41:05',NULL),(4,'Sa Pa','BAC','Thị xã vùng cao trong dãy Hoàng Liên Sơn, cửa ngõ đến Fansipan, thung lũng Mường Hoa và các bản làng.','https://images.unsplash.com/photo-1570366583862-f91883984fde?auto=format&fit=crop&w=1600&q=82','2026-09-28 18:32:23','2026-10-12 00:41:05',NULL),(15,'Hà Nội','BAC','Thủ đô nghìn năm văn hiến với khu phố cổ, hồ Hoàn Kiếm, Văn Miếu và nền ẩm thực đường phố phong phú.','https://images.unsplash.com/photo-1509030450996-dd1a26dda07a?auto=format&fit=crop&w=1600&q=82','2026-10-12 00:41:05','2026-10-12 00:41:05',NULL),(16,'Hạ Long','BAC','Điểm đến bên vịnh Hạ Long, nổi bật với cảnh quan đảo đá vôi, hang động và hành trình du thuyền.','https://images.unsplash.com/photo-1643029891412-92f9a81a8c16?auto=format&fit=crop&w=1600&q=82','2026-10-12 00:41:05','2026-10-12 00:41:05',NULL),(17,'Ninh Bình','BAC','Vùng đất cố đô với quần thể Tràng An, sông Ngô Đồng và cảnh quan núi đá vôi đặc trưng.','https://images.unsplash.com/photo-1531737212413-667205e1cda7?auto=format&fit=crop&w=1600&q=82','2026-10-12 00:41:05','2026-10-12 00:41:05',NULL),(18,'Huế','TRUNG','Cố đô bên sông Hương với hệ thống cung điện, lăng tẩm triều Nguyễn và chùa Thiên Mụ.','https://images.unsplash.com/photo-1618165220283-e85246c4171c?auto=format&fit=crop&w=1600&q=82','2026-10-12 00:41:05','2026-10-12 00:41:05',NULL),(19,'Cần Thơ','NAM','Trung tâm của miền Tây Nam Bộ bên sông Hậu, nổi tiếng với chợ nổi Cái Răng, bến Ninh Kiều và vườn cây.','https://images.unsplash.com/photo-1543411789-1a67a2ac05c6?auto=format&fit=crop&w=1600&q=82','2026-10-12 00:41:05','2026-10-12 00:41:05',NULL),(26,'Hà Giang','BAC','Miền cao địa đầu Tổ quốc với Cao nguyên đá Đồng Văn, đèo Mã Pì Lèng, sông Nho Quế và các phiên chợ vùng cao.','https://images.unsplash.com/photo-1657445449727-017d2f9f44e7?auto=format&fit=crop&w=1600&q=82','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(27,'Cao Bằng','BAC','Tỉnh biên giới Đông Bắc nổi bật với thác Bản Giốc, động Ngườm Ngao, khu di tích Pác Bó và cảnh quan công viên địa chất.','https://images.unsplash.com/photo-1772165009296-2e86e417d62d?auto=format&fit=crop&w=1600&q=82','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(28,'Mộc Châu','BAC','Cao nguyên của Sơn La với đồi chè, thung lũng mận, rừng thông và khí hậu mát mẻ theo mùa.','https://images.unsplash.com/photo-1744254525928-170fe34f13c8?auto=format&fit=crop&w=1600&q=82','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(29,'Cát Bà','BAC','Quần đảo thuộc Hải Phòng, cửa ngõ khám phá vịnh Lan Hạ, làng chài Việt Hải và Vườn quốc gia Cát Bà.','https://images.unsplash.com/photo-1722471467241-5327b98a4976?auto=format&fit=crop&w=1600&q=82','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(30,'Quảng Bình','TRUNG','Điểm đến hang động và thiên nhiên với Phong Nha - Kẻ Bàng, sông Chày, bãi biển Nhật Lệ và hệ sinh thái núi đá vôi.','https://images.unsplash.com/photo-1758298134993-743b156eb23a?auto=format&fit=crop&w=1600&q=82','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(31,'Nha Trang','TRUNG','Thành phố biển bên vịnh Nha Trang, phù hợp cho hành trình tham quan đảo, lặn ngắm san hô và tìm hiểu văn hóa Chăm.','https://images.unsplash.com/photo-1533002832-1721d16b4bb9?auto=format&fit=crop&w=1600&q=82','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(32,'Đà Lạt','TRUNG','Đô thị cao nguyên Lâm Viên với rừng thông, hồ, thác, vườn hoa và các công trình kiến trúc đặc trưng.','https://images.unsplash.com/photo-1669413487832-a8733f063ca8?auto=format&fit=crop&w=1600&q=82','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(33,'Quy Nhơn','TRUNG','Thành phố biển Nam Trung Bộ với bán đảo Phương Mai, Eo Gió, Kỳ Co và dấu tích văn hóa Chăm.','https://images.unsplash.com/photo-1689428962568-fe58ab6cc4cd?auto=format&fit=crop&w=1600&q=82','2026-10-12 01:07:46','2026-10-12 01:09:34',NULL),(34,'Mũi Né','TRUNG','Khu du lịch ven biển Phan Thiết nổi bật với đồi cát, làng chài, Bàu Trắng và các hoạt động thể thao biển.','https://images.unsplash.com/photo-1714271511582-3483dcf0eb71?auto=format&fit=crop&w=1600&q=82','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(35,'TP. Hồ Chí Minh','NAM','Đô thị lớn nhất Việt Nam với hệ thống bảo tàng, công trình lịch sử, khu phố ẩm thực và nhịp sống hiện đại.','https://images.unsplash.com/photo-1583417319070-4a69db38a482?auto=format&fit=crop&w=1600&q=82','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(36,'Vũng Tàu','NAM','Thành phố biển gần TP. Hồ Chí Minh với Bãi Sau, Bãi Trước, Núi Nhỏ, Bạch Dinh và làng nghề ven biển.','https://images.unsplash.com/photo-1623596711744-c10ed15581d9?auto=format&fit=crop&w=1600&q=82','2026-10-12 01:07:46','2026-10-12 01:09:34',NULL),(37,'Tây Ninh','NAM','Điểm đến văn hóa - tâm linh với núi Bà Đen, Tòa Thánh Cao Đài và hồ Dầu Tiếng.','https://images.unsplash.com/photo-1767757430757-acd414f24fa4?auto=format&fit=crop&w=1600&q=82','2026-10-12 01:07:46','2026-10-12 01:09:34',NULL),(38,'Bến Tre','NAM','Xứ dừa bên các nhánh sông Mekong, phù hợp với trải nghiệm xuồng nhỏ, vườn cây, làng nghề và homestay cộng đồng.','https://images.unsplash.com/photo-1695927521805-d60927fc790d?auto=format&fit=crop&w=1600&q=82','2026-10-12 01:07:46','2026-10-12 01:09:34',NULL),(39,'Châu Đốc','NAM','Thành phố biên giới của An Giang với núi Sam, miếu Bà Chúa Xứ, làng Chăm và rừng tràm Trà Sư.','https://images.unsplash.com/photo-1677552926138-f7dbb71b226f?auto=format&fit=crop&w=1600&q=82','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(40,'Côn Đảo','NAM','Quần đảo ngoài khơi Nam Bộ có giá trị lịch sử, hệ sinh thái biển, bãi tắm yên tĩnh và Vườn quốc gia Côn Đảo.','https://images.unsplash.com/photo-1686073769358-57d716df2f3b?auto=format&fit=crop&w=1600&q=82','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL);
/*!40000 ALTER TABLE `destinations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `hotels`
--

DROP TABLE IF EXISTS `hotels`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `hotels` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(150) COLLATE utf8mb4_unicode_ci NOT NULL,
  `star_rating` tinyint NOT NULL DEFAULT '3',
  `address` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `contact_phone` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=36 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `hotels`
--

LOCK TABLES `hotels` WRITE;
/*!40000 ALTER TABLE `hotels` DISABLE KEYS */;
INSERT INTO `hotels` VALUES (1,'Mường Thanh Luxury Đà Nẵng',5,'270 Võ Nguyên Giáp, phường Ngũ Hành Sơn, Đà Nẵng','02363956789','2026-09-28 18:32:23','2026-10-12 00:41:05'),(2,'Vinpearl Resort & Spa Phú Quốc',5,'Bãi Dài, Gành Dầu, Phú Quốc, Kiên Giang','02973550550','2026-09-28 18:32:23','2026-09-28 18:32:23'),(8,'Vinpearl Resort & Spa Hạ Long',5,'Đảo Rều, phường Bãi Cháy, Hạ Long, Quảng Ninh','02033857858','2026-10-12 00:41:05','2026-10-12 00:41:05'),(9,'Silk Path Grand Sapa Resort & Spa',5,'Đồi Quan 6, Tổ 5, Sa Pa, Lào Cai','02143788555','2026-10-12 00:41:05','2026-10-12 00:41:05'),(10,'Salinda Resort Phu Quoc Island',5,'Ấp Cửa Lấp, đặc khu Phú Quốc, An Giang','02973990011','2026-10-12 00:41:05','2026-10-12 00:41:05');
/*!40000 ALTER TABLE `hotels` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `payment_reconciliation_events`
--

DROP TABLE IF EXISTS `payment_reconciliation_events`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `payment_reconciliation_events` (
  `id` int NOT NULL AUTO_INCREMENT,
  `source` varchar(40) COLLATE utf8mb4_unicode_ci NOT NULL,
  `external_transaction_id` varchar(150) COLLATE utf8mb4_unicode_ci NOT NULL,
  `event_hash` char(64) COLLATE utf8mb4_unicode_ci NOT NULL,
  `payment_id` int DEFAULT NULL,
  `transfer_content` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `amount` decimal(12,2) NOT NULL,
  `transaction_time` datetime DEFAULT NULL,
  `status` enum('MATCHED','UNMATCHED','DUPLICATE','REJECTED') COLLATE utf8mb4_unicode_ci NOT NULL,
  `raw_data` json DEFAULT NULL,
  `processed_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_reconciliation_source_external` (`source`,`external_transaction_id`),
  UNIQUE KEY `payment_reconciliation_events_source_external_transaction_id` (`source`,`external_transaction_id`),
  KEY `fk_reconciliation_payment` (`payment_id`),
  KEY `idx_reconciliation_status_time` (`status`,`processed_at`),
  CONSTRAINT `fk_reconciliation_payment` FOREIGN KEY (`payment_id`) REFERENCES `payments` (`id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `payment_reconciliation_events`
--

LOCK TABLES `payment_reconciliation_events` WRITE;
/*!40000 ALTER TABLE `payment_reconciliation_events` DISABLE KEYS */;
/*!40000 ALTER TABLE `payment_reconciliation_events` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `payments`
--

DROP TABLE IF EXISTS `payments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `payments` (
  `id` int NOT NULL AUTO_INCREMENT,
  `booking_id` int NOT NULL,
  `payment_method` enum('CASH','BANK_TRANSFER','VIETQR') COLLATE utf8mb4_unicode_ci NOT NULL,
  `payment_purpose` enum('DEPOSIT','BALANCE','FULL') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'FULL',
  `amount` decimal(12,2) NOT NULL,
  `transaction_id` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `status` enum('PENDING','SUCCESS','FAILED','REFUNDED') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'PENDING',
  `payment_time` datetime DEFAULT NULL,
  `proof_url` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `proof_uploaded_at` datetime DEFAULT NULL,
  `provider` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `response_data` json DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_payments_trans_id` (`transaction_id`),
  KEY `idx_payments_booking` (`booking_id`),
  KEY `idx_payments_trans_id` (`transaction_id`),
  KEY `idx_payments_booking_status` (`booking_id`,`status`),
  CONSTRAINT `fk_payments_booking` FOREIGN KEY (`booking_id`) REFERENCES `bookings` (`id`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=78 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `payments`
--

LOCK TABLES `payments` WRITE;
/*!40000 ALTER TABLE `payments` DISABLE KEYS */;
INSERT INTO `payments` VALUES (21,22,'VIETQR','FULL',19470000.00,'DEMO-TXN-001','SUCCESS','2026-01-05 09:45:00',NULL,NULL,NULL,'{\"source\": \"seed\", \"description\": \"Giao dịch mẫu Lê Việt Travel\"}','2026-01-05 09:45:00','2026-10-13 07:56:11'),(22,23,'BANK_TRANSFER','FULL',17980000.00,'DEMO-TXN-002','SUCCESS','2026-02-06 10:45:00',NULL,NULL,NULL,'{\"source\": \"seed\", \"description\": \"Giao dịch mẫu Lê Việt Travel\"}','2026-02-06 10:45:00','2026-10-13 07:56:11'),(23,24,'VIETQR','FULL',15180000.00,'DEMO-TXN-003','SUCCESS','2026-04-07 11:45:00',NULL,NULL,NULL,'{\"source\": \"seed\", \"description\": \"Giao dịch mẫu Lê Việt Travel\"}','2026-04-07 11:45:00','2026-10-13 07:56:11'),(24,25,'BANK_TRANSFER','FULL',22470000.00,'DEMO-TXN-004','SUCCESS','2026-05-08 12:45:00',NULL,NULL,NULL,'{\"source\": \"seed\", \"description\": \"Giao dịch mẫu Lê Việt Travel\"}','2026-05-08 12:45:00','2026-10-13 07:56:11'),(25,26,'CASH','FULL',15960000.00,'DEMO-TXN-005','SUCCESS','2026-06-09 13:45:00',NULL,NULL,NULL,'{\"source\": \"seed\", \"description\": \"Giao dịch mẫu Lê Việt Travel\"}','2026-06-09 13:45:00','2026-10-13 07:56:11'),(26,27,'VIETQR','FULL',24670000.00,'DEMO-TXN-006','SUCCESS','2026-07-10 14:45:00',NULL,NULL,NULL,'{\"source\": \"seed\", \"description\": \"Giao dịch mẫu Lê Việt Travel\"}','2026-07-10 14:45:00','2026-10-13 07:56:11'),(27,28,'BANK_TRANSFER','FULL',8990000.00,NULL,'FAILED',NULL,NULL,NULL,NULL,'{\"source\": \"seed\", \"description\": \"Giao dịch mẫu Lê Việt Travel\"}','2026-09-11 15:45:00','2026-10-13 07:56:11'),(28,29,'VIETQR','FULL',19470000.00,NULL,'FAILED',NULL,NULL,NULL,NULL,'{\"rejectedAt\": \"2026-10-13T02:35:50.101Z\", \"rejectedBy\": \"admin-console\"}','2026-10-12 16:45:00','2026-10-13 09:35:50'),(29,30,'VIETQR','FULL',9980000.00,NULL,'FAILED',NULL,NULL,NULL,NULL,'{\"reason\": \"PAYMENT_TIMEOUT\"}','2026-10-12 16:45:00','2026-10-14 01:22:54'),(32,34,'VIETQR','FULL',8780000.00,NULL,'FAILED',NULL,NULL,NULL,NULL,'{\"reason\": \"PAYMENT_TIMEOUT\"}','2026-10-13 07:50:11','2026-10-14 01:22:54'),(34,36,'VIETQR','FULL',5490000.00,'VIETQR-20261013-093502-34','SUCCESS','2026-10-13 09:35:02',NULL,NULL,NULL,'{\"approvedAt\": \"2026-10-13T02:35:02.429Z\", \"approvedBy\": \"admin-console\"}','2026-10-13 09:34:14','2026-10-13 09:35:02');
/*!40000 ALTER TABLE `payments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `reviews`
--

DROP TABLE IF EXISTS `reviews`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `reviews` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int NOT NULL,
  `tour_id` int NOT NULL,
  `booking_id` int NOT NULL,
  `rating` tinyint NOT NULL,
  `comment` text COLLATE utf8mb4_unicode_ci,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `booking_id` (`booking_id`),
  KEY `idx_reviews_tour` (`tour_id`),
  KEY `idx_reviews_user` (`user_id`),
  KEY `idx_reviews_tour_created` (`tour_id`,`created_at`),
  CONSTRAINT `fk_reviews_booking` FOREIGN KEY (`booking_id`) REFERENCES `bookings` (`id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_reviews_tour` FOREIGN KEY (`tour_id`) REFERENCES `tours` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_reviews_user` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT,
  CONSTRAINT `chk_reviews_rating` CHECK ((`rating` between 1 and 5))
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `reviews`
--

LOCK TABLES `reviews` WRITE;
/*!40000 ALTER TABLE `reviews` DISABLE KEYS */;
INSERT INTO `reviews` VALUES (4,2,46,36,4,'nice','2026-10-15 01:08:04','2026-10-15 01:08:04');
/*!40000 ALTER TABLE `reviews` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `schema_migrations`
--

DROP TABLE IF EXISTS `schema_migrations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `schema_migrations` (
  `id` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `executed_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `schema_migrations`
--

LOCK TABLES `schema_migrations` WRITE;
/*!40000 ALTER TABLE `schema_migrations` DISABLE KEYS */;
INSERT INTO `schema_migrations` VALUES ('001-harden-constraints','2026-09-29 00:01:40'),('002-add-query-indexes','2026-09-29 00:02:16'),('003-add-refunded-payment-status','2026-09-29 00:08:42'),('004-add-tour-video-url','2026-10-12 00:41:00'),('005-replace-vnpay-with-vietqr','2026-10-13 07:56:10'),('006-booking-lifecycle-and-commercial-terms','2026-10-14 01:08:47'),('007-payment-reconciliation-and-verification','2026-10-14 01:08:48');
/*!40000 ALTER TABLE `schema_migrations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tour_departures`
--

DROP TABLE IF EXISTS `tour_departures`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tour_departures` (
  `id` int NOT NULL AUTO_INCREMENT,
  `tour_id` int NOT NULL,
  `start_date` date NOT NULL,
  `end_date` date NOT NULL,
  `departure_city` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `meeting_point` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `meeting_time` datetime DEFAULT NULL,
  `capacity` int NOT NULL,
  `booked_seats` int NOT NULL DEFAULT '0',
  `adult_price` decimal(12,2) NOT NULL,
  `child_price` decimal(12,2) NOT NULL,
  `infant_price` decimal(12,2) NOT NULL DEFAULT '0.00',
  `senior_price` decimal(12,2) DEFAULT NULL,
  `single_room_surcharge` decimal(12,2) NOT NULL DEFAULT '0.00',
  `terms_override` text COLLATE utf8mb4_unicode_ci,
  `hotel_id` int DEFAULT NULL,
  `vehicle_id` int DEFAULT NULL,
  `guide_id` int DEFAULT NULL,
  `status` enum('OPEN','CLOSED','COMPLETED','CANCELLED') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'OPEN',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `deleted_at` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_departures_hotel` (`hotel_id`),
  KEY `fk_departures_vehicle` (`vehicle_id`),
  KEY `fk_departures_guide` (`guide_id`),
  KEY `idx_departures_tour` (`tour_id`),
  KEY `idx_departures_start_date` (`start_date`),
  KEY `idx_departures_status` (`status`),
  KEY `idx_departures_status_date` (`status`,`start_date`),
  CONSTRAINT `fk_departures_guide` FOREIGN KEY (`guide_id`) REFERENCES `tour_guides` (`id`) ON DELETE SET NULL,
  CONSTRAINT `fk_departures_hotel` FOREIGN KEY (`hotel_id`) REFERENCES `hotels` (`id`) ON DELETE SET NULL,
  CONSTRAINT `fk_departures_tour` FOREIGN KEY (`tour_id`) REFERENCES `tours` (`id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_departures_vehicle` FOREIGN KEY (`vehicle_id`) REFERENCES `vehicles` (`id`) ON DELETE SET NULL,
  CONSTRAINT `chk_departure_capacity` CHECK (((`capacity` > 0) and (`booked_seats` >= 0) and (`booked_seats` <= `capacity`))),
  CONSTRAINT `chk_departure_dates` CHECK ((`end_date` >= `start_date`)),
  CONSTRAINT `chk_departure_prices` CHECK (((`adult_price` > 0) and (`child_price` >= 0)))
) ENGINE=InnoDB AUTO_INCREMENT=153 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tour_departures`
--

LOCK TABLES `tour_departures` WRITE;
/*!40000 ALTER TABLE `tour_departures` DISABLE KEYS */;
INSERT INTO `tour_departures` VALUES (7,7,'2026-11-12','2026-11-15',NULL,NULL,NULL,28,6,7490000.00,4490000.00,0.00,NULL,0.00,NULL,1,NULL,NULL,'OPEN','2026-09-29 00:18:26','2026-10-14 01:22:53',NULL),(12,7,'2026-11-13','2026-11-16',NULL,NULL,NULL,28,0,7490000.00,4490000.00,0.00,NULL,0.00,NULL,1,NULL,NULL,'OPEN','2026-10-12 00:41:05','2026-10-12 00:41:05',NULL),(13,7,'2026-12-11','2026-12-14',NULL,NULL,NULL,28,0,7490000.00,4490000.00,0.00,NULL,0.00,NULL,1,NULL,NULL,'OPEN','2026-10-12 00:41:05','2026-10-12 00:41:05',NULL),(14,7,'2027-01-08','2027-01-11',NULL,NULL,NULL,28,0,7490000.00,4490000.00,0.00,NULL,0.00,NULL,1,NULL,NULL,'OPEN','2026-10-12 00:41:05','2026-10-12 00:41:05',NULL),(15,12,'2026-11-14','2026-11-17',NULL,NULL,NULL,24,2,8990000.00,5390000.00,0.00,NULL,0.00,NULL,8,NULL,NULL,'OPEN','2026-10-12 00:41:05','2026-10-13 07:30:39',NULL),(16,12,'2026-12-12','2026-12-15',NULL,NULL,NULL,24,0,8990000.00,5390000.00,0.00,NULL,0.00,NULL,8,NULL,NULL,'OPEN','2026-10-12 00:41:05','2026-10-12 00:41:05',NULL),(17,12,'2027-01-09','2027-01-12',NULL,NULL,NULL,24,0,8990000.00,5390000.00,0.00,NULL,0.00,NULL,8,NULL,NULL,'OPEN','2026-10-12 00:41:05','2026-10-12 00:41:05',NULL),(18,13,'2026-11-08','2026-11-10',NULL,NULL,NULL,24,4,4990000.00,2990000.00,0.00,NULL,0.00,NULL,9,NULL,NULL,'OPEN','2026-10-12 00:41:05','2026-10-14 01:22:54',NULL),(19,13,'2026-12-13','2026-12-15',NULL,NULL,NULL,24,0,4990000.00,2990000.00,0.00,NULL,0.00,NULL,9,NULL,NULL,'OPEN','2026-10-12 00:41:05','2026-10-12 00:41:05',NULL),(20,13,'2027-01-10','2027-01-12',NULL,NULL,NULL,24,0,4990000.00,2990000.00,0.00,NULL,0.00,NULL,9,NULL,NULL,'OPEN','2026-10-12 00:41:05','2026-10-12 00:41:05',NULL),(21,14,'2026-11-09','2026-11-12',NULL,NULL,NULL,24,5,9490000.00,5690000.00,0.00,NULL,0.00,NULL,10,NULL,NULL,'OPEN','2026-10-12 00:41:05','2026-10-14 01:22:54',NULL),(22,14,'2026-12-14','2026-12-17',NULL,NULL,NULL,24,0,9490000.00,5690000.00,0.00,NULL,0.00,NULL,10,NULL,NULL,'OPEN','2026-10-12 00:41:05','2026-10-12 00:41:05',NULL),(23,14,'2027-01-11','2027-01-14',NULL,NULL,NULL,24,0,9490000.00,5690000.00,0.00,NULL,0.00,NULL,10,NULL,NULL,'OPEN','2026-10-12 00:41:05','2026-10-12 00:41:05',NULL),(24,15,'2026-11-10','2026-11-11',NULL,NULL,NULL,28,0,2690000.00,1590000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 00:41:05','2026-10-12 00:41:05',NULL),(25,15,'2026-12-08','2026-12-09',NULL,NULL,NULL,28,0,2690000.00,1590000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 00:41:05','2026-10-12 00:41:05',NULL),(26,15,'2027-01-12','2027-01-13',NULL,NULL,NULL,28,0,2690000.00,1590000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 00:41:05','2026-10-12 00:41:05',NULL),(27,16,'2026-11-11','2026-11-13',NULL,NULL,NULL,24,0,4290000.00,2590000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 00:41:05','2026-10-12 00:41:05',NULL),(28,16,'2026-12-09','2026-12-11',NULL,NULL,NULL,24,0,4290000.00,2590000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 00:41:05','2026-10-12 00:41:05',NULL),(29,16,'2027-01-13','2027-01-15',NULL,NULL,NULL,24,0,4290000.00,2590000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 00:41:05','2026-10-12 00:41:05',NULL),(36,23,'2026-11-12','2026-11-15',NULL,NULL,NULL,20,0,6890000.00,4190000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(37,23,'2026-12-10','2026-12-13',NULL,NULL,NULL,20,0,6890000.00,4190000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(38,23,'2027-01-14','2027-01-17',NULL,NULL,NULL,20,0,6890000.00,4190000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(39,24,'2026-11-13','2026-11-15',NULL,NULL,NULL,22,0,5290000.00,3190000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(40,24,'2026-12-11','2026-12-13',NULL,NULL,NULL,22,0,5290000.00,3190000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(41,24,'2027-01-08','2027-01-10',NULL,NULL,NULL,22,0,5290000.00,3190000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(42,25,'2026-11-14','2026-11-15',NULL,NULL,NULL,28,0,2890000.00,1690000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(43,25,'2026-12-12','2026-12-13',NULL,NULL,NULL,28,0,2890000.00,1690000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(44,25,'2027-01-09','2027-01-10',NULL,NULL,NULL,28,0,2890000.00,1690000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(45,26,'2026-11-08','2026-11-10',NULL,NULL,NULL,22,0,5790000.00,3490000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(46,26,'2026-12-13','2026-12-15',NULL,NULL,NULL,22,0,5790000.00,3490000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(47,26,'2027-01-10','2027-01-12',NULL,NULL,NULL,22,0,5790000.00,3490000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(48,27,'2026-11-09','2026-11-10',NULL,NULL,NULL,20,0,5990000.00,3590000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(49,27,'2026-12-14','2026-12-15',NULL,NULL,NULL,20,0,5990000.00,3590000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(50,27,'2027-01-11','2027-01-12',NULL,NULL,NULL,20,0,5990000.00,3590000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(51,28,'2026-11-10','2026-11-11',NULL,NULL,NULL,28,0,2990000.00,1790000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(52,28,'2026-12-08','2026-12-09',NULL,NULL,NULL,28,0,2990000.00,1790000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(53,28,'2027-01-12','2027-01-13',NULL,NULL,NULL,28,0,2990000.00,1790000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(54,29,'2026-11-11','2026-11-13',NULL,NULL,NULL,26,0,6490000.00,3890000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(55,29,'2026-12-09','2026-12-11',NULL,NULL,NULL,26,0,6490000.00,3890000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(56,29,'2027-01-13','2027-01-15',NULL,NULL,NULL,26,0,6490000.00,3890000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(57,30,'2026-11-12','2026-11-14',NULL,NULL,NULL,24,0,5290000.00,3190000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(58,30,'2026-12-10','2026-12-12',NULL,NULL,NULL,24,0,5290000.00,3190000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(59,30,'2027-01-14','2027-01-16',NULL,NULL,NULL,24,0,5290000.00,3190000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(60,31,'2026-11-13','2026-11-16',NULL,NULL,NULL,24,0,7290000.00,4390000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(61,31,'2026-12-11','2026-12-14',NULL,NULL,NULL,24,0,7290000.00,4390000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(62,31,'2027-01-08','2027-01-11',NULL,NULL,NULL,24,0,7290000.00,4390000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(63,32,'2026-11-14','2026-11-16',NULL,NULL,NULL,28,0,6190000.00,3690000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(64,32,'2026-12-12','2026-12-14',NULL,NULL,NULL,28,0,6190000.00,3690000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(65,32,'2027-01-09','2027-01-11',NULL,NULL,NULL,28,0,6190000.00,3690000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(66,33,'2026-11-08','2026-11-11',NULL,NULL,NULL,24,0,7890000.00,4690000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(67,33,'2026-12-13','2026-12-16',NULL,NULL,NULL,24,0,7890000.00,4690000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(68,33,'2027-01-10','2027-01-13',NULL,NULL,NULL,24,0,7890000.00,4690000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(69,34,'2026-11-09','2026-11-11',NULL,NULL,NULL,26,0,4890000.00,2890000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(70,34,'2026-12-14','2026-12-16',NULL,NULL,NULL,26,0,4890000.00,2890000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(71,34,'2027-01-11','2027-01-13',NULL,NULL,NULL,26,0,4890000.00,2890000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(72,35,'2026-11-10','2026-11-12',NULL,NULL,NULL,24,0,5690000.00,3390000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(73,35,'2026-12-08','2026-12-10',NULL,NULL,NULL,24,0,5690000.00,3390000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(74,35,'2027-01-12','2027-01-14',NULL,NULL,NULL,24,0,5690000.00,3390000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(75,36,'2026-11-11','2026-11-13',NULL,NULL,NULL,26,0,4490000.00,2690000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(76,36,'2026-12-09','2026-12-11',NULL,NULL,NULL,26,0,4490000.00,2690000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(77,36,'2027-01-13','2027-01-15',NULL,NULL,NULL,26,0,4490000.00,2690000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(78,37,'2026-11-12','2026-11-16',NULL,NULL,NULL,24,0,9790000.00,5890000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(79,37,'2026-12-10','2026-12-14',NULL,NULL,NULL,24,0,9790000.00,5890000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(80,37,'2027-01-14','2027-01-18',NULL,NULL,NULL,24,0,9790000.00,5890000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(81,38,'2026-11-13','2026-11-15',NULL,NULL,NULL,24,0,4590000.00,2790000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(82,38,'2026-12-11','2026-12-13',NULL,NULL,NULL,24,0,4590000.00,2790000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(83,38,'2027-01-08','2027-01-10',NULL,NULL,NULL,24,0,4590000.00,2790000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(84,39,'2026-11-14','2026-11-17',NULL,NULL,NULL,24,0,6990000.00,4190000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(85,39,'2026-12-12','2026-12-15',NULL,NULL,NULL,24,0,6990000.00,4190000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(86,39,'2027-01-09','2027-01-12',NULL,NULL,NULL,24,0,6990000.00,4190000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(87,40,'2026-11-08','2026-11-09',NULL,NULL,NULL,30,0,2790000.00,1690000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(88,40,'2026-12-13','2026-12-14',NULL,NULL,NULL,30,0,2790000.00,1690000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(89,40,'2027-01-10','2027-01-11',NULL,NULL,NULL,30,0,2790000.00,1690000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(90,41,'2026-11-09','2026-11-10',NULL,NULL,NULL,28,0,2990000.00,1790000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(91,41,'2026-12-14','2026-12-15',NULL,NULL,NULL,28,0,2990000.00,1790000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(92,41,'2027-01-11','2027-01-12',NULL,NULL,NULL,28,0,2990000.00,1790000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(93,42,'2026-11-10','2026-11-11',NULL,NULL,NULL,20,0,3190000.00,1890000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(94,42,'2026-12-08','2026-12-09',NULL,NULL,NULL,20,0,3190000.00,1890000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(95,42,'2027-01-12','2027-01-13',NULL,NULL,NULL,20,0,3190000.00,1890000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(96,43,'2026-11-11','2026-11-13',NULL,NULL,NULL,24,0,4490000.00,2690000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(97,43,'2026-12-09','2026-12-11',NULL,NULL,NULL,24,0,4490000.00,2690000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(98,43,'2027-01-13','2027-01-15',NULL,NULL,NULL,24,0,4490000.00,2690000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(99,44,'2026-11-12','2026-11-14',NULL,NULL,NULL,20,0,8990000.00,5390000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(100,44,'2026-12-10','2026-12-12',NULL,NULL,NULL,20,0,8990000.00,5390000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(101,44,'2027-01-14','2027-01-16',NULL,NULL,NULL,20,0,8990000.00,5390000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(102,45,'2026-11-13','2026-11-17',NULL,NULL,NULL,22,0,12490000.00,7490000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(103,45,'2026-12-11','2026-12-15',NULL,NULL,NULL,22,0,12490000.00,7490000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(104,45,'2027-01-08','2027-01-12',NULL,NULL,NULL,22,0,12490000.00,7490000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(105,46,'2026-11-14','2026-11-16',NULL,NULL,NULL,24,1,5490000.00,3290000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-14 01:22:54',NULL),(106,46,'2026-12-12','2026-12-14',NULL,NULL,NULL,24,0,5490000.00,3290000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(107,46,'2027-01-09','2027-01-11',NULL,NULL,NULL,24,0,5490000.00,3290000.00,0.00,NULL,0.00,NULL,NULL,NULL,NULL,'OPEN','2026-10-12 01:07:46','2026-10-12 01:07:46',NULL);
/*!40000 ALTER TABLE `tour_departures` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tour_destinations`
--

DROP TABLE IF EXISTS `tour_destinations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tour_destinations` (
  `tour_id` int NOT NULL,
  `destination_id` int NOT NULL,
  PRIMARY KEY (`tour_id`,`destination_id`),
  KEY `fk_td_destination` (`destination_id`),
  CONSTRAINT `fk_td_destination` FOREIGN KEY (`destination_id`) REFERENCES `destinations` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_td_tour` FOREIGN KEY (`tour_id`) REFERENCES `tours` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tour_destinations`
--

LOCK TABLES `tour_destinations` WRITE;
/*!40000 ALTER TABLE `tour_destinations` DISABLE KEYS */;
INSERT INTO `tour_destinations` VALUES (7,1),(32,1),(7,2),(32,2),(14,3),(45,3),(13,4),(12,15),(16,15),(29,15),(12,16),(27,16),(29,16),(12,17),(28,17),(7,18),(31,18),(15,19),(46,19),(23,26),(24,27),(25,28),(26,29),(30,30),(31,30),(33,31),(37,31),(34,32),(37,32),(35,33),(36,34),(38,35),(39,35),(40,36),(41,37),(39,38),(42,38),(43,39),(46,39),(44,40);
/*!40000 ALTER TABLE `tour_destinations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tour_guides`
--

DROP TABLE IF EXISTS `tour_guides`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tour_guides` (
  `id` int NOT NULL AUTO_INCREMENT,
  `full_name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `phone_number` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `email` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `experience_years` tinyint DEFAULT '1',
  `languages` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT '1',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=33 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tour_guides`
--

LOCK TABLES `tour_guides` WRITE;
/*!40000 ALTER TABLE `tour_guides` DISABLE KEYS */;
INSERT INTO `tour_guides` VALUES (1,'Nguyễn Văn Minh','0912345678','minh.guide@leviet.com',5,'Tiếng Việt, Tiếng Anh',1,'2026-09-28 18:32:23','2026-09-28 18:32:23'),(2,'Lê Thị Tuyết Mai','0923456789','mai.guide@leviet.com',3,'Tiếng Việt, Tiếng Hàn',1,'2026-09-28 18:32:23','2026-09-28 18:32:23');
/*!40000 ALTER TABLE `tour_guides` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tour_itineraries`
--

DROP TABLE IF EXISTS `tour_itineraries`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tour_itineraries` (
  `id` int NOT NULL AUTO_INCREMENT,
  `tour_id` int NOT NULL,
  `day_number` int NOT NULL,
  `title` varchar(200) COLLATE utf8mb4_unicode_ci NOT NULL,
  `description` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_itinerary_tour_day` (`tour_id`,`day_number`),
  KEY `idx_itinerary_tour_day` (`tour_id`,`day_number`),
  CONSTRAINT `fk_itinerary_tour` FOREIGN KEY (`tour_id`) REFERENCES `tours` (`id`) ON DELETE CASCADE,
  CONSTRAINT `chk_itinerary_day_positive` CHECK ((`day_number` > 0))
) ENGINE=InnoDB AUTO_INCREMENT=125 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tour_itineraries`
--

LOCK TABLES `tour_itineraries` WRITE;
/*!40000 ALTER TABLE `tour_itineraries` DISABLE KEYS */;
INSERT INTO `tour_itineraries` VALUES (4,7,1,'Đà Nẵng - Sơn Trà - Mỹ Khê','Đón khách tại Đà Nẵng, tham quan bán đảo Sơn Trà và chùa Linh Ứng. Buổi chiều tự do dạo biển Mỹ Khê; buổi tối khám phá khu vực sông Hàn.','2026-09-29 00:18:26','2026-10-12 00:41:05'),(5,7,2,'Đà Nẵng - Hải Vân - Cố đô Huế','Khởi hành đi Huế qua cung đường Hải Vân. Tham quan Đại Nội, chùa Thiên Mụ và dạo bên sông Hương trước khi trở về Đà Nẵng.','2026-09-29 00:18:26','2026-10-12 00:41:05'),(6,7,3,'Ngũ Hành Sơn - Phố cổ Hội An','Tham quan danh thắng Ngũ Hành Sơn, di chuyển đến Hội An. Dạo bộ qua Chùa Cầu, các hội quán và những tuyến phố di sản ven sông Hoài.','2026-09-29 00:18:26','2026-10-12 00:41:05'),(9,7,4,'Hội An - Đà Nẵng - Tiễn khách','Buổi sáng tự do thưởng thức ẩm thực địa phương và mua đặc sản. Trả phòng, trở lại Đà Nẵng và kết thúc hành trình.','2026-10-12 00:41:05','2026-10-12 00:41:05'),(10,12,1,'Hà Nội - Hồ Hoàn Kiếm - Phố cổ','Đón khách tại Hà Nội. Tham quan khu vực hồ Hoàn Kiếm, đền Ngọc Sơn và đi bộ qua các tuyến phố nghề trong khu phố cổ.','2026-10-12 00:41:05','2026-10-12 00:41:05'),(11,12,2,'Hà Nội - Hoa Lư - Tràng An','Di chuyển đến Ninh Bình, tìm hiểu khu di tích Cố đô Hoa Lư và trải nghiệm tuyến thuyền xuyên qua cảnh quan núi đá vôi của Tràng An.','2026-10-12 00:41:05','2026-10-12 00:41:05'),(12,12,3,'Ninh Bình - Vịnh Hạ Long','Khởi hành đi Hạ Long. Lên tàu tham quan vịnh, ngắm các đảo đá vôi và trải nghiệm điểm hang động theo tuyến khai thác trong ngày.','2026-10-12 00:41:05','2026-10-12 00:41:05'),(13,12,4,'Hạ Long - Hà Nội','Buổi sáng nghỉ ngơi bên vịnh hoặc khám phá Bãi Cháy. Trả phòng, trở về Hà Nội và kết thúc hành trình.','2026-10-12 00:41:05','2026-10-12 00:41:05'),(14,13,1,'Hà Nội - Sa Pa - Bản Cát Cát','Khởi hành từ Hà Nội đến Sa Pa. Buổi chiều đi bộ tham quan bản Cát Cát, tìm hiểu nghề thủ công và không gian cư trú truyền thống của người H’Mông.','2026-10-12 00:41:05','2026-10-12 00:41:05'),(15,13,2,'Fansipan - Thị xã Sa Pa','Tham quan khu vực Fansipan bằng hệ thống cáp treo và tàu hỏa leo núi theo điều kiện thời tiết. Buổi chiều tự do khám phá nhà thờ đá và trung tâm Sa Pa.','2026-10-12 00:41:05','2026-10-12 00:41:05'),(16,13,3,'Mường Hoa - Tả Van - Hà Nội','Tham quan thung lũng Mường Hoa và khu vực Tả Van, ngắm ruộng bậc thang theo mùa. Trở về Hà Nội vào cuối ngày.','2026-10-12 00:41:05','2026-10-12 00:41:05'),(17,14,1,'Đến Phú Quốc - Dương Đông','Đón khách tại sân bay Phú Quốc, nhận phòng. Buổi chiều tham quan Dinh Cậu; buổi tối tự do khám phá chợ đêm Dương Đông.','2026-10-12 00:41:05','2026-10-12 00:41:05'),(18,14,2,'Nam đảo - Quần đảo An Thới','Khám phá khu vực Nam đảo, đi cáp treo vượt biển đến Hòn Thơm hoặc tham gia tuyến canô tham quan các đảo tùy điều kiện vận hành và thời tiết.','2026-10-12 00:41:05','2026-10-12 00:41:05'),(19,14,3,'Hàm Ninh - Vườn tiêu - Nghề làm nước mắm','Thăm làng chài Hàm Ninh, vườn tiêu và cơ sở sản xuất nước mắm để tìm hiểu các nghề truyền thống gắn với đời sống trên đảo.','2026-10-12 00:41:05','2026-10-12 00:41:05'),(20,14,4,'Phú Quốc - Tiễn sân bay','Tự do nghỉ ngơi, tắm biển và mua đặc sản. Trả phòng, di chuyển ra sân bay và kết thúc chuyến đi.','2026-10-12 00:41:05','2026-10-12 00:41:05'),(21,15,1,'TP. Hồ Chí Minh - Bình Thủy - Ninh Kiều','Khởi hành đến Cần Thơ. Tham quan nhà cổ Bình Thủy, dạo bến Ninh Kiều và trải nghiệm ẩm thực địa phương bên sông Hậu.','2026-10-12 00:41:05','2026-10-12 00:41:05'),(22,15,2,'Chợ nổi Cái Răng - Vườn cây - Trở về','Đi thuyền từ sáng sớm để quan sát hoạt động mua bán tại chợ nổi Cái Răng. Ghé làng nghề hoặc vườn cây theo mùa trước khi trở về TP. Hồ Chí Minh.','2026-10-12 00:41:05','2026-10-12 00:41:05'),(23,16,1,'Hồ Hoàn Kiếm - Phố cổ Hà Nội','Đón khách, tham quan hồ Hoàn Kiếm và đền Ngọc Sơn. Đi bộ qua khu phố cổ, thưởng thức món ăn địa phương tại các cơ sở phù hợp khẩu vị.','2026-10-12 00:41:05','2026-10-12 00:41:05'),(24,16,2,'Ba Đình - Văn Miếu - Múa rối nước','Tham quan khu vực Quảng trường Ba Đình, Văn Miếu - Quốc Tử Giám và một bảo tàng phù hợp lịch mở cửa. Buổi tối xem biểu diễn múa rối nước.','2026-10-12 00:41:05','2026-10-12 00:41:05'),(25,16,3,'Hồ Tây - Chợ địa phương - Tiễn khách','Dạo khu vực hồ Tây, ghé chợ hoặc phố ẩm thực để tìm hiểu nguyên liệu và món ăn Hà Nội. Trả phòng và kết thúc chương trình.','2026-10-12 00:41:05','2026-10-12 00:41:05'),(29,23,1,'Hà Nội - Hà Giang - Quản Bạ','Khởi hành sớm từ Hà Nội, đến thành phố Hà Giang và tiếp tục qua dốc Bắc Sum, cổng trời Quản Bạ. Nghỉ đêm khu vực Yên Minh.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(30,23,2,'Yên Minh - Lũng Cú - Đồng Văn','Đi qua các điểm địa chất tiêu biểu của Cao nguyên đá, tham quan dinh thự họ Vương và cột cờ Lũng Cú trước khi về phố cổ Đồng Văn.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(31,23,3,'Đồng Văn - Mã Pì Lèng - sông Nho Quế','Chinh phục đèo Mã Pì Lèng, ngắm hẻm Tu Sản và trải nghiệm thuyền trên sông Nho Quế khi điều kiện vận hành cho phép.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(32,23,4,'Đồng Văn - Hà Giang - Hà Nội','Trở lại thành phố Hà Giang theo cung đường Quản Bạ, dừng nghỉ và mua đặc sản địa phương trước khi về Hà Nội.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(33,24,1,'Hà Nội - Cao Bằng','Khởi hành theo tuyến cao tốc và quốc lộ đến Cao Bằng, nhận phòng và tự do tìm hiểu ẩm thực khu vực trung tâm thành phố.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(34,24,2,'Thác Bản Giốc - động Ngườm Ngao','Tham quan thác Bản Giốc ở huyện Trùng Khánh, sau đó khám phá hệ thống nhũ đá tại động Ngườm Ngao theo tuyến mở cửa.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(35,24,3,'Pác Bó - Hà Nội','Tham quan khu di tích Pác Bó, suối Lê Nin và núi Các Mác; trở về Hà Nội vào cuối ngày.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(36,25,1,'Hà Nội - cao nguyên Mộc Châu','Di chuyển đến Mộc Châu, tham quan đồi chè và một điểm vườn hoa hoặc vườn mận đang vào mùa; nhận phòng và nghỉ đêm.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(37,25,2,'Rừng thông - thác Dải Yếm - Hà Nội','Dạo rừng thông Bản Áng, tham quan thác Dải Yếm theo tình hình thời tiết, thưởng thức nông sản địa phương rồi trở về Hà Nội.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(38,26,1,'Hà Nội - Hải Phòng - Cát Bà','Di chuyển đến Hải Phòng, qua tuyến phà hoặc cáp treo đang vận hành để sang Cát Bà; nhận phòng và dạo thị trấn.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(39,26,2,'Vịnh Lan Hạ - làng Việt Hải','Lên tàu tham quan vịnh Lan Hạ, chèo kayak tại khu vực được cấp phép và ghé làng Việt Hải tùy lịch tàu, thủy triều và thời tiết.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(40,26,3,'Vườn quốc gia Cát Bà - Hà Nội','Tham quan một tuyến ngắn trong Vườn quốc gia Cát Bà, trả phòng và trở về Hà Nội.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(41,27,1,'Hà Nội - cảng Hạ Long - ngủ đêm trên vịnh','Đón khách tại Hà Nội, làm thủ tục lên du thuyền, dùng bữa trưa và tham quan hang động hoặc điểm đảo theo lịch của tàu.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(42,27,2,'Bình minh trên vịnh - Hà Nội','Tham gia hoạt động buổi sáng, trả cabin, dùng bữa sớm và rời tàu tại cảng trước khi trở về Hà Nội.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(43,28,1,'Hà Nội - Hoa Lư - Tràng An','Đến Ninh Bình, tham quan khu di tích Cố đô Hoa Lư và đi thuyền theo tuyến đang khai thác tại Tràng An.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(44,28,2,'Hang Múa - Tam Cốc - Hà Nội','Chinh phục điểm ngắm cảnh Hang Múa theo thể trạng, dạo khu vực Tam Cốc và trở về Hà Nội vào buổi chiều.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(45,29,1,'Đến Hà Nội - khu phố cổ','Đón khách, nhận phòng và tham quan hồ Hoàn Kiếm, đền Ngọc Sơn cùng các phố nghề trong khu phố cổ.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(46,29,2,'Hà Nội - vịnh Hạ Long','Di chuyển đến Hạ Long, lên tàu tham quan vịnh theo tuyến trong ngày, ghé hang động và dùng bữa trên tàu.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(47,29,3,'Hạ Long - Hà Nội - tiễn khách','Nghỉ ngơi tại Bãi Cháy, trả phòng và về Hà Nội; kết thúc tại điểm hẹn hoặc sân bay theo lựa chọn dịch vụ.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(48,30,1,'Đồng Hới - biển Nhật Lệ','Đón khách tại ga hoặc sân bay Đồng Hới, nhận phòng và nghỉ ngơi; buổi chiều tự do dạo biển Nhật Lệ.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(49,30,2,'Động Phong Nha - sông Son','Đi thuyền trên sông Son vào động Phong Nha theo tuyến đang mở, tìm hiểu địa chất và đời sống khu vực Sơn Trạch.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(50,30,3,'Động Thiên Đường - tiễn khách','Tham quan tuyến phổ thông tại động Thiên Đường, trở lại Đồng Hới và kết thúc chương trình.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(51,31,1,'Đến Huế - sông Hương','Đón khách, nhận phòng và tham quan chùa Thiên Mụ; buổi tối tự do khám phá ẩm thực Huế.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(52,31,2,'Đại Nội - lăng vua Nguyễn','Tham quan Đại Nội và một công trình lăng tẩm tiêu biểu đang mở cửa, sau đó nghỉ ngơi bên sông Hương.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(53,31,3,'Huế - Phong Nha - Đồng Hới','Khởi hành đi Quảng Bình, đi thuyền tham quan động Phong Nha và nghỉ đêm tại Đồng Hới.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(54,31,4,'Đồng Hới - tiễn khách','Dạo biển Nhật Lệ hoặc tham quan một điểm nội thành phù hợp giờ tàu, chuyến bay; kết thúc hành trình.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(55,32,1,'Đà Nẵng - Sơn Trà - Mỹ Khê','Đón khách, tham quan bán đảo Sơn Trà và chùa Linh Ứng; buổi chiều nghỉ tại biển Mỹ Khê.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(56,32,2,'Khu du lịch Bà Nà','Di chuyển đến Bà Nà, sử dụng cáp treo và tham quan các khu vực đang mở; trở lại Đà Nẵng cuối ngày.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(57,32,3,'Ngũ Hành Sơn - Hội An - tiễn khách','Tham quan Ngũ Hành Sơn rồi dạo phố cổ Hội An, Chùa Cầu và các hội quán trước khi kết thúc.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(58,33,1,'Đến Nha Trang - biển Trần Phú','Đón khách, nhận phòng và tự do nghỉ biển tại khu vực trung tâm dọc đường Trần Phú.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(59,33,2,'Du ngoạn vịnh Nha Trang','Đi tàu hoặc canô theo tuyến đảo được phép khai thác, tham gia bơi hoặc lặn ngắm san hô tùy thời tiết và thể trạng.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(60,33,3,'Tháp Bà Ponagar - Hòn Chồng','Tìm hiểu di sản Chăm tại tháp Bà Ponagar, tham quan Hòn Chồng và trải nghiệm tắm bùn theo gói đã chọn.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(61,33,4,'Nha Trang - tiễn khách','Tự do mua đặc sản, trả phòng và di chuyển ra ga hoặc sân bay Cam Ranh.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(62,34,1,'Đến Đà Lạt - hồ Xuân Hương','Đón khách, nhận phòng, dạo hồ Xuân Hương và quảng trường Lâm Viên; buổi tối tự do khám phá chợ Đà Lạt.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(63,34,2,'Ngoại ô Đà Lạt - rừng thông - thác','Tham quan một tuyến ngoại ô gồm rừng thông, nông trại và thác nước đang mở cửa, ưu tiên trải nghiệm có kiểm soát an toàn.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(64,34,3,'Kiến trúc Đà Lạt - tiễn khách','Thăm ga Đà Lạt hoặc một công trình kiến trúc tiêu biểu, mua nông sản và kết thúc chương trình.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(65,35,1,'Đến Quy Nhơn - Ghềnh Ráng','Đón khách, nhận phòng và tham quan khu vực Ghềnh Ráng; buổi tối trải nghiệm ẩm thực hải sản địa phương.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(66,35,2,'Kỳ Co - Eo Gió','Di chuyển ra bán đảo Phương Mai, tham quan Eo Gió và đến Kỳ Co bằng phương tiện được phép tùy điều kiện biển.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(67,35,3,'Tháp Chăm - tiễn khách','Tham quan một cụm tháp Chăm tiêu biểu gần Quy Nhơn, trả phòng và tiễn khách.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(68,36,1,'TP. Hồ Chí Minh - Mũi Né','Khởi hành đến Mũi Né, nhận phòng và tự do nghỉ biển; dạo khu vực Hàm Tiến vào buổi tối.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(69,36,2,'Bàu Trắng - đồi cát - làng chài','Khởi hành sớm ngắm bình minh tại đồi cát, tham quan Bàu Trắng và làng chài, trở về resort nghỉ ngơi.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(70,36,3,'Suối Tiên - TP. Hồ Chí Minh','Đi bộ tuyến Suối Tiên phù hợp điều kiện dòng chảy, trả phòng và trở về TP. Hồ Chí Minh.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(71,37,1,'Đến Nha Trang - nghỉ biển','Đón khách tại Cam Ranh, về trung tâm nhận phòng và nghỉ ngơi tại bãi biển Trần Phú.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(72,37,2,'Vịnh Nha Trang','Tham gia chương trình tàu hoặc canô tham quan đảo theo điều kiện thời tiết; chiều tự do tại thành phố.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(73,37,3,'Nha Trang - Đà Lạt','Tham quan tháp Bà Ponagar trước khi di chuyển theo quốc lộ lên Đà Lạt, nhận phòng và dạo chợ đêm.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(74,37,4,'Đà Lạt ngoại ô','Khám phá một cung ngoại ô gồm rừng thông, nông trại, hồ hoặc thác đang mở cửa.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(75,37,5,'Đà Lạt - tiễn khách','Dạo hồ Xuân Hương, mua đặc sản và di chuyển ra sân bay Liên Khương.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(76,38,1,'Trung tâm Quận 1','Đón khách, tham quan các công trình trung tâm theo lịch mở cửa như Bưu điện Thành phố, đường sách và khu vực Nhà hát Thành phố.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(77,38,2,'Bảo tàng - Chợ Lớn','Tham quan một bảo tàng lịch sử, sau đó khám phá không gian văn hóa và ẩm thực tại khu vực Chợ Lớn.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(78,38,3,'Chợ Bến Thành - tiễn khách','Dạo khu vực chợ Bến Thành, mua đặc sản có nguồn gốc rõ ràng và tiễn khách tại sân bay hoặc điểm hẹn.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(79,39,1,'Đến TP. Hồ Chí Minh','Đón khách, nhận phòng và tham quan các công trình lịch sử ở khu vực trung tâm Quận 1.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(80,39,2,'Địa đạo Củ Chi','Di chuyển đến khu di tích địa đạo Củ Chi, nghe thuyết minh lịch sử và trải nghiệm tuyến tham quan phù hợp thể trạng.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(81,39,3,'TP. Hồ Chí Minh - Bến Tre','Đi thuyền trên nhánh sông Mekong, ghé cơ sở làm sản phẩm từ dừa và trải nghiệm xuồng nhỏ tại tuyến được phép khai thác.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(82,39,4,'Bến Tre - TP. Hồ Chí Minh - tiễn khách','Thăm vườn cây theo mùa, trở về TP. Hồ Chí Minh và kết thúc chương trình.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(83,40,1,'TP. Hồ Chí Minh - Vũng Tàu - Bãi Sau','Khởi hành buổi sáng, nhận phòng và nghỉ biển tại Bãi Sau; buổi chiều dạo Bãi Trước.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(84,40,2,'Bạch Dinh - Núi Nhỏ - trở về','Tham quan Bạch Dinh và một điểm ngắm cảnh tại Núi Nhỏ phù hợp thể trạng, dùng bữa trưa rồi trở về TP. Hồ Chí Minh.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(85,41,1,'TP. Hồ Chí Minh - Tòa Thánh - Tây Ninh','Di chuyển đến Tây Ninh, tham quan Tòa Thánh Cao Đài theo quy định tại điểm đến và thưởng thức đặc sản địa phương.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(86,41,2,'Núi Bà Đen - TP. Hồ Chí Minh','Sử dụng cáp treo lên khu vực núi Bà Đen, tham quan các công trình mở cửa rồi trở về TP. Hồ Chí Minh.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(87,42,1,'TP. Hồ Chí Minh - Bến Tre - làng nghề','Đến Bến Tre, đi thuyền qua các cù lao, thăm cơ sở chế biến sản phẩm từ dừa và dùng bữa với nguyên liệu địa phương.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(88,42,2,'Vườn cây - xuồng nhỏ - trở về','Thăm vườn cây theo mùa, trải nghiệm xuồng chèo trên kênh phù hợp thủy triều rồi trở về TP. Hồ Chí Minh.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(89,43,1,'TP. Hồ Chí Minh - Châu Đốc','Di chuyển đến Châu Đốc, nhận phòng và tham quan khu vực núi Sam, miếu Bà Chúa Xứ theo quy định tại điểm.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(90,43,2,'Rừng tràm Trà Sư - làng Chăm','Đi xuồng máy và xuồng chèo trên tuyến rừng tràm đang vận hành, sau đó tìm hiểu nghề dệt và đời sống cộng đồng Chăm.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(91,43,3,'Chợ Châu Đốc - TP. Hồ Chí Minh','Tham quan chợ địa phương, lựa chọn đặc sản có nhãn mác rồi trở về TP. Hồ Chí Minh.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(92,44,1,'Đến Côn Đảo - thị trấn','Đón khách tại sân bay Côn Đảo, nhận phòng và tham quan các địa điểm trung tâm thị trấn.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(93,44,2,'Hệ thống di tích - bãi biển','Tham quan bảo tàng và một số điểm thuộc hệ thống di tích lịch sử, buổi chiều nghỉ tại bãi Đầm Trầu hoặc bãi biển phù hợp thời tiết.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(94,44,3,'Cảnh quan sinh thái - tiễn sân bay','Khám phá một tuyến ngắn do Vườn quốc gia hướng dẫn hoặc tự do nghỉ biển, sau đó tiễn sân bay.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(95,45,1,'Đến Phú Quốc - Dương Đông','Đón khách tại sân bay, nhận phòng; chiều tham quan Dinh Cậu và tối tự do tại chợ đêm Dương Đông.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(96,45,2,'Nam đảo - An Thới','Tham quan Nam đảo, trải nghiệm cáp treo hoặc tuyến canô tùy gói dịch vụ và điều kiện thời tiết.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(97,45,3,'Hàm Ninh - vườn tiêu - nước mắm','Tìm hiểu làng chài Hàm Ninh, vườn tiêu và cơ sở sản xuất nước mắm truyền thống.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(98,45,4,'Ngày nghỉ dưỡng tự do','Dành trọn ngày cho nghỉ biển, dịch vụ tại resort hoặc lựa chọn trải nghiệm riêng có bảo hiểm phù hợp.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(99,45,5,'Phú Quốc - tiễn sân bay','Trả phòng, mua đặc sản tại cơ sở có nguồn gốc rõ ràng và di chuyển ra sân bay.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(100,46,1,'TP. Hồ Chí Minh - Cần Thơ','Di chuyển đến Cần Thơ, tham quan nhà cổ Bình Thủy và dạo bến Ninh Kiều vào buổi tối.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(101,46,2,'Chợ nổi Cái Răng - Châu Đốc','Đi thuyền từ sáng sớm quan sát hoạt động chợ nổi, sau đó khởi hành đến Châu Đốc và tham quan núi Sam.','2026-10-12 01:07:46','2026-10-12 01:07:46'),(102,46,3,'Rừng tràm Trà Sư - TP. Hồ Chí Minh','Tham quan rừng tràm theo tuyến xuồng đang khai thác, dùng bữa trưa và trở về TP. Hồ Chí Minh.','2026-10-12 01:07:46','2026-10-12 01:07:46');
/*!40000 ALTER TABLE `tour_itineraries` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tours`
--

DROP TABLE IF EXISTS `tours`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tours` (
  `id` int NOT NULL AUTO_INCREMENT,
  `category_id` int NOT NULL,
  `code` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `name` varchar(200) COLLATE utf8mb4_unicode_ci NOT NULL,
  `duration_days` int NOT NULL,
  `duration_nights` int NOT NULL,
  `overview` text COLLATE utf8mb4_unicode_ci,
  `included_services` text COLLATE utf8mb4_unicode_ci,
  `excluded_services` text COLLATE utf8mb4_unicode_ci,
  `cancellation_policy` text COLLATE utf8mb4_unicode_ci,
  `change_policy` text COLLATE utf8mb4_unicode_ci,
  `deposit_percent` decimal(5,2) NOT NULL DEFAULT '0.00',
  `child_policy` text COLLATE utf8mb4_unicode_ci,
  `infant_policy` text COLLATE utf8mb4_unicode_ci,
  `senior_policy` text COLLATE utf8mb4_unicode_ci,
  `pickup_points` json DEFAULT NULL,
  `meeting_instructions` text COLLATE utf8mb4_unicode_ci,
  `luggage_policy` text COLLATE utf8mb4_unicode_ci,
  `insurance_details` text COLLATE utf8mb4_unicode_ci,
  `visa_requirements` text COLLATE utf8mb4_unicode_ci,
  `required_documents` text COLLATE utf8mb4_unicode_ci,
  `terms_and_conditions` text COLLATE utf8mb4_unicode_ci,
  `thumbnail` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `images` json DEFAULT NULL,
  `video_url` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT '1',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `deleted_at` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `code` (`code`),
  KEY `idx_tours_code` (`code`),
  KEY `idx_tours_category` (`category_id`),
  KEY `idx_tours_active_created` (`is_active`,`created_at`),
  CONSTRAINT `fk_tours_category` FOREIGN KEY (`category_id`) REFERENCES `categories` (`id`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=92 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tours`
--

LOCK TABLES `tours` WRITE;
/*!40000 ALTER TABLE `tours` DISABLE KEYS */;
INSERT INTO `tours` VALUES (7,2,'DANANG-HOIAN-HUE-4N3D','Đà Nẵng - Huế - Hội An 4 ngày 3 đêm',4,3,'Hành trình kết nối ba trung tâm du lịch tiêu biểu của miền Trung. Du khách khám phá bán đảo Sơn Trà và biển Mỹ Khê, đi qua đèo Hải Vân đến quần thể di tích Cố đô Huế, sau đó trải nghiệm Ngũ Hành Sơn và không gian phố cổ Hội An bên sông Hoài.',NULL,NULL,NULL,NULL,0.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?auto=format&fit=crop&w=1600&q=82','[\"https://images.unsplash.com/photo-1603852452378-a4e8d84324a2?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1563354860-799d15199ac3?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/flagged/photo-1549874613-8ea0adee1a82?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1618165220283-e85246c4171c?auto=format&fit=crop&w=1600&q=82\"]','https://www.youtube.com/watch?v=twb6d3t1SO0',1,'2026-09-29 00:18:26','2026-10-12 00:41:05',NULL),(12,4,'HANOI-NINHBINH-HALONG-4N3D','Hà Nội - Ninh Bình - Hạ Long 4 ngày 3 đêm',4,3,'Hành trình miền Bắc cân bằng giữa chiều sâu văn hóa Hà Nội và hai vùng cảnh quan karst nổi tiếng. Lịch trình đi thuyền tại Tràng An, tìm hiểu Cố đô Hoa Lư và khám phá vịnh Hạ Long với các đảo đá vôi đặc trưng.',NULL,NULL,NULL,NULL,0.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'https://images.unsplash.com/photo-1643029891412-92f9a81a8c16?auto=format&fit=crop&w=1600&q=82','[\"https://images.unsplash.com/photo-1531737212413-667205e1cda7?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1504457047772-27faf1c00561?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1697850084120-4896a446a04d?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1509030450996-dd1a26dda07a?auto=format&fit=crop&w=1600&q=82\"]','https://www.youtube.com/watch?v=Lgvc0l1UyaU',1,'2026-10-12 00:41:05','2026-10-12 00:41:05',NULL),(13,3,'SAPA-FANSIPAN-3N2D','Sa Pa - Fansipan - Mường Hoa 3 ngày 2 đêm',3,2,'Chuyến đi tập trung vào cảnh quan Hoàng Liên Sơn và đời sống vùng cao Sa Pa. Du khách có thời gian tham quan bản Cát Cát, chinh phục khu vực đỉnh Fansipan và đi qua thung lũng Mường Hoa - Tả Van.',NULL,NULL,NULL,NULL,0.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'https://images.unsplash.com/photo-1570366583862-f91883984fde?auto=format&fit=crop&w=1600&q=82','[\"https://images.unsplash.com/photo-1732099083114-4fb49a280eec?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1694152363060-cc26bde73872?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1570366583862-f91883984fde?auto=format&fit=crop&w=1600&q=82\"]','https://www.youtube.com/watch?v=YHBqLih80uw',1,'2026-10-12 00:41:05','2026-10-12 00:41:05',NULL),(14,1,'PHUQUOC-4N3D','Phú Quốc - Nam đảo - Hàm Ninh 4 ngày 3 đêm',4,3,'Kỳ nghỉ kết hợp bãi biển, trải nghiệm Nam đảo và văn hóa bản địa Phú Quốc. Hành trình đưa du khách đến thị trấn Dương Đông, quần đảo An Thới, làng chài Hàm Ninh, vườn tiêu và các cơ sở nghề làm nước mắm truyền thống.',NULL,NULL,NULL,NULL,0.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'https://images.unsplash.com/photo-1526139334526-f591a54b477c?auto=format&fit=crop&w=1600&q=82','[\"https://images.unsplash.com/photo-1746292448726-9e75b5f1067d?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1693294603830-f44c9511d643?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1526139334526-f591a54b477c?auto=format&fit=crop&w=1600&q=82\"]','https://www.youtube.com/watch?v=Lgvc0l1UyaU',1,'2026-10-12 00:41:05','2026-10-12 00:44:59',NULL),(15,16,'CANTHO-CAIRANG-2N1D','Cần Thơ - Chợ nổi Cái Răng 2 ngày 1 đêm',2,1,'Hành trình ngắn khám phá nhịp sống bên sông Hậu. Du khách tham quan nhà cổ Bình Thủy, bến Ninh Kiều, dậy sớm đi thuyền trên chợ nổi Cái Răng và trải nghiệm vườn cây, làng nghề địa phương.',NULL,NULL,NULL,NULL,0.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'https://images.unsplash.com/photo-1543411789-1a67a2ac05c6?auto=format&fit=crop&w=1600&q=82','[\"https://images.unsplash.com/photo-1677552926138-f7dbb71b226f?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1580630873708-e0475b1856c4?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1543411789-1a67a2ac05c6?auto=format&fit=crop&w=1600&q=82\"]','https://www.youtube.com/watch?v=04Kf_0kppPM',1,'2026-10-12 00:41:05','2026-10-12 00:41:05',NULL),(16,15,'HANOI-VANHOA-AMTHUC-3N2D','Hà Nội văn hóa và ẩm thực 3 ngày 2 đêm',3,2,'Chương trình khám phá Hà Nội ở nhịp độ vừa phải, kết hợp các địa danh lịch sử, không gian hồ và khu phố cổ. Điểm nhấn là trải nghiệm những món ăn tiêu biểu như phở, bún chả, bánh cuốn và cà phê theo khẩu vị cá nhân.',NULL,NULL,NULL,NULL,0.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'https://images.unsplash.com/photo-1509030450996-dd1a26dda07a?auto=format&fit=crop&w=1600&q=82','[\"https://images.unsplash.com/photo-1726346234848-a6c0e78efd8c?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1509030450996-dd1a26dda07a?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1504457047772-27faf1c00561?auto=format&fit=crop&w=1600&q=82\"]','https://www.youtube.com/watch?v=04Kf_0kppPM',1,'2026-10-12 00:41:05','2026-10-12 00:41:05',NULL),(23,3,'HAGIANG-DONGVAN-4N3D','Hà Giang - Đồng Văn - Mã Pì Lèng 4 ngày 3 đêm',4,3,'Cung đường vòng Hà Giang dành cho du khách yêu cảnh quan núi cao và văn hóa bản địa, đi qua Quản Bạ, Yên Minh, Cao nguyên đá Đồng Văn, cột cờ Lũng Cú và đèo Mã Pì Lèng.',NULL,NULL,NULL,NULL,0.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'https://images.unsplash.com/photo-1657445449727-017d2f9f44e7?auto=format&fit=crop&w=1600&q=82','[\"https://images.unsplash.com/photo-1657445449727-017d2f9f44e7?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1570366583862-f91883984fde?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1694152363060-cc26bde73872?auto=format&fit=crop&w=1600&q=82\"]','https://www.youtube.com/watch?v=YHBqLih80uw',1,'2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(24,4,'CAOBANG-BANGIOC-3N2D','Cao Bằng - Bản Giốc - Pác Bó 3 ngày 2 đêm',3,2,'Hành trình Đông Bắc kết hợp cảnh quan thác Bản Giốc, hang động Ngườm Ngao và không gian lịch sử Pác Bó, phù hợp với nhóm nhỏ yêu thiên nhiên.',NULL,NULL,NULL,NULL,0.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'https://images.unsplash.com/photo-1772165009296-2e86e417d62d?auto=format&fit=crop&w=1600&q=82','[\"https://images.unsplash.com/photo-1772165009296-2e86e417d62d?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1657445449727-017d2f9f44e7?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1531737212413-667205e1cda7?auto=format&fit=crop&w=1600&q=82\"]','https://www.youtube.com/watch?v=YHBqLih80uw',1,'2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(25,3,'MOCCHAU-2N1D','Mộc Châu mùa hoa và đồi chè 2 ngày 1 đêm',2,1,'Chuyến đi cuối tuần đến cao nguyên Mộc Châu, ghé đồi chè, rừng thông và các vườn hoa hoặc vườn quả theo đúng mùa khai thác.',NULL,NULL,NULL,NULL,0.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'https://images.unsplash.com/photo-1744254525928-170fe34f13c8?auto=format&fit=crop&w=1600&q=82','[\"https://images.unsplash.com/photo-1744254525928-170fe34f13c8?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1570366583862-f91883984fde?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1732099083114-4fb49a280eec?auto=format&fit=crop&w=1600&q=82\"]','https://www.youtube.com/watch?v=YHBqLih80uw',1,'2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(26,1,'CATBA-LANHA-3N2D','Cát Bà - vịnh Lan Hạ 3 ngày 2 đêm',3,2,'Hành trình khám phá quần đảo Cát Bà, kết hợp đi tàu trên vịnh Lan Hạ, thăm làng Việt Hải và dành thời gian nghỉ biển.',NULL,NULL,NULL,NULL,0.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'https://images.unsplash.com/photo-1722471467241-5327b98a4976?auto=format&fit=crop&w=1600&q=82','[\"https://images.unsplash.com/photo-1722471467241-5327b98a4976?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1700738258173-e160e1809ac5?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1643029891412-92f9a81a8c16?auto=format&fit=crop&w=1600&q=82\"]','https://www.youtube.com/watch?v=Lgvc0l1UyaU',1,'2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(27,1,'HALONG-2N1D','Du thuyền vịnh Hạ Long 2 ngày 1 đêm',2,1,'Kỳ nghỉ ngắn trên vịnh Hạ Long với lịch trình du thuyền, hang động và hoạt động chèo kayak hoặc thuyền nan theo tuyến được cấp phép.',NULL,NULL,NULL,NULL,0.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'https://images.unsplash.com/photo-1643029891412-92f9a81a8c16?auto=format&fit=crop&w=1600&q=82','[\"https://images.unsplash.com/photo-1643029891412-92f9a81a8c16?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1697850084120-4896a446a04d?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1722471467241-5327b98a4976?auto=format&fit=crop&w=1600&q=82\"]','https://www.youtube.com/watch?v=Lgvc0l1UyaU',1,'2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(28,4,'NINHBINH-2N1D','Ninh Bình - Tràng An - Hang Múa 2 ngày 1 đêm',2,1,'Chuyến đi ngắn khám phá di sản Tràng An, Cố đô Hoa Lư và điểm ngắm toàn cảnh Hang Múa, bố trí thời gian vừa phải để hạn chế di chuyển gấp.',NULL,NULL,NULL,NULL,0.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'https://images.unsplash.com/photo-1531737212413-667205e1cda7?auto=format&fit=crop&w=1600&q=82','[\"https://images.unsplash.com/photo-1531737212413-667205e1cda7?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1504457047772-27faf1c00561?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1657445449727-017d2f9f44e7?auto=format&fit=crop&w=1600&q=82\"]','https://www.youtube.com/watch?v=YHBqLih80uw',1,'2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(29,15,'HANOI-HALONG-3N2D','Hà Nội - Hạ Long 3 ngày 2 đêm',3,2,'Tuyến cơ bản dành cho khách lần đầu đến miền Bắc, kết hợp khu phố cổ Hà Nội và một ngày du thuyền ngắm cảnh vịnh Hạ Long.',NULL,NULL,NULL,NULL,0.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'https://images.unsplash.com/photo-1509030450996-dd1a26dda07a?auto=format&fit=crop&w=1600&q=82','[\"https://images.unsplash.com/photo-1509030450996-dd1a26dda07a?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1643029891412-92f9a81a8c16?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1697850084120-4896a446a04d?auto=format&fit=crop&w=1600&q=82\"]','https://www.youtube.com/watch?v=Lgvc0l1UyaU',1,'2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(30,4,'QUANGBINH-PHONGNHA-3N2D','Quảng Bình - Phong Nha 3 ngày 2 đêm',3,2,'Hành trình khám phá Di sản thiên nhiên thế giới Phong Nha - Kẻ Bàng với tuyến động Phong Nha, động Thiên Đường và thời gian nghỉ tại biển Nhật Lệ.',NULL,NULL,NULL,NULL,0.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'https://images.unsplash.com/photo-1758298134993-743b156eb23a?auto=format&fit=crop&w=1600&q=82','[\"https://images.unsplash.com/photo-1758298134993-743b156eb23a?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1618165220283-e85246c4171c?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?auto=format&fit=crop&w=1600&q=82\"]','https://www.youtube.com/watch?v=YHBqLih80uw',1,'2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(31,2,'HUE-QUANGBINH-4N3D','Huế - Quảng Bình - Phong Nha 4 ngày 3 đêm',4,3,'Tuyến di sản kết nối quần thể di tích Cố đô Huế với Phong Nha - Kẻ Bàng, cân bằng tham quan lịch sử, kiến trúc và thiên nhiên.',NULL,NULL,NULL,NULL,0.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'https://images.unsplash.com/photo-1618165220283-e85246c4171c?auto=format&fit=crop&w=1600&q=82','[\"https://images.unsplash.com/photo-1618165220283-e85246c4171c?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1758298134993-743b156eb23a?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1618165220283-e85246c4171c?auto=format&fit=crop&w=1600&q=82\"]','https://www.youtube.com/watch?v=YHBqLih80uw',1,'2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(32,2,'DANANG-BANA-HOIAN-3N2D','Đà Nẵng - Bà Nà - Hội An 3 ngày 2 đêm',3,2,'Chương trình ba ngày kết hợp cảnh quan thành phố Đà Nẵng, khu du lịch Bà Nà và không gian di sản phố cổ Hội An.',NULL,NULL,NULL,NULL,0.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?auto=format&fit=crop&w=1600&q=82','[\"https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1603852452378-a4e8d84324a2?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1563354860-799d15199ac3?auto=format&fit=crop&w=1600&q=82\"]','https://www.youtube.com/watch?v=twb6d3t1SO0',1,'2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(33,1,'NHATRANG-4N3D','Nha Trang - vịnh đảo 4 ngày 3 đêm',4,3,'Kỳ nghỉ biển Nha Trang kết hợp chuyến tàu tham quan vịnh, tháp Bà Ponagar, Hòn Chồng và khoảng thời gian thư giãn tự do.',NULL,NULL,NULL,NULL,0.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'https://images.unsplash.com/photo-1533002832-1721d16b4bb9?auto=format&fit=crop&w=1600&q=82','[\"https://images.unsplash.com/photo-1533002832-1721d16b4bb9?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1526139334526-f591a54b477c?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1746292448726-9e75b5f1067d?auto=format&fit=crop&w=1600&q=82\"]','https://www.youtube.com/watch?v=Lgvc0l1UyaU',1,'2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(34,4,'DALAT-3N2D','Đà Lạt - rừng thông - thác nước 3 ngày 2 đêm',3,2,'Kỳ nghỉ cao nguyên với hồ Xuân Hương, các công trình kiến trúc trung tâm, rừng thông và một tuyến thác phù hợp điều kiện thời tiết.',NULL,NULL,NULL,NULL,0.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'https://images.unsplash.com/photo-1669413487832-a8733f063ca8?auto=format&fit=crop&w=1600&q=82','[\"https://images.unsplash.com/photo-1669413487832-a8733f063ca8?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1570366583862-f91883984fde?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1531737212413-667205e1cda7?auto=format&fit=crop&w=1600&q=82\"]','https://www.youtube.com/watch?v=YHBqLih80uw',1,'2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(35,1,'QUYNHON-3N2D','Quy Nhơn - Kỳ Co - Eo Gió 3 ngày 2 đêm',3,2,'Tuyến biển Nam Trung Bộ khám phá bán đảo Phương Mai, Kỳ Co, Eo Gió và các di tích Chăm quanh Quy Nhơn.',NULL,NULL,NULL,NULL,0.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'https://images.unsplash.com/photo-1689428962568-fe58ab6cc4cd?auto=format&fit=crop&w=1600&q=82','[\"https://images.unsplash.com/photo-1689428962568-fe58ab6cc4cd?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1563354860-799d15199ac3?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1603852452378-a4e8d84324a2?auto=format&fit=crop&w=1600&q=82\"]','https://www.youtube.com/watch?v=Lgvc0l1UyaU',1,'2026-10-12 01:07:46','2026-10-12 01:09:35',NULL),(36,1,'MUINE-3N2D','Mũi Né - Bàu Trắng - làng chài 3 ngày 2 đêm',3,2,'Chuyến nghỉ biển Phan Thiết - Mũi Né với đồi cát, Bàu Trắng, suối Tiên và nhịp sống tại làng chài ven bờ.',NULL,NULL,NULL,NULL,0.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'https://images.unsplash.com/photo-1714271511582-3483dcf0eb71?auto=format&fit=crop&w=1600&q=82','[\"https://images.unsplash.com/photo-1714271511582-3483dcf0eb71?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1526139334526-f591a54b477c?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1693294603830-f44c9511d643?auto=format&fit=crop&w=1600&q=82\"]','https://www.youtube.com/watch?v=Lgvc0l1UyaU',1,'2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(37,4,'NHATRANG-DALAT-5N4D','Nha Trang - Đà Lạt 5 ngày 4 đêm',5,4,'Hành trình kết hợp khí hậu biển Nha Trang và cao nguyên Đà Lạt, dành hai ngày cho mỗi điểm để tránh lịch trình di chuyển quá dày.',NULL,NULL,NULL,NULL,0.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'https://images.unsplash.com/photo-1533002832-1721d16b4bb9?auto=format&fit=crop&w=1600&q=82','[\"https://images.unsplash.com/photo-1533002832-1721d16b4bb9?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1669413487832-a8733f063ca8?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1526139334526-f591a54b477c?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1570366583862-f91883984fde?auto=format&fit=crop&w=1600&q=82\"]','https://www.youtube.com/watch?v=Lgvc0l1UyaU',1,'2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(38,15,'HCMC-CITY-3N2D','TP. Hồ Chí Minh văn hóa và ẩm thực 3 ngày 2 đêm',3,2,'Chương trình tìm hiểu lịch sử đô thị Sài Gòn - TP. Hồ Chí Minh qua bảo tàng, công trình kiến trúc trung tâm, Chợ Lớn và ẩm thực địa phương.',NULL,NULL,NULL,NULL,0.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'https://images.unsplash.com/photo-1583417319070-4a69db38a482?auto=format&fit=crop&w=1600&q=82','[\"https://images.unsplash.com/photo-1583417319070-4a69db38a482?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1509030450996-dd1a26dda07a?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1726346234848-a6c0e78efd8c?auto=format&fit=crop&w=1600&q=82\"]','https://www.youtube.com/watch?v=04Kf_0kppPM',1,'2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(39,15,'HCMC-CUCHI-BENTRE-4N3D','TP. Hồ Chí Minh - Củ Chi - Bến Tre 4 ngày 3 đêm',4,3,'Tuyến miền Nam dành cho khách lần đầu, kết hợp trung tâm TP. Hồ Chí Minh, địa đạo Củ Chi và trải nghiệm sông nước - làng nghề tại Bến Tre.',NULL,NULL,NULL,NULL,0.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'https://images.unsplash.com/photo-1583417319070-4a69db38a482?auto=format&fit=crop&w=1600&q=82','[\"https://images.unsplash.com/photo-1583417319070-4a69db38a482?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1695927521805-d60927fc790d?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1677552926138-f7dbb71b226f?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1580630873708-e0475b1856c4?auto=format&fit=crop&w=1600&q=82\"]','https://www.youtube.com/watch?v=04Kf_0kppPM',1,'2026-10-12 01:07:46','2026-10-12 01:09:35',NULL),(40,1,'VUNGTAU-2N1D','Vũng Tàu nghỉ biển 2 ngày 1 đêm',2,1,'Kỳ nghỉ cuối tuần tại thành phố biển Vũng Tàu, kết hợp thời gian tắm biển, tham quan Bạch Dinh và các điểm nhìn trên Núi Nhỏ.',NULL,NULL,NULL,NULL,0.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'https://images.unsplash.com/photo-1623596711744-c10ed15581d9?auto=format&fit=crop&w=1600&q=82','[\"https://images.unsplash.com/photo-1623596711744-c10ed15581d9?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1693294603830-f44c9511d643?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1746292448726-9e75b5f1067d?auto=format&fit=crop&w=1600&q=82\"]','https://www.youtube.com/watch?v=Lgvc0l1UyaU',1,'2026-10-12 01:07:46','2026-10-12 01:09:35',NULL),(41,15,'TAYNINH-BADEN-2N1D','Tây Ninh - núi Bà Đen 2 ngày 1 đêm',2,1,'Hành trình ngắn tìm hiểu văn hóa Tây Ninh, tham quan Tòa Thánh Cao Đài và chinh phục khu vực núi Bà Đen bằng cáp treo.',NULL,NULL,NULL,NULL,0.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'https://images.unsplash.com/photo-1767757430757-acd414f24fa4?auto=format&fit=crop&w=1600&q=82','[\"https://images.unsplash.com/photo-1767757430757-acd414f24fa4?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1531737212413-667205e1cda7?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1580630873708-e0475b1856c4?auto=format&fit=crop&w=1600&q=82\"]','https://www.youtube.com/watch?v=04Kf_0kppPM',1,'2026-10-12 01:07:46','2026-10-12 01:09:35',NULL),(42,16,'BENTRE-2N1D','Bến Tre xứ dừa 2 ngày 1 đêm',2,1,'Chương trình cộng đồng tại Bến Tre với thuyền trên sông, làng nghề dừa, vườn cây và lưu trú quy mô nhỏ.',NULL,NULL,NULL,NULL,0.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'https://images.unsplash.com/photo-1695927521805-d60927fc790d?auto=format&fit=crop&w=1600&q=82','[\"https://images.unsplash.com/photo-1695927521805-d60927fc790d?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1677552926138-f7dbb71b226f?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1580630873708-e0475b1856c4?auto=format&fit=crop&w=1600&q=82\"]','https://www.youtube.com/watch?v=04Kf_0kppPM',1,'2026-10-12 01:07:46','2026-10-12 01:09:35',NULL),(43,16,'CHAUDOC-3N2D','Châu Đốc - Trà Sư - núi Sam 3 ngày 2 đêm',3,2,'Khám phá vùng biên An Giang qua rừng tràm Trà Sư, núi Sam, các công trình tín ngưỡng và cộng đồng cư dân ven sông.',NULL,NULL,NULL,NULL,0.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'https://images.unsplash.com/photo-1677552926138-f7dbb71b226f?auto=format&fit=crop&w=1600&q=82','[\"https://images.unsplash.com/photo-1677552926138-f7dbb71b226f?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1543411789-1a67a2ac05c6?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1580630873708-e0475b1856c4?auto=format&fit=crop&w=1600&q=82\"]','https://www.youtube.com/watch?v=04Kf_0kppPM',1,'2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(44,1,'CONDAO-3N2D','Côn Đảo lịch sử và sinh thái 3 ngày 2 đêm',3,2,'Hành trình tìm hiểu lịch sử Côn Đảo kết hợp bãi biển, cảnh quan vườn quốc gia và trải nghiệm sinh thái có kiểm soát.',NULL,NULL,NULL,NULL,0.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'https://images.unsplash.com/photo-1686073769358-57d716df2f3b?auto=format&fit=crop&w=1600&q=82','[\"https://images.unsplash.com/photo-1686073769358-57d716df2f3b?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1526139334526-f591a54b477c?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1746292448726-9e75b5f1067d?auto=format&fit=crop&w=1600&q=82\"]','https://www.youtube.com/watch?v=Lgvc0l1UyaU',1,'2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(45,1,'PHUQUOC-5N4D','Phú Quốc nghỉ dưỡng trọn vẹn 5 ngày 4 đêm',5,4,'Phiên bản nghỉ dưỡng dài ngày tại Phú Quốc, kết hợp Dương Đông, Nam đảo, làng chài, nghề truyền thống và một ngày hoàn toàn tự do.',NULL,NULL,NULL,NULL,0.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'https://images.unsplash.com/photo-1526139334526-f591a54b477c?auto=format&fit=crop&w=1600&q=82','[\"https://images.unsplash.com/photo-1526139334526-f591a54b477c?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1746292448726-9e75b5f1067d?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1693294603830-f44c9511d643?auto=format&fit=crop&w=1600&q=82\"]','https://www.youtube.com/watch?v=Lgvc0l1UyaU',1,'2026-10-12 01:07:46','2026-10-12 01:07:46',NULL),(46,16,'CANTHO-CHAUDOC-3N2D','Cần Thơ - Châu Đốc 3 ngày 2 đêm',3,2,'Tuyến đồng bằng sông Cửu Long kết nối chợ nổi Cái Răng, nhà cổ Bình Thủy với rừng tràm Trà Sư và không gian văn hóa Châu Đốc.',NULL,NULL,NULL,NULL,0.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'https://images.unsplash.com/photo-1543411789-1a67a2ac05c6?auto=format&fit=crop&w=1600&q=82','[\"https://images.unsplash.com/photo-1543411789-1a67a2ac05c6?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1677552926138-f7dbb71b226f?auto=format&fit=crop&w=1600&q=82\", \"https://images.unsplash.com/photo-1580630873708-e0475b1856c4?auto=format&fit=crop&w=1600&q=82\"]','https://www.youtube.com/watch?v=04Kf_0kppPM',1,'2026-10-12 01:07:46','2026-10-12 01:07:46',NULL);
/*!40000 ALTER TABLE `tours` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user_verification_tokens`
--

DROP TABLE IF EXISTS `user_verification_tokens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_verification_tokens` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int DEFAULT NULL,
  `target` varchar(190) COLLATE utf8mb4_unicode_ci NOT NULL,
  `token_type` enum('PASSWORD_RESET','EMAIL_VERIFICATION','PHONE_VERIFICATION') COLLATE utf8mb4_unicode_ci NOT NULL,
  `token_hash` char(64) COLLATE utf8mb4_unicode_ci NOT NULL,
  `expires_at` datetime NOT NULL,
  `consumed_at` datetime DEFAULT NULL,
  `attempts` int NOT NULL DEFAULT '0',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_verification_token_hash` (`token_hash`),
  KEY `fk_verification_tokens_user` (`user_id`),
  KEY `idx_verification_target_type` (`target`,`token_type`,`expires_at`),
  CONSTRAINT `fk_verification_tokens_user` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_verification_tokens`
--

LOCK TABLES `user_verification_tokens` WRITE;
/*!40000 ALTER TABLE `user_verification_tokens` DISABLE KEYS */;
INSERT INTO `user_verification_tokens` VALUES (1,2,'khachhang@gmail.com','PASSWORD_RESET','af9c332792b83076321973334609a149c190295b97659e1e3b73a91d7101c827','2026-10-14 01:57:11','2026-10-14 01:27:12',1,'2026-10-14 01:27:11','2026-10-14 01:27:12'),(2,2,'khachhang@gmail.com','EMAIL_VERIFICATION','62f4668e1290071ded9b819deed763c6c0e2860fc5b028a8c460cbb5e193cf23','2026-10-14 02:27:12','2026-10-14 01:27:12',1,'2026-10-14 01:27:12','2026-10-14 01:27:12'),(3,2,'0901234567','PHONE_VERIFICATION','02d8b7f90f464cb632d00d33fb52106fde8587e10ef719e3cf15e4ff9dec5d1e','2026-10-14 01:37:12','2026-10-14 01:27:12',1,'2026-10-14 01:27:12','2026-10-14 01:27:12');
/*!40000 ALTER TABLE `user_verification_tokens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `id` int NOT NULL AUTO_INCREMENT,
  `full_name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `email` varchar(150) COLLATE utf8mb4_unicode_ci NOT NULL,
  `email_verified_at` datetime DEFAULT NULL,
  `password` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `phone_number` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `phone_verified_at` datetime DEFAULT NULL,
  `role` enum('ADMIN','CUSTOMER') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'CUSTOMER',
  `avatar` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT '1',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `email` (`email`),
  KEY `idx_users_email` (`email`),
  KEY `idx_users_role` (`role`),
  KEY `idx_users_role_active` (`role`,`is_active`)
) ENGINE=InnoDB AUTO_INCREMENT=28 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'Quản Trị Viên Hệ Thống','admin@leviet.com',NULL,'$2a$10$QeQLtY8VMXnC8/TtS98aOOBoGkS56RpvQO4e2yJI3aX2j7YLqtDlW','0905123456',NULL,'ADMIN',NULL,1,'2026-09-28 18:32:23','2026-09-28 18:32:23'),(2,'Nguyễn Văn Du Khách','khachhang@gmail.com','2026-10-14 01:27:12','$2a$10$f1ydCXWwEgPuWeVGWJ0uFuY8IdEIKUcZncMU1WT7hBn6KlCmfkBp.','0901234567','2026-10-14 01:27:12','CUSTOMER',NULL,1,'2026-09-28 18:32:23','2026-10-14 01:27:12'),(3,'Tran Van Test','testuser@gmail.com',NULL,'$2a$10$z6sW.hwLwd0xpAxkeYUHLO6ZrwNF8cQ.1lykkgZQfy6IKvGqMxu0W','0912999888',NULL,'CUSTOMER',NULL,1,'2026-09-28 18:37:12','2026-09-28 18:37:12'),(7,'Nguyễn Minh Anh','minhanh.demo@leviet.com',NULL,'$2a$10$ao2A6XfYTPPezGKeieUw9.jVjBpiROC30JqM.G.8S14FEZUirIRzS','0905111222',NULL,'CUSTOMER',NULL,1,'2026-10-13 07:30:39','2026-10-13 07:30:39'),(8,'Trần Hoàng Nam','hoangnam.demo@leviet.com',NULL,'$2a$10$4NQSAjfqAQee9A50w1dGAe3sCOv.OXpfB7QTwLoaPQIXp6pfGuHty','0905333444',NULL,'CUSTOMER',NULL,1,'2026-10-13 07:30:39','2026-10-13 07:30:39'),(9,'Lê Thu Trang','thutrang.demo@leviet.com',NULL,'$2a$10$2qqHvUUC9qRgxnzR6pw3sONfQ2KVBr7BDxaR9bM5Y33WAFxCDPizy','0905555666',NULL,'CUSTOMER',NULL,1,'2026-10-13 07:30:39','2026-10-13 07:30:39');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `vehicles`
--

DROP TABLE IF EXISTS `vehicles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `vehicles` (
  `id` int NOT NULL AUTO_INCREMENT,
  `vehicle_type` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `license_plate` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `seat_capacity` int NOT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `license_plate` (`license_plate`)
) ENGINE=InnoDB AUTO_INCREMENT=33 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `vehicles`
--

LOCK TABLES `vehicles` WRITE;
/*!40000 ALTER TABLE `vehicles` DISABLE KEYS */;
INSERT INTO `vehicles` VALUES (1,'Hyundai Universe 45 chỗ','43B-098.76',45,'2026-09-28 18:32:23','2026-09-28 18:32:23'),(2,'Ford Transit 16 chỗ','43B-012.34',16,'2026-09-28 18:32:23','2026-09-28 18:32:23');
/*!40000 ALTER TABLE `vehicles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `vouchers`
--

DROP TABLE IF EXISTS `vouchers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `vouchers` (
  `id` int NOT NULL AUTO_INCREMENT,
  `code` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `discount_type` enum('PERCENT','FIXED') COLLATE utf8mb4_unicode_ci NOT NULL,
  `discount_value` decimal(12,2) NOT NULL,
  `min_booking_amount` decimal(12,2) NOT NULL DEFAULT '0.00',
  `max_usage` int NOT NULL DEFAULT '100',
  `used_count` int NOT NULL DEFAULT '0',
  `start_date` datetime NOT NULL,
  `end_date` datetime NOT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT '1',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `code` (`code`),
  KEY `idx_vouchers_code` (`code`),
  KEY `idx_vouchers_active_dates` (`is_active`,`start_date`,`end_date`),
  CONSTRAINT `chk_voucher_dates` CHECK ((`end_date` > `start_date`)),
  CONSTRAINT `chk_voucher_usage` CHECK (((`max_usage` > 0) and (`used_count` >= 0) and (`used_count` <= `max_usage`))),
  CONSTRAINT `chk_voucher_value` CHECK (((`discount_value` > 0) and ((`discount_type` <> _utf8mb4'PERCENT') or (`discount_value` <= 100))))
) ENGINE=InnoDB AUTO_INCREMENT=41 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `vouchers`
--

LOCK TABLES `vouchers` WRITE;
/*!40000 ALTER TABLE `vouchers` DISABLE KEYS */;
INSERT INTO `vouchers` VALUES (1,'HELLOSUMMER','PERCENT',10.00,1000000.00,100,0,'2026-09-28 18:32:23','2026-12-27 18:32:23',1,'2026-09-28 18:32:23','2026-09-28 18:32:23'),(7,'LEVITWELCOME','PERCENT',8.00,3000000.00,500,0,'2026-10-13 07:56:11','2027-04-11 07:56:11',1,'2026-10-12 00:41:05','2026-10-13 07:56:11');
/*!40000 ALTER TABLE `vouchers` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-11-05 20:59:32
