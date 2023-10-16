# SAS_Dumper

![AutoBuild][workflow_b] [![release][release_b]][release] [![Download][download_b]][release] [![License][license_b]][license]

[中文说明](README.md)

### Журнал изменений

| Версия SAS_Dumper                                                      | Совместимая версия ASF |
| ---------------------------------------------------------------------- | :--------------------: |
| [1.0.13.0](https://github.com/chr233/SAS_Dumper/releases/tag/1.0.13.0) |   5.4.10.3             |
| [1.0.12.1](https://github.com/chr233/SAS_Dumper/releases/tag/1.0.12.1) |    5.4.4.5             |
| [1.0.8.1](https://github.com/chr233/SAS_Dumper/releases/tag/1.0.8.1)   |   5.4.2.13             |

## Команды

|    Команда  |  Доступ  | Описание               |
| :---------: | :------: | :----------------------|
|    `SAS`    | `Master` | Экспорт токена в файл  |
| `SASDUMPER` | `Master` | Просмотр версии плагина|
|  `SASTEST`  | `Master` | 测试与 SAS 后台能否连接 |
|   `SASON`   | `Master` | 启用自动汇报            |
|  `SASTOFF`  | `Master` | 禁用自动汇报            |
| `SASFRESH`  | `Master` |  Принудительная очистка кэша токенов     |
| `SASMANUAL` | `Master` | 手动汇报所有在线机器人  |

## Конфигурация плагина

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

## Cсылка на скачивание

[Releases](https://github.com/chr233/SAS_Dumper/releases)

[workflow_b]: https://img.shields.io/github/actions/workflow/status/chr233/SAS_Dumper/autobuild.yml?logo=github
[download_b]: https://img.shields.io/github/downloads/chr233/SAS_Dumper/total
[release]: https://github.com/chr233/SAS_Dumper/releases
[release_b]: https://img.shields.io/github/v/release/chr233/SAS_Dumper
[license]: https://github.com/chr233/SAS_Dumper/blob/master/license
[license_b]: https://img.shields.io/github/license/chr233/SAS_Dumper
