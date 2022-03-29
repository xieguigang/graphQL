import graphQL

for file in list.files("K:\metadb\graphDb", pattern = "*.graph"):   
    kb = read.knowledge_table(file)
    csv_file = `${dirname(file)}/${basename(file)}.terms.csv`
    
    write.csv(kb, file = csv_file, row.names = False)