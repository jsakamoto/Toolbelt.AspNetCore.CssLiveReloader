namespace Toolbelt.AspNetCore.CssLiveReloader {

    const apiBase = '/Toolbelt.AspNetCore.CssLiveReloader/';
    const conn = new EventSource(apiBase + 'EventSource');

    const qq: { [key: string]: (null | number)[] } = {};

    let connectedOnce = false;
    let lastReloadedTime = new Date();

    conn.onopen = onConnected;

    conn.addEventListener('css-changed', (ev: any) => {
        lastReloadedTime = new Date();
        const url = ev.data;
        if (typeof (qq[url]) === 'undefined') qq[url] = [];
        if (qq[url].length < 5) {
            qq[url].push(null);
            if (qq[url].length === 1) reloadCSS(url);
        }
    });

    function getLinks(): HTMLLinkElement[] {
        return Array.from(document.head.querySelectorAll(`link[rel='stylesheet']`));
    }

    function onConnected(): void {
        const hrefs = getLinks().map(link => stripReloadToken(link.href));
        fetch(apiBase + 'WatchRequest', {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ connectedOnce, lastReloadedTime, hrefs })
        })
        if (connectedOnce === false) {
            lastReloadedTime = new Date();
            connectedOnce = true;
        }
    }

    function reloadCSS(url: string): void {
        const links = getLinks();
        for (const link of links) {

            if (stripReloadToken(link.href) === url) {

                const newLink = document.createElement('link');
                newLink.rel = 'stylesheet';
                let href = stripReloadToken(link.getAttribute('href')!);

                href += (href.indexOf('?') === -1 ? '?' : '&') + '136bb8a9-b749-47e9-92e7-8b46e4a4f657=' + (new Date().getTime());
                newLink.href = href;

                newLink.onload = () => {
                    link.remove();
                    const timeoutTimerId = qq[url].shift();
                    if (timeoutTimerId !== null) clearTimeout(timeoutTimerId);
                    if (qq[url].length > 0) reloadCSS(url);
                }
                link.insertAdjacentElement('beforebegin', newLink);

                qq[url][0] = setTimeout(() => {
                    newLink.remove();
                    qq[url].shift();
                    if (qq[url].length > 0) reloadCSS(url);
                }, 3000);

                return;
            }
        }

        qq[url].shift();
    }

    function stripReloadToken(url: string): string {
        return url.replace(/(\?|&)(136bb8a9-b749-47e9-92e7-8b46e4a4f657=\d+&?)/, '$1').replace(/(\?|&)?$/, '');
    }
}