"use strict";

import { build as _build } from "esbuild";
import { copy } from "esbuild-plugin-copy";
import { sassPlugin } from "esbuild-sass-plugin";
import { cpSync, rmSync } from "fs";
import { join } from "path";

const dist = "./templates";

const template = "./templates-src/NetCord";

const loader = {
  ".eot": "file",
  ".svg": "file",
  ".ttf": "file",
  ".woff": "file",
  ".woff2": "file",
};

build();

export async function build() {
  await buildNetCordTemplate();
  copyToDist();
}

async function buildNetCordTemplate() {
  const config = {
    bundle: true,
    format: "esm",
    splitting: true,
    minify: true,
    sourcemap: true,
    outExtension: {
      ".css": ".min.css",
      ".js": ".min.js",
    },
    outdir: `${template}/public`,
    entryPoints: [
      `${template}/src/docfx.ts`,
      `${template}/src/search-worker.ts`,
    ],
    external: ["./main.js"],
    plugins: [
      sassPlugin(),
      copy({
        assets: {
          from: [`${template}/src/*.js`],
          to: ["./"],
        },
      }),
      copy({
        assets: {
          from: [`${template}/src/*.css`],
          to: ["./"],
        },
      }),
      copy({
        assets: {
          from: [`${template}/fonts/*`],
          to: ["./"],
        },
      }),
    ],
    loader,
  };

  await _build(config);
}

function copyToDist() {
  rmSync(dist, { recursive: true, force: true });
  cpSync(template, join(dist, "NetCord"), {
    recursive: true,
    overwrite: true,
    filter,
  });

  function filter(src) {
    const segments = src.split(/[/\\]/);
    return (
      !segments.includes("node_modules") &&
      !segments.includes("package-lock.json") &&
      !segments.includes("src")
    );
  }
}
