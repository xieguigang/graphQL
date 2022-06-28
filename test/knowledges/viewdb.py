from base import HDS
import console

setwd(@dir)

hds = HDS::openStream("./aspirin.graph")

cat("\n\n")

# print(HDS::files(hds))
console::log(HDS::tree(hds))