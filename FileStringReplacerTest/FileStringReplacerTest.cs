using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using FileStringReplacer;
using System.Collections.Generic;

namespace FileStringReplacerTest
{
    [TestClass]
    public class FileStringReplacerTest
    {
        public readonly string executingDirectory = System.Reflection.Assembly.GetExecutingAssembly().Location;
        public string TestFilesDirectory { get { return Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetParent(executingDirectory).FullName).FullName).FullName, "TestFiles"); } }

        [TestMethod]
        public void FixedIdentifierReplacementTest()
        {
            string identifier = "IDENTIFIER1";
            string fullTestFilePath = Path.Combine(TestFilesDirectory, "original_file_1.txt");
            string modifiedFilePath = Path.Combine(TestFilesDirectory, "modified_file_1.txt");

            FileStringReplacer.FileStringReplacer.ReplaceIdentifierInstances(fullTestFilePath, identifier, "REPLACEMENT1", modifiedFilePath);

            Assert.IsTrue(File.Exists(modifiedFilePath) && !File.ReadAllText(modifiedFilePath).Contains(identifier));
        }

        [TestMethod]
        public void MultipleFixedIdentifiersReplacementTest()
        {
            string[] identifiers = new string[2] { "IDENTIFIER1", "IDENTIFIER2" };
            string fullTestFilePath = Path.Combine(TestFilesDirectory, "original_file_2.txt");
            string modifiedFilePath = Path.Combine(TestFilesDirectory, "modified_file_2.txt");

            Func<FoundIdentifier, string> getReplacement = (FoundIdentifier _identifier) =>
            {
                switch (_identifier.Identifier)
                {
                    case "IDENTIFIER1":
                        return "REPLACEMENT1";
                    case "IDENTIFIER2":
                        return "REPLACEMENT2";
                    default:
                        return "DEFAULT";
                }
            };

            FileStringReplacer.FileStringReplacer.ReplaceIdentifierInstances(fullTestFilePath, identifiers, getReplacement, modifiedFilePath);

            string allText = File.ReadAllText(modifiedFilePath);

            Assert.IsTrue(File.Exists(modifiedFilePath) && !allText.Contains(identifiers[0]) && !allText.Contains(identifiers[1]));
        }

        [TestMethod]
        public void MultipleMethodGeneratedIdentifiersReplacementTest()
        {
            string[] identifiers = new string[2] { "IDENTIFIER1", "IDENTIFIER2" };
            string fullTestFilePath = Path.Combine(TestFilesDirectory, "original_file_3.txt");
            string modifiedFilePath = Path.Combine(TestFilesDirectory, "modified_file_3.txt");

            Func<string, IEnumerable<string>> getFullIdentifiers = (string line) =>
            {
                List<string> _identifiers = new List<string>();
                if (line.Contains(identifiers[0]))
                    _identifiers.Add(identifiers[0]);
                if (line.Contains(identifiers[1]))
                    _identifiers.Add(identifiers[1]);
                return _identifiers;
            };

            Func<FoundIdentifier, string> getReplacement = (FoundIdentifier _identifier) =>
            {
                switch (_identifier.Identifier)
                {
                    case "IDENTIFIER1":
                        return "REPLACEMENT1";
                    case "IDENTIFIER2":
                        return "REPLACEMENT2";
                    default:
                        return "DEFAULT";
                }
            };

            FileStringReplacer.FileStringReplacer.ReplaceIdentifierInstances(fullTestFilePath, getFullIdentifiers, getReplacement, modifiedFilePath);

            string allText = File.ReadAllText(modifiedFilePath);

            Assert.IsTrue(File.Exists(modifiedFilePath) && !allText.Contains(identifiers[0]) && !allText.Contains(identifiers[1]));
        }

        [TestMethod]
        public void DyamicMethodGeneratedIdenfiersTest()
        {
            //quite similar to the test above tbh, but wanted to make sure this would work perfectly in the situation it was actually designed for!
            string fullTestFilePath = Path.Combine(TestFilesDirectory, "original_file_4.txt");
            string modifiedFilePath = Path.Combine(TestFilesDirectory, "modified_file_4.txt");

            string leadingIdentifier = "<<<<", trailingIdentifier = ">>>>";

            Func<string, IEnumerable<string>> getFullIdentifiers = (string line) =>
            {
                List<string> _identifiers = new List<string>();
                if (line.Contains(leadingIdentifier) && line.Contains(trailingIdentifier))
                {
                    int startPoint = 0;
                    do
                    {
                        startPoint = line.IndexOf(leadingIdentifier, startPoint);
                        if (startPoint > -1)
                        {
                            int endPoint = line.IndexOf(trailingIdentifier, startPoint) + trailingIdentifier.Length;
                            string fullIdentifier = line.Substring(startPoint, endPoint - startPoint);
                            _identifiers.Add(fullIdentifier);
                            line = line.Replace(fullIdentifier, "");
                        }
                    } while (startPoint > -1);
                }

                return _identifiers;
            };

            Func<FoundIdentifier, string> getReplacement = (FoundIdentifier _identifier) =>
            {
                if (_identifier.Identifier.Contains("<<<<like this>>>>"))
                    return "(first replacement)";
                if (_identifier.Identifier.Contains("<<<<here is another>>>>"))
                    return "(second replacement)";
                if (_identifier.Identifier.Contains("<<<<hello>>>>"))
                    return "hi";
                if (_identifier.Identifier.Contains("<<<<people>>>>"))
                    return "peeps";
                else return "missing replacement";
            };

            FileStringReplacer.FileStringReplacer.ReplaceIdentifierInstances(fullTestFilePath, getFullIdentifiers, getReplacement, modifiedFilePath);

            string allText = File.ReadAllText(modifiedFilePath);

            Assert.IsTrue(File.Exists(modifiedFilePath) && !allText.Contains(leadingIdentifier) && !allText.Contains(trailingIdentifier));
        }
    }
}
