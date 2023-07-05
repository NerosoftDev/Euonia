using Nerosoft.Euonia.Mapping.Tests.Models;

namespace Nerosoft.Euonia.Mapping.Tests;

public class Test2
{
    [Fact]
    public void Test()
    {
        var source = new SourceObject2
        {
            Id = 1,
            FirstName = "FirstName",
            LastName = "LastName",
            Address = "New York",
            Age = 20
        };
        
        var destination = TypeAdapter.ProjectedAs<DestinationObject2>(source);
        Assert.Equal(source.Id, destination.Id);
        Assert.Equal($"{source.FirstName} {source.LastName}", destination.FullName);
        Assert.Equal(source.Address, destination.Address);
        Assert.Equal(source.Age, destination.Age);
    }
}