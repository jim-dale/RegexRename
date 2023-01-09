Feature: RegexRename

Scenario: Get a list of files matching the pattern *.json in a given folder
	When the 'test-data' folder is searched for files matching '*.json'
	Then the files list should be:
		| file                   |
		| test-data\\file 1.json |
		| test-data\\file 2.json |

Scenario: Get a list of files matching the pattern *.json in a given folder and all sub-folders
	When the 'test-data' folder is searched for files matching '**/*.json'
	Then the files list should be:
		| file                              |
		| test-data\\file 1.json            |
		| test-data\\file 2.json            |
		| test-data\\subfolder\\file 1.json |
		| test-data\\subfolder\\file 2.json |

Scenario: File processor
	Given I have a file name processor with the following parameters:
		| Input Pattern            | Output Pattern                  | Variables               |
		| file (?<FileNumber>\\d+) | {FileNumber:D4} - new file name | FileNumber,System.Int32 |
	When I transform the following file names:
		| Input       |
		| file 1.json |
	Then I expect the following output:
		| Output               |
		| 0001 - new file name |
