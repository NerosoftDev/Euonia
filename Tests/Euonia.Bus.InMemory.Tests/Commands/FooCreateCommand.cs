using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nerosoft.Euonia.Bus.InMemory.Tests.Commands;

//[Channel("foo.create")]
public class FooCreateCommand : IQueue<int>
{
}
