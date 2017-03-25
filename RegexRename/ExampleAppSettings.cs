
namespace RegexRename
{
    using System.Collections.Generic;

    public static class ExampleAppSettings
    {
        public static AppSettings Example = new AppSettings
        {
            DefaultProfileName = "BankStatement",
            Profiles = new List<RenameProfile>
            {
                new RenameProfile
                {
                    Name = "BankStatement",
                    BaseFolder = "C:\\Bank\\Statements",
                    FileSearchPattern = "*.pdf",
                    Recurse = false,
                    SourceRegex = "(?i)^Statement--(?<SortCode>[\\d]{6})-(?<AccountNumber>[\\d]*)--(?<FromDay>[\\d]{1,2})-(?<FromMonth>[\\d]{1,2})-(?<FromYear>[\\d]{4})-(?<ToDay>[\\d]{1,2})-(?<ToMonth>[\\d]{1,2})-(?<ToYear>[\\d]{4}).pdf$",
                    DestRegex = "{FromYear:D4}-{FromMonth:D2}-{FromDay:D2} - {ToYear:D4}-{ToMonth:D2}-{ToDay:D2} {SortCode}-{AccountNumber:D8}.pdf",
                    VariableTypes = new Dictionary<string,string>
                    {
                        { "SortCode", "System.String" },
                        { "AccountNumber", "System.Int32" },
                        { "FromDay", "System.Int32" },
                        { "FromMonth", "System.Int32" },
                        { "FromYear", "System.Int32" },
                        { "ToDay","System.Int32" },
                        { "ToMonth", "System.Int32" },
                        { "ToYear", "System.Int32" }
                    }
                },
                new RenameProfile
                {
                    Name = "BankCOI",
                    BaseFolder = "C:\\Bank\\Certificate of Interest",
                    FileSearchPattern = "*.pdf",
                    Recurse = false,
                    SourceRegex = "(?i)^COI-(?<SortCode>[\\d]{6})-(?<AccountNumber>[\\d]*)--(?<Day>[\\d]{1,2})-(?<Month>[\\d]{1,2})-(?<Year>[\\d]{4}).pdf$",
                    DestRegex = "{Year:D4}-{Month:D2}-{Day:D2} COI {SortCode}-{AccountNumber:D8}.pdf",
                    VariableTypes = new Dictionary<string,string>
                    {
                        { "SortCode", "System.String" },
                        { "AccountNumber", "System.Int32" },
                        { "Day", "System.Int32" },
                        { "Month", "System.Int32" },
                        { "Year", "System.Int32" }
                    }
                }
            }
        };
    }
}
