import graphQL

kb = MsgFile::open(`${@dir}/aspirin.graph`)

print("unweighted similarity between C01405 and HMDB0001879:")
print(Query::similarity(kb, "C01405", "HMDB0001879"))

print("unweighted similarity between Aspirin and ChEBI: 15365:")
print(Query::similarity(kb, "Aspirin", "ChEBI:15365"))

print("unweighted similarity between Aspirin and Acetylsalicylic acid:")
print(Query::similarity(kb, "Aspirin", "Acetylsalicylic acid"))

print("unweighted similarity between Aspirin and Water:")
print(Query::similarity(kb, "Aspirin", "Water"))

print("unweighted similarity between dihydrogen oxide and Water:")
print(Query::similarity(kb, "dihydrogen oxide", "water"))


