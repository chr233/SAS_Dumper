# SAS_Dumper

![AutoBuild][workflow_b] [![release][release_b]][release] [![Download][download_b]][release] [![License][license_b]][license]

## 说明

本项目已过时, 并且停止维护, 已有新的多线程打赏插件替代本项目, 联系 [chr@chrxw.com](chr@chrxw.com)

## 适配说明

| 插件版本                                                               | 兼容 ASF 版本 |
| ---------------------------------------------------------------------- | :-----------: |
| [1.0.13.0](https://github.com/chr233/SAS_Dumper/releases/tag/1.0.13.0) |   5.4.10.3    |
| [1.0.12.1](https://github.com/chr233/SAS_Dumper/releases/tag/1.0.12.1) |    5.4.4.5    |
| [1.0.8.1](https://github.com/chr233/SAS_Dumper/releases/tag/1.0.8.1)   |   5.4.2.13    |

## 命令列表

|    命令     |   权限   | 说明                    |
| :---------: | :------: | :---------------------- |
|    `SAS`    | `Master` | 导出 Token 到文件       |
| `SASDUMPER` | `Master` | 查看插件版本            |
|  `SASTEST`  | `Master` | 测试与 SAS 后台能否连接 |
|   `SASON`   | `Master` | 启用自动汇报            |
|  `SASTOFF`  | `Master` | 禁用自动汇报            |
| `SASFRESH`  | `Master` | 强制刷新 Token 缓存     |
| `SASMANUAL` | `Master` | 手动汇报所有在线机器人  |

## 配置说明

ASF.json

```json
{
  "SASConfig": {
    "Enabled": true,
    "SASUrl": "http://SASBOTSHOST:PORT/",
    "SASPasswd": "PASSWD",
    "FeedbackPeriod": 60
  }
}
```

## 下载链接

[Releases](https://github.com/chr233/SAS_Dumper/releases)

[workflow_b]: https://img.shields.io/github/actions/workflow/status/chr233/SAS_Dumper/autobuild.yml?logo=github
[download_b]: https://img.shields.io/github/downloads/chr233/SAS_Dumper/total
[release]: https://github.com/chr233/SAS_Dumper/releases
[release_b]: https://img.shields.io/github/v/release/chr233/SAS_Dumper
[license]: https://github.com/chr233/SAS_Dumper/blob/master/license
[license_b]: https://img.shields.io/github/license/chr233/SAS_Dumper
