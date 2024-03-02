"use strict";

import { execSync } from "child_process";
import { build as buildTemplates } from "./build-templates.js";

build();

async function build() {
  await buildTemplates();
  execSync(`docfx ${process.argv.slice(2).join(" ")}`, { stdio: "inherit" });
}
