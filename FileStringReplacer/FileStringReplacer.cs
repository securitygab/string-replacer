using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Benji7425.FileStringManipulator.Replacer
{
    public static class FileStringReplacer
    {
        /// <summary>
        /// Replaces all instances of 'identifier' found in 'file' with 'replacement'
        /// </summary>
        /// <param name="file">File to read from</param>
        /// <param name="identifier">Identifier to replace</param>
        /// <param name="replacement">String to replace identifier with</param>
        /// <param name="newFile">Optional new file to write to (if blank, will overwrite 'file')</param>
        public static void ReplaceIdentifierInstances(string file, string identifier, string replacement, string newFile = null)
        {
            //just always return replacement in getReplacement, as it is fixed
            ReplaceIdentifierInstances(file, identifier, (FoundIdentifier x) => { return replacement; }, newFile);
        }

        /// <summary>
        /// Replaces all instances of 'identifier' found in 'file' with the result of 'getReplacement(identifier)'
        /// </summary>
        /// <param name="file">File to read from</param>
        /// <param name="identifier">Identifier to replace</param>
        /// <param name="getReplacement">Func that takes the identifier found, and returns its replacement</param>
        /// <param name="newFile">Optional new file to write to (if blank, will overwrite 'file')</param>
        public static void ReplaceIdentifierInstances(string file, string identifier, Func<string, string> getReplacement, string newFile = null)
        {
            //create a Func<FoundIdentifier, string> from our passed Func<string, string> to pass again
            Func<FoundIdentifier, string> _getReplacement = (FoundIdentifier _identifier) => { return getReplacement(_identifier.Identifier); };
            ReplaceIdentifierInstances(file, identifier, _getReplacement, newFile);
        }

        /// <summary>
        /// Replaces all instances of 'identifier' found in 'file' with the result of 'getReplacement'
        /// </summary>
        /// <param name="file">File to read from</param>
        /// <param name="identifier">Identifier to replace</param>
        /// <param name="getReplacement">Func that takes a FoundIdentifier, and returns the identifier's replacement</param>
        /// <param name="newFile">Optional new file to write to (if blank, will overwrite 'file')</param>
        public static void ReplaceIdentifierInstances(string file, string identifier, Func<FoundIdentifier, string> getReplacement, string newFile = null)
        {
            //create an IEnumerable with our one identifier to pass in
            ReplaceIdentifierInstances(file, Enumerable.Repeat(identifier, 1), getReplacement, newFile);
        }

        /// <summary>
        /// Replaces all instances of each identifier in 'identifiers' found in 'file' with the result of 'getReplacement'
        /// </summary>
        /// <param name="file">File to read from</param>
        /// <param name="identifiers">All the identifiers to replace</param>
        /// <param name="getReplacement">Func that takes a FoundIdentifier, and returns the identifier's replacement</param>
        /// <param name="newFile">Optional new file to write to (if blank, will overwrite 'file')</param>
        public static void ReplaceIdentifierInstances(string file, IEnumerable<string> identifiers, Func<FoundIdentifier, string> getReplacement, string newFile = null)
        {
            //just always return identifiers in getIdentifiers, as they are fixed
            ReplaceIdentifierInstances(file, (string x) => { return identifiers; }, getReplacement, newFile);
        }

        /// <summary>
        /// Replaces all each identifier returned by 'getFullIdentifiers(line)' in 'file' with the result of 'getReplacement'
        /// </summary>
        /// <param name="file">File to read from</param>
        /// <param name="getFullIdentifiers">Func that takes a line, and returns all the identifiers within the lin</param>
        /// <param name="getReplacement">Func that takes a FoundIdentifier, and returns the identifier's replacement</param>
        /// <param name="newFile">Optional new file to write to (if blank, will overwrite 'file')</param>
        public static void ReplaceIdentifierInstances(string file, Func<string, IEnumerable<string>> getFullIdentifiers, Func<FoundIdentifier, string> getReplacement, string newFile = null)
        {
            //all other overloads call this overload, which (at the time of writing this comment at least) is the only overload that actually calls DoReplacement
            DoReplacement(file, newFile ?? file, getFullIdentifiers, getReplacement);
        }

        /// <summary>
        /// For each line in 'readFile', replaces all identifiers supplied by 'getFullIdentifiers(line)' with 'getReplacement(identifier, identifierStartIdx, line)' and writes to 'writeFile'
        /// </summary>
        /// <param name="readFile">The file to read all the lines from</param>
        /// <param name="writeFile">The file to write the modified lines back into</param>
        /// <param name="getFullIdentifiers">Func that takes a line, and returns all the identifiers within that line</param>
        /// <param name="getReplacement">Func that a FoundIdentifier, and returns its replacement</param>
        private static void DoReplacement(string readFile, string writeFile, Func<string, IEnumerable<string>> getFullIdentifiers, Func<FoundIdentifier, string> getReplacement)
        {
            //make sure the file we are writing to exists
            if (!File.Exists(writeFile))
                File.Create(writeFile).Close();

            using (StreamReader reader = new StreamReader(readFile))
            using (StreamWriter writer = new StreamWriter(writeFile))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    int lineCount = 0;

                    //the caller should provide a function which will return all the full identifiers to be replaced in this line, when provided with a line
                    IEnumerable<string> fullIdentifiers = getFullIdentifiers(line);

                    if (fullIdentifiers.Count() > 0)
                    {
                        StringBuilder stringBuilder = new StringBuilder(line);

                        foreach (string thisIdentifier in fullIdentifiers)
                        {
                            //use a do while for each identifier as there may be multiple instances within the same line
                            int replaceStartPoint = 0;
                            do
                            {
                                replaceStartPoint = stringBuilder.IndexOf(thisIdentifier, replaceStartPoint);

                                if (replaceStartPoint > -1)
                                {
                                    int replaceEndPoint = replaceStartPoint + thisIdentifier.Length;
                                    stringBuilder.Remove(replaceStartPoint, replaceEndPoint - replaceStartPoint);

                                    //get the replacement by invoking the Func the caller passed
                                    string replacement = getReplacement(new FoundIdentifier(thisIdentifier, replaceStartPoint, line, lineCount));
                                    stringBuilder.Insert(replaceStartPoint, replacement);
                                }
                            } while (replaceStartPoint > -1);
                        }

                        //when we have done all our replacements, serialize the line back out to a string
                        line = stringBuilder.ToString();
                        lineCount++;
                    }

                    //whether or not we have changed it, we still need to write the line into the new file
                    writer.WriteLine(line);
                }
            }
        }
    }
}
