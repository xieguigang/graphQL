# Schema update report

Update report for schema of ``text_mining_current.sql`` updates to new model ``text_mining.sql``
### Updates for ``text_graph``

Field data attribute of current table ``weight`` has been updated:

```sql
ALTER TABLE `text_mining`.`text_graph` CHANGE COLUMN `weight` `weight` double UNSIGNED NOT NULL DEFAULT 0 COMMENT '' ;
```

Field data attribute of current table ``count`` has been updated:

```sql
ALTER TABLE `text_mining`.`text_graph` CHANGE COLUMN `count` `count` int (11) UNSIGNED NOT NULL DEFAULT 0 COMMENT '' ;
```

### Updates for ``token_class``

Current database schema didn't has this table, a new table will be created:

```sql
CREATE TABLE IF NOT EXISTS `token_class` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `class` VARCHAR(255) NOT NULL,
  `add_time` DATETIME NOT NULL DEFAULT now(),
  PRIMARY KEY (`id`))
ENGINE = InnoDB;

```

### Updates for ``word_token``

Update table description comment:

```sql
ALTER TABLE `text_mining`.`word_token` COMMENT = 'A word token in a given text segment' ;
```

Add a new data field ``class``:

```sql
ALTER TABLE `text_mining`.`word_token` ADD COLUMN `class` int (11) UNSIGNED NOT NULL DEFAULT 1 COMMENT '' ;
```

