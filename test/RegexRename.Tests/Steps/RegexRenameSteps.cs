
namespace RegexRename.Tests.Steps;

using System.Linq;
using FluentAssertions.Execution;
using RegexRename.Support;
using RegexRename.Tests.Support;

[Binding]
public sealed class RegexRenameSteps
{
    private string folderPath = string.Empty;
    private InputSource? sut;

    [When(@"the '([^']*)' folder is searched for files matching '([^']*)'")]
    public void WhenTheFolderIsSearchedForFilesMatching(string folderPath, string searchPattern)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(folderPath));
        ArgumentException.ThrowIfNullOrEmpty(nameof(searchPattern));

        this.folderPath = folderPath;
        this.sut = new InputSource(folderPath, searchPattern);
    }

    [Then(@"the files list should be:")]
    public void ThenTheFilesListShouldBe(Table table)
    {
        using (AssertionScope scope = new())
        {
            this.sut.Should().NotBeNull();
            table.Rows.Should().NotBeNull();
            table.RowCount.Should().BeGreaterThan(0);
            table.Rows[0].Count.Should().Be(1);
        }

        IEnumerable<string> actual = this.sut!.GetFiles();

        IEnumerable<string> expected = table.Rows.Select(row => Path.Combine(Directory.GetCurrentDirectory(), this.folderPath, row[0]));

        expected.Should().BeEquivalentTo(actual);
    }

    [When(@"the '([^']*)' folder is processed for files matching '([^']*)'")]
    public async Task WhenTheFolderIsProcessedForFilesMatching(string folderPath, string searchPattern)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(folderPath));
        ArgumentException.ThrowIfNullOrEmpty(nameof(searchPattern));

        var args = new string[]
        {
            "-f", folderPath,
            "-s", searchPattern,
            "-w"
        };

        using ConsoleOutput o = new();
        await RegexRename.Program.Main(args);

        string text = o.GetOuput();
        Console.WriteLine(text);
    }

    [Then(@"the output should include:")]
    public void ThenTheOutputShouldInclude(Table table)
    {
        throw new PendingStepException();
    }
}
