-- MySQL dump 10.13  Distrib 5.7.15, for Linux (x86_64)
--
-- Host: localhost    Database: Auth
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
-- Table structure for table `Departments`
--

DROP TABLE IF EXISTS `Departments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `Departments` (
  `Id` varchar(36) NOT NULL,
  `Code` varchar(36) DEFAULT NULL,
  `ContactNumber` varchar(36) DEFAULT NULL,
  `CreateTime` datetime(6) DEFAULT NULL,
  `CreateUserId` varchar(36) NOT NULL,
  `IsDeleted` int(11) NOT NULL,
  `Manager` varchar(36) DEFAULT NULL,
  `Name` varchar(36) DEFAULT NULL,
  `ParentId` varchar(36) NOT NULL,
  `Remarks` longtext,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Departments`
--

LOCK TABLES `Departments` WRITE;
/*!40000 ALTER TABLE `Departments` DISABLE KEYS */;
INSERT INTO `Departments` VALUES ('5ba753e9-a7f3-46ad-efa7-08d411e2b01c','bjwlpw','123',NULL,'00000000-0000-0000-0000-000000000000',0,'admin','票务部','c1e3d054-9479-4216-efa3-08d411e2b01c','jjj1'),('7d1e4c3e-6bd7-45f1-efa2-08d411e2b01c','bjfhh',NULL,NULL,'00000000-0000-0000-0000-000000000000',0,'admin','北京凤凰汇','e20af586-bca7-42bd-efa1-08d411e2b01c',NULL),('c1e3d054-9479-4216-efa3-08d411e2b01c','bjwl',NULL,NULL,'00000000-0000-0000-0000-000000000000',0,'admin','北京万柳','e20af586-bca7-42bd-efa1-08d411e2b01c',NULL),('c8e3b2a9-65d1-4813-efa5-08d411e2b01c','bjfhhpw',NULL,NULL,'00000000-0000-0000-0000-000000000000',0,'admin','票务部','7d1e4c3e-6bd7-45f1-efa2-08d411e2b01c',NULL),('cf6c5650-2b2f-4941-a1f4-ee0b83a20d16',NULL,NULL,NULL,'00000000-0000-0000-0000-000000000000',0,NULL,'集团总部','00000000-0000-0000-0000-000000000000',NULL),('d152cf9f-a903-402f-efa6-08d411e2b01c','bjwlmp',NULL,NULL,'00000000-0000-0000-0000-000000000000',0,'admin','卖品部','c1e3d054-9479-4216-efa3-08d411e2b01c',NULL),('e20af586-bca7-42bd-efa1-08d411e2b01c',NULL,'88888888',NULL,'00000000-0000-0000-0000-000000000000',0,NULL,'嘉禾总部','00000000-0000-0000-0000-000000000000',NULL),('f6878182-dbd5-4291-efa4-08d411e2b01c','bjfhhmp',NULL,NULL,'00000000-0000-0000-0000-000000000000',0,'admin','卖品部','7d1e4c3e-6bd7-45f1-efa2-08d411e2b01c',NULL);
/*!40000 ALTER TABLE `Departments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Menus`
--

DROP TABLE IF EXISTS `Menus`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `Menus` (
  `Id` varchar(36) NOT NULL,
  `Code` varchar(36) DEFAULT NULL,
  `Icon` varchar(36) DEFAULT NULL,
  `Name` varchar(36) DEFAULT NULL,
  `ParentId` varchar(36) NOT NULL,
  `Remarks` varchar(72) DEFAULT NULL,
  `SerialNumber` int(11) NOT NULL,
  `Type` int(11) NOT NULL,
  `Url` varchar(72) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Menus`
--

LOCK TABLES `Menus` WRITE;
/*!40000 ALTER TABLE `Menus` DISABLE KEYS */;
INSERT INTO `Menus` VALUES ('5d19d612-5be6-4641-40ce-08d411e1c635','User','fa fa-link','用户管理','00000000-0000-0000-0000-000000000000',NULL,2,0,'/User/Index'),('a1eed053-d90e-4a61-c75d-08d4a7dda632','Authority','fa fa-link','权限管理','00000000-0000-0000-0000-000000000000',NULL,0,0,'/Authority/Index'),('a5700578-69f4-4f34-40cd-08d411e1c635','Role','fa fa-link','角色管理','00000000-0000-0000-0000-000000000000',NULL,1,0,'/Role/Index'),('c3407fe8-7c6c-43e2-40cf-08d411e1c635','Menu','fa fa-link','功能管理','00000000-0000-0000-0000-000000000000',NULL,3,0,'/Menu/Index'),('e507ea16-fbb9-499e-40cc-08d411e1c635','Department','fa fa-link','组织机构管理','00000000-0000-0000-0000-000000000000',NULL,0,0,'/Department/Index');
/*!40000 ALTER TABLE `Menus` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `RoleMenus`
--

DROP TABLE IF EXISTS `RoleMenus`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `RoleMenus` (
  `RoleId` varchar(36) NOT NULL,
  `MenuId` varchar(36) NOT NULL,
  `MenuId1` varchar(36) DEFAULT NULL,
  PRIMARY KEY (`RoleId`,`MenuId`),
  KEY `IX_RoleMenus_MenuId` (`MenuId`),
  CONSTRAINT `FK_RoleMenus_Menus_MenuId` FOREIGN KEY (`MenuId`) REFERENCES `Menus` (`Id`) ON DELETE NO ACTION,
  CONSTRAINT `FK_RoleMenus_Roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `Roles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `RoleMenus`
--

LOCK TABLES `RoleMenus` WRITE;
/*!40000 ALTER TABLE `RoleMenus` DISABLE KEYS */;
INSERT INTO `RoleMenus` VALUES ('18d7d743-f941-4fb9-f8d9-08d44034cc7d','a1eed053-d90e-4a61-c75d-08d4a7dda632',NULL),('5b8f3a30-3757-4eee-f8d7-08d44034cc7d','c3407fe8-7c6c-43e2-40cf-08d411e1c635',NULL),('5f1d15ee-3f66-4999-7b08-08d42afe3399','5d19d612-5be6-4641-40ce-08d411e1c635',NULL),('5f1d15ee-3f66-4999-7b08-08d42afe3399','a1eed053-d90e-4a61-c75d-08d4a7dda632',NULL),('5f1d15ee-3f66-4999-7b08-08d42afe3399','a5700578-69f4-4f34-40cd-08d411e1c635',NULL),('5f1d15ee-3f66-4999-7b08-08d42afe3399','c3407fe8-7c6c-43e2-40cf-08d411e1c635',NULL),('5f1d15ee-3f66-4999-7b08-08d42afe3399','e507ea16-fbb9-499e-40cc-08d411e1c635',NULL),('918a9190-72c8-44f5-4397-08d46761e009','5d19d612-5be6-4641-40ce-08d411e1c635',NULL),('c5b3ecb6-42f4-444f-f8d8-08d44034cc7d','a5700578-69f4-4f34-40cd-08d411e1c635',NULL),('cbeff800-97d9-4236-f8d6-08d44034cc7d','e507ea16-fbb9-499e-40cc-08d411e1c635',NULL);
/*!40000 ALTER TABLE `RoleMenus` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Roles`
--

DROP TABLE IF EXISTS `Roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `Roles` (
  `Id` varchar(36) NOT NULL,
  `Code` varchar(36) DEFAULT NULL,
  `CreateTime` datetime(6) DEFAULT NULL,
  `CreateUserId` varchar(36) NOT NULL,
  `Name` varchar(36) DEFAULT NULL,
  `Remarks` varchar(72) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Roles`
--

LOCK TABLES `Roles` WRITE;
/*!40000 ALTER TABLE `Roles` DISABLE KEYS */;
INSERT INTO `Roles` VALUES ('18d7d743-f941-4fb9-f8d9-08d44034cc7d','005','2017-01-19 14:45:33.673072','72e5b5f5-24f1-46e1-8309-08d411e1c631','权限管理','权限管理'),('5b8f3a30-3757-4eee-f8d7-08d44034cc7d','003','2017-01-19 14:38:07.107779','72e5b5f5-24f1-46e1-8309-08d411e1c631','功能管理','功能管理'),('5f1d15ee-3f66-4999-7b08-08d42afe3399','001','2016-12-23 14:37:42.224647','72e5b5f5-24f1-46e1-8309-08d411e1c631','admin','超级管理'),('918a9190-72c8-44f5-4397-08d46761e009','006','2017-03-10 11:02:21.708947','72e5b5f5-24f1-46e1-8309-08d411e1c631','用户管理','用户管理1'),('c5b3ecb6-42f4-444f-f8d8-08d44034cc7d','004','2017-01-19 14:45:11.320270','72e5b5f5-24f1-46e1-8309-08d411e1c631','角色管理','角色管理'),('cbeff800-97d9-4236-f8d6-08d44034cc7d','002','2017-01-19 14:31:25.171547','72e5b5f5-24f1-46e1-8309-08d411e1c631','组织管理','组织管理');
/*!40000 ALTER TABLE `Roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `UserRoles`
--

DROP TABLE IF EXISTS `UserRoles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `UserRoles` (
  `UserId` varchar(36) NOT NULL,
  `RoleId` varchar(36) NOT NULL,
  PRIMARY KEY (`UserId`,`RoleId`),
  KEY `IX_UserRoles_RoleId` (`RoleId`),
  KEY `IX_UserRoles_UserId` (`UserId`),
  CONSTRAINT `FK_UserRoles_Roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `Roles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_UserRoles_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `UserRoles`
--

LOCK TABLES `UserRoles` WRITE;
/*!40000 ALTER TABLE `UserRoles` DISABLE KEYS */;
INSERT INTO `UserRoles` VALUES ('13ae6bff-09ae-4d3e-88c3-08d4429da613','18d7d743-f941-4fb9-f8d9-08d44034cc7d'),('a6e2a79a-45cd-4aff-a342-08d4183bcef0','18d7d743-f941-4fb9-f8d9-08d44034cc7d'),('05bbca8a-8826-4f9e-776e-08d454a5c5d6','5b8f3a30-3757-4eee-f8d7-08d44034cc7d'),('a6e2a79a-45cd-4aff-a342-08d4183bcef0','5b8f3a30-3757-4eee-f8d7-08d44034cc7d'),('72e5b5f5-24f1-46e1-8309-08d411e1c631','5f1d15ee-3f66-4999-7b08-08d42afe3399'),('05bbca8a-8826-4f9e-776e-08d454a5c5d6','918a9190-72c8-44f5-4397-08d46761e009'),('05bbca8a-8826-4f9e-776e-08d454a5c5d6','c5b3ecb6-42f4-444f-f8d8-08d44034cc7d'),('a6e2a79a-45cd-4aff-a342-08d4183bcef0','c5b3ecb6-42f4-444f-f8d8-08d44034cc7d'),('05bbca8a-8826-4f9e-776e-08d454a5c5d6','cbeff800-97d9-4236-f8d6-08d44034cc7d');
/*!40000 ALTER TABLE `UserRoles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Users`
--

DROP TABLE IF EXISTS `Users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `Users` (
  `Id` varchar(36) NOT NULL,
  `CreateTime` datetime(6) DEFAULT NULL,
  `CreateUserId` varchar(36) NOT NULL,
  `DepartmentId` varchar(36) NOT NULL,
  `EMail` varchar(36) DEFAULT NULL,
  `IsDeleted` int(11) NOT NULL,
  `LastLoginTime` datetime(6) NOT NULL,
  `LoginTimes` int(11) NOT NULL,
  `MobileNumber` varchar(36) DEFAULT NULL,
  `Name` varchar(36) DEFAULT NULL,
  `Password` varchar(72) DEFAULT NULL,
  `Remarks` varchar(72) DEFAULT NULL,
  `UserName` varchar(36) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Users_DepartmentId` (`DepartmentId`),
  CONSTRAINT `FK_Users_Departments_DepartmentId` FOREIGN KEY (`DepartmentId`) REFERENCES `Departments` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Users`
--

LOCK TABLES `Users` WRITE;
/*!40000 ALTER TABLE `Users` DISABLE KEYS */;
INSERT INTO `Users` VALUES ('05bbca8a-8826-4f9e-776e-08d454a5c5d6','2017-02-14 14:50:33.134610','72e5b5f5-24f1-46e1-8309-08d411e1c631','e20af586-bca7-42bd-efa1-08d411e2b01c','alxcia@163.com',0,'0001-01-01 00:00:00.000000',0,'13811177528','alxcia','123456','alxcia','alxcia'),('060efa7e-7381-4d3f-bf65-08d48bb1bcd9','2017-04-25 16:04:45.970143','72e5b5f5-24f1-46e1-8309-08d411e1c631','e20af586-bca7-42bd-efa1-08d411e2b01c','9',0,'0001-01-01 00:00:00.000000',0,'9','9','9','9','9'),('08d4b7be-d4c4-915a-1eb3-6526503bd369','2017-06-29 16:53:49.000000','226b44f4-9afc-4dbd-d2c6-08d40ad7befc','e20af586-bca7-42bd-efa1-08d411e2b01c','12',0,'2017-06-29 16:54:52.562000',0,'12','12','12','12','12'),('08d4bde6-d48c-5036-d723-8a3d8b8d9054','2017-06-28 13:30:47.201944','72e5b5f5-24f1-46e1-8309-08d411e1c631','e20af586-bca7-42bd-efa1-08d411e2b01c','13',0,'0001-01-01 00:00:00.000000',0,'13','13','13','13','13'),('08d4c1ee-edcf-ecdc-6781-9c0a3d210a7f','2017-07-03 16:38:48.771787','72e5b5f5-24f1-46e1-8309-08d411e1c631','e20af586-bca7-42bd-efa1-08d411e2b01c','14',0,'0001-01-01 00:00:00.000000',0,'14','14','14','14','14'),('08d4c1ef-a7d9-62ff-84ce-bba9a3cf17d1','2017-07-03 16:38:48.771787','72e5b5f5-24f1-46e1-8309-08d411e1c631','e20af586-bca7-42bd-efa1-08d411e2b01c','14',0,'0001-01-01 00:00:00.000000',0,'14','14','14','14','14'),('08d4c1f0-5db2-7f2f-8f78-229f381838e4','2017-07-03 16:38:48.771787','72e5b5f5-24f1-46e1-8309-08d411e1c631','e20af586-bca7-42bd-efa1-08d411e2b01c','14',0,'0001-01-01 00:00:00.000000',0,'14','14','14','14','14'),('08d4c1f1-456f-59ed-d91a-4c5df58ea291','2017-07-03 16:38:48.771787','72e5b5f5-24f1-46e1-8309-08d411e1c631','e20af586-bca7-42bd-efa1-08d411e2b01c','14',0,'0001-01-01 00:00:00.000000',0,'14','14','14','14','14'),('08d4c1f1-b189-efbc-dd95-2cc6c7d5194a','2017-07-03 16:58:20.199029','72e5b5f5-24f1-46e1-8309-08d411e1c631','e20af586-bca7-42bd-efa1-08d411e2b01c','15',0,'0001-01-01 00:00:00.000000',0,'15','15','15','15','15'),('08d4c1f2-866a-f890-fa25-6d13f98dadec','2017-07-03 16:58:20.199029','72e5b5f5-24f1-46e1-8309-08d411e1c631','e20af586-bca7-42bd-efa1-08d411e2b01c','15',0,'0001-01-01 00:00:00.000000',0,'15','15','15','15','15'),('08d4c1f3-52ea-7fa0-6148-1580dcdecf80','2017-07-03 16:58:20.199029','72e5b5f5-24f1-46e1-8309-08d411e1c631','e20af586-bca7-42bd-efa1-08d411e2b01c','15',0,'0001-01-01 00:00:00.000000',0,'15','15','15','15','15'),('08d4c1f6-af9f-6eac-324b-8e1385003ce9','2017-07-03 17:34:12.666343','72e5b5f5-24f1-46e1-8309-08d411e1c631','e20af586-bca7-42bd-efa1-08d411e2b01c','16',0,'0001-01-01 00:00:00.000000',0,'16','16','16','16','16'),('0ad96163-e577-42c9-19f4-08d48b9df64b','2017-04-25 13:46:50.421000','72e5b5f5-24f1-46e1-8309-08d411e1c631','e20af586-bca7-42bd-efa1-08d411e2b01c','8',0,'0001-01-01 00:00:00.000000',0,'8','8','8','8','8'),('13ae6bff-09ae-4d3e-88c3-08d4429da613','2017-01-22 16:07:02.275027','72e5b5f5-24f1-46e1-8309-08d411e1c631','e20af586-bca7-42bd-efa1-08d411e2b01c','jianshanhuangquan@sohu.com',0,'2017-07-03 17:59:13.266523',47,'13699278496','clare','123456','clare','clare'),('13cd1022-5f30-4751-a66d-08d4abec131c','2017-06-05 16:22:56.801729','72e5b5f5-24f1-46e1-8309-08d411e1c631','e20af586-bca7-42bd-efa1-08d411e2b01c','10',0,'0001-01-01 00:00:00.000000',0,'10','10','10','10','10'),('195883a9-bb54-4ee1-a66e-08d4abec131c','2017-06-05 16:23:33.998549','72e5b5f5-24f1-46e1-8309-08d411e1c631','e20af586-bca7-42bd-efa1-08d411e2b01c','11',0,'0001-01-01 00:00:00.000000',0,'11','11','11','111','11'),('4ef3bba9-1c2b-4e60-7587-08d47017f4be','2017-03-21 13:08:44.071863','72e5b5f5-24f1-46e1-8309-08d411e1c631','e20af586-bca7-42bd-efa1-08d411e2b01c','4',0,'0001-01-01 00:00:00.000000',0,'4','4','33','4','4'),('69bc44c8-9226-48d3-7669-08d48af64e80','2017-04-24 17:42:58.468665','72e5b5f5-24f1-46e1-8309-08d411e1c631','e20af586-bca7-42bd-efa1-08d411e2b01c','6',0,'2017-05-31 15:29:31.110190',3,'6','6','6','6','6'),('72e5b5f5-24f1-46e1-8309-08d411e1c631',NULL,'00000000-0000-0000-0000-000000000000','cf6c5650-2b2f-4941-a1f4-ee0b83a20d16',NULL,0,'2017-07-04 18:08:25.024533',328,NULL,'超级管理员','123456',NULL,'admin'),('7f81c3c0-4c41-4f10-8e67-6c75501669a0','2017-04-25 13:46:50.421000','72e5b5f5-24f1-46e1-8309-08d411e1c631','e20af586-bca7-42bd-efa1-08d411e2b01c','9',0,'0001-01-01 00:00:00.000000',0,'9','9','9','9','9'),('8bcb2b2f-7846-4adf-93e9-08d470217e3b','2017-03-21 14:14:05.089842','72e5b5f5-24f1-46e1-8309-08d411e1c631','e20af586-bca7-42bd-efa1-08d411e2b01c','5',0,'0001-01-01 00:00:00.000000',0,'5','5','5','5','5'),('a6e2a79a-45cd-4aff-a342-08d4183bcef0','2016-11-29 17:40:48.884606','72e5b5f5-24f1-46e1-8309-08d411e1c631','e20af586-bca7-42bd-efa1-08d411e2b01c','jiaoyue2002@163.com',0,'2017-05-31 15:21:45.011593',1,'13811177528','jiaoyue','123456','jy','adwan'),('aaf98eae-1d52-452f-fe07-08d48b88b268','2017-04-25 11:10:57.274024','72e5b5f5-24f1-46e1-8309-08d411e1c631','e20af586-bca7-42bd-efa1-08d411e2b01c','7',0,'0001-01-01 00:00:00.000000',0,'7','7','7','7','7');
/*!40000 ALTER TABLE `Users` ENABLE KEYS */;
UNLOCK TABLES;


CREATE TABLE `UserIndexs` (
  `UserId` varchar(36) NOT NULL,
  `UserName` varchar(72) DEFAULT NULL,
    `Password` varchar(72) DEFAULT NULL,
  `DepartmentId` varchar(36) NOT NULL,
  PRIMARY KEY (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

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
INSERT INTO `__EFMigrationsHistory` VALUES ('20161106080421_Initial','1.0.1');
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

-- Dump completed on 2017-07-06 16:56:37
