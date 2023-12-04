# mkdocs-net

[![Build Status](https://drone.feinian.net/api/badges/feinian/mkdocs-net/status.svg)](https://drone.feinian.net/feinian/mkdocs-net)

Host your own markdown docs website with docker, Integrated with your private git repository like gogs or gitlab server[(中文说明)](./README_cn.md).

## features

- Build your own Markdown static website using Docker
- Synchronize updates with private git server in real time
- Supports private git servers such as Gogs, GitLab, etc.
- SSR output, SEO friendly

Git Repository: [GitHub](https://github.com/dukecheng/mkdocs-net) [Feinian's Git](https://git.feinian.net/feinian/mkdocs-net)

[View Official Site](https://mkdocs.feinian.net)

![Intro](./docs/assets/intro.png)

## UseCloud

https://mkdocs.feinian.net

## standalone deploy

use follow docker run script to host your own mkdocs website

## Deploy

```bash
docker run -d -it --name mkdocs -p 6363:80 \
-v /data/mkdocs:/app_data \
--restart=always \
hub.feinian.net/feinian/mkdocs_net:latest
```

## Update

```bash
docker pull hub.feinian.net/feinian/mkdocs_net:latest && \
docker stop mkdocs && \
docker rm mkdocs && \
docker run -d -it --name mkdocs -p 6363:8080 \
-v /data/mkdocs:/app_data \
--restart=always \
hub.feinian.net/feinian/mkdocs_net:latest
```
