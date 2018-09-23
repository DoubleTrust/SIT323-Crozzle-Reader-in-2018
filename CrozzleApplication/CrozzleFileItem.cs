using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CrozzleApplication
{
    class CrozzleFileItem
    {
        #region constants - symbols
        // Moved to CrozzleKeys.resx
        #endregion

        #region properties - errors
        public static List<String> Errors { get; set; }
        #endregion

        #region properties
        private String OriginalItem { get; set; }
        public Boolean Valid { get; set; } = false;
        public String Name { get; set; }
        public KeyValue KeyValue { get; set; }
        #endregion

        #region properties - testing the type of the crozzle file item
        // This part is removed because we used another way to find the type of crozzle item
        /*public Boolean IsConfigurationFile
        {
            get { return (Regex.IsMatch(Name, @"^" + ConfigurationFileSymbol + @"$")); }
        }

        public Boolean IsWordListFile
        {
            get { return (Regex.IsMatch(Name, @"^" + WordListFileSymbol + @"$")); }
        }

        public Boolean IsRows
        {
            get { return (Regex.IsMatch(Name, @"^" + RowsSymbol + @"$")); }
        }

        public Boolean IsColumns
        {
            get { return (Regex.IsMatch(Name, @"^" + ColumnsSymbol + @"$")); }
        }

        public Boolean IsRow
        {
            get { return (Regex.IsMatch(Name, @"^" + RowSymbol + @"$")); }
        }

        public Boolean IsColumn
        {
            get { return (Regex.IsMatch(Name, @"^" + ColumnSymbol + @"$")); }
        }*/
        #endregion

        #region constructors
        public CrozzleFileItem(String originalItemData)
        {
            OriginalItem = originalItemData;
            Valid = false;
            Name = null;
            KeyValue = null;
        }
        #endregion

        #region parsing
        public static Boolean TryParse(List<String> crozzleFileGroup, out CzlGroup aCrozzleFileGroup)
        {
            Errors = new List<String>();
            aCrozzleFileGroup = new CzlGroup();
            
            String CrozzleLine = "";
            String SymbolMark = "NULL";
            
            foreach (String line in crozzleFileGroup)
            {
                if (String.IsNullOrEmpty(line))
                    break;
                // Discard comment.
                else if (line.Contains("//"))
                {
                    int index = line.IndexOf("//");
                    CrozzleLine = line.Remove(index);
                }

                else
                    CrozzleLine = line;
                CrozzleLine = CrozzleLine.Trim();
                
                // Use Crozzle object to store name and keyvalue
                CrozzleFileItem aCrozzleFileItem = new CrozzleFileItem(CrozzleLine);

                if (Regex.IsMatch(line, @"^\s*$"))
                    aCrozzleFileItem.Name = CrozzleKeys.NoCrozzleItem;
                else
                {   // Process the file-dependencies block
                    if (Regex.IsMatch(CrozzleLine, @"^" + CrozzleKeys.FileDependencies + @".*"))
                    {
                        SymbolMark = "FILE-OPEN";
                        aCrozzleFileGroup.CzlTitle = CrozzleKeys.FileDependencies;
                    }
                    else if (Regex.IsMatch(CrozzleLine, @"^" + CrozzleKeys.ConfigData + @".*") && SymbolMark == "FILE-OPEN")
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(CrozzleLine, CrozzleKeys.ConfigData, out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aCrozzleFileItem.Name = CrozzleKeys.ConfigData;
                        aCrozzleFileItem.KeyValue = aKeyValue;
                    }
                    else if (Regex.IsMatch(CrozzleLine, @"^" + CrozzleKeys.SequenceData + @".*") && SymbolMark == "FILE-OPEN")
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(CrozzleLine, CrozzleKeys.SequenceData, out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aCrozzleFileItem.Name = CrozzleKeys.SequenceData;
                        aCrozzleFileItem.KeyValue = aKeyValue;
                    }
                    else if (Regex.IsMatch(CrozzleLine, @"^" + CrozzleKeys.EndFileDependencies + @".*"))
                        SymbolMark = "NULL";

                    // Process the crozzle-size block
                    else if (Regex.IsMatch(CrozzleLine, @"^" + CrozzleKeys.CrozzleSize + @".*"))
                    {
                        SymbolMark = "SIZE-OPEN";
                        aCrozzleFileGroup.CzlTitle = CrozzleKeys.CrozzleSize;
                    }
                    else if (Regex.IsMatch(CrozzleLine, @"^" + CrozzleKeys.Size + @".*") && SymbolMark == "SIZE-OPEN")
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(CrozzleLine, CrozzleKeys.Size, out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aCrozzleFileItem.Name = CrozzleKeys.Size;
                        aCrozzleFileItem.KeyValue = aKeyValue;
                    }
                    else if (Regex.IsMatch(CrozzleLine, @"^" + CrozzleKeys.EndCrozzleSize + @".*"))
                        SymbolMark = "NULL";

                    // Process horizontal-sequences block
                    else if (Regex.IsMatch(CrozzleLine, @"^" + CrozzleKeys.HorizontalSequence + @".*"))
                    {
                        SymbolMark = "HORIZON-OPEN";
                        aCrozzleFileGroup.CzlTitle = CrozzleKeys.HorizontalSequence;
                    }
                    else if (Regex.IsMatch(CrozzleLine, @"^" + CrozzleKeys.HSequence + @".*") && SymbolMark == "HORIZON-OPEN" )
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(CrozzleLine, CrozzleKeys.HSequence, out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aCrozzleFileItem.Name = CrozzleKeys.HSequence;
                        aCrozzleFileItem.KeyValue = aKeyValue;
                    }
                    else if (Regex.IsMatch(CrozzleLine, @"^" + "LOCATION" + @".*") && SymbolMark == "HORIZON-OPEN")
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(CrozzleLine, "LOCATION", out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aCrozzleFileItem.Name = "LOCATION";
                        aCrozzleFileItem.KeyValue = aKeyValue;
                    }
                    else if (Regex.IsMatch(CrozzleLine, @"^" + CrozzleKeys.EndHorizontalSequence + @".*"))
                        SymbolMark = "NULL";

                    // Process vertical-sequences block
                    else if (Regex.IsMatch(CrozzleLine, @"^" + CrozzleKeys.VerticalSequence + @".*"))
                    {
                        SymbolMark = "VERTICAL-OPEN";
                        aCrozzleFileGroup.CzlTitle = CrozzleKeys.VerticalSequence;
                    }
                    else if (Regex.IsMatch(CrozzleLine, @"^" + CrozzleKeys.VSequence + @".*") && SymbolMark == "VERTICAL-OPEN")
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(CrozzleLine, CrozzleKeys.VSequence, out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aCrozzleFileItem.Name = CrozzleKeys.VSequence;
                        aCrozzleFileItem.KeyValue = aKeyValue;
                    }
                    else if (Regex.IsMatch(CrozzleLine, @"^" + CrozzleKeys.EndVerticalSequence + @".*"))
                        SymbolMark = "NULL";
                }
                // Determine whether the line is valid
                aCrozzleFileItem.Valid = Errors.Count == 0;
                // Add the core content in each block
                if (aCrozzleFileItem.Name != null)
                    aCrozzleFileGroup.Lines.Add(aCrozzleFileItem);
            }

            // Check th Validity
            Boolean Valid = true;
            foreach (CrozzleFileItem line in aCrozzleFileGroup.Lines)
            {
                if (line.Valid == false)
                    Valid = false;
            }
            return (Valid);
        }
        #endregion
    }
}