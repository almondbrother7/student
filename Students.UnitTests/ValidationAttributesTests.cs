using System.ComponentModel.DataAnnotations;
using Students.Models.Validation;
using Xunit;

public class ValidationAttributesTests
{
    [Fact] public void PastDate_Accepts_PastDate() =>
        Assert.True(new PastDateAttribute().IsValid(DateTime.Today.AddDays(-1)));

    [Fact] public void PastDate_Rejects_TodayOrFuture()
    {
        var attr = new PastDateAttribute();
        Assert.False(attr.IsValid(DateTime.Today));
        Assert.False(attr.IsValid(DateTime.Today.AddDays(1)));
    }

    [Theory]
    [InlineData("K", true)]
    [InlineData("k", true)]
    [InlineData("1", true)]
    [InlineData("12", true)]
    [InlineData("0", false)]
    [InlineData("13", false)]
    [InlineData("X", false)]
    public void GradeAttribute_Validates(string input, bool expected) =>
        Assert.Equal(expected, new GradeAttribute().IsValid(input));

    [Theory]
    [InlineData("321-555-0101", true)]
    [InlineData("(321) 555-0101", true)]
    [InlineData("+1 321 555 0101", true)]
    [InlineData("3215550101", true)]
    [InlineData("555-0101", false)]
    public void UsPhone_Validates(string input, bool expected) =>
        Assert.Equal(expected, new UsPhoneAttribute().IsValid(input));
}
