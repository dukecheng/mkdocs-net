# mkdocs-net

使用Docker构建你自己的Markdown静态网站，支持Gogs, GitLab等私有git服务器[(English Introduction)](./README.md)

[![Build Status](https://drone.feinian.net/api/badges/feinian/mkdocs-net/status.svg)](https://drone.feinian.net/feinian/mkdocs-net)


Git Repository: [GitHub](https://github.com/dukecheng/mkdocs-net) [Feinian's Git](https://git.feinian.net/feinian/mkdocs-net)

[查看官网](https://mkdocs.feinian.net)

## Deploy
```bash
docker run -d -it --name mkdocs -p 6363:80 -v /data/mkdocs:/app_data --restart=always hub.feinian.net/feinian/mkdocs_net:latest
```

## Update
```bash
docker pull hub.feinian.net/feinian/mkdocs_net:latest && docker stop mkdocs && docker rm mkdocs && docker run -d -it --name mkdocs -p 6363:80 -v /data/mkdocs:/app_data --restart=always hub.feinian.net/feinian/mkdocs_net:latest
```