# Практическая работа №4 — вариант 4

## Назначение
Консольный эмулятор UNIX-подобной оболочки с виртуальной файловой системой (VFS),
конфигурацией TOML и выполнением стартового скрипта.

Реализация выполнена на C# / .NET 8.

## Соответствие варианту 4

### Этап 1 — REPL
- CLI;
- prompt формируется из `Environment.UserName` и `Environment.MachineName`;
- простой парсер: команда и аргументы разделяются пробелами;
- команды-заглушки заменены рабочими командами `ls` и `cd` в следующих этапах;
- `exit` завершает работу;
- ошибки неизвестных команд и неверных аргументов выводятся пользователю.

### Этап 2 — конфигурация
Поддерживаются параметры:
- `--vfs PATH` — путь к XML-файлу VFS;
- `--script PATH` — путь к стартовому скрипту;
- `--config PATH` — путь к TOML-файлу конфигурации.

TOML содержит `vfs_path` и `script_path`. Значения из TOML имеют приоритет над CLI.
Ошибки чтения TOML сообщаются пользователю. При запуске печатается отладочная
информация по всем параметрам.

Стартовый скрипт выполняется последовательно. Ошибочные строки пропускаются,
при этом на экран выводятся и ввод, и вывод.

### Этап 3 — VFS
VFS загружается из XML и полностью находится в памяти. Файловая система хоста
не изменяется командами оболочки. Текстовые файлы задаются содержимым XML; для
двоичных данных поддерживается `encoding="base64"` с декодированием в память.

Примеры:
- `vfs_examples/minimal.xml` — минимальный;
- `vfs_examples/sample.xml` — несколько файлов и каталогов;
- `vfs_examples/deep.xml` — три уровня вложенности;
- `vfs_examples/binary.xml` — пример base64.

### Этап 4 — основные команды
Реализованы:
- `ls [PATH]`
- `cd PATH`
- `cat PATH`
- `who`
- `head PATH`
- `head -n NUMBER PATH`

### Этап 5 — дополнительная команда
- `touch PATH` — создаёт пустой файл только в памяти VFS.

## Структура репозитория

```text
src/        исходный код приложения
 tests/     автономный тестовый проект
scripts/    скрипты запуска и стартовый сценарий
config/     TOML-конфигурация
vfs_examples/ XML-примеры VFS
```

## Сборка

Требуется .NET 8 SDK.

```bash
dotnet build src/ShellEmulator.csproj
```

## Тесты

```bash
dotnet run --project tests/ShellEmulator.Tests.csproj
```

## Запуск

Интерактивный режим:

```bash
dotnet run --project src/ShellEmulator.csproj
```

С VFS и стартовым скриптом:

```bash
dotnet run --project src/ShellEmulator.csproj -- \
  --vfs vfs_examples/sample.xml \
  --script scripts/start.txt
```

С TOML-конфигурацией:

```bash
dotnet run --project src/ShellEmulator.csproj -- --config config/config.toml
```

Утилиты `scripts/run.sh` и `scripts/run.bat` вызывают приложение тем же способом.

## Примеры команд

```text
user@host:/$ ls
etc/
home/

user@host:/$ cd home
user@host:/home$ ls
docs/
welcome.txt

user@host:/home$ cat welcome.txt
Welcome to the ...

user@host:/home$ touch created.txt
user@host:/home$ ls
created.txt
docs/
welcome.txt
```

## Git

Для этапов предусмотрена Conventional/Scoped Commit история:
- `feat(repl): add interactive shell prototype`
- `feat(config): add TOML configuration and scripts`
- `feat(vfs): add in-memory XML virtual file system`
- `feat(commands): add ls cd cat who head`
- `feat(touch): add in-memory touch command`
