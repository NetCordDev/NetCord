# Voice

## Installation

# [Dynamic Linking](#tab/dynamic)

### Windows

For dynamic linking on Windows, you need to use the dynamic link libraries (`libsodium.dll` and optionally `opus.dll`). Here's how to set it up:
- Download or build the dynamic link libraries (`libsodium.dll` and optionally `opus.dll`) compatible with your development environment.
- Place these DLL files in the runtime directory of your application. This is the folder where your application's executable is located.

### Linux and MacOS

Dynamic linking on Linux and MacOS involves using shared libraries (`libsodium.so` and optionally `opus.so`). **You can install them using your system's package manager if available** or follow these steps to install them manually:
- Download or build the shared libraries (`libsodium.so` and optionally `opus.so`) that are compatible with your development environment.
- Place these `.so` files in the runtime directory of your application, which is the folder where your application's executable is located.

# [Static Linking](#tab/static)

> [!NOTE]
> Static linking requires [Native AOT](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot) compilation.

### Windows

When using static linking on Windows, you need to link to the static libraries (`libsodium.lib` and optionally `opus.lib`). Here are the steps to set up static linking in your application:
- Download or build the static libraries (`libsodium.lib` and optionally `opus.lib`) compatible with your development environment.
- Link these libraries in your project settings. Ensure that you specify the correct paths to these libraries:
  ```xml
  <ItemGroup>
    <NativeLibrary Include="C:\path\to\libsodium.lib" Condition="$(RuntimeIdentifier.StartsWith('win'))" />
    <DirectPInvoke Include="libsodium" />
    
    <NativeLibrary Include="C:\path\to\opus.lib" Condition="$(RuntimeIdentifier.StartsWith('win'))" />
    <DirectPInvoke Include="opus" />
  </ItemGroup>
  ```

You don't need to place any DLL files in the runtime directory, as static linking embeds the library code directly into your application, eliminating the need for separate DLL files.

### Linux and MacOS

Static linking on Linux and MacOS involves linking your application with the static libraries (`libsodium.a` and optionally `opus.a`). **You can install them using your system's package manager if available** or download or build the static libraries (`libsodium.a` and optionally `opus.a`) compatible with your development environment manually.

Link these libraries in your project settings. Make sure you specify the correct paths to these libraries:
```xml
<ItemGroup>
  <NativeLibrary Include="/path/to/libsodium.a" Condition="!$(RuntimeIdentifier.StartsWith('win'))" />
  <DirectPInvoke Include="libsodium" />

  <NativeLibrary Include="/path/to/opus.a" Condition="!$(RuntimeIdentifier.StartsWith('win'))" />
  <DirectPInvoke Include="opus" />
</ItemGroup>
```

Since you're statically linking, you won't need to place any `.so` files in the runtime directory. The necessary code from the libraries will be included directly in your application.

***

#### Installation External Links

| Library   | Installation Link                      |
|-----------|----------------------------------------|
| libsodium | https://doc.libsodium.org/installation |
| opus      | https://opus-codec.org/downloads       |

## Example Usage

> [!NOTE]
> In the following examples streams and @NetCord.Gateway.Voice.VoiceClient instances are not disposed because they should be stored somewhere and disposed later.

### Sending Voice
[!code-cs[VoiceModule.cs](Voice/VoiceModule.cs#L11-L103)]

### Receiving Voice
[!code-cs[VoiceModule.cs](Voice/VoiceModule.cs#L104-L147)]