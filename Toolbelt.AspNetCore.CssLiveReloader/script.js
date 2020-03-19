"use strict";
var Toolbelt;
(function (Toolbelt) {
    var AspNetCore;
    (function (AspNetCore) {
        var CssLiveReloader;
        (function (CssLiveReloader) {
            const conn = new EventSource('/Toolbelt.AspNetCore.CssLiveReloader/EventSource');
            const qq = {};
            conn.addEventListener('css-changed', (ev) => {
                const url = ev.data;
                if (typeof (qq[url]) === 'undefined')
                    qq[url] = [];
                qq[url].push(null);
                if (qq[url].length === 1)
                    reloadCSS(url);
            });
            function reloadCSS(url) {
                const links = document.head.querySelectorAll(`link[rel='stylesheet']`);
                for (let i = 0; i < links.length; i++) {
                    const link = links[i];
                    if (stripReloadToken(link.href) === url) {
                        const newLink = document.createElement('link');
                        newLink.rel = 'stylesheet';
                        let href = stripReloadToken(link.getAttribute('href'));
                        href += (href.indexOf('?') === -1 ? '?' : '&') + '136bb8a9-b749-47e9-92e7-8b46e4a4f657=' + (new Date().getTime());
                        newLink.href = href;
                        newLink.onload = () => {
                            link.remove();
                            qq[url].pop();
                            if (qq[url].length > 0)
                                reloadCSS(url);
                        };
                        link.insertAdjacentElement('beforebegin', newLink);
                        return;
                    }
                }
                qq[url].pop();
            }
            function stripReloadToken(url) {
                return url.replace(/(\?|&)(136bb8a9-b749-47e9-92e7-8b46e4a4f657=\d+&?)/, '$1').replace(/(\?|&)?$/, '');
            }
        })(CssLiveReloader = AspNetCore.CssLiveReloader || (AspNetCore.CssLiveReloader = {}));
    })(AspNetCore = Toolbelt.AspNetCore || (Toolbelt.AspNetCore = {}));
})(Toolbelt || (Toolbelt = {}));
