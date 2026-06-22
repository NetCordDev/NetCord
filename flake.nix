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
      in
      {
        default = pkgs.mkShell {
          packages = [
            dotnet
          ];

          DOTNET_ROOT = dotnetRoot;
        };

        natives = pkgs.mkShell.override {
          stdenv = pkgs.stdenv.override (old: {
            hostPlatform = old.hostPlatform // { darwinMinVersion = "12.0"; };
            targetPlatform = old.targetPlatform // { darwinMinVersion = "12.0"; };
          });
        } ({
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

          VCPKG_FORCE_SYSTEM_BINARIES = "1";
          CC = "clang";
          CXX = "clang++";
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
