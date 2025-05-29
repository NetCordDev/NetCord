### Convenient TL;DR one-liner

> [!WARNING]
> TODO: Add actual command to install from nuget

Minimal template
```bash
dotnet new install NetCord.Templates && \
dotnet new netcord --name MyNewBot --framework net9.0
```
Or if you want complete template with all features
```bash
dotnet new install NetCord.Templates && \
dotnet new netcord --add-text-commands --add-application-commands --add-component-interactions --name MyNewBot --framework net9.0
```


> [!IMPORTANT]
> In case of local testing,  
> all commands should be run from the root of the repository.

### Packing
```bash
dotnet pack ./ProjectTemplates/ -c Release -o ./nupkgs
```

### Installing
Nuget

> [!WARNING]
> TODO: Add actual command to install from nuget

```bash
dotnet new install NetCord.Templates
```
Local
```bash
dotnet new install ./nupkgs/*
```

### Uninstalling
```bash
dotnet new uninstall NetCord.Templates
```

### Updating
```bash
dotnet new update
```

### Usage
`dotnet new netcord [options] [template options]`  
Use `dotnet new netcord --help` to see all available options.  
Example command: 
```bash
dotnet new netcord \
  --add-text-commands \
  --add-application-commands \
  --add-component-interactions \
  --name MyNewBot \
  --framework net9.0
```