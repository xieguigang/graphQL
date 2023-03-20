# starter script for run novocell web server

# stop web server
docker stop sample_pool
docker rm sample_pool

docker run -tid --name sample_pool \
	-p 83:80 \
	--cap-add SYS_ADMIN --device /dev/fuse --security-opt apparmor=unconfined \
	-v "$(which docker):/bin/docker" \
	-v "/var/run/docker.sock:/run/docker.sock" \
	-v "/home/xieguigang/sample_pool/etc/apache_configs:/etc/httpd/vhost" \
	-v "/home/xieguigang/sample_pool/etc/php.ini:/etc/php.ini" \
	-v "/home/xieguigang/sample_pool/FastRWeb:/var/FastRWeb" \
	-v "/home/xieguigang/sample_pool:/opt/sample_pool" \
	-v "/mnt/smb3/SamplePool:/opt/sample_pool/WebContext/" \
	-v "/home/xieguigang/MSI:/home/xieguigang/MSI" \
	-v "/tmp/Rscript/:/var/FastRWeb/tmp/:rw" \
	-v "/tmp/apache/:/tmp/apache/:rw" \
	-v "/tmp/php/:/tmp/:rw" \
	--privileged=true \
	dotnet:6.0-php8.NET  /usr/sbin/init

docker exec -it sample_pool systemctl restart httpd

docker exec -it sample_pool ls -l /etc/httpd/logs/

docker exec -it sample_pool cat /etc/httpd/logs/error.log
docker exec -it sample_pool cat /etc/httpd/logs/error_log
docker exec -it sample_pool cat /etc/httpd/logs/access.log