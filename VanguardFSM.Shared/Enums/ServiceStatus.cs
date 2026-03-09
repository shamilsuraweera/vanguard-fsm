namespace VanguardFSM.Shared.Enums;

public enum ServiceStatus
{
    Incoming = 0,    // Customer submitted
    Dispatching = 1, // Admin reviewing
    Dispatched, // Task assigned to a worker via SignalR
    Assigned = 2,    // Worker selected
    EnRoute = 3,     // Worker traveling
    OnSite = 4,      // Work started
    InProgress = 5, // Work ongoing
    Verification = 6,// Customer reviewing
    Finalizing = 7,  // Payment/Log processing
    Completed = 8    // Archived
}