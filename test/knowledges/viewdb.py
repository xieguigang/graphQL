from base import HDS
import console
import JSON

setwd(@dir)

hds = HDS::openStream("./aspirin.graph")

cat("\n\n")

# print(HDS::files(hds))
console::log(HDS::tree(hds))

str(json_decode(HDS::getText(hds, "/knowledge_blocks.json")))
str(json_decode(HDS::getText(hds, "/summary.json")))