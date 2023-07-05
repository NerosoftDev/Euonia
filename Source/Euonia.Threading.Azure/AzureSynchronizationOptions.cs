namespace Nerosoft.Euonia.Threading.Azure;

public sealed record AzureSynchronizationOptions(TimeoutValue Duration, TimeoutValue RenewalCadence, TimeoutValue MinBusyWaitSleepTime, TimeoutValue MaxBusyWaitSleepTime);