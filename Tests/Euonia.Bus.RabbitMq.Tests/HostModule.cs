using Nerosoft.Euonia.Bus.RabbitMq;
using Nerosoft.Euonia.Modularity;

namespace Nerosoft.Euonia.Bus.Tests;

[DependsOn(typeof(ServiceBusModule), typeof(RabbitMqBusModule))]
public class HostModule : ModuleContextBase
{
}