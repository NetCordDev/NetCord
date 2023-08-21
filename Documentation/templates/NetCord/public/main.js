let ligatures = localStorage.getItem("ligatures");

if (!ligatures)
    localStorage.setItem("ligatures", ligatures = "0");

document.documentElement.setAttribute("ligatures", ligatures);

function changeLigatures() {
    const ligatures = localStorage.getItem("ligatures");
    const newLigatures = ligatures == "1" ? "0" : "1";

    localStorage.setItem("ligatures", newLigatures);
    document.documentElement.setAttribute("ligatures", newLigatures);
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
        onclick: 'window.docfx.changeLigatures()',
    }
];

export default {
    changeLigatures,
    iconLinks
};
