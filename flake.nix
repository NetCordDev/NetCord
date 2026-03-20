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
        in
        {
            default = pkgs.mkShell {
              packages = [
                dotnet
              ];
            };
        }
    );
  };
}
