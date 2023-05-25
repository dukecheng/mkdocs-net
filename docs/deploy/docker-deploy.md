# Docker Deploy

```bash
docker run -d -it --name mkdocs -p 6363:80 \
-v /data/mkdocs:/app_data --restart=always \
hub.feinian.net/feinian/mkdocs_net:latest
```

## Update

```bash
docker pull hub.feinian.net/feinian/mkdocs_net:latest && \
docker stop mkdocs && docker rm mkdocs && \
docker run -d -it --name mkdocs -p 6363:80 \
-v /data/mkdocs:/app_data --restart=always \
hub.feinian.net/feinian/mkdocs_net:latest
```
