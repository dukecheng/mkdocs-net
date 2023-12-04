# mkdocs-net

使用 Docker 构建你自己的 Markdown 静态网站，支持 Gogs, GitLab 等私有 git 服务器[(English Introduction)](./README.md)

## 功能

- 使用 Docker 构建你自己的 Markdown 静态网站
- 实时与私有 git 服务器同步更新
- 支持 Gogs, GitLab 等私有 git 服务器
- SSR输出,对SEO友好

[![Build Status](https://drone.feinian.net/api/badges/feinian/mkdocs-net/status.svg)](https://drone.feinian.net/feinian/mkdocs-net)

Git Repository: [GitHub](https://github.com/dukecheng/mkdocs-net) [Feinian's Git](https://git.feinian.net/feinian/mkdocs-net)

[查看官网](https://mkdocs.feinian.net)

![Intro](./docs/assets/intro.png)



## 使用云

<https://mkdocs.feinian.net>

## 独立部署

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

## Apis
* 名字;字符串;MKDocs 中的项目名称
* 请求路径;字符串;mkdocs 中项目的请求路径
* 主机;字符串;用于获取 Markdown 文件的项目的主机
* 主机类型;int;项目主机类型（1：gogs，2：gitlab，3：github）
* 项目路径;字符串;Git 仓库中项目的项目路径（对于 GitLab，它是 projectId）
* 维基文件夹;字符串;git 存储库中项目的 wiki 文件夹
* 索引文件;字符串;Wiki 文件夹中项目的索引文件
* 默认视图;字符串;项目的默认视图，它是分支名称或标签名称
* 是UseVersionViews;布尔值;是使用项目的版本视图，如果为 true，它将使用项目的版本视图，否则，它将使用 mkdocs 的版本视图
* visiable模式;int;项目的可见模式，0：公共，1：私有，2：密码
* 请求标头;数组;请求标头，它是字符串的键值对数组，作为 HTTP 标头发送到远程 API
```json
{
  "id": "1731545948036927488",
  "name": "mkdocs-net-official", 
  "requestPath": "mkdocs-net-official",
  "host": "https://raw.githubusercontent.com",
  "hostType": 3,
  "projectPath": "dukecheng/mkdocs-net",
  "wikiFolder": "docs",
  "indexFile": "README.md",
  "defaultView": "master",
  "isUseNavMenu": true,
  "isUseVersionViews": false,
  "visiableMode": 0,
  "requestHeaders": [
    {
      "key": "string",
      "value": "string"
    }
  ]
}
```