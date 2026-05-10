# Installing Native Dependencies

NetCord relies on several native libraries for high-performance audio processing and encryption. NetCord provides prebuilt native binaries via NuGet packages, which is the recommended way to manage these dependencies.

## Native Dependencies Context

- **Libdave**: Essential for voice connection interactions.
- **Libsodium**: Used for encryption. While NetCord defaults to native AES-GCM (which does not require Libsodium), Libsodium is a highly recommended production dependency. It acts as a fallback for the XChaCha20-Poly1305 encryption mode, ensuring your bot remains compatible if Discord switches to this mode for your connection.
- **Opus**: A versatile audio codec required for any classes in NetCord prefixed with `Opus` (e.g., audio encoding/decoding).
- **Zstd**: Used for efficient payload compression.

## NuGet Packages (Recommended)

NetCord distributes these dependencies as per-RID (Runtime Identifier) packages. This model automates binary resolution, ensuring the correct libraries are provided for your target platform.

### Supported Platforms
Packages are available for:

| Platform | RID | NativeAOT Support | <div style="text-align: center;">Size <br/> ![Storage Requirement](https://img.shields.io/badge/Runtime-Package-blue)</div> |
|----------|-----|-------------------------|-----------|
| Windows x64 | `win-x64` | ✓ (Static CRT /MT) | ![Windows x64](https://img.shields.io/badge/6_MB-42_MB_(176_MB)-blue) |
| Windows ARM64 | `win-arm64` | ✓ (Static CRT /MT) | ![Windows ARM64](https://img.shields.io/badge/6_MB-41_MB_(183_MB)-blue) |
| Linux x64 | `linux-x64` | ✓ (Dynamic CRT) | ![Linux x64](https://img.shields.io/badge/10_MB-9_MB_(28_MB)-blue) |
| Linux ARM64 | `linux-arm64` | ✓ (Dynamic CRT) | ![Linux ARM64](https://img.shields.io/badge/10_MB-8_MB_(26_MB)-blue) |
| ⓘ macOS x64 | `osx-x64` | ✓ (Dynamic CRT) | ![macOS x64](https://img.shields.io/badge/15_MB-11_MB_(28_MB)-blue) |
| ⓘ macOS ARM64 | `osx-arm64` | ✓ (Dynamic CRT) | ![macOS ARM64](https://img.shields.io/badge/13_MB-10_MB_(25_MB)-blue) |

ⓘ Currently, the macOS packages are built and published, but functions verification tests are skipped due to limitations with `dyld`. 
They are still expected to work correctly, but we recommend testing on macOS before production use.

## Dynamic Linking
For standard .NET applications, you can simply reference the relevant packages for your target platforms. The runtimes will be correctly copied and loaded across platforms.

```xml
<ItemGroup>
  <!-- Example for cross-platform support -->
  <PackageReference Include="NetCord.Natives.win-x64" Version="1.0.0" />
  <PackageReference Include="NetCord.Natives.linux-x64" Version="1.0.0" />
</ItemGroup>
```

## Usage with NativeAOT (Static Linking)

When using [NativeAOT](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot), static libraries are automatically linked from the NuGet package, embedding the necessary native code directly into your executable.

### Requirement for DirectPInvoke
When targeting NativeAOT, you must ensure that all used native libraries are explicitly registered using `<DirectPInvoke>` in your project file to ensure they are properly included during the AOT compilation process:

```xml
<ItemGroup>
  <DirectPInvoke Include="libdave" />
  <DirectPInvoke Include="libsodium" />
  <DirectPInvoke Include="opus" />
  <DirectPInvoke Include="zstd" />
</ItemGroup>
```

### Excluding Bundled Dependencies
If you have provided your own native binaries and need to exclude the ones bundled in the NetCord NuGet package to avoid conflicts, use the `NetCordExcludeNatives` property.

```xml
<PropertyGroup>
  <NetCordExcludeNatives>opus</NetCordExcludeNatives>
</PropertyGroup>
```

## Custom Manual/System-wide Installation

If you are developing for an unsupported platform or require a custom-built native binary, you may choose to handle native dependencies manually.

1. **Obtain Binaries**: Use your system's package manager (e.g., `apt`, `brew`, `dnf`) if available, or download the binaries from the official sources listed below.
2. **Placement**: Ensure the native libraries are available to your application at runtime. On Windows, place `.dll` files in your application's output directory. On Unix-like systems, ensure shared libraries are in your `LD_LIBRARY_PATH` or system standard paths.

### Installation External Links

| Library   | Installation Link                           |
|-----------|---------------------------------------------|
| Libdave   | https://github.com/discord/libdave/releases |
| Libsodium | https://doc.libsodium.org/installation      |
| Opus      | https://opus-codec.org/downloads            |
| Zstd      | https://github.com/facebook/zstd/releases   |

## Troubleshooting
- If you encounter issues with native dependencies, ensure that the correct versions are installed and that they are accessible to your application.
- Use tools like `ldd` (Linux) or `Dependency Walker` (Windows) to verify that your application is correctly linking against the native libraries.

---

## Extra Notes on Native Dependencies

The native packaging model is designed to make managing dependencies as transparent and reliable as possible.

### What You Get
*   **Ready-to-use binaries**: We provide prebuilt runtime binaries for standard .NET and static libraries for NativeAOT.
*   **Automatic Configuration**: MSBuild files are included in the packages to automatically handle paths and linking requirements, so you don't have to manually configure build settings.

### Built-in Compliance
All necessary licenses and copyright notices from the original native project sources are bundled automatically within each package under the `licenses/` directory.

| Library | License | Remarks |
| :--- | :--- | :--- |
| **Libdave** | MIT | Discord voice communication |
| **Libsodium** | ISC | Used for encryption |
| **OpenSSL** | Apache 2.0 | Cryptographic operations |
| **Opus** | BSD-3-Clause | Audio codec |
| **Zstd** | BSD-3-Clause | Compression |

### How It’s Built
*   **Reproducible builds**: Dependencies are strictly pinned using [vcpkg](https://github.com/microsoft/vcpkg) baselines. This means every build result should be identical, regardless of who or what runs the build.
