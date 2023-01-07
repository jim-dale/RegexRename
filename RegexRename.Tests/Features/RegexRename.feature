Feature: Calculator

Scenario: Get a list of files matching the pattern *.json in a folder
	When the 'test-data' folder is searched for files matching '*.json'
	Then the files list should be:
		| file                   |
		| test-data\\file 1.json |
		| test-data\\file 2.json |

Scenario: Get a list of files matching the pattern *.json in a folder and all sub-folders
	When the 'test-data' folder is searched for files matching '**/*.json'
	Then the files list should be:
		| file                              |
		| test-data\\file 1.json            |
		| test-data\\file 2.json            |
		| test-data\\subfolder\\file 1.json |
		| test-data\\subfolder\\file 2.json |
