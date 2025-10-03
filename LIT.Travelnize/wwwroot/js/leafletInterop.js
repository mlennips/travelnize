window.travelnizeLeaflet = {
    maps: {},
    init: function (id, lat, lon, zoom, tileUrl, attribution, markerColor, options, dotNetRef) {
        if (!id) return;
        if (this.maps[id]) return;
        const el = document.getElementById(id);
        if (!el) return;

        options = options || {};
        const interactionDisabled = options.disableInteraction === true;

        const map = L.map(id, {
            zoomControl: options.zoomControl !== false,
            attributionControl: options.attributionControl !== false,
            dragging: !interactionDisabled,
            scrollWheelZoom: interactionDisabled ? false : true,
            doubleClickZoom: !interactionDisabled,
            boxZoom: !interactionDisabled,
            keyboard: !interactionDisabled,
            tap: !interactionDisabled,
            touchZoom: interactionDisabled ? false : true
        });

        if (options.zoomControl === false && map.zoomControl) {
            map.zoomControl.remove();
        }

        const layer = L.tileLayer(
            tileUrl || 'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png',
            {
                maxZoom: 19,
                attribution: attribution || '&copy; <a href="https://www.openstreetmap.org/copyright">OSM</a>'
            });
        layer.addTo(map);

        const marker = L.marker([lat, lon], markerColor ? { icon: this._coloredIcon(markerColor), draggable: true } : { draggable: true })
            .addTo(map);

        map.setView([lat, lon], zoom);

        // Callback für Marker Drag
        if (dotNetRef) {
            marker.on('dragend', function (e) {
                var pos = e.target.getLatLng();
                dotNetRef.invokeMethodAsync('OnMapCoordinateChanged', pos.lat, pos.lng);
            });
            map.on('click', function (e) {
                marker.setLatLng(e.latlng);
                dotNetRef.invokeMethodAsync('OnMapCoordinateChanged', e.latlng.lat, e.latlng.lng);
            });
        }

        this.maps[id] = { map, marker };

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