// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

import { html } from "lit-html";
import { Theme } from "./options";
import { options } from "./helper";

function setTheme(theme: Theme) {
  localStorage.setItem("theme", theme);
  if (theme === "auto") {
    document.documentElement.setAttribute(
      "data-bs-theme",
      window.matchMedia("(prefers-color-scheme: dark)").matches
        ? "dark"
        : "light",
    );
  } else {
    document.documentElement.setAttribute("data-bs-theme", theme);
  }
}

async function getDefaultTheme() {
  return (
    (localStorage.getItem("theme") as Theme) ||
    (await options()).defaultTheme ||
    "auto"
  );
}

export async function initTheme() {
  setTheme(await getDefaultTheme());
}

export function onThemeChange(callback: (theme: "light" | "dark") => void) {
  return new MutationObserver(() => callback(getTheme())).observe(
    document.documentElement,
    { attributes: true, attributeFilter: ["data-bs-theme"] },
  );
}

export function getTheme(): "light" | "dark" {
  return document.documentElement.getAttribute("data-bs-theme") as
    | "light"
    | "dark";
}

export async function themePicker(refresh: () => void) {
  const theme = getTheme();
  const icon = theme === "light" ? "sun" : "moon";

  return html`<button
    class="btn border-0 icon-tooltip"
    aria-label="Toggle theme"
    @click="${toggleTheme}"
    tooltip="Toggle theme"
    ><i class="bi bi-${icon}"></i
  ></button>`;

  function toggleTheme(e) {
    e.preventDefault();
    setTheme(getTheme() == "light" ? "dark" : "light");
    refresh();
  }
}
