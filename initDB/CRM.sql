-- MySQL dump 10.13  Distrib 5.7.15, for Linux (x86_64)
--
-- Host: localhost    Database: CRM
-- ------------------------------------------------------
-- Server version	5.7.15-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `Commodity`
--

DROP TABLE IF EXISTS `Commodity`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `Commodity` (
  `Id` char(36) NOT NULL,
  `Des` varchar(150) DEFAULT NULL,
  `MaxGouNum` int(11) NOT NULL,
  `MaxNum` int(11) NOT NULL,
  `Name` varchar(150) DEFAULT NULL,
  `Price` decimal(65,30) NOT NULL,
  `Url` varchar(150) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Commodity`
--

LOCK TABLES `Commodity` WRITE;
/*!40000 ALTER TABLE `Commodity` DISABLE KEYS */;
INSERT INTO `Commodity` VALUES ('309352c0-4e2b-485e-a03e-6bcb66e91cbb','飘花筋脉石精品',10,1000,'花脉精品',1000.000000000000000000000000000000,NULL),('e2b8dbf7-ef4b-45c0-ad1b-2b39baa8c666','内蒙本地筋脉石精品',10,1000,'本地精品',1000.000000000000000000000000000000,NULL);
/*!40000 ALTER TABLE `Commodity` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Payment`
--

DROP TABLE IF EXISTS `Payment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `Payment` (
  `Id` char(36) NOT NULL,
  `ChannelCode` varchar(36) DEFAULT NULL,
  `PaymentAccount` varchar(72) DEFAULT NULL,
  `PaymentAmount` decimal(65,30) NOT NULL,
  `PaymentMode` varchar(36) DEFAULT NULL,
  `SecKillOrderId` char(36) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Payment_SecKillOrderId` (`SecKillOrderId`),
  CONSTRAINT `FK_Payment_SecKillOrder_SecKillOrderId` FOREIGN KEY (`SecKillOrderId`) REFERENCES `SecKillOrder` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Payment`
--

LOCK TABLES `Payment` WRITE;
/*!40000 ALTER TABLE `Payment` DISABLE KEYS */;
/*!40000 ALTER TABLE `Payment` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `SecKillOrder`
--

DROP TABLE IF EXISTS `SecKillOrder`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `SecKillOrder` (
  `Id` char(36) NOT NULL,
  `CommodityId` char(36) NOT NULL,
  `CreatTime` datetime(6) NOT NULL,
  `Num` int(11) NOT NULL,
  `OrderStatus` int(11) NOT NULL,
  `PayOutTime` datetime(6) NOT NULL,
  `PayTime` datetime(6) NOT NULL,
  `UserId` char(36) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_SecKillOrder_CommodityId` (`CommodityId`),
  KEY `IX_SecKillOrder_UserId` (`UserId`),
  CONSTRAINT `FK_SecKillOrder_Commodity_CommodityId` FOREIGN KEY (`CommodityId`) REFERENCES `Commodity` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_SecKillOrder_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `User` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `SecKillOrder`
--

LOCK TABLES `SecKillOrder` WRITE;
/*!40000 ALTER TABLE `SecKillOrder` DISABLE KEYS */;
/*!40000 ALTER TABLE `SecKillOrder` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `User`
--

DROP TABLE IF EXISTS `User`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `User` (
  `Id` char(36) NOT NULL,
  `Address` varchar(200) DEFAULT NULL,
  `EMail` varchar(100) DEFAULT NULL,
  `MobileNumber` varchar(36) DEFAULT NULL,
  `NickName` varchar(72) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `User`
--

LOCK TABLES `User` WRITE;
/*!40000 ALTER TABLE `User` DISABLE KEYS */;
INSERT INTO `User` VALUES ('17fb84bb-ac70-4e61-878a-ef1ae6e7accd','常营北小街2号院5号楼1407','jiaoyue2002@163.com','13811177528','jiaoyue'),('e7366551-3c17-4a99-bd62-9edbe0a037e0','常营北小街2号院5号楼1407','tugongshenle@163.com','13811177528','jiaoyuxin');
/*!40000 ALTER TABLE `User` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `__EFMigrationsHistory`
--

DROP TABLE IF EXISTS `__EFMigrationsHistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `__EFMigrationsHistory` (
  `MigrationId` varchar(100) NOT NULL,
  `ProductVersion` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__EFMigrationsHistory`
--

LOCK TABLES `__EFMigrationsHistory` WRITE;
/*!40000 ALTER TABLE `__EFMigrationsHistory` DISABLE KEYS */;
INSERT INTO `__EFMigrationsHistory` VALUES ('20170706060652_AddCRM','1.1.2');
/*!40000 ALTER TABLE `__EFMigrationsHistory` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2017-07-06 17:06:45
