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

        nuget = pkgs.runCommand "unwrapped-nuget" {} ''
          mkdir -p $out/bin
          install -m 755 ${pkgs.nuget}/lib/Nuget/nuget.exe $out/bin/nuget
        '';

        dotnetRoot = "${dotnet.unwrapped}/share/dotnet";

        darwinPackages = pkgs.lib.optionals pkgs.stdenv.hostPlatform.isDarwin (with pkgs; [
          libiconv
          apple-sdk
        ]);
      in
      {
        default = pkgs.mkShell {
          packages = [
            dotnet
          ];

          DOTNET_ROOT = dotnetRoot;
        };

        natives = pkgs.mkShell.override {
          stdenv = if pkgs.clangStdenv.isLinux then
            pkgs.clangStdenv
          else pkgs.clangStdenv.override (old: {
            hostPlatform = old.hostPlatform // { darwinMinVersion = "12.0"; };
            targetPlatform = old.targetPlatform // { darwinMinVersion = "12.0"; };
          });
        } ({
          packages = [
            dotnet
            nuget
            pkgs.mono
            pkgs.vcpkg-tool
            pkgs.cmake
            pkgs.ninja
            pkgs.pkg-config
            pkgs.autoconf
            pkgs.autoconf-archive
            pkgs.automake
            pkgs.libtool
            pkgs.git
            pkgs.zip
            pkgs.unzip
            pkgs.perl
            pkgs.curl
          ] ++ darwinPackages;

          DOTNET_ROOT = dotnetRoot;

          VCPKG_FORCE_SYSTEM_BINARIES = "1";

          shellHook = ''
            export VCPKG_ROOT="$PWD/Natives/NetCord.Natives/vcpkg"

            ln -sf ${pkgs.vcpkg-tool}/bin/vcpkg $VCPKG_ROOT/vcpkg
          '';
        } // pkgs.lib.optionalAttrs pkgs.stdenv.hostPlatform.isDarwin {
          SDKROOT = "${pkgs.apple-sdk}/SDKs/MacOSX.sdk";
          MACOSX_DEPLOYMENT_TARGET = "12.0";
          VCPKG_ENV_PASSTHROUGH = "MACOSX_DEPLOYMENT_TARGET,SDKROOT";
        });

        natives-musl = pkgs.mkShell.override {
          stdenv = pkgs.pkgsMusl.clangStdenv;
        } ({
          packages = [
            dotnet
            nuget
            pkgs.mono
            pkgs.vcpkg-tool
            pkgs.cmake
            pkgs.ninja
            pkgs.pkg-config
            pkgs.autoconf
            pkgs.autoconf-archive
            pkgs.automake
            pkgs.libtool
            pkgs.git
            pkgs.zip
            pkgs.unzip
            pkgs.perl
            pkgs.curl
          ] ++ darwinPackages;

          DOTNET_ROOT = dotnetRoot;

          VCPKG_FORCE_SYSTEM_BINARIES = "1";

          shellHook = ''
            export VCPKG_ROOT="$PWD/Natives/NetCord.Natives/vcpkg"

            ln -sf ${pkgs.vcpkg-tool}/bin/vcpkg $VCPKG_ROOT/vcpkg
          '';
        } // pkgs.lib.optionalAttrs pkgs.stdenv.hostPlatform.isDarwin {
          SDKROOT = "${pkgs.apple-sdk}/SDKs/MacOSX.sdk";
          MACOSX_DEPLOYMENT_TARGET = "12.0";
          VCPKG_ENV_PASSTHROUGH = "MACOSX_DEPLOYMENT_TARGET,SDKROOT";
        });

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
