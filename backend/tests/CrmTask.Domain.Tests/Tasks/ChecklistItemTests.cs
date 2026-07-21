using CrmTask.Domain.Tasks;
using FluentAssertions;
using Xunit;

namespace CrmTask.Domain.Tests.Tasks;

public class ChecklistItemTests
{
    [Fact]
    public void Create_TextBox_WithoutOptions_Succeeds()
    {
        var item = ChecklistItem.Create("توضیحات", ChecklistFieldType.TextBox, options: null);

        item.Id.Should().NotBeEmpty();
        item.Label.Should().Be("توضیحات");
        item.FieldType.Should().Be(ChecklistFieldType.TextBox);
        item.Options.Should().BeEmpty();
        item.Value.Should().BeNull();
    }

    [Theory]
    [InlineData(ChecklistFieldType.Dropdown)]
    [InlineData(ChecklistFieldType.ListBox)]
    public void Create_ChoiceField_WithoutOptions_Throws(ChecklistFieldType fieldType)
    {
        var act = () => ChecklistItem.Create("وضعیت", fieldType, options: null);

        act.Should().Throw<ArgumentException>().WithParameterName("options");
    }

    [Theory]
    [InlineData(ChecklistFieldType.Dropdown)]
    [InlineData(ChecklistFieldType.ListBox)]
    public void Create_ChoiceField_WithOptions_Succeeds(ChecklistFieldType fieldType)
    {
        var item = ChecklistItem.Create("وضعیت", fieldType, ["انجام‌شده", "در حال انجام"]);

        item.Options.Should().Equal("انجام‌شده", "در حال انجام");
    }

    [Fact]
    public void Create_WithMissingLabel_Throws()
    {
        var act = () => ChecklistItem.Create(string.Empty, ChecklistFieldType.TextBox, null);

        act.Should().Throw<ArgumentException>().WithParameterName("label");
    }

    [Fact]
    public void SetValue_ForDropdown_WithValueNotInOptions_Throws()
    {
        var item = ChecklistItem.Create("وضعیت", ChecklistFieldType.Dropdown, ["انجام‌شده", "در حال انجام"]);

        var act = () => item.SetValue("نامعتبر");

        act.Should().Throw<ArgumentException>().WithParameterName("value");
    }

    [Fact]
    public void SetValue_ForDropdown_WithValueInOptions_Succeeds()
    {
        var item = ChecklistItem.Create("وضعیت", ChecklistFieldType.Dropdown, ["انجام‌شده", "در حال انجام"]);

        item.SetValue("انجام‌شده");

        item.Value.Should().Be("انجام‌شده");
    }

    [Theory]
    [InlineData("true")]
    [InlineData("false")]
    public void SetValue_ForCheckbox_AcceptsTrueOrFalse(string value)
    {
        var item = ChecklistItem.Create("تایید شده؟", ChecklistFieldType.Checkbox, null);

        item.SetValue(value);

        item.Value.Should().Be(value);
    }

    [Fact]
    public void SetValue_ForCheckbox_WithNonBooleanValue_Throws()
    {
        var item = ChecklistItem.Create("تایید شده؟", ChecklistFieldType.Checkbox, null);

        var act = () => item.SetValue("شاید");

        act.Should().Throw<ArgumentException>().WithParameterName("value");
    }

    [Fact]
    public void SetValue_ForTextBox_AcceptsAnyText()
    {
        var item = ChecklistItem.Create("توضیحات", ChecklistFieldType.TextBox, null);

        item.SetValue("همه چیز مرتب است");

        item.Value.Should().Be("همه چیز مرتب است");
    }
}
