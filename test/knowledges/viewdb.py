from base import HDS
import console
import JSON
import graphQL

setwd(@dir)

hds = HDS::openStream("./aspirin.graph")

cat("\n\n")

# print(HDS::files(hds))
console::log(HDS::tree(hds))

# str(json_decode(HDS::getText(hds, "/knowledge_blocks.json")))
str(json_decode(HDS::getText(hds, "/summary.json")))

close(hds)

index = MsgFile::open("./aspirin.graph",  seekIndex= True)

print(getTerms(index))

str(seekTerm(index, "50-78-2"))