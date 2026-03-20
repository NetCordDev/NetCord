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
        version = globalJson.sdk.version;
        majorVersion = builtins.head (builtins.splitVersion version);
        dotnet = pkgs."dotnet-sdk_${majorVersion}";

        docfx = pkgs.buildDotnetGlobalTool {
          pname = "docfx";
          version = "2.78.3";
          nugetHash = "sha256-hLb6OmxqXOOxFaq/N+aZ0sAzEYjU0giX3c1SWQtKDbs=";
          dotnet-sdk = dotnet;
        };
      in
      {
        default = pkgs.mkShell {
          packages = [
            dotnet
          ];
        };

        docs = pkgs.mkShell {
          packages = [
            docfx
            dotnet
            pkgs.nodejs_25
          ];
        };
      }
    );
  };
}
