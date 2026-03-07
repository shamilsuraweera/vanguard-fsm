namespace VanguardFSM.Shared.Enums;

public enum ServiceStatus
{
    Incoming = 0,    // Customer submitted
    Dispatching = 1, // Admin reviewing
    Assigned = 2,    // Worker selected
    EnRoute = 3,     // Worker traveling
    OnSite = 4,      // Work started
    Verification = 5,// Customer reviewing
    Finalizing = 6,  // Payment/Log processing
    Completed = 7    // Archived
}