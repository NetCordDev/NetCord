---
title: Setting Up Your .NET Project for NetCord Bot Development
description: Configure your C# project for Discord bot development with NetCord using .NET Generic Host or bare-bones approach. Includes configuration setup.
omitAppTitle: true
keywords: Discord, project setup, .NET, C#, Generic Host, configuration, project configuration, bot development, setup
section: Getting Started
published_time: '2025-12-15T00:00:00Z'
modified_time: '2025-12-15T00:00:00Z'
---

# Project Setup

Before you can use NetCord, you need to set up your .NET project. This guide covers both approaches: the modern .NET Generic Host (recommended for professional bots) and the bare-bones approach (for simple scripts).

> [!NOTE]
> If you haven't installed NetCord yet, see the [Installation Guide](installation.md).

## Creating a New Project {#creating-project}

First, create a new .NET console project:

```bash
dotnet new console -n MyBot
cd MyBot
```

Then install the NetCord packages:

```bash
dotnet add package NetCord --prerelease
dotnet add package NetCord.Services --prerelease
dotnet add package NetCord.Hosting --prerelease
dotnet add package NetCord.Hosting.Services --prerelease
```

## Program.cs Setup {#program-setup}

Choose your approach based on your needs. The Generic Host is recommended for production bots, while Bare Bones is simpler for learning or prototypes.

## [**Generic Host**](#tab/generic-host)

Replace your `Program.cs` with:

[!code-cs[Generic Host Minimal Setup](BareBones/ProjectSetup.cs#GenericHostMinimal)]

This minimal setup:
- Creates a host using your project's default configuration
- Registers Discord Gateway services
- Keeps your bot running indefinitely

## [**Bare Bones**](#tab/bare-bones)

Replace your `Program.cs` with:

[!code-cs[Bare Bones Minimal](BareBones/ProjectSetup.cs#BareBonesMinimal)]

This approach:
- No configuration files needed
- Direct token in code (fine for learning, not production!)
- Simpler for quick prototyping
- No dependency injection or other framework features

***

## Configuration {#configuration}

## [**Generic Host**](#tab/generic-host)

Create an `appsettings.json` file in your project root and keep it checked in with placeholder values:

```json
{
  "Discord": {
    "Token": "YOUR_BOT_TOKEN_HERE"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  }
}
```

Then create an `appsettings.Development.json` file (not committed) with your real token:

```json
{
  "Discord": {
    "Token": "YOUR_REAL_BOT_TOKEN_HERE"
  }
}

```

When the environment is `Development`, .NET will automatically load `appsettings.Development.json` on top of `appsettings.json`.

> [!IMPORTANT]
> Add `appsettings.Development.json` to your `.gitignore` file to prevent accidentally committing your token:
> ```
> # .gitignore
> appsettings.*.json
> !appsettings.json
> ```

To use your configuration in code:

[!code-cs[Loading Configuration](BareBones/ProjectSetup.cs#GenericHostWithConfiguration)]

## [**Bare Bones**](#tab/bare-bones)

For slightly better security, load the token from an environment variable:

[!code-cs[Bare Bones with Environment Variable](BareBones/ProjectSetup.cs#BareBonesEnvVar)]

Set the environment variable before running:

```bash
# Windows PowerShell
$env:DISCORD_BOT_TOKEN = "your-token-here"
dotnet run

# Windows Command Prompt
set DISCORD_BOT_TOKEN=your-token-here
dotnet run

# Linux/macOS
export DISCORD_BOT_TOKEN="your-token-here"
dotnet run
```

***

## Benefits {#benefits}

## [**Generic Host**](#tab/generic-host)

- **Dependency Injection:** Register and inject services throughout your bot
- **Configuration:** Manage settings from `appsettings.json`, environment variables, or other sources
- **Logging:** Built-in structured logging with multiple providers
- **Lifetime Management:** Proper startup/shutdown and resource cleanup
- **Extensibility:** Easy to add features like health checks, diagnostics, etc.

**Use Generic Host when:**
- Building a production bot
- You need multiple features (commands, logging, configuration, etc.)
- Your project will grow over time
- You want professional patterns and best practices

## [**Bare Bones**](#tab/bare-bones)

- **Simplicity:** Minimal code to get started
- **Fast prototyping:** No configuration files or setup
- **Easy to understand:** Direct, explicit code with no abstractions
- **Lightweight:** No framework dependencies beyond NetCord itself

**Use Bare Bones when:**
- Learning NetCord for the first time
- Building a simple, single-feature bot
- Writing a quick prototype
- Working on something you'll throw away

***

> [!NOTE]
> You can always migrate from bare-bones to Generic Host later. The core bot code doesn't change much.

## Project File Setup {#project-file}

Your `.csproj` should look like this:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="NetCord" Version="*" />
    <PackageReference Include="NetCord.Services" Version="*" />
    <PackageReference Include="NetCord.Hosting" Version="*" />
    <PackageReference Include="NetCord.Hosting.Services" Version="*" />
  </ItemGroup>

</Project>
```

## Testing Your Setup {#testing-setup}

Try running your project to ensure everything compiles:

```bash
dotnet build
```

If you get compilation errors, double-check:
- Your token is valid and properly formatted
- All NuGet packages installed successfully
- Your project targets .NET 9 or higher

---

## Navigation

← **Previous:** [Creating Your Bot](creating-your-bot.md) | **Next:** [Your First Response](your-first-response.md) →

## See Also

- [.NET Generic Host Integration](../dotnet-integration/generic-host.md) - Advanced configuration patterns
- [Configuration Management](../dotnet-integration/configuration.md) - Managing secrets and settings
- [Structured Logging](../dotnet-integration/logging.md) - Logging best practices

