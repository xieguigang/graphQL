require(graphQL);

printLinksSource = function(path) {
	print(path);
	# print(substr(edgeSource(path), 1, 5));
	print(edgeSource(path));
	
	cat("\n\n");
}

for(file in list.files("D:\Database\20220306\multiples\graphDb", pattern = "*.graph")) {
printLinksSource(file);
}


# printLinksSource("D:\biodeep\graphdb\multiples\graphDb\flavonoid_base.graph");