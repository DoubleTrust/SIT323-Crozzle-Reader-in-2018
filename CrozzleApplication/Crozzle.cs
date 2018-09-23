using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace CrozzleApplication
{
    class Crozzle
    {
        #region constants
        public static readonly Char[] StringDelimiters = new Char[] { '"' };
        #endregion

        #region properties - errors
        public static List<String> Errors { get; set; }
        private List<String> CrozzleGridErrors { get; set; }

        public String FileErrorsTXT
        {
            get
            {
                int errorNumber = 1;
                String errors = "START PROCESSING FILE: " + CrozzleFileName + "\r\n";

                foreach (String error in Crozzle.Errors)
                    errors += "error " + errorNumber++ + ": " + error + "\r\n";
                errors += "END PROCESSING FILE: " + CrozzleFileName + "\r\n";

                return (errors);
            }
        }

        public String FileErrorsHTML
        {
            get
            {
                int errorNumber = 1;
                String errors = "<p style=\"font-weight:bold\">START PROCESSING FILE: " + CrozzleFileName + "</p>";

                foreach (String error in Crozzle.Errors)
                    errors += "<p>error " + errorNumber++ + ": " + error + "</p>";
                errors += "<p style=\"font-weight:bold\">END PROCESSING FILE: " + CrozzleFileName + "</p>";

                return (errors);
            }
        }

        public String ErrorsTXT
        {
            get
            {
                int errorNumber = 1;
                String errors = "START PROCESSING CROZZLE:\r\n";

                foreach (String error in CrozzleGridErrors)
                    errors += "error " + errorNumber++ + ": " + error + "\r\n";
                errors += "END PROCESSING CROZZLE:\r\n";

                return (errors);
            }
        }

        public String ErrorsHTML
        {
            get
            {
                int errorNumber = 1;
                String errors = "<p style=\"font-weight:bold\">START PROCESSING CROZZLE:</p>";

                foreach (String error in CrozzleGridErrors)
                    errors += "<p>error " + errorNumber++ + ": " + error + "</p>";
                errors += "<p style=\"font-weight:bold\">END PROCESSING CROZZLE:</p>";

                return (errors);
            }
        }
        #endregion

        #region properties - crozzle validity
        public Boolean FileValid { get; set; } = false;
        public Boolean ValidityChecked { get; set; } = false;
        public Boolean CrozzleValid { get; set; } = false;
        #endregion

        #region properties - file names
        public String CrozzlePath { get; set; }
        public String CrozzleFileName { get; set; }
        public String CrozzleDirectoryName { get; set; }
        public string ConfigurationPath { get; set; }
        public string WordListPath { get; set; }
        #endregion

        #region properties
        public WordList WordList { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public WordDataList WordDataList { get; set; }
        public Configuration Configuration { get; set; }
        private List<String[]> CrozzleRows { get; set; }
        private List<String[]> CrozzleColumns { get; set; }
        public CrozzleSequences CrozzleSequences { get; set; }
        #endregion

        #region constructors
        public Crozzle(String path, Configuration aConfiguration, WordList aWordList)
        {
            CrozzlePath = path;
            CrozzleFileName = Path.GetFileName(path);
            CrozzleDirectoryName = Path.GetDirectoryName(path);

            Configuration = aConfiguration;
            WordList = aWordList;
        }

        public Crozzle()
        { }
        #endregion

        #region parsing
        public static Boolean TryParse(String path, Configuration aConfiguration, WordList wordList, out Crozzle aCrozzle)
        {
            Errors = new List<String>();
            aCrozzle = new Crozzle(path, aConfiguration, wordList);

            // Open file.
            StreamReader fileIn = new StreamReader(path);
            List<TitleSequence> wordData = new List<TitleSequence>();

            // Validate file.
            while (!fileIn.EndOfStream)
            {
                // Create a Block to store a group of data
                List<String> Block = new List<String>();

                // Read a line. 
                String line = fileIn.ReadLine();
                while (!String.IsNullOrEmpty(line)) 
                {
                    Block.Add(line);
                    line = fileIn.ReadLine();
                }

                // Parse a block in crozzle file
                CzlGroup aCrozzleFileGroup;
                if (CrozzleFileItem.TryParse(Block, out aCrozzleFileGroup))
                {
                    String title = aCrozzleFileGroup.CzlTitle;

                    for (int i = 0; i < aCrozzleFileGroup.Lines.Count; i++)
                    {
                        // Whether the title equals to "FILE-DEPENDENCIES"
                        if (title == CrozzleKeys.FileDependencies)
                        {
                            if (aCrozzleFileGroup.Lines[i].Name == CrozzleKeys.ConfigData)
                            {
                                String configurationPath = aCrozzleFileGroup.Lines[i].KeyValue.Value;
                                if (configurationPath == null)
                                    Errors.Add(CrozzleFileErrors.ConfigurationFilenameMissing);
                                else
                                {
                                    configurationPath = configurationPath.Trim();

                                    if (Validator.IsDelimited(configurationPath, StringDelimiters))
                                        configurationPath = configurationPath.Trim(StringDelimiters);

                                    if (!Path.IsPathRooted(configurationPath))
                                    {
                                        String directoryName = Path.GetDirectoryName(path);
                                        configurationPath = directoryName + @"\" + configurationPath;
                                    }

                                    aCrozzle.ConfigurationPath = configurationPath;
                                }
                            }
                            else if (aCrozzleFileGroup.Lines[i].Name == CrozzleKeys.SequenceData)
                            {
                                String wordListPath = aCrozzleFileGroup.Lines[i].KeyValue.Value;
                                if (wordListPath == null)
                                    Errors.Add(CrozzleFileErrors.ConfigurationFilenameMissing);
                                else
                                {
                                    wordListPath = wordListPath.Trim();

                                    if (Validator.IsDelimited(wordListPath, StringDelimiters))
                                        wordListPath = wordListPath.Trim(StringDelimiters);

                                    if (!Path.IsPathRooted(wordListPath))
                                    {
                                        String directoryName = Path.GetDirectoryName(path);
                                        wordListPath = directoryName + @"\" + wordListPath;
                                    }

                                    aCrozzle.WordListPath = wordListPath;
                                }
                            }

                        }

                        // Whether the title equals to "CROZZLE-SIZE"
                        else if (title == CrozzleKeys.CrozzleSize)
                        {
                            int rows, columns;
                            String[] coordinate = aCrozzleFileGroup.Lines[i].KeyValue.Value.Split(',');
                            if (Validator.IsInt32(coordinate[0].Trim(), out rows))
                                aCrozzle.Rows = rows;
                            else
                                Errors.Add(String.Format(CrozzleFileErrors.RowError, aCrozzleFileGroup.Lines[i].KeyValue.OriginalKeyValue, Errors[0]));

                            if (Validator.IsInt32(coordinate[1].Trim(), out columns))
                                aCrozzle.Columns = columns;
                            else
                                Errors.Add(String.Format(CrozzleFileErrors.ColumnError, aCrozzleFileGroup.Lines[i].KeyValue.OriginalKeyValue, Errors[1]));
                        }

                        // Whether the title equals to "HORIZONTAL-SEQUENCES" or "VERTICAL-SEQUENCE"
                        else if (title == CrozzleKeys.HorizontalSequence || title == CrozzleKeys.VerticalSequence)
                        {
                            TitleSequence temp = new TitleSequence(title, aCrozzleFileGroup.Lines[i].KeyValue.OriginalKeyValue);
                            wordData.Add(temp);
                        }

                        // If there exist unidentified titles
                        else
                            Errors.AddRange(CrozzleFileItem.Errors);
                    }
                }             
            }
            // Close files.
            fileIn.Close();

            // Get potential word data list.
            WordDataList wordDataList;
            if (!WordDataList.TryParse(wordData, aCrozzle, out wordDataList))
                Errors.AddRange(WordDataList.Errors);
            aCrozzle.WordDataList = wordDataList;


            // Validate file sections.
            // Check the configuration file name.
            if (aCrozzle.Configuration != null)
                if (aCrozzle.Configuration.ConfigurationPath != aCrozzle.ConfigurationPath)
                    Errors.Add(String.Format(CrozzleFileErrors.ConfigurationFilenameError, aCrozzle.ConfigurationPath, aCrozzle.Configuration.ConfigurationPath));

            // Check the word list file name.
            if (aCrozzle.WordList != null)
                if (aCrozzle.WordList.WordlistPath != aCrozzle.WordListPath)
                    Errors.Add(String.Format(CrozzleFileErrors.WordlistFilenameError, aCrozzle.WordListPath, aCrozzle.WordList.WordlistFileName));

            // Raw word data of horizontal and vertical words were obtained when reading the crozzle file,
            // but now we need to create crozzle rows and crozzle columns that represent the crozzle.
            aCrozzle.CreateCrozzleRows(aCrozzle.WordDataList);
            aCrozzle.CreateCrozzleColumns(aCrozzle.WordDataList);


            // Store validity.
            aCrozzle.FileValid = Errors.Count == 0;
            return (aCrozzle.FileValid);
        }        
        #endregion

        #region validate crozzle
        public void Validate()
        {
            CrozzleGridErrors = new List<String>();

            // Indicate that validity has been attempted.
            ValidityChecked = true;

            // Get all sequences of 2 or more letters.
            CrozzleSequences = new CrozzleSequences(CrozzleRows, CrozzleColumns, Configuration);
            // Check that the number of rows is within limits.
            if (Rows < Configuration.MinimumNumberOfRows || Rows > Configuration.MaximumNumberOfRows)
                CrozzleGridErrors.Add(String.Format(CrozzleFileErrors.RowCountError, Rows, Configuration.MinimumNumberOfRows, Configuration.MaximumNumberOfRows));

            // Check that the number of columns is within limits.
            if (Columns < Configuration.MinimumNumberOfColumns || Columns > Configuration.MaximumNumberOfColumns)
                CrozzleGridErrors.Add(String.Format(CrozzleFileErrors.ColumnCountError, Columns, Configuration.MinimumNumberOfColumns, Configuration.MaximumNumberOfColumns));

            // Check that the number of words is within limits.
            if (WordDataList != null)
            {
                // Check that the number of horizontal words is within limits.
                if (WordDataList.HorizontalCount < Configuration.MinimumHorizontalWords || WordDataList.HorizontalCount > Configuration.MaximumHorizontalWords)
                    CrozzleGridErrors.Add(String.Format(CrozzleErrors.HorizontalWordCountError, WordDataList.HorizontalCount, Configuration.MinimumHorizontalWords, Configuration.MaximumHorizontalWords));

                // Check that the number of vertical words is within limits.
                if (WordDataList.VerticalCount < Configuration.MinimumVerticalWords || WordDataList.VerticalCount > Configuration.MaximumVerticalWords)
                    CrozzleGridErrors.Add(String.Format(CrozzleErrors.VerticalWordCountError, WordDataList.VerticalCount, Configuration.MinimumVerticalWords, Configuration.MaximumVerticalWords));
            }

            // Check that the number of vertical words that intersect a horizontal word is within limits.
            CrozzleSequences.CheckHorizontalIntersections(Configuration.MinimumIntersectionsInHorizontalWords, Configuration.MaximumIntersectionsInHorizontalWords);
            
            // Check that the number of horizontal words that intersect a vertical word is within limits.
            CrozzleSequences.CheckVerticalIntersections(Configuration.MinimumIntersectionsInVerticalWords, Configuration.MaximumIntersectionsInVerticalWords);

            // Check that the number of duplicate words is within limits.
            CrozzleSequences.CheckDuplicateWords(Configuration.MinimumNumberOfTheSameWord, Configuration.MaximumNumberOfTheSameWord);

            // Check that the number of groups of connected words is within the limit.
            CrozzleSequences.CheckConnectivity(Configuration.MinimumNumberOfGroups, Configuration.MaximumNumberOfGroups, CrozzleRows, CrozzleColumns);

            // Check that each horizontal word fits into the grid.
            if (WordDataList != null && WordDataList.HorizontalWordData != null)
                foreach (WordData hWordData in WordDataList.HorizontalWordData)
                    if (hWordData.Location.Column + hWordData.Letters.Length - 1 > Columns)
                        CrozzleGridErrors.Add(String.Format(CrozzleErrors.HorizontalWordSizeError, hWordData.Letters));

            // Check that each vertical word fits into the grid.
            if (WordDataList != null && WordDataList.VerticalWordData != null)
                foreach (WordData vWordData in WordDataList.VerticalWordData)
                    if (vWordData.Location.Row + vWordData.Letters.Length - 1 > Rows)
                        CrozzleGridErrors.Add(String.Format(CrozzleErrors.VerticalWordSizeError, vWordData.Letters));

            // Check that each sequence is in the wordlist.
            CrozzleSequences.FindMissingWords(WordList);

            // Is this crozzle valid?
            CrozzleValid = true;
            if (CrozzleSequences.ErrorsDetected)
            {
                CrozzleValid = false;
                CrozzleGridErrors.AddRange(CrozzleSequences.ErrorMessages);
            }
        }
        #endregion

        #region scoring
        public String Score()
        {
            String score = Configuration.InvalidCrozzleScore;

            if (CrozzleValid)
                score = CrozzleScore().ToString();

            return (score);
        }

        private int CrozzleScore()
        {
            int score = 0;
            // Get the point per word from wordlist
            Configuration.PointsPerWord = WordList.getPointsPerWord();
            // Increase the score for each word.
            score += CrozzleSequences.Count * Configuration.PointsPerWord;

            // Increase the score for intersecting letters.
            List<Char> intersectingLetters = CrozzleSequences.GetIntersectingLetters();
            foreach (Char letter in intersectingLetters)
                score += Configuration.IntersectingPointsPerLetter[(int)letter - (int)'A'];

            // Get all letters.
            List<Char> allLetters = new List<Char>();

            foreach (String[] letters in CrozzleRows)
                foreach (String letter in letters)
                    if (letter[0] != ' ')
                        allLetters.Add(letter[0]);

            // Remove each intersecting letter from allLetters.
            List<Char> nonIntersectingLetters = allLetters;
            foreach (Char letter in intersectingLetters)
                nonIntersectingLetters.Remove(letter);

            // Increase the score for non-intersecting letters.
            foreach (Char letter in nonIntersectingLetters)
                score += Configuration.NonIntersectingPointsPerLetter[(int)letter - (int)'A'];

            return (score);
        }
        #endregion

        #region crozzle representation
        private void CreateCrozzleRows(WordDataList wordDataList)
        {
            // Create a List to store String arrays, one String[] for each row, one String for each letter.
            CrozzleRows = new List<String[]>();

            // Create and store empty rows into the list.
            for (int i = 0; i < Rows; i++)
            {
                String[] row = new String[Columns];
                for (int j = 0; j < row.Length; j++)
                    row[j] = " ";
                CrozzleRows.Add(row);
            }

            // Store HORIZONTAL words into the rows.
            foreach (WordData hWordData in wordDataList.HorizontalWordData)
            {
                if (hWordData.Location.Row >= 1 && hWordData.Location.Row <= Rows &&
                    hWordData.Location.Column >= 1 && hWordData.Location.Column <= Columns)
                {
                    // Store each letter into the approriate row.
                    String[] row = CrozzleRows[hWordData.Location.Row - 1];
                    int col = hWordData.Location.Column - 1;
                    foreach (Char c in hWordData.Letters)
                        if (col < Columns)
                            row[col++] = new String(c, 1);
                }
            }

            // Store VERTICAL words into the rows.
            foreach (WordData vWordData in wordDataList.VerticalWordData)
            {
                if (vWordData.Location.Row >= 1 && vWordData.Location.Row <= Rows &&
                    vWordData.Location.Column >= 1 && vWordData.Location.Column <= Columns)
                {
                    // Store each letter into the ith row, but the same column location.
                    int i = vWordData.Location.Row - 1;
                    foreach (Char c in vWordData.Letters)
                        if (i < Rows)
                        {
                            String[] row = CrozzleRows[i];
                            row[vWordData.Location.Column - 1] = new String(c, 1);
                            i++;
                        }
                }
            }
        }

        private void CreateCrozzleColumns(WordDataList wordDataList)
        {
            // Create a List to store String arrays, one String[] for each column, one String for each letter.
            CrozzleColumns = new List<String[]>();

            // Create and store empty columns into the list.
            for (int i = 0; i < Columns; i++)
            {
                String[] column = new String[Rows];
                for (int j = 0; j < column.Length; j++)
                    column[j] = " ";
                CrozzleColumns.Add(column);
            }

            // Store VERTICAL words into the columns.
            foreach (WordData vWordData in wordDataList.VerticalWordData)
            {
                if (vWordData.Location.Row >= 1 && vWordData.Location.Row <= Rows &&
                    vWordData.Location.Column >= 1 && vWordData.Location.Column <= Columns)
                {
                    // Store each letter into the approriate column.
                    String[] column = CrozzleColumns[vWordData.Location.Column - 1];
                    int row = vWordData.Location.Row - 1;
                    foreach (Char c in vWordData.Letters)
                        if (row < Rows)
                            column[row++] = new String(c, 1);
                }
            }

            // Store HORIZONTAL words into the columns.
            foreach (WordData hWordData in wordDataList.HorizontalWordData)
            {
                if (hWordData.Location.Row >= 1 && hWordData.Location.Row <= Rows &&
                    hWordData.Location.Column >= 1 && hWordData.Location.Column <= Columns)
                {
                    // Store each letter into the ith column, but the same row location.
                    int i = hWordData.Location.Column - 1;
                    foreach (Char c in hWordData.Letters)
                        if (i < Columns)
                        {
                            String[] column = CrozzleColumns[i];
                            column[hWordData.Location.Row - 1] = new String(c, 1);
                            i++;
                        }
                }
            }
        }

        public String ToStringHTML()
        {
            String crozzleHTML = "";
            String style = Configuration.Style;

            style += @"<style>
                       .empty { background-color: " + Configuration.BGcolourEmptyTD + @"; }
                       .nonempty { background-color: " + Configuration.BGcolourNonEmptyTD + @"; }
                       </style>";

            if (CrozzleRows != null)
            {
                crozzleHTML += @"<!DOCTYPE html><html><head>";
                crozzleHTML += style;
                crozzleHTML += @"</head><body>";
                crozzleHTML += @"<table>";

                foreach (String[] letters in CrozzleRows)
                {
                    crozzleHTML += @"<tr>";
                    foreach (String letter in letters)
                    {
                        if (letter == " ")
                            crozzleHTML += @"<td class=""empty"">" + letter + @"</td>";
                        else
                            crozzleHTML += @"<td class=""nonempty"">" + letter + @"</td>";
                    }
                    crozzleHTML += @"</tr>";
                }
                crozzleHTML += @"</table>";

                if (FileValid)
                    crozzleHTML += @"<p>Crozzle file is valid.</p>";
                else
                    crozzleHTML += @"<p>Crozzle file is invalid.</p>";

                if (Configuration.Valid)
                    crozzleHTML += @"<p>Configuration file is valid.</p>";
                else
                    crozzleHTML += @"<p>Configuration file is invalid.</p>";

                if (WordList.Valid)
                    crozzleHTML += @"<p>Word list file is valid.</p>";
                else
                    crozzleHTML += @"<p>Word list file is invalid.</p>";

                if (ValidityChecked && FileValid && Configuration != null && Configuration.Valid && WordList != null && WordList.Valid)
                    crozzleHTML += @"<p>Score = " + Score() + @"</p>";
                else
                    crozzleHTML += @"<p></p>";

                crozzleHTML += @"</body></html>";
            }

            return (crozzleHTML);
        }
        #endregion

        #region log errors
        public void LogFileErrors(String errors)
        {
            StreamWriter logfile = new StreamWriter(CrozzleDirectoryName + @"\" + Configuration.LogFileName, true);
            logfile.WriteLine(errors);
            logfile.Close();
        }
        #endregion

        #region get Crozzle Row data
        public List<String[]> getCrozzleRows()
        {
            return this.CrozzleRows;
        }
        #endregion
        #region get Crozzle Column data
        public List<String[]> getCrozzleColumns()
        {
            return this.CrozzleColumns;
        }
        #endregion

    }
}