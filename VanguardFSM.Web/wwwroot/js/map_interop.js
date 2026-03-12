// Development: Global map variable
var map;

window.mapFunctions = {
    // Functionality: Create the map instance and set initial view
    initialize: function (elementId, lat, lng) {
        if (map) {
            map.remove(); // Cleanup existing map to prevent "already initialized" errors
        }
        
        map = L.map(elementId).setView([lat, lng], 13);

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '© OpenStreetMap contributors'
        }).addTo(map);
    },

    addWorkerMarker: function (lat, lng, name, workerId, dotNetHelper) {
        if (!map) {
            console.error("Map not initialized. Call initialize first.");
            return;
        }

        let marker = L.marker([lat, lng]).addTo(map);

        // Functionality: Button calls back to C# 'HandleAssignTask' method
        let popupContent = `
            <div style="text-align:center;">
                <strong>${name}</strong><br/>
                <button class="btn btn-sm btn-success mt-2" 
                        onclick="window.dotNetRef.invokeMethodAsync('HandleAssignTask', ${workerId})">
                    Assign Task
                </button>
            </div>
        `;
        // Store the dotNetHelper globally so the popup can access it reliably
        window.dotNetRef = dotNetHelper;

        marker.bindPopup(popupContent);
    }
};