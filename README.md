## This project is full created by Dengisan Ramadanoski

## Links
## Website: https://dengisan.nl/
## Twitter: https://twitter.com/syotexgames/
## Instagram: https://twitter.com/syotexgames_

## Invite link (This is a dutch server)
## https://discord.gg/fmJgEqv

# file-string-replacer
.NET library for replacing strings within files

## What it does
Replaces specified "identifiers" in a file with a replacement string.
Multiple identifiers and replacements can be specified, or alternatively you can pass a func which returns a replacement when invoked with an identifier.

## What you might use it for
- Generating a file from a template based on predefined rules
- Generating a file from a template based on user input
- Removing certain pieces of information from a file

## How it works
The code goes through the specified file a line at a time, checking each time if it contains the specified identifier, or any of the identifiers returned by a Func parameter

## Test it yourself
The solution includes a unit test project. Inside *\FileStringReplacerTest\TestFiles\* there are a number of original files, which will get modified by the tests. You can look at the originals, and then look at the *modified_file_x.txt* files after the tests have been run to see the replacement.

## How to include in your project
Options:
- Download my pre-compiled dll from this repo's [releases page](https://dengisan.nl/astramgg-github/) - include in your solution folder somewhere, and reference it in your project
- Clone as [subtree](https://blogs.atlassian.com/2013/05/alternatives-to-git-submodule-git-subtree/) into the git repo for your project - again, reference my project in your solution
- Download source code as zip and place in your project file - reference my project in your solution

## How to use

All methods have an optional newFile parameter at the end. If not passed, the function will just overwrite the same file it reads from, else it will write to the specified file

#### Simple use, single identifier, single replacement

    string originalFile = "C:\whatever_template.txt", newFile = "C:\whatever.txt";
    string identifier = "REPLACEME", replacement = "REPLACEMENT";
    
    FileStringReplacer.ReplaceIdentifierInstances(originalFile, identifier, replacement, newFile);

#### Using Func to determine replacement

    string originalFile = "C:\whatever_template.txt", newFile = "C:\whatever.txt";
    string identifier = "REPLACEME";
    
    //takes a FoundIdentifier instances with details about the identifier found
    Func<FoundIdentifier, string> getReplacement = (FoundIdentifier _identifier) => {
        string replacement = "";
        //do whatever to determine the value of 'replacement' based on the details in _identifier
        return replacement;
    }
    
    FileStringReplacer.ReplaceIdentifierInstances(originalFile, identifier, getReplacement, newFile);
    
#### Passing in multiple identifiers as IEnumerable\<string>

    string originalFile = "C:\whatever_template.txt", newFile = "C:\whatever.txt";
    IEnumerable<string> identifiers = new string[2] { "REPLACEME", "REPLACEMETOO", "ANDME!" };
    
    //takes a FoundIdentifier instances with details about the identifier found
    Func<FoundIdentifier, string> getReplacement = (FoundIdentifier _identifier) => {
        string replacement = "";
        //do whatever to determine the value of 'replacement' based on the details in _identifier
        return replacement;
    }
    
    FileStringReplacer.ReplaceIdentifierInstances(originalFile, identifiers, getReplacement, newFile);
    
#### Custom Func to extract all identifiers from each line
Pass in a function which takes a line, and returns an IEnumerable\<string> with all the identifiers found in that line

    string originalFile = "C:\whatever_template.txt", newFile = "C:\whatever.txt";
    
    //takes an entire line as a string
    Func<string, IEnumerable<string>> getFullIdentifiers = (string line) => {
        IEnumerable<string> _identifiers = new List<string>();
        //extract all the identifiers from 'line' however you need to
        return _identifiers;
    }
    
    //takes a FoundIdentifier instances with details about the identifier found
    Func<FoundIdentifier, string> getReplacement = (FoundIdentifier _identifier) => {
        string replacement = "";
        //do whatever to determine the value of 'replacement' based on the details in _identifier
        return replacement;
    }
    
    FileStringReplacer.ReplaceIdentifierInstances(originalFile, getFullIdentifiers, getReplacement, newFile);
    
## FoundIdentifier class
Contains information about an identifier found within a line

    public class FoundIdentifier{
        public string Identifier { get; private set; }
        public int IndexOf { get; set; }
        public string LineFound { get; private set; }
        public int LineFoundIdx { get; private set; }
    }
	
	#Everything is customized made by Dengisan Ramadanoski
	# Do not claim any rights from this
	# Any help, join the discord server. Do not forget (It's a dutch server)
