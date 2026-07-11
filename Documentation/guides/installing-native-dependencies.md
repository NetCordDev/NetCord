# Installing Native Dependencies

NetCord relies on several native libraries for high-performance audio processing and encryption. NetCord provides prebuilt native binaries via NuGet packages, which is the recommended way to manage these dependencies.

## Native Dependencies

- **Libsodium**: Used for encryption and provides fallback encryption modes. NetCord attempts to use the platform's native AES-GCM encryption for voice connections. However, Libsodium becomes essential in two scenarios:
  1. The native AES-GCM is unavailable on the system.
  2. The target Discord voice server does not support AES-GCM encryption.
- **Opus**: The standard audio codec used by Discord for voice chat.
- **Zstandard (Zstd)**: Required for compressing WebSocket payloads when enabled.

## NuGet Packages (Recommended)

NetCord distributes these dependencies as individually packaged NuGet references — one package per native library. You can reference only the libraries your project actually uses, or use the meta-package to pull them all in for convenience.

| Package                     | Native library                            | NuGet                                                                                                                                                                |
|-----------------------------|-------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `NetCord.Natives`           | **Meta-package** (references all below)   | [![NetCord.Natives](https://img.shields.io/nuget/v/NetCord.Natives?label=NetCord.Natives)](https://www.nuget.org/packages/NetCord.Natives)                                         |
| `NetCord.Natives.Dave`      | libdave, mlspp, hpke, openssl (libcrypto) | [![NetCord.Natives.Dave](https://img.shields.io/nuget/v/NetCord.Natives.Dave?label=NetCord.Natives.Dave)](https://www.nuget.org/packages/NetCord.Natives.Dave)                     |
| `NetCord.Natives.Sodium`    | libsodium                                 | [![NetCord.Natives.Sodium](https://img.shields.io/nuget/v/NetCord.Natives.Sodium?label=NetCord.Natives.Sodium)](https://www.nuget.org/packages/NetCord.Natives.Sodium)             |
| `NetCord.Natives.Opus`      | opus                                      | [![NetCord.Natives.Opus](https://img.shields.io/nuget/v/NetCord.Natives.Opus?label=NetCord.Natives.Opus)](https://www.nuget.org/packages/NetCord.Natives.Opus)                     |
| `NetCord.Natives.Zstandard` | zstd                                      | [![NetCord.Natives.Zstandard](https://img.shields.io/nuget/v/NetCord.Natives.Zstandard?label=NetCord.Natives.Zstandard)](https://www.nuget.org/packages/NetCord.Natives.Zstandard) |

### Supported Platforms

Each package ships runtime binaries (`.dll` / `.so` / `.dylib`) for all supported platforms and static libraries for NativeAOT:

| Platform         | RID                | NativeAOT Support  |
|------------------|--------------------|--------------------|
| Windows x64      | `win-x64`          | ✓ (Static CRT /MT) |
| Windows ARM64    | `win-arm64`        | ✓ (Static CRT /MT) |
| Linux x64        | `linux-x64`        | ✓ (Dynamic CRT)    |
| Linux ARM64      | `linux-arm64`      | ✓ (Dynamic CRT)    |
| Linux musl x64   | `linux-musl-x64`   | ✓ (Dynamic CRT)    |
| Linux musl ARM64 | `linux-musl-arm64` | ✓ (Dynamic CRT)    |
| ⓘ macOS x64      | `osx-x64`          | ✓ (Dynamic CRT)    |
| ⓘ macOS ARM64    | `osx-arm64`        | ✓ (Dynamic CRT)    |

> ⓘ macOS packages are built and published, but function verification tests are skipped due to limitations with `dyld`. They are expected to work correctly, but we recommend testing on macOS before production use.

## Dynamic Linking

For standard .NET applications, reference the packages for the native libraries you need. The correct runtime binaries are automatically copied and loaded.

You can add these packages using the .NET CLI or directly to your `.csproj` file:

```bash
# Add individual packages...
dotnet add package NetCord.Natives.Dave

# ...or just add the meta-package for all of them
dotnet add package NetCord.Natives
```
```xml
<ItemGroup>
  <!-- Use individual packages... -->
  <PackageReference Include="NetCord.Natives.Dave" Version="*"/>
  
  <!-- ...or just use the meta-package for all of them -->
  <PackageReference Include="NetCord.Natives" Version="*"/>
</ItemGroup>
```

## NativeAOT (Static Linking)

When using [NativeAOT](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot), static libraries are automatically linked from the NuGet package, embedding the necessary native code directly into your executable.

You must explicitly register each native library with `<DirectPInvoke>` so the AOT compiler includes it:

```xml
<ItemGroup>
  <DirectPInvoke Include="libdave" />
  <DirectPInvoke Include="libsodium" />
  <DirectPInvoke Include="opus" />
  <DirectPInvoke Include="zstd" />
</ItemGroup>
```

When `DirectPInvoke` is set, the corresponding dynamic libraries are automatically excluded from the publish output.

### Excluding libraries manually

If you are managing dependencies via NuGet and want to exclude a library, you can simply omit its `<PackageReference>`. 

However, if you are building NetCord (and/or NetCord.Natives) from source (or using the meta-package) and want to provide your own binaries, use the `NetCordExcludeNatives` property. This explicitly prevents the specified libraries from being copied to your output directory **and** stops them from being statically linked during NativeAOT compilation.

```xml
<PropertyGroup>
  <NetCordExcludeNatives>libdave;libsodium</NetCordExcludeNatives>
</PropertyGroup>
```

## Custom Manual/System-wide Installation

If you are developing for an unsupported platform or require a custom-built native binary, you may handle native dependencies manually.

1. **Obtain Binaries**: Use your system's package manager (e.g., `apt`, `brew`, `dnf`) if available, or download the binaries from the official sources listed below.
2. **Placement**: Ensure the native libraries are available to your application at runtime. On Windows, place `.dll` files in your application's output directory. On Unix-like systems, ensure shared libraries are in your `LD_LIBRARY_PATH` or system standard paths.

### Installation Links

| Library   | Link                                        |
|-----------|---------------------------------------------|
| Libdave   | https://github.com/discord/libdave/releases |
| Libsodium | https://doc.libsodium.org/installation      |
| Opus      | https://opus-codec.org/downloads            |
| Zstd      | https://github.com/facebook/zstd/releases   |

## Troubleshooting

- Ensure that the correct versions are installed and accessible to your application.
- Use tools like `ldd` (Linux) or `Dependency Walker` (Windows) to verify that your application is correctly linking against the native libraries.

---

## Extra Notes on Native Dependencies

The native packaging model is designed to make managing dependencies as transparent and reliable as possible.

### What You Get
*   **Ready-to-use binaries**: We provide prebuilt runtime binaries for standard .NET and static libraries for NativeAOT.
*   **Automatic Configuration**: MSBuild files are included in the packages to automatically handle paths and linking requirements, so you don't have to manually configure build settings.

### Built-in Compliance

All necessary licenses and copyright notices from the original native project sources are bundled within each package under the `licenses/` directory.

| Library       | License      | Remarks                     |
|---------------|--------------|-----------------------------| 
| **Libdave**   | MIT          | Discord voice communication |
| **Libsodium** | ISC          | Used for encryption         |
| **OpenSSL**   | Apache 2.0   | Cryptographic operations    |
| **Opus**      | BSD-3-Clause | Audio codec                 |
| **Zstd**      | BSD-3-Clause | Compression                 |

### Reproducible Builds

Dependencies are strictly pinned using [vcpkg](https://github.com/microsoft/vcpkg) baselines, ensuring identical build results regardless of when or where the build runs.
