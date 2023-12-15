# Schema update report

Update report for schema of ``graphdb.sql`` updates to new model ``graphdb_upgrade.sql``
### Updates for ``graph``

Update table description comment:

```sql
ALTER TABLE `graphql`.`graph` COMMENT = 'the connection links between the knowledge nodes data' ;
```

Field data attribute of current table ``weight`` has been updated:

```sql
ALTER TABLE `graphql`.`graph` CHANGE COLUMN `weight` `weight` double UNSIGNED NOT NULL DEFAULT 0 COMMENT 'weight value of current connection link' ;
```

### Updates for ``hash_index``

Update table description comment:

```sql
ALTER TABLE `graphql`.`hash_index` COMMENT = 'the synonym list of the knowledge data' ;
```

### Updates for ``knowledge``

Update table description comment:

```sql
ALTER TABLE `graphql`.`knowledge` COMMENT = 'knowlege data pool' ;
```

Description comment of data field has been updated:

```sql
ALTER TABLE `graphql`.`knowledge` CHANGE COLUMN `id` `id` int (11) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'usually be the FN-1a hashcode of the \'key + node_type\' term' ;
```
Field data attribute of current table ``graph_size`` has been updated:

```sql
ALTER TABLE `graphql`.`knowledge` CHANGE COLUMN `graph_size` `graph_size` int (11) UNSIGNED NOT NULL DEFAULT 0 COMMENT 'the number of connected links to current knowledge node' ;
```

Field data attribute of current table ``knowledge_term`` has been updated:

```sql
ALTER TABLE `graphql`.`knowledge` CHANGE COLUMN `knowledge_term` `knowledge_term` int (11) UNSIGNED NOT NULL DEFAULT 0 COMMENT 'default zero means not assigned, and any positive integer means this property data has been assigned to a specific knowledge' ;
```

### Updates for ``knowledge_cache``

### Updates for ``knowledge_vocabulary``

Update table description comment:

```sql
ALTER TABLE `graphql`.`knowledge_vocabulary` COMMENT = 'the knowledge term type, category or class data label. the word ontology class data table' ;
```

Field data attribute of current table ``ancestor`` has been updated:

```sql
ALTER TABLE `graphql`.`knowledge_vocabulary` CHANGE COLUMN `ancestor` `ancestor` int (11) UNSIGNED NOT NULL DEFAULT 0 COMMENT 'the parent node of current ontology term' ;
```

Field data attribute of current table ``level`` has been updated:

```sql
ALTER TABLE `graphql`.`knowledge_vocabulary` CHANGE COLUMN `level` `level` int (11) UNSIGNED NOT NULL DEFAULT 0 COMMENT 'the level of current ontology tern node on the family tree' ;
```

Field data attribute of current table ``count`` has been updated:

```sql
ALTER TABLE `graphql`.`knowledge_vocabulary` CHANGE COLUMN `count` `count` int (11) UNSIGNED NOT NULL DEFAULT 0 COMMENT 'hit counts in the knowledge table' ;
```

