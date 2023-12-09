let ligatures = localStorage.getItem("ligatures");

if (!ligatures)
    localStorage.setItem("ligatures", ligatures = "0");

document.documentElement.setAttribute("ligatures", ligatures);

function toggleLigatures() {
    const ligatures = localStorage.getItem("ligatures");
    const newLigatures = ligatures == "1" ? "0" : "1";

    localStorage.setItem("ligatures", newLigatures);
    document.documentElement.setAttribute("ligatures", newLigatures);
}

function removeSnowflakes() {
    const snowflakeContainer = document.getElementById('snow-container');
    snowflakeContainer.replaceChildren();
}

function generateSnowflakes() {
    const snowflakeContainer = document.getElementById('snow-container');

    for (let i = 0; i < 9; i++) {
        const snowflake = document.createElement('div');
        snowflake.className = 'bi bi-snow snowflake';
        snowflakeContainer.appendChild(snowflake);
    }
}

function toggleSnow() {
    const snow = localStorage.getItem("snow");
    let newSnow;
    if (snow == "1") {
        removeSnowflakes();
        newSnow = "0";
    }
    else {
        generateSnowflakes();
        newSnow = "1";
    }

    localStorage.setItem("snow", newSnow);
    document.documentElement.setAttribute("snow", newSnow);
}

const iconLinks = [
    {
        title: 'GitHub',
        icon: 'bi bi-github',
        href: 'https://github.com/KubaZ2/NetCord'
    },
    {
        title: 'Discord',
        icon: 'bi bi-discord',
        href: 'https://discord.gg/meaSHTGyUH'
    },
    {
        title: 'Toggle ligatures',
        icon: 'icon icon-ligatures',
        onclick: 'window.docfx.toggleLigatures()',
    }
];

const month = new Date().getMonth();
if (month == 11) {
    let snow = localStorage.getItem("snow");

    if (!snow)
        localStorage.setItem("snow", snow = "1");

    document.documentElement.setAttribute("snow", snow);

    if (snow == "1")
        generateSnowflakes();

    iconLinks.push(
        {
            title: 'Toggle snow',
            icon: 'bi icon-snow',
            onclick: 'window.docfx.toggleSnow()',
        });
}

export default {
    toggleLigatures,
    toggleSnow,
    iconLinks
};
