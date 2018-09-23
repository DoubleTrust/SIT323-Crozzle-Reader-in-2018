using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CrozzleApplication
{
    class WordData
    {
        #region constants
        public const String OrientationRow = "HORIZONTAL-SEQUENCES";
        public const String OrientationColumn = "VERTICAL-SEQUENCES";
        public const int NumberOfFields = 2;
        #endregion

        #region properties - errors
        public static List<String> Errors { get; set; }
        #endregion

        #region properties
        private String[] OriginalWordData { get; set; }
        public Boolean Valid { get; set; } = false;
        public Orientation Orientation { get; set; }
        public Coordinate Location { get; set; }
        public String Letters { get; set; }

        #endregion

        #region properties - testing
        public Boolean IsHorizontal
        {
            get { return (Orientation.IsHorizontal); }
        }

        public Boolean IsVertical
        {
            get { return (Orientation.IsVertical); }
        }
        #endregion

        #region constructors
        public WordData(String[] originalWordDataData)
        {
            OriginalWordData = originalWordDataData;
        }

        public WordData(String direction, int row, int column, String sequence)
        {
            OriginalWordData = new String[] { direction, row.ToString(), column.ToString(), sequence};
            Orientation anOrientation;
            if (!Orientation.TryParse(direction, out anOrientation))
                Errors.AddRange(CrozzleApplication.Orientation.Errors);
            Orientation = anOrientation;
            Location = new Coordinate(row, column);
            Letters = sequence;
        }

        public WordData()
        {
            OriginalWordData = null;
            Valid = false;
            Orientation = null;
            Location = null;
            Letters = null;
       
        }
        #endregion

        #region parsing
        public static Boolean TryParse(TitleSequence Block, Crozzle aCrozzle, out WordData aWordData)
        {
            String[] originalWordData = Block.SequenceLine.Split(new Char[] { ',' }, 2);

            Errors = new List<String>();
            aWordData = new WordData();
            String[] OldOriginalWordData = new string[4];
            

            // Check that the original word data has the correct number of fields.
            if (originalWordData.Length != NumberOfFields)
                Errors.Add(String.Format(WordDataErrors.FieldCountError, originalWordData.Length, Block.SequenceLine, NumberOfFields));
       
            // If the title symbol exist
            if (Block.Title.Length > 0)
            {        
                // Check whether the title is an orientation value.
                Orientation anOrientation;
                if (!Orientation.TryParse(Block.Title, out anOrientation))
                    Errors.AddRange(Orientation.Errors);
                
                aWordData.Orientation = anOrientation;

                if (anOrientation.Valid)
                {
                    // Handle the name & sequence side
                    String[] WordName = originalWordData[0].Split('='); 

                    // Figure out whether WordName = ['SEQUENCE','(words)']
                    if (WordName[0] != "SEQUENCE")
                        Errors.Add(String.Format(WordDataErrors.MissingSymbol, Block.SequenceLine));                 
                    else
                    {   // Figure out whether the word is missing
                        if (String.IsNullOrEmpty(WordName[1]))
                            Errors.Add(String.Format(WordDataErrors.BlankFieldError, Block.SequenceLine, WordName[0]));                    
                        // Find out whether the word is alphabet
                        else if (!Regex.IsMatch(WordName[1], Configuration.allowedCharacters))
                            Errors.Add(String.Format(WordDataErrors.AlphabeticError, WordName[1]));
                        else
                            aWordData.Letters = WordName[1];
                    }

                    // Handle the coordinate side
                    String[] WordCoordinate= originalWordData[1].Split('=');

                    // If the format is correct
                    if (WordName[0] == "SEQUENCE")
                    {
                        //find out whether the right side is seperated by equeal symbol
                        if (WordCoordinate.Length != 2)
                            Errors.Add(String.Format(WordDataErrors.MissingSymbol, Block.SequenceLine));

                        // Find out whether the coordinate exists
                        else if (String.IsNullOrEmpty(WordCoordinate[1]))
                            Errors.Add(String.Format(WordDataErrors.BlankFieldError, Block.SequenceLine, WordCoordinate[0]));
                        else
                        {
                            // Get the Coordinate in string format 
                            String[] CoordinateString = WordCoordinate[1].Split(',');

                            // Find out whether the CoordinateString is complete
                            if (CoordinateString.Length != 2)
                                Errors.Add(String.Format(WordDataErrors.CoordinateIncomplete, CoordinateString[0], originalWordData[1]));
                            else
                            {
                                // Get the coordinate of each word
                                String rowValue = CoordinateString[0];
                                String columnValue = CoordinateString[1];

                                if (rowValue.Length > 0 && columnValue.Length > 0)
                                {
                                    Coordinate aCoordinate;
                                    if (!Coordinate.TryParse(rowValue, columnValue, aCrozzle, out aCoordinate))
                                        Errors.AddRange(Coordinate.Errors);
                                    aWordData.Location = aCoordinate;
                                }
                            }
                        }
                    }
                }
            }

            // Switch to the old format 
            if (aWordData.Orientation != null)
                OldOriginalWordData[0] = aWordData.Orientation.Direction;
            else if (aWordData.Location != null)
            {
                OldOriginalWordData[1] = aWordData.Location.Row.ToString();
                OldOriginalWordData[3] = aWordData.Location.Column.ToString();
            }
            else if (aWordData.Letters != null)
                OldOriginalWordData[2] = aWordData.Letters;

            // Pass the old format to old OldOriginalWordData
            aWordData.OriginalWordData = OldOriginalWordData;

            aWordData.Valid = Errors.Count == 0;
            return (aWordData.Valid);
        }
        #endregion
    }
}