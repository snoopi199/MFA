# Devart.SqlServer.Mfa repository

Репозиторий исходников для формирования Devart.SqlServer.Mfa nuget пакета.
Azure представляет возможность мултифакторной аутентификации для соединения с сервером БД.
Как это выглядит: https://docs.microsoft.com/en-us/azure/azure-sql/database/authentication-mfa-ssms-overview

Для поддержки данного режима создан дополнительный слой логики, который использует Microsoft.IdentityModel.ActiveDirectory.
Главный класс интеграции - AuthenticationProvider, который конфигурирует соединение для возможности получения authentication token.