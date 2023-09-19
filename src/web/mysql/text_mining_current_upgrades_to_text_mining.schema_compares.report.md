# Schema update report

Update report for schema of ``text_mining_current.sql`` updates to new model ``text_mining.sql``
### Updates for ``text_graph``

Field data attribute of current table ``weight`` has been updated:

```sql
ALTER TABLE `text_mining`.`text_graph` CHANGE COLUMN `weight` `weight` double UNSIGNED NOT NULL DEFAULT 0 COMMENT '' ;
```

Add a new data field ``count``:

```sql
ALTER TABLE `text_mining`.`text_graph` ADD COLUMN `count` int (11) UNSIGNED NOT NULL DEFAULT 0 COMMENT '' ;
```

### Updates for ``word_token``

Update table description comment:

```sql
ALTER TABLE `text_mining`.`word_token` COMMENT = 'A word token in a given text segment' ;
```

