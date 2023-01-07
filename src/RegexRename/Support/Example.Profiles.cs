namespace RegexRename.Support;

using RegexRename.Models;

public static class Example
{
#pragma warning disable CA2211 // Non-constant fields should not be visible
    public static ProfileConfiguration ProfileConfiguration = new()
#pragma warning restore CA2211 // Non-constant fields should not be visible
    {
        DefaultProfile = "BankStatement",
        Items =
        {
            {
                "BankStatement",
                new Profile
                {
                    Folder = @"C:\Bank\Statements",
                    FileSearchPattern = "**/*.pdf",
                    InputPattern = @"(?i)^Statement--(?<SortCode>[\d]{6})-(?<AccountNumber>[\d]*)--(?<FromDay>[\d]{1,2})-(?<FromMonth>[\d]{1,2})-(?<FromYear>[\d]{4})-(?<ToDay>[\d]{1,2})-(?<ToMonth>[\d]{1,2})-(?<ToYear>[\d]{4}).pdf$",
                    OutputPattern = "{FromYear:D4}-{FromMonth:D2}-{FromDay:D2} - {ToYear:D4}-{ToMonth:D2}-{ToDay:D2} {SortCode}-{AccountNumber:D8}.pdf",
                    VariableTypes =
                    {
                        { "SortCode", typeof(string).FullName! },
                        { "AccountNumber", typeof(int).FullName! },
                        { "FromDay", typeof(int).FullName! },
                        { "FromMonth", typeof(int).FullName ! },
                        { "FromYear", typeof(int).FullName! },
                        { "ToDay", typeof(int).FullName! },
                        { "ToMonth", typeof(int).FullName! },
                        { "ToYear", typeof(int).FullName!}
                    }
                }
            },
            {
                "BankCOI",
                new Profile
                {
                    Folder = @"C:\Bank\Certificate of Interest",
                    FileSearchPattern = "*.pdf",
                    InputPattern = @"(?i)^COI-(?<SortCode>[\d]{6})-(?<AccountNumber>[\d]*)--(?<Day>[\d]{1,2})-(?<Month>[\d]{1,2})-(?<Year>[\d]{4}).pdf$",
                    OutputPattern = "{Year:D4}-{Month:D2}-{Day:D2} COI {SortCode}-{AccountNumber:D8}.pdf",
                    VariableTypes =
                    {
                        { "SortCode", typeof(string).FullName! },
                        { "AccountNumber", typeof(int).FullName! },
                        { "Day", typeof(int).FullName ! },
                        { "Month", typeof(int).FullName ! },
                        { "Year", typeof(int).FullName ! }
                    }
                }
            }
        }
    };
}
