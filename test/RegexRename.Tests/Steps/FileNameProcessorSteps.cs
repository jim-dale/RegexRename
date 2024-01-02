namespace RegexRename.Tests.Steps;

using System.Linq;
using FluentAssertions.Execution;
using RegexRename.Support;

[Binding]
public sealed class FileNameProcessorSteps
{
    private FileNameProcessor? sut;
    private readonly List<string> output = [];

    [Given(@"I have a file name processor with the following parameters:")]
    public void GivenIHaveAFileNameProcessorWithTheFollowingParameters(Table table)
    {
        ArgumentNullException.ThrowIfNull(table);

        using (var scope = new AssertionScope())
        {
            table.Rows.Count.Should().BeGreaterThan(0);
            table.Rows[0].Count.Should().Be(3);
        }

        var inputPattern = table.Rows[0][0];
        var outputPattern = table.Rows[0][1];
        var variableDefinitions = table.Rows[0][2];

        inputPattern.Should().NotBeNullOrWhiteSpace();
        outputPattern.Should().NotBeNullOrWhiteSpace();
        variableDefinitions.Should().NotBeNullOrWhiteSpace();

        var variables = CommandLine.ParseVariableDeclarations(variableDefinitions);
        variables.Should().NotBeNull();

        this.sut = new FileNameProcessor(inputPattern, outputPattern, variables!);
    }

    [When(@"I transform the following file names:")]
    public void WhenITransformTheFollowingFileNames(Table table)
    {
        ArgumentNullException.ThrowIfNull(table);

        this.sut.Should().NotBeNull();
        using (var scope = new AssertionScope())
        {
            table.Rows.Count.Should().BeGreaterThan(0);
            table.Rows[0].Count.Should().Be(1);
        }

        foreach (var row in table.Rows)
        {
            var output = this.sut!.TransformFileName(row[0]);
            output.Should().NotBeNull();
            this.output.Add(output!);
        }
    }

    [Then(@"I expect the following output:")]
    public void ThenIExpectTheFollowingOutput(Table table)
    {
        ArgumentNullException.ThrowIfNull(table);

        using (var scope = new AssertionScope())
        {
            table.Rows.Count.Should().BeGreaterThan(0);
            table.Rows[0].Count.Should().Be(1);
        }

        var expected = table.Rows.Select(r => r[0]).ToList();

        expected.Sort();
        this.output.Sort();

        expected.Should().BeEquivalentTo(this.output);
    }
}
