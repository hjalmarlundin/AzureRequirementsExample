namespace tests;

using FluentAssertions;
using source;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var sut = new MySimpleClass();

        var amount = sut.GetTotalAmount();

        amount.Should().Be(0);
    }
}