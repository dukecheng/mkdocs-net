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

## Apis
* name; string; name of the project in mkdocs
* requestPath; string; request path of the project in mkdocs
* host; string; host of the project to fetch markdown files
* hostType; int; host type of the project(1: gogs, 2: gitlab, 3: github)
* projectPath; string; project path of the project in git repository(for gitlab, it is projectId)
* wikiFolder; string; wiki folder of the project in git repository
* indexFile; string; index file of the project in wiki folder
* defaultView; string; default view of the project, it is branch name or tag name
* isUseNavMenu; bool; is use nav menu of the project, if true, it will use nav menu of the project, otherwise, it will use nav menu of the mkdocs
* isUseVersionViews; bool; is use version views of the project, if true, it will use version views of the project, otherwise, it will use version views of the mkdocs
* visiableMode; int; visiable mode of the project, 0: public, 1: private, 2: password
* requestHeaders; array; request headers of the project, it is an array of key-value pairs of string send to remote api as http headers
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