-- MySQL dump 10.13  Distrib 8.0.27, for Win64 (x86_64)
--
-- Host: localhost    Database: luyenthi
-- ------------------------------------------------------
-- Server version	8.0.27

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
-- Dumping data for table `aspnetusers`
--

LOCK TABLES `aspnetusers` WRITE;
/*!40000 ALTER TABLE `aspnetusers` DISABLE KEYS */;
INSERT INTO `aspnetusers` (`Id`, `Provider`, `CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy`, `FirstName`, `LastName`, `BirthDay`, `Gender`, `AvatarUrl`, `ActiveCode`, `UserName`, `NormalizedUserName`, `Email`, `NormalizedEmail`, `EmailConfirmed`, `PasswordHash`, `SecurityStamp`, `ConcurrencyStamp`, `PhoneNumber`, `PhoneNumberConfirmed`, `TwoFactorEnabled`, `LockoutEnd`, `LockoutEnabled`, `AccessFailedCount`) VALUES ('08d99920-adbb-446e-8bf0-80798b4bf519','google','2021-10-27 15:06:28.204927','0001-01-01 00:00:00.000000',NULL,NULL,'Tiệp','Nguyễn Thái','2000-06-20 00:00:00.000000',1,'https://res.cloudinary.com/nguyentiep/image/upload/v1635497500/Luyenthi/User/08d99920-adbb-446e-8bf0-80798b4bf519.png',NULL,'nguyenthaitiep206@gmail.com','NGUYENTHAITIEP206@GMAIL.COM','nguyenthaitiep206@gmail.com','NGUYENTHAITIEP206@GMAIL.COM',1,NULL,'F7EZBNLI34O7GUFTBKVTNWEWNXJQMJNH','53edb715-2cb2-4855-8edd-49434d5f87a2','0819200620',0,0,NULL,1,0),('cb3850a2-0a32-4cee-a175-08df5ec6169b','luyenthi','0001-01-01 00:00:00.000000','0001-01-01 00:00:00.000000',NULL,NULL,'Tiệp','Nguyễn','2000-06-20 00:00:00.000000',0,NULL,NULL,'admin',NULL,'nguyenthaitiep206@gmail.com',NULL,1,'AQAAAAEAACcQAAAAECyHQI0Xi9Rtj/51X4Jy5hspfdk39qsMgX+jJBEA+0dN49rOAUe3AaJOvX94DONmIA==',NULL,'ef9d7ef3-dbe9-41a9-bc43-eb1bf54bcbe0','0819200620',1,0,NULL,0,0);
/*!40000 ALTER TABLE `aspnetusers` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2021-10-30  3:00:02
