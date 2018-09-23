using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;

namespace CrozzleApplication
{
    class WordList
    {
        #region constants
        private static readonly Char[] WordSeparators = new Char[] { '\n', '\r' };
        #endregion

        #region properties - errors
        public static List<String> Errors { get; set; }

        public String FileErrors
        {
            get
            {
                int errorNumber = 1;
                String errors = "START PROCESSING FILE: " + WordlistFileName + "\r\n";

                foreach (String error in WordList.Errors)
                    errors += "error " + errorNumber++ + ": " + error + "\r\n";
                errors += "END PROCESSING FILE: " + WordlistFileName + "\r\n";

                return (errors);
            }
        }

        public String FileErrorsHTML
        {
            get
            {
                int errorNumber = 1;
                String errors = "<p style=\"font-weight:bold\">START PROCESSING FILE: " + WordlistFileName + "</p>";

                foreach (String error in WordList.Errors)
                    errors += "<p>error " + errorNumber++ + ": " + error + "</p>";
                errors += "<p style=\"font-weight:bold\">END PROCESSING FILE: " + WordlistFileName + "</p>";

                return (errors);
            }
        }
        #endregion

        #region properties - filenames
        public String WordlistPath { get; set; }
        public String WordlistFileName { get; set; }
        public String WordlistDirectoryName { get; set; }
        #endregion

        #region properties
        public String[] OriginalList { get; set; }
        public Boolean Valid { get; set; } = false;
        public List<Validator> List { get; set; }

        public Validator Validation { get; set;}

        public int Count
        {
            get { return (List.Count); }
        }
        #endregion
      
        #region constructors
        public WordList(String path, Configuration aConfiguration)
        {
            WordlistPath = path;
            WordlistFileName = Path.GetFileName(path);
            WordlistDirectoryName = Path.GetDirectoryName(path);
            List = new List<Validator>();
        }
        #endregion

        #region parsing
        public static Boolean TryParse(String path, Configuration aConfiguration, out WordList aWordList)
        {
            StreamReader fileIn = new StreamReader(path);

            Errors = new List<String>();
            aWordList = new WordList(path, aConfiguration);

            // Split the original wordlist from the file and store it in a temporarily list
            String[] TempList = fileIn.ReadToEnd().Split(WordSeparators);

            // Find the first line in ".seq" for validation
            aWordList.Validation = new Validator(TempList[0]);          
            // Clear the first line
            for (int i = 0; i < TempList.Length-1; i++)
                TempList[i] = TempList[i + 1];
            // Clear the last line
            TempList[TempList.Length - 1] = "";


            // Use Lambda to append the list to originalList property
            aWordList.OriginalList = TempList.Where(s => !String.IsNullOrEmpty(s)).ToArray();

            // Find out whether wordlist contains duplicated content
            String duplicate = Validator.ContainsDuplicates(aWordList.OriginalList);
            if (duplicate != "")
                Errors.Add(String.Format(WordListErrors.DuplicateError, duplicate));

            // Check each field in the wordlist.
            int TotalScore = 0, TotalLength = 0, TotalASCII = 0, TotalHash = 0;
            foreach (String potentialWord in aWordList.OriginalList)
            {
                Validator line = new Validator(potentialWord);

                // If it goes to the last line
                if (String.IsNullOrEmpty(potentialWord))
                    break;
                TotalScore += line.Score;
                TotalLength += line.Length;
                TotalASCII += line.ASCII;
                TotalHash += line.Hash;

                // Check the word and word length
                if (line.Length > 0)
                {
                    
                    // Check that the field is alphabetic
                    if (Regex.IsMatch(line.Word, Configuration.allowedCharacters))
                    {
                        aWordList.Add(line);
                        // Check if the actual length of word is equal to its pre-defined length
                        if (line.Word.Length != line.Length)
                            Errors.Add(String.Format(WordListErrors.WordLengthError, line.Word, line.Length)); 

                        /*if (line.Word.Length == line.Length)
                            aWordList.Add(line);
                        else
                            Errors.Add(String.Format(WordListErrors.WordLengthError, line.Word, line.Length));*/
                    }
                    else
                        Errors.Add(String.Format(WordListErrors.AlphabeticError, line.Word, line.OriginalLine));

                    // Check the word ASCII
                    if (line.GetWordASCII() == line.ASCII)
                    {
                        //TotalASCII += line.ASCII;
                    }
                    else
                    {
                        Errors.Add(String.Format(WordListErrors.WordASCIIError, line.Word, line.ASCII));
                    }
                    // Check the word Hash
                    if ((line.Length + line.Score + line.ASCII) == line.Hash)
                    {
                        //TotalHash += line.Hash;
                    }
                    else
                    {
                        Errors.Add(String.Format(WordListErrors.WordHashError, line.OriginalLine, line.Hash));
                    }
                }
                else
                {
                    if (line.Word.Length > 0)
                        Errors.Add(String.Format(WordListErrors.InvalidWordError, line.Word, line.OriginalLine));
                    else
                        Errors.Add(String.Format(WordListErrors.MissingWordError, line.OriginalLine));                     
                }
            }

            // Compare the total values above and those in the first line in ".seq"
            if (TotalLength != aWordList.Validation.Length)
                Errors.Add(String.Format(WordListErrors.TotalLengthError, TotalLength, aWordList.Validation.Length));
            if (TotalHash != aWordList.Validation.Hash)
                Errors.Add(String.Format(WordListErrors.TotalHashError, TotalHash, aWordList.Validation.Hash));
            if (TotalScore != aWordList.Validation.Score)
                Errors.Add(String.Format(WordListErrors.TotalScoreError, TotalScore, aWordList.Validation.Score));
            if (TotalASCII != aWordList.Validation.ASCII)
                Errors.Add(String.Format(WordListErrors.TotalASCIIError, TotalASCII, aWordList.Validation.ASCII));

            // Check the minimmum word limit.
            if (aWordList.Count < aConfiguration.MinimumNumberOfUniqueWords)
                Errors.Add(String.Format(WordListErrors.MinimumSizeError, aWordList.Count, aConfiguration.MinimumNumberOfUniqueWords));

            // Check the maximum word limit.
            if (aWordList.Count > aConfiguration.MaximumNumberOfUniqueWords)
                Errors.Add(String.Format(WordListErrors.MaximumSizeError, aWordList.Count, aConfiguration.MaximumNumberOfUniqueWords));

            aWordList.Valid = Errors.Count == 0;
            return (aWordList.Valid);
        }
        #endregion

        #region list functions
        public void Add(Validator letters)
        {
            List.Add(letters);
        }

        public Boolean Contains(String letters)
        {
            // Use Lambda to return the line which contains the specific word
            return (List.Exists(line => line.Word == letters));
        }

        public int getPointsPerWord()
        {
            // Return the score of each point
            return List[1].Score;
        }
        #endregion

    }
}
