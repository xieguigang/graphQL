# starter script for run graphdb web server

# stop web server
docker stop graphdb
docker rm graphdb

docker run -tid --name graphdb \
	-p 88:80 \
	--cap-add SYS_ADMIN --device /dev/fuse --security-opt apparmor=unconfined \
	-v "$(which docker):/bin/docker" \
	-v "/var/run/docker.sock:/run/docker.sock" \
	-v "/home/xieguigang/graphQL/src/web/etc/apache_configs:/etc/httpd/vhost" \
	-v "/home/xieguigang/graphQL/src/web/etc/php.ini:/etc/php.ini" \
	-v "/home/xieguigang/graphQL/src/web/FastRWeb:/var/FastRWeb" \
	-v "/home/xieguigang/graphQL/src/web:/opt/graphdb" \
	-v "/tmp/Rscript/:/var/FastRWeb/tmp/:rw" \
	-v "/tmp/apache/:/tmp/apache/:rw" \
	-v "/tmp/php/:/tmp/:rw" \
	--privileged=true \
	dotnet:6.0-php8.NET  /usr/sbin/init

docker exec -it graphdb systemctl restart httpd

docker exec -it graphdb ls -l /etc/httpd/logs/

docker exec -it graphdb cat /etc/httpd/logs/error.log
docker exec -it graphdb cat /etc/httpd/logs/error_log
docker exec -it graphdb cat /etc/httpd/logs/access.log