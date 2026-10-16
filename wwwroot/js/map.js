// Leaflet + OpenStreetMap Interop for Cebu Dental Tourism Hub
// Keyless, zero billing, key coordinates Metro Cebu [10.3157, 123.8854]

window.cebuMaps = window.cebuMaps || {};

// Muted look without a tile API key: desaturate OSM tiles, keep labels legible.
function ensureMutedTileStyle() {
    if (document.getElementById('cebu-tile-style')) return;
    const style = document.createElement('style');
    style.id = 'cebu-tile-style';
    style.textContent = '.cebu-leaflet-map-wrapper .leaflet-tile-pane { filter: grayscale(1) contrast(1.05); }';
    document.head.appendChild(style);
}

// Ensure Leaflet styles and scripts are loaded dynamically if not in head
function ensureLeafletLoaded() {
    return new Promise((resolve, reject) => {
        if (window.L) {
            resolve();
            return;
        }

        if (!document.getElementById('leaflet-css')) {
            const link = document.createElement('link');
            link.id = 'leaflet-css';
            link.rel = 'stylesheet';
            link.href = 'https://unpkg.com/leaflet@1.9.4/dist/leaflet.css';
            link.crossOrigin = '';
            document.head.appendChild(link);
        }

        if (!document.getElementById('leaflet-js')) {
            const script = document.createElement('script');
            script.id = 'leaflet-js';
            script.src = 'https://unpkg.com/leaflet@1.9.4/dist/leaflet.js';
            script.crossOrigin = '';
            script.onload = () => resolve();
            script.onerror = () => reject(new Error('Failed to load Leaflet script'));
            document.head.appendChild(script);
        } else {
            const existingScript = document.getElementById('leaflet-js');
            existingScript.addEventListener('load', () => resolve());
        }
    });
}

function createCustomPinIcon(isHighlighted) {
    const bgColor = isHighlighted ? '#e76f51' : '#00a896';
    const borderColor = '#ffffff';
    const svgHtml = `
        <div style="position: relative; width: 26px; height: 32px; filter: drop-shadow(0 2px 4px rgba(0,0,0,0.35)); transition: transform 0.2s ease;">
            <svg viewBox="0 0 36 44" width="26" height="32" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M18 0C8.058 0 0 8.058 0 18c0 13.5 18 26 18 26s18-12.5 18-26C36 8.058 27.942 0 18 0z" fill="${bgColor}" stroke="${borderColor}" stroke-width="2"/>
                <!-- Tooth / Medical cross icon -->
                <path d="M14 11h8c1.5 0 2.5 1 2.5 2.5v1c0 1.5-1 2.5-2.5 2.5h-8C12.5 17 11.5 16 11.5 14.5v-1C11.5 12 12.5 11 14 11z" fill="#ffffff" opacity="0.9"/>
                <path d="M13 16c0 4 1.5 8 3 8s1.5-3 2-3 0.5 3 2 3 3-4 3-8H13z" fill="#ffffff"/>
            </svg>
        </div>
    `;

    return window.L.divIcon({
        className: 'cebu-custom-pin',
        html: svgHtml,
        iconSize: [26, 32],
        iconAnchor: [13, 32],
        popupAnchor: [0, -30]
    });
}

window.cebuMapInterop = {
    initMap: async function (elementId, dotNetRef, pins) {
        await ensureLeafletLoaded();
        ensureMutedTileStyle();

        const container = document.getElementById(elementId);
        if (!container) return;

        // Clean up previous instance if any
        if (window.cebuMaps[elementId] && window.cebuMaps[elementId].map) {
            try {
                window.cebuMaps[elementId].map.remove();
            } catch (e) {
                console.warn('Error removing old map:', e);
            }
        }

        // Metro Cebu center
        const defaultCenter = [10.3157, 123.8854];
        const defaultZoom = 12;

        const map = window.L.map(elementId, {
            center: defaultCenter,
            zoom: defaultZoom,
            zoomControl: true,
            scrollWheelZoom: true
        });

        // Muted OSM tiles via CSS grayscale (keyless — CARTO now requires an API key)
        window.L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 19,
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright" target="_blank">OpenStreetMap</a> contributors'
        }).addTo(map);

        window.cebuMaps[elementId] = {
            map: map,
            dotNetRef: dotNetRef,
            markers: {},
            activePinId: null
        };

        this.updatePins(elementId, pins);
        
        // Invalidate size after render to fix any container sizing glitches
        setTimeout(() => {
            if (map) map.invalidateSize();
        }, 250);
    },

    updatePins: function (elementId, pins) {
        const state = window.cebuMaps[elementId];
        if (!state || !state.map) return;

        // Remove old markers
        for (const id in state.markers) {
            state.markers[id].remove();
        }
        state.markers = {};

        if (!pins || pins.length === 0) return;

        const bounds = window.L.latLngBounds();

        pins.forEach(pin => {
            if (!pin.latitude || !pin.longitude) return;

            const isSelected = state.activePinId === pin.id;
            const icon = createCustomPinIcon(isSelected);

            const marker = window.L.marker([pin.latitude, pin.longitude], { icon: icon }).addTo(state.map);

            const popupContent = `
                <div style="font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; min-width: 180px; padding: 2px;">
                    <div style="font-weight: 700; color: #0f172a; font-size: 14px; margin-bottom: 2px;">${pin.name}</div>
                    <div style="color: #64748b; font-size: 12px; margin-bottom: 8px;">
                        <span style="display: inline-block; width: 8px; height: 8px; border-radius: 50%; background-color: #00a896; margin-right: 4px;"></span>
                        ${pin.area}, ${pin.city}
                    </div>
                    <div style="border-top: 1px solid #e2e8f0; padding-top: 6px; display: flex; justify-content: space-between; align-items: center;">
                        <a href="/clinics/${pin.id}" style="display: inline-block; background-color: #00627d; color: #ffffff; padding: 4px 10px; border-radius: 4px; font-size: 11px; font-weight: 600; text-decoration: none;">
                            View Clinic & Pricing &rarr;
                        </a>
                    </div>
                </div>
            `;

            marker.bindPopup(popupContent);

            marker.on('click', () => {
                if (state.dotNetRef) {
                    state.dotNetRef.invokeMethodAsync('OnPinSelectedFromJs', pin.id);
                }
            });

            state.markers[pin.id] = marker;
            bounds.extend([pin.latitude, pin.longitude]);
        });

        if (pins.length > 1 && bounds.isValid()) {
            state.map.fitBounds(bounds, { padding: [30, 30], maxZoom: 14 });
        } else if (pins.length === 1) {
            state.map.setView([pins[0].latitude, pins[0].longitude], 14);
        }
    },

    highlightPin: function (elementId, pinId) {
        const state = window.cebuMaps[elementId];
        if (!state || !state.map) return;

        state.activePinId = pinId;

        // Reset all icons and highlight the selected one
        for (const id in state.markers) {
            const marker = state.markers[id];
            const isTarget = parseInt(id) === parseInt(pinId);
            marker.setIcon(createCustomPinIcon(isTarget));
            if (isTarget) {
                state.map.panTo(marker.getLatLng(), { animate: true, duration: 0.6 });
                marker.openPopup();
            }
        }
    },

    destroyMap: function (elementId) {
        const state = window.cebuMaps[elementId];
        if (state && state.map) {
            try {
                state.map.remove();
            } catch (e) {
                console.warn('Error destroying map:', e);
            }
            delete window.cebuMaps[elementId];
        }
    }
};
