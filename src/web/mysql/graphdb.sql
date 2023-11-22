CREATE DATABASE  IF NOT EXISTS `graphql` /*!40100 DEFAULT CHARACTER SET utf8mb3 */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `graphql`;
-- MySQL dump 10.13  Distrib 8.0.34, for Win64 (x86_64)
--
-- Host: localhost    Database: graphql
-- ------------------------------------------------------
-- Server version	8.0.33

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
-- Table structure for table `graph`
--

DROP TABLE IF EXISTS `graph`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `graph` (
  `id` int unsigned NOT NULL AUTO_INCREMENT,
  `from_node` int unsigned NOT NULL COMMENT 'the unique id of the knowledge data',
  `to_node` int unsigned NOT NULL COMMENT 'the unique id of the knowledge data',
  `link_type` int unsigned NOT NULL COMMENT 'the connection type between the two knowdge node, the enumeration text string value could be found in the knowledge vocabulary table',
  `weight` double unsigned NOT NULL DEFAULT '0' COMMENT 'weight value of current connection link',
  `add_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'the create time of the current knowledge link',
  `note` text COMMENT 'description text about current knowledge link',
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`),
  KEY `source_index` (`from_node`) /*!80000 INVISIBLE */,
  KEY `target_index` (`to_node`) /*!80000 INVISIBLE */,
  KEY `term_index` (`link_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COMMENT='the connection links between the knowledge nodes data';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `hash_index`
--

DROP TABLE IF EXISTS `hash_index`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `hash_index` (
  `id` int unsigned NOT NULL AUTO_INCREMENT,
  `synonym` text NOT NULL,
  `hashcode` char(32) NOT NULL COMMENT 'tolower(md5(tolower(synonym)))',
  `map` int unsigned NOT NULL COMMENT 'the synonym id mapping to the knowledge data term',
  `add_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`),
  KEY `hash_map` (`hashcode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COMMENT='the synonym list of the knowledge data';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `knowledge`
--

DROP TABLE IF EXISTS `knowledge`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `knowledge` (
  `id` int unsigned NOT NULL COMMENT 'usually be the FN-1a hashcode of the ''key + node_type'' term',
  `key` varchar(255) NOT NULL COMMENT 'the unique key of current knowledge node data',
  `display_title` varchar(2048) NOT NULL COMMENT 'the display title text of current knowledge node data',
  `node_type` int unsigned NOT NULL COMMENT 'the node type enumeration number value, string value could be found in the knowledge vocabulary table',
  `graph_size` int unsigned NOT NULL DEFAULT '0' COMMENT 'the number of connected links to current knowledge node',
  `add_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'add time of current knowledge node data',
  `description` longtext NOT NULL COMMENT 'the description text about current knowledge data',
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`),
  KEY `key_index` (`key`) /*!80000 INVISIBLE */,
  KEY `type_index` (`node_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COMMENT='knowlege data pool';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `knowledge_vocabulary`
--

DROP TABLE IF EXISTS `knowledge_vocabulary`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `knowledge_vocabulary` (
  `id` int unsigned NOT NULL AUTO_INCREMENT,
  `vocabulary` varchar(1024) NOT NULL COMMENT 'the short knowledge term type',
  `hashcode` varchar(32) NOT NULL,
  `ancestor` int unsigned NOT NULL DEFAULT '0' COMMENT 'the parent node of current ontology term',
  `level` int unsigned NOT NULL DEFAULT '0' COMMENT 'the level of current ontology tern node on the family tree',
  `add_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `color` char(7) NOT NULL DEFAULT '#123456' COMMENT 'html color code of current knowledge ontology term',
  `count` int unsigned NOT NULL DEFAULT '0' COMMENT 'hit counts in the knowledge table',
  `description` mediumtext COMMENT 'the description text value about current type of the knowledge term',
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`),
  UNIQUE KEY `vocabulary_UNIQUE` (`vocabulary`),
  KEY `term_index` (`vocabulary`) /*!80000 INVISIBLE */,
  KEY `color_index` (`color`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb3 COMMENT='the knowledge term type, category or class data label. the word ontology class data table';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping routines for database 'graphql'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2023-11-22 13:31:18
