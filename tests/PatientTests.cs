namespace tests;

using FluentAssertions;
using source;

public class PatientTests
{
    [Theory]
    [Requirement(3038)]
    [InlineData(80, false)]
    [InlineData(10, false)]
    [InlineData(81, true)]
    public void OnlyPatientsAboveACertainThreshold(int vitalSign, bool expectedResult)
    {
        var sut = new Patient(vitalSign);

        var result = sut.CanUseDevice();

        result.Should().Be(expectedResult);
    }
}