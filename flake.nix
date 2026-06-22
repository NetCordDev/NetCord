{
  description = ".NET env";

  inputs = {
    nixpkgs.url = "github:nixos/nixpkgs?ref=nixos-unstable";
  };

  outputs = { nixpkgs, ... }:
  let
    supportedArch = [
      "x86_64-linux"
      "aarch64-linux"
      "x86_64-darwin"
      "aarch64-darwin"
    ];

    forAllArch = nixpkgs.lib.genAttrs supportedArch;
  in
  {
    devShells = forAllArch (arch:
      let
        pkgs = nixpkgs.legacyPackages.${arch};

        globalJson = builtins.fromJSON (builtins.readFile ./global.json);
        version = builtins.splitVersion globalJson.sdk.version;

        major = builtins.elemAt version 0;
        minor = builtins.elemAt version 1;

        dotnet = pkgs.dotnetCorePackages."sdk_${major}_${minor}-bin";

        docfx = pkgs.buildDotnetGlobalTool {
          pname = "docfx";
          version = "2.78.3";
          nugetHash = "sha256-hLb6OmxqXOOxFaq/N+aZ0sAzEYjU0giX3c1SWQtKDbs=";
          dotnet-sdk = dotnet;
        };

        dotnetRoot = "${dotnet.unwrapped}/share/dotnet";

        darwinPackages = pkgs.lib.optionals pkgs.stdenv.hostPlatform.isDarwin (with pkgs; [
          libiconv
          apple-sdk
        ]);

        sdkRoot = pkgs.lib.optionalString pkgs.stdenv.hostPlatform.isDarwin
          "${pkgs.apple-sdk}/SDKs/MacOSX.sdk";
      in
      {
        default = pkgs.mkShell {
          packages = [
            dotnet
          ];

          DOTNET_ROOT = dotnetRoot;
        };

        natives = pkgs.mkShell {
          packages = [
            dotnet
            pkgs.cmake
            pkgs.ninja
            pkgs.pkg-config
            pkgs.autoconf
            pkgs.autoconf-archive
            pkgs.automake
            pkgs.libtool
          ] ++ darwinPackages;

          DOTNET_ROOT = dotnetRoot;

          MACOSX_DEPLOYMENT_TARGET = "12.0";

          # 1. Force vcpkg to use Nix's CMake and Ninja instead of Homebrew/System tools
          VCPKG_FORCE_SYSTEM_BINARIES = "1";

          # 2. Explicitly point to the Nix Clang wrapper
          CC = "clang";
          CXX = "clang++";

          shellHook = pkgs.lib.optionalString pkgs.stdenv.hostPlatform.isDarwin ''
            # Provide SDKROOT for MSBuild and Apple tools that expect it.
            # We omit manual LDFLAGS/CFLAGS because Nix's Clang wrapper handles them automatically.
            export SDKROOT="${sdkRoot}"
          '';
        };

        docs = pkgs.mkShell {
          packages = [
            docfx
            dotnet
            pkgs.nodejs_26
          ];

          DOTNET_ROOT = dotnetRoot;
        };
      }
    );
  };
}
