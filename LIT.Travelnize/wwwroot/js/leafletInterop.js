window.travelnizeLeaflet = {
    maps: {},
    /**
     * options (optional):
     * {
     *   zoomControl: false,              // Standard: true
     *   attributionControl: true/false,  // Standard: true
     *   disableInteraction: true/false   // Wenn true: macht Karte "statisch"
     * }
     */
    init: function (id, lat, lon, zoom, tileUrl, attribution, markerColor, options) {
        if (!id) return;
        if (this.maps[id]) return;
        const el = document.getElementById(id);
        if (!el) return;

        options = options || {};

        const interactionDisabled = options.disableInteraction === true;

        const map = L.map(id, {
            zoomControl: options.zoomControl !== false,          // default true
            attributionControl: options.attributionControl !== false, // default true
            dragging: !interactionDisabled,
            scrollWheelZoom: interactionDisabled ? false : true,
            doubleClickZoom: !interactionDisabled,
            boxZoom: !interactionDisabled,
            keyboard: !interactionDisabled,
            tap: !interactionDisabled,
            touchZoom: interactionDisabled ? false : true
        });

        if (options.zoomControl === false && map.zoomControl) {
            // (Nur falls Leaflet trotzdem einen Control angelegt hat)
            map.zoomControl.remove();
        }

        const layer = L.tileLayer(
            tileUrl || 'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png',
            {
                maxZoom: 19,
                attribution: attribution || '&copy; <a href="https://www.openstreetmap.org/copyright">OSM</a>'
            });
        layer.addTo(map);

        const marker = L.marker([lat, lon], markerColor ? { icon: this._coloredIcon(markerColor) } : undefined)
            .addTo(map);

        map.setView([lat, lon], zoom);

        this.maps[id] = { map, marker };

        // Größe nach Render sicherstellen
        setTimeout(() => map.invalidateSize(), 50);
    },
    update: function (id, lat, lon, zoom, markerColor) {
        const ctx = this.maps[id];
        if (!ctx) return;
        if (lat != null && lon != null) {
            ctx.marker.setLatLng([lat, lon]);
            if (markerColor) {
                ctx.marker.setIcon(this._coloredIcon(markerColor));
            }
            if (zoom != null) {
                ctx.map.setView([lat, lon], zoom);
            } else {
                ctx.map.setView([lat, lon]);
            }
        }
    },
    dispose: function (id) {
        const ctx = this.maps[id];
        if (!ctx) return;
        ctx.map.remove();
        delete this.maps[id];
    },
    _coloredIcon: function (color) {
        return L.divIcon({
            className: 'travelnize-marker',
            html: `<div class="tnz-marker-pin" style="background:${color};"></div>`,
            iconSize: [20, 20],
            iconAnchor: [10, 10]
        });
    }
};