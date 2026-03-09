// Functionality: This bridges Leaflet (JS) and Blazor (C#)
window.mapFunctions = {
    addWorkerMarker: function (lat, lng, name, workerId, dotNetHelper) {
        if (typeof map === 'undefined') {
            console.error("Map not initialized");
            return;
        }

        let marker = L.marker([lat, lng]).addTo(map);

        // This button calls back to your C# 'HandleAssignTask' method
        let popupContent = `
            <div style="text-align:center;">
                <strong>${name}</strong><br/>
                <button class="btn btn-sm btn-success mt-2" 
                        onclick="dotNetHelper.invokeMethodAsync('HandleAssignTask', ${workerId})">
                    Assign Task
                </button>
            </div>
        `;

        marker.bindPopup(popupContent);
    }
};