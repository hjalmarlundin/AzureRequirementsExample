namespace tests;

using FluentAssertions;
using source;

public class UnitTest2
{
    [Fact]
    [Requirement(8910)]
    public void Test1()
    {
        var sut = new MySimpleClass();

        var amount = sut.GetTotalAmount();

        amount.Should().Be(0);
    }

    [Fact]
    public void Test2()
    {
        var sut = new MySimpleClass();

        var amount = sut.GetTotalAmount();

        amount.Should().Be(0);
    }
}