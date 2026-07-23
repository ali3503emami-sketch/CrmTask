using CrmTask.Domain.ReferenceData;
using FluentAssertions;
using Xunit;

namespace CrmTask.Domain.Tests.ReferenceData;

public class ReferenceListItemTests
{
    [Fact]
    public void Create_WithValidTitle_SetsProperties()
    {
        var item = ReferenceListItem.Create(ReferenceListKind.Position, "مسئول دفتر");

        item.Id.Should().NotBeEmpty();
        item.Kind.Should().Be(ReferenceListKind.Position);
        item.Title.Should().Be("مسئول دفتر");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithMissingTitle_Throws(string? title)
    {
        var act = () => ReferenceListItem.Create(ReferenceListKind.CustomerCategory, title!);

        act.Should().Throw<ArgumentException>().WithParameterName("title");
    }
}
