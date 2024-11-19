let ligatures = localStorage.getItem("ligatures");

if (!ligatures) localStorage.setItem("ligatures", (ligatures = "0"));

document.documentElement.setAttribute("ligatures", ligatures);

function toggleLigatures() {
  const ligatures = localStorage.getItem("ligatures");
  const newLigatures = ligatures == "1" ? "0" : "1";

  localStorage.setItem("ligatures", newLigatures);
  document.documentElement.setAttribute("ligatures", newLigatures);
}

function removeSnowflakes() {
  const snowflakeContainer = document.getElementById("snow-container");
  snowflakeContainer.replaceChildren();
}

function generateSnowflakes() {
  const snowflakeContainer = document.getElementById("snow-container");

  const delays = [];
  for (let i = 0; i < 9; i++) delays.push(i);

  for (let i = 0; i < 9; i++) {
    const snowflake = document.createElement("div");
    snowflake.className = "bi bi-snow snowflake";
    snowflake.style.left = `${(i + 1) * 10}%`;
    snowflake.style.animationDelay = `${delays.splice(Math.floor(Math.random() * delays.length), 1)[0]}s`;
    snowflakeContainer.appendChild(snowflake);
  }
}

function toggleSnow() {
  const snow = localStorage.getItem("snow");
  let newSnow;
  if (snow == "1") {
    removeSnowflakes();
    newSnow = "0";
  } else {
    generateSnowflakes();
    newSnow = "1";
  }

  localStorage.setItem("snow", newSnow);
  document.documentElement.setAttribute("snow", newSnow);
}

let icons = [
  {
    icon: "bi bi-github",
    tooltip: "Star on GitHub",
    href: "https://github.com/NetCordDev/NetCord",
  },
  {
    icon: "bi bi-discord",
    tooltip: "Join us on Discord",
    href: "https://discord.gg/meaSHTGyUH",
  },
  {
    icon: "icon icon-ligatures",
    tooltip: "Toggle ligatures",
    onclick: toggleLigatures,
  },
];

const month = new Date().getMonth();
if (month == 11) {
  let snow = localStorage.getItem("snow");

  if (!snow) localStorage.setItem("snow", (snow = "1"));

  document.documentElement.setAttribute("snow", snow);

  if (snow == "1") generateSnowflakes();

  const snowIcon = {
    icon: "bi icon-snow",
    tooltip: "Toggle snow",
    onclick: toggleSnow,
  };
  icons = [snowIcon, ...icons];
}

export default {
  icons,
};
