using Nerosoft.Euonia.Mapping.Tests.Models;

namespace Nerosoft.Euonia.Mapping.Tests
{
	public class Test3
	{
		[Fact]
		public void Test()
		{
			var source = new SourceObject3
			{
				FirstName = "FirstName",
				LastName = "LastName"
			};

			var destination = TypeAdapter.ProjectedAs<DestinationObject3>(source);
			Assert.Equal($"{source.FirstName} {source.LastName}", destination.FullName);
		}
	}
}