namespace Nerosoft.Euonia.Threading.Azure;

/// <summary>
/// Options for <see cref="AzureLockProvider"/>.
/// </summary>
/// <param name="Duration"></param>
/// <param name="RenewalCadence"></param>
/// <param name="MinBusyWaitSleepTime"></param>
/// <param name="MaxBusyWaitSleepTime"></param>
public sealed record AzureSynchronizationOptions(TimeoutValue Duration, TimeoutValue RenewalCadence, TimeoutValue MinBusyWaitSleepTime, TimeoutValue MaxBusyWaitSleepTime);