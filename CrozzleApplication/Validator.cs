using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace CrozzleApplication
{
    public class Validator
    {
        #region properties - errors
        public static List<String> Errors { get; set; }
        #endregion

        #region pattern checking
        // Check that a string matches a pattern.
        public static Boolean IsMatch(String input, String pattern)
        {
            Errors = new List<String>();

            if (!Regex.IsMatch(input, pattern))
                Errors.Add(String.Format(ValidatorErrors.PatternError, pattern));
            
            return (Errors.Count == 0);
        }
        #endregion

        #region type checking
        // Check that a numeric field is an integer.
        public static Boolean IsInt32(String field, out int anInteger)
        {
            int n = -1;
            Errors = new List<String>();

            if (!Int32.TryParse(field, out n))
                Errors.Add(ValidatorErrors.IntegerError);

            anInteger = n;
            return (Errors.Count == 0);
        }

        // Check that a string field is a Boolean.
        public static Boolean IsBoolean(String field, out Boolean aBoolean)
        {   
            Boolean b = true;
            Errors = new List<String>();

            if (!Boolean.TryParse(field, out b))
                Errors.Add(ValidatorErrors.BooleanError);

            aBoolean = b;
            return (Errors.Count == 0);
        }

        // Check that a string field is a hex colour code such as #56ab7f.
        public static Boolean IsHexColourCode(String hexColour)
        {
            Errors = new List<String>();

            if (!Regex.IsMatch(hexColour, @"^#[0-9a-fA-F]{6}$"))
                Errors.Add(ValidatorErrors.HexColourCodeError);

            return (Errors.Count == 0);
        }

        // Check that a string field is a hex colour code such as #56ab7f.
        public static Boolean IsFilename(String name)
        {
            Errors = new List<String>();

            if (name.IndexOfAny(Path.GetInvalidPathChars()) != -1)
                    Errors.Add(ValidatorErrors.FilenameError);

            return (Errors.Count == 0);
        }

        // Check that a string field is delimited by double quotes.
        public static Boolean IsDelimited(String field, Char[] delimiters)
        {
            Boolean delimited = false;

            foreach (Char delimiter in delimiters)
                if (Regex.IsMatch(field, @"^" + delimiter + ".*" + delimiter + "$"))
                    delimited = true;

            Errors = new List<String>();
            if (!delimited)
                Errors.Add(String.Format(ValidatorErrors.DelimiterError, new String(delimiters)));

            return (delimited);
        }

        // Check that a string field is an HTML style.
        public static Boolean IsStyleTag(String style)
        {
            Errors = new List<String>();

            if (!Regex.IsMatch(style, @"^<[sS][tT][yY][lL][eE]>.*</[sS][tT][yY][lL][eE]>$"))
                Errors.Add(ValidatorErrors.StyleError);

            return (Errors.Count == 0);
        }
        #endregion

        #region range checking
        // Check that a numeric field is in range.
        public static Boolean TryRange(int n, int lowerLimit, int upperLimit)
        {
            Errors = new List<String>();

            if (n < lowerLimit || n > upperLimit)
                Errors.Add(String.Format(ValidatorErrors.RangeError, lowerLimit, upperLimit));

            return (Errors.Count == 0);
        }
        #endregion

        // This function is designed to find duplicated word data in Test.seq file, referenced from https://social.msdn.microsoft.com/Forums/vstudio/en-US/d0a8f9c6-2e14-4831-afb6-5e30b00bd69c/how-to-find-if-integer-array-include-duplicate-values?forum=csharpgeneral
        #region find duplicates
        public static String ContainsDuplicates(String[] a)
        {
            String duplicate = "";
            for (int i = 0; i < a.Length; i++)
            {
                for (int j = i + 1; j < a.Length; j++)
                {
                    if (a[i] == a[j])
                        duplicate = a[i];
                        
                }
            }
            return duplicate;
        }
        #endregion

        #region WordListData
        public String OriginalLine { get; set; }
        public String Word { get; set; }
        public int Score { get; set; }
        public int Length { get; set; }
        public int ASCII { get; set; }
        public int Hash { get; set; }

        #endregion

    #region Constructor
    public Validator(String OriginalList)
        {
            OriginalLine = OriginalList;

            // Break up the list
            String[] Data = OriginalList.Split(',');
            if (Data.Length == 5)
            {
                // If the line is not the first line
                Word = Data[0];
                Score = Int32.Parse(Data[1]);
                Length = Int32.Parse(Data[2]);
                ASCII = Int32.Parse(Data[3]);
                Hash = Int32.Parse(Data[4]);
            }
            else
            {   // If the line is the first line
                Word = Data[0] + Data[1];
                Score = Int32.Parse(Data[2]);
                Length = Int32.Parse(Data[3]);
                ASCII = Int32.Parse(Data[4]);
                Hash = Int32.Parse(Data[5]);
            }
        }
        #endregion

        #region Get the actual ASCII of each word 
        public int GetWordASCII()
        {
            int ASCII = 0;
            foreach (char character in Word)
            {
                ASCII += (int)character;
            }
            return ASCII;
        }
        #endregion


    }
}
