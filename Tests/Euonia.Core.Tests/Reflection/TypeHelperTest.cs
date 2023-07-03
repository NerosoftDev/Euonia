using Nerosoft.Euonia.Reflection;

namespace Nerosoft.Euonia.Core.Tests.Reflection;

public class TypeHelperTest
{
    [Fact]
    public void TestCoerceValueStringToInt32()
    {
        var result = TypeHelper.CoerceValue(typeof(int), typeof(string), "1024");
        Assert.Equal(1024, result);
    }

    [Fact]
    public void TestCoerceValueStringToInt32_Negative()
    {
        var result = TypeHelper.CoerceValue(typeof(int), typeof(string), "-1024");
        Assert.Equal(-1024, result);
    }

    [Fact]
    public void TestCoerceValueStringToInt64()
    {
        var result = TypeHelper.CoerceValue(typeof(long), typeof(string), "9223372036854775807");
        Assert.Equal(9223372036854775807, result);
    }

    [Fact]
    public void TestCoerceValueStringToInt64_Negative()
    {
        var result = TypeHelper.CoerceValue(typeof(long), typeof(string), "-9223372036854775807");
        Assert.Equal(-9223372036854775807, result);
    }

    [Fact]
    public void TestCoerceValueStringToInt64_ValueOutOfRange()
    {
        Assert.Throws<ArgumentException>(() => TypeHelper.CoerceValue(typeof(long), typeof(string), "9223372036854775810"));
    }
}
