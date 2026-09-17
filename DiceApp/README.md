# DiceApp

A simple Windows Forms dice rolling app built with C# and .NET 10.

## Features

* Roll dice with a simple interface
* Fast and lightweight
* Native Windows desktop app

## Requirements

* Windows
* .NET 10 SDK, if building from source

## Build

```bash
dotnet build -c Release
```

To create a standalone Windows executable:

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

The published executable can be found in:

```text
bin/Release/net10.0-windows/win-x64/publish/
```

## License

See the repository license for details.
