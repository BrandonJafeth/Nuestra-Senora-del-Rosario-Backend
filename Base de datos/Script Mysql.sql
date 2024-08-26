/*
SQLyog Ultimate v11.11 (64 bit)
MySQL - 8.0.34 : Database - nuestra_senora_del_rosario
*********************************************************************
*/

/*!40101 SET NAMES utf8 */;

/*!40101 SET SQL_MODE=''*/;

/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
CREATE DATABASE /*!32312 IF NOT EXISTS*/`nuestra_senora_del_rosario` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;

USE `nuestra_senora_del_rosario`;

/*Table structure for table `aboutussections` */

DROP TABLE IF EXISTS `aboutussections`;

CREATE TABLE `aboutussections` (
  `Id_About_us` int NOT NULL AUTO_INCREMENT,
  `Subtitle_About_Us` varchar(500) COLLATE utf8mb4_unicode_ci NOT NULL,
  `MissionTitle_About_US` varchar(15) COLLATE utf8mb4_unicode_ci NOT NULL,
  `MissionDescription_About_US` varchar(500) COLLATE utf8mb4_unicode_ci NOT NULL,
  `VisionTitle_About_US` varchar(15) COLLATE utf8mb4_unicode_ci NOT NULL,
  `VisionDescription_About_US` varchar(500) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`Id_About_us`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

/*Data for the table `aboutussections` */

LOCK TABLES `aboutussections` WRITE;

insert  into `aboutussections`(`Id_About_us`,`Subtitle_About_Us`,`MissionTitle_About_US`,`MissionDescription_About_US`,`VisionTitle_About_US`,`VisionDescription_About_US`) values (1,'Somos un hogar dedicado a brindar amor y cuidado a nuestros residentes, asegurando que cada día sea una experiencia digna y feliz.','Misión','Ofrecer calidad de vida a los Adultos mayores, a través de un servicio sostenible de hospedaje, alimentación, higiene, vestuario, salud física, mental, espiritual, en un ambiente que les haga sentirse en familia a través del personal que lo desempeñaran con mística y amor evangélico.','Visión','Somos una institución comprometida con los adultos mayores en abandono, riesgo familiar y social, del Cantón de Santa Cruz y sus alrededores.');

UNLOCK TABLES;

/*Table structure for table `herosections` */

DROP TABLE IF EXISTS `herosections`;

CREATE TABLE `herosections` (
  `Id_Hero` int NOT NULL AUTO_INCREMENT,
  `Subtitle_Hero` varchar(200) COLLATE utf8mb4_unicode_ci NOT NULL,
  `HeroImage_Url` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `HeroText_button` varchar(15) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Hero_Title` varchar(150) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`Id_Hero`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

/*Data for the table `herosections` */

LOCK TABLES `herosections` WRITE;

insert  into `herosections`(`Id_Hero`,`Subtitle_Hero`,`HeroImage_Url`,`HeroText_button`,`Hero_Title`) values (2,'Un hogar lleno de amor y cuidado, donde cada día es una oportunidad para sonreír y vivir con dignidad','https://raw.githubusercontent.com/BrandonJafeth/Images-Nuestra-Se-ora-del-Rosario/08909f6de9deae8edd499d1dfa7faf79dbd4635a/Hero-Images/Image-Hero-Sectiom.avif','Sobre nosotros','Hogar de Ancianos Nuestra Señora del Rosario');

UNLOCK TABLES;

/*Table structure for table `navbaritems` */

DROP TABLE IF EXISTS `navbaritems`;

CREATE TABLE `navbaritems` (
  `Id_Nav_It` int NOT NULL AUTO_INCREMENT,
  `Title_Nav` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `UrlNav` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Order_Item_Nav` int NOT NULL,
  `IsActive` tinyint(1) DEFAULT '1',
  `ParentId` int DEFAULT NULL,
  PRIMARY KEY (`Id_Nav_It`),
  KEY `ParentId` (`ParentId`),
  CONSTRAINT `navbaritems_ibfk_1` FOREIGN KEY (`ParentId`) REFERENCES `navbaritems` (`Id_Nav_It`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

/*Data for the table `navbaritems` */

LOCK TABLES `navbaritems` WRITE;

insert  into `navbaritems`(`Id_Nav_It`,`Title_Nav`,`UrlNav`,`Order_Item_Nav`,`IsActive`,`ParentId`) values (1,'Inicio','/inicio',1,1,NULL),(2,'Sobre Nosotros','/sobre-nosotros',2,1,NULL),(3,'Servicios','/servicios',3,1,NULL),(4,'Solicitudes','/solicitudes',4,1,NULL),(5,'Donaciones','/solicitudes/donaciones',5,1,4),(6,'Voluntariado','/solicitudes/voluntariado',6,1,4),(7,'Proceso de ingreso','/solicitudes/proceso-ingreso',7,1,4),(8,'Galería','/galeria',8,1,NULL),(9,'Contactos','/contactos',9,1,NULL);

UNLOCK TABLES;

/*Table structure for table `registrationsections` */

DROP TABLE IF EXISTS `registrationsections`;

CREATE TABLE `registrationsections` (
  `Id_RegistrationSection` int NOT NULL AUTO_INCREMENT,
  `Registration_MoreInfoPrompt` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `RegistrationText_Button` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Registration_SupportMessage` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `RegistrationImage_Url` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`Id_RegistrationSection`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

/*Data for the table `registrationsections` */

LOCK TABLES `registrationsections` WRITE;

insert  into `registrationsections`(`Id_RegistrationSection`,`Registration_MoreInfoPrompt`,`RegistrationText_Button`,`Registration_SupportMessage`,`RegistrationImage_Url`) values (1,'¿Deseas ver más información sobre el proceso de solicitud de ingreso?','Ver más Información','Nuestro equipo está disponible para responder cualquier pregunta y proporcionarte toda la información necesaria para que puedas tomar la mejor decisión para tu ser querido.','https://raw.githubusercontent.com/BrandonJafeth/Images-Nuestra-Se-ora-del-Rosario/08909f6de9deae8edd499d1dfa7faf79dbd4635a/Registration-Section/Image-Registration-Section.avif');

UNLOCK TABLES;

/*Table structure for table `servicesections` */

DROP TABLE IF EXISTS `servicesections`;

CREATE TABLE `servicesections` (
  `Id_ServiceSection` int NOT NULL AUTO_INCREMENT,
  `Title_Card_SV` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Image_Card_SV_Url` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Description_Card_SV` varchar(500) COLLATE utf8mb4_unicode_ci NOT NULL,
  `SVText_button` varchar(15) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`Id_ServiceSection`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

/*Data for the table `servicesections` */

LOCK TABLES `servicesections` WRITE;

insert  into `servicesections`(`Id_ServiceSection`,`Title_Card_SV`,`Image_Card_SV_Url`,`Description_Card_SV`,`SVText_button`) values (1,'Enfermería','https://raw.githubusercontent.com/BrandonJafeth/Images-Nuestra-Se-ora-del-Rosario/08909f6de9deae8edd499d1dfa7faf79dbd4635a/Services-Images/Service-Image%201.avif','Atención médica personalizada las 24 horas.','Ver más'),(2,'Comedor','https://raw.githubusercontent.com/BrandonJafeth/Images-Nuestra-Se-ora-del-Rosario/08909f6de9deae8edd499d1dfa7faf79dbd4635a/Services-Images/Service-Image%202.avif','Comidas saludables y balanceadas.','Ver más'),(3,'Fisioterapia','https://raw.githubusercontent.com/BrandonJafeth/Images-Nuestra-Se-ora-del-Rosario/08909f6de9deae8edd499d1dfa7faf79dbd4635a/Services-Images/Service-Image%203.avif','Sesiones de fisioterapia personalizadas','Ver más'),(4,'Actividades Recreativas','https://raw.githubusercontent.com/BrandonJafeth/Images-Nuestra-Se-ora-del-Rosario/08909f6de9deae8edd499d1dfa7faf79dbd4635a/Services-Images/Service-Image%204.avif','Variedad de actividades recreativas y sociales.','Ver más');

UNLOCK TABLES;

/*Table structure for table `sitesettings` */

DROP TABLE IF EXISTS `sitesettings`;

CREATE TABLE `sitesettings` (
  `Id_Site_Settings` int NOT NULL AUTO_INCREMENT,
  `SiteTitle` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Icon_HGA_Url` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`Id_Site_Settings`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

/*Data for the table `sitesettings` */

LOCK TABLES `sitesettings` WRITE;

insert  into `sitesettings`(`Id_Site_Settings`,`SiteTitle`,`Icon_HGA_Url`) values (1,'Nuestra Señora del Rosario','https://raw.githubusercontent.com/BrandonJafeth/Images-Nuestra-Se-ora-del-Rosario/08909f6de9deae8edd499d1dfa7faf79dbd4635a/Icons-Hogar/Icon-Hogar.avif');

UNLOCK TABLES;

/*Table structure for table `titlesections` */

DROP TABLE IF EXISTS `titlesections`;

CREATE TABLE `titlesections` (
  `Id_TitleSection` int NOT NULL AUTO_INCREMENT,
  `Title_Text_Section` varchar(60) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Description_Section` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`Id_TitleSection`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

/*Data for the table `titlesections` */

LOCK TABLES `titlesections` WRITE;

insert  into `titlesections`(`Id_TitleSection`,`Title_Text_Section`,`Description_Section`) values (1,'Sobre nosotros',NULL),(2,'Nuestros Servicios','Brindamos distintos servicios de los cuales destacamos'),(3,'SOLICITUD DE INSCRIPCION Y REQUISITOS','En el Hogar de Ancianos Nuestra Señora del Rosario, nos dedicamos a brindar una vida digna y confortable a nuestros queridos residentes.');

UNLOCK TABLES;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
