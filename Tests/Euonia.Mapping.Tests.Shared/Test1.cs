using Nerosoft.Euonia.Mapping.Tests.Models;

namespace Nerosoft.Euonia.Mapping.Tests;

public class Test1
{
    [Fact]
    public void SimpleTest()
    {
        var source = new SourceObject1
        {
            Id = 1,
            Name = "Name", Address = "New York",
            Age = 20
        };

        var destination = TypeAdapter.ProjectedAs<DestinationObject1>(source);
        Assert.Equal(source.Id, destination.Id);
        Assert.Equal(source.Name, destination.Name);
        Assert.Equal(source.Address, destination.Address);
        Assert.Equal(source.Age, destination.Age);
    }
}