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
          version = "2.81.0";
          nugetHash = "sha256-9AQN5UUVVgchNLx8wqXB0GKZGi3sqf0hEaXISGCtPD4=";
          dotnet-sdk = dotnet;
        };

        dotnetRoot = "${dotnet.unwrapped}/share/dotnet";
      in
      {
        default = pkgs.mkShell {
          packages = [
            dotnet
          ];

          DOTNET_ROOT = dotnetRoot;
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
