# Installing Native Dependencies

This is a hidden guide that is not visible in the guides index! If you are reading this, you are probably looking for information on how to install native dependencies for HTTP interactions or voice.

## HTTP Interactions

For HTTP interactions, [Libsodium](https://doc.libsodium.org/installation) is required.

## Voice

For voice:
- [Libdave](https://github.com/discord/libdave) is required.
- [Libsodium](https://doc.libsodium.org/installation) **is not generally** required, but it **is highly recommended for production bots**. It is caused by the fact that generally @NetCord.Gateway.Voice.Encryption.Aes256GcmRtpSizeEncryption, which does not require Libsodium, is supported by Discord and is used by default. However, there is a small chance that Discord will not support this encryption mode for your connection. In this case, @NetCord.Gateway.Voice.Encryption.XChaCha20Poly1305RtpSizeEncryption, which does require Libsodium, is used by default.
- [Opus](https://opus-codec.org/downloads) is only required when you are using `Opus` prefixed classes.

## Installation

### [Dynamic Linking](#tab/dynamic)

For dynamic linking, you can install Libsodium for the most popular platforms by referencing the [official Libsodium NuGet package](https://www.nuget.org/packages/libsodium).

### Manual or System-wide Installation

#### Windows

For dynamic linking on Windows, you need to use the dynamic link libraries (`libdave`, `libsodium`, and/or `opus`). Here's how to set it up:
- Download or build the dynamic link libraries (`libdave`, `libsodium`, and/or `opus`) compatible with your development environment.

- Place these files in the runtime directory of your application. This is the folder where your application's executable is located.

#### Linux and MacOS

Dynamic linking on Linux and MacOS involves using shared libraries (`libdave`, `libsodium`, and/or `opus`). You can install them using your system's package manager if available or follow these steps to install them manually:
- Download or build the shared libraries (`libdave`, `libsodium`, and/or `opus`) that are compatible with your development environment.

- Place these files in the runtime directory of your application, which is the folder where your application's executable is located.

### [Static Linking](#tab/static)

> [!NOTE]
> Static linking requires [Native AOT](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot) compilation.

#### Windows

When using static linking on Windows, you need to link to the static libraries (`libdave`, `libsodium`, and/or `opus`). Here are the steps to set up static linking in your application:
- Download or build the static libraries (`libdave`, `libsodium`, and/or `opus`) compatible with your development environment.
- Link these libraries in your project settings. Ensure that you specify the correct paths to these libraries:
  ```xml
  <ItemGroup>
    <NativeLibrary Include="C:\path\to\libdave" Condition="$(RuntimeIdentifier.StartsWith('win'))" />
    <DirectPInvoke Include="libdave" />

    <NativeLibrary Include="C:\path\to\libsodium" Condition="$(RuntimeIdentifier.StartsWith('win'))" />
    <DirectPInvoke Include="libsodium" />
    
    <NativeLibrary Include="C:\path\to\opus" Condition="$(RuntimeIdentifier.StartsWith('win'))" />
    <DirectPInvoke Include="opus" />
  </ItemGroup>
  ```

You don't need to place any of these files in the runtime directory, as static linking embeds the library code directly into your application, eliminating the need for separate files.

#### Linux and MacOS

Static linking on Linux and MacOS involves linking your application with the static libraries (`libdave`, `libsodium`, and/or `opus`). You can install them using your system's package manager if available or download or build the static libraries (`libdave`, `libsodium`, and/or `opus`) compatible with your development environment manually.

Link these libraries in your project settings. Make sure you specify the correct paths to these libraries:
```xml
<ItemGroup>
  <NativeLibrary Include="/path/to/libdave" Condition="!$(RuntimeIdentifier.StartsWith('win'))" />
  <DirectPInvoke Include="libdave" />

  <NativeLibrary Include="/path/to/libsodium" Condition="!$(RuntimeIdentifier.StartsWith('win'))" />
  <DirectPInvoke Include="libsodium" />

  <NativeLibrary Include="/path/to/opus" Condition="!$(RuntimeIdentifier.StartsWith('win'))" />
  <DirectPInvoke Include="opus" />
</ItemGroup>
```

Since you're statically linking, you won't need to place any of these files in the runtime directory. The necessary code from the libraries will be included directly in your application.

***

#### Installation External Links

| Library   | Installation Link                           |
|-----------|---------------------------------------------|
| Libdave   | https://github.com/discord/libdave/releases |
| Libsodium | https://doc.libsodium.org/installation      |
| Opus      | https://opus-codec.org/downloads            |
