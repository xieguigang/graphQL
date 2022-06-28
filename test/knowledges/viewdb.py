from base import HDS

setwd(@dir)

hds = HDS::openStream("./aspirin.graph")

print(hds.files)