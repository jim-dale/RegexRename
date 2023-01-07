
namespace RegexRename.Tests.Steps;

using System.Linq;
using RegexRename.Support;

[Binding]
public sealed class RegexRenameSteps
{
    private readonly TestContext testContext;
    private InputSource? sut;

    public RegexRenameSteps(TestContext testContext)
    {
        this.testContext = testContext;
    }

    [When(@"the '([^']*)' folder is searched for files matching '([^']*)'")]
    public void WhenTheFolderIsSearchedForFilesMatching(string folderPath, string searchPattern)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(folderPath));
        ArgumentException.ThrowIfNullOrEmpty(nameof(searchPattern));

        this.sut = new InputSource(folderPath, searchPattern);
    }

    [Then(@"the files list should be:")]
    public void ThenTheFilesListShouldBe(Table table)
    {
        this.sut.Should().NotBeNull();
        table.Rows.Should().NotBeNull();
        table.RowCount.Should().BeGreaterThan(0);
        table.Rows[0].Count.Should().Be(1);

        var actual = this.sut!.GetFiles().Order();

        var expected = table.Rows.Select(row => Path.Combine(this.testContext.TestDeploymentDir, row[0])).Order();

        expected.Should().BeEquivalentTo(actual);
    }
}