"use strict";
var Toolbelt;
(function (Toolbelt) {
    var AspNetCore;
    (function (AspNetCore) {
        var CssLiveReloader;
        (function (CssLiveReloader) {
            const apiBase = '/Toolbelt.AspNetCore.CssLiveReloader/';
            const conn = new EventSource(apiBase + 'EventSource');
            const qq = {};
            conn.onopen = onConnected;
            conn.addEventListener('css-changed', (ev) => {
                const url = ev.data;
                if (typeof (qq[url]) === 'undefined')
                    qq[url] = [];
                if (qq[url].length < 5) {
                    qq[url].push(null);
                    if (qq[url].length === 1)
                        reloadCSS(url);
                }
            });
            function getLinks() {
                return Array.from(document.head.querySelectorAll(`link[rel='stylesheet']`));
            }
            function onConnected() {
                const hrefs = getLinks().map(link => stripReloadToken(link.href));
                fetch(apiBase + 'WatchRequest', {
                    method: 'POST',
                    headers: {
                        'Accept': 'application/json',
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(hrefs)
                });
            }
            function reloadCSS(url) {
                const links = getLinks();
                for (const link of links) {
                    if (stripReloadToken(link.href) === url) {
                        const newLink = document.createElement('link');
                        newLink.rel = 'stylesheet';
                        let href = stripReloadToken(link.getAttribute('href'));
                        href += (href.indexOf('?') === -1 ? '?' : '&') + '136bb8a9-b749-47e9-92e7-8b46e4a4f657=' + (new Date().getTime());
                        newLink.href = href;
                        newLink.onload = () => {
                            link.remove();
                            const timeoutTimerId = qq[url].shift();
                            if (timeoutTimerId !== null)
                                clearTimeout(timeoutTimerId);
                            if (qq[url].length > 0)
                                reloadCSS(url);
                        };
                        link.insertAdjacentElement('beforebegin', newLink);
                        qq[url][0] = setTimeout(() => {
                            newLink.remove();
                            qq[url].shift();
                            if (qq[url].length > 0)
                                reloadCSS(url);
                        }, 3000);
                        return;
                    }
                }
                qq[url].shift();
            }
            function stripReloadToken(url) {
                return url.replace(/(\?|&)(136bb8a9-b749-47e9-92e7-8b46e4a4f657=\d+&?)/, '$1').replace(/(\?|&)?$/, '');
            }
        })(CssLiveReloader = AspNetCore.CssLiveReloader || (AspNetCore.CssLiveReloader = {}));
    })(AspNetCore = Toolbelt.AspNetCore || (Toolbelt.AspNetCore = {}));
})(Toolbelt || (Toolbelt = {}));
