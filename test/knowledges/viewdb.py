from base import HDS

setwd(@dir)

hds = HDS::openStream("./aspirin.graph")

print(HDS::files(hds))