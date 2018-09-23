using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CrozzleApplication
{
    class ConfigurationFileItem
    {
        #region constants - symbols
        const String ColonSymbol = ":";
        const String AtoZ = @"^([A-Z]=|[A-Z]\b).*$"; 
        #endregion
    

        #region properties - errors
        public static List<String> Errors { get; set; }
        #endregion

        #region properties
        public String OriginalItem { get; set; }
        public Boolean Valid { get; set; }
        public String Name { get; set; }
        public KeyValue KeyValue { get; set; }

        public String InvalidTitle { get; set; }
        #endregion

        #region properties - testing the type of the configuration item
        // This part is removed because we used another way to find the type of configuration item
        /*public Boolean IsLogFile
        {
            get { return (Regex.IsMatch(Name, @"^" + LogfileName + @"$")); }
        }

        public Boolean IsMinimumNumberOfUniqueWords
        {
            get { return (Regex.IsMatch(Name, @"^" + MinimumNumberOfUniqueWords + @"$")); }
        }

        public Boolean IsMaximumNumberOfUniqueWords
        {
            get { return (Regex.IsMatch(Name, @"^" + MaximumNumberOfUniqueWords + @"$")); }
        }

        public Boolean IsInvalidCrozzleScore
        {
            get { return (Regex.IsMatch(Name, @"^" + InvalidCrozzleScore + @"$")); }
        }

        public Boolean IsUppercase
        {
            get { return (Regex.IsMatch(Name, @"^" + Uppercase + @"$")); }
        }

        public Boolean IsStyle
        {
            get { return (Regex.IsMatch(Name, @"^" + Style + @"$")); }
        }

        public Boolean IsBGcolourEmptyTD
        {
            get { return (Regex.IsMatch(Name, @"^" + BGcolourEmptyTD + @"$")); }
        }

        public Boolean IsBGcolourNonEmptyTD
        {
            get { return (Regex.IsMatch(Name, @"^" + BGcolourNonEmptyTD + @"$")); }
        }

        public Boolean IsMinimumNumberOfRows
        {
            get { return (Regex.IsMatch(Name, @"^" + MinimumNumberOfRows + @"$")); }
        }

        public Boolean IsMaximumNumberOfRows
        {
            get { return (Regex.IsMatch(Name, @"^" + MaximumNumberOfRows + @"$")); }
        }

        public Boolean IsMinimumNumberOfColumns
        {
            get { return (Regex.IsMatch(Name, @"^" + MinimumNumberOfColumns + @"$")); }
        }

        public Boolean IsMaximumNumberOfColumns
        {
            get { return (Regex.IsMatch(Name, @"^" + MaximumNumberOfColumns + @"$")); }
        }

        public Boolean IsMinimumHorizontalWords
        {
            get { return (Regex.IsMatch(Name, @"^" + MinimumHorizontalWords + @"$")); }
        }

        public Boolean IsMaximumHorizontalWords
        {
            get { return (Regex.IsMatch(Name, @"^" + MaximumHorizontalWords + @"$")); }
        }

        public Boolean IsMinimumVerticalWords
        {
            get { return (Regex.IsMatch(Name, @"^" + MinimumVerticalWords + @"$")); }
        }

        public Boolean IsMaximumVerticalWords
        {
            get { return (Regex.IsMatch(Name, @"^" + MaximumVerticalWords + @"$")); }
        }

        public Boolean IsMinimumIntersectionsInHorizontalWords
        {
            get { return (Regex.IsMatch(Name, @"^" + MinimumIntersectionsInHorizontalWord + @"$")); }
        }

        public Boolean IsMaximumIntersectionsInHorizontalWords
        {
            get { return (Regex.IsMatch(Name, @"^" + MaximumIntersectionsInHorizontalWord + @"$")); }
        }

        public Boolean IsMinimumIntersectionsInVerticalWords
        {
            get { return (Regex.IsMatch(Name, @"^" + MinimumIntersectionsInVerticalWord + @"$")); }
        }

        public Boolean IsMaximumIntersectionsInVerticalWords
        {
            get { return (Regex.IsMatch(Name, @"^" + MaximumIntersectionsInVerticalWord + @"$")); }
        }

        public Boolean IsMinimumNumberOfTheSameWord
        {
            get { return (Regex.IsMatch(Name, @"^" + MinimumNumberOfTheSameWord + @"$")); }
        }

        public Boolean IsMaximumNumberOfTheSameWord
        {
            get { return (Regex.IsMatch(Name, @"^" + MaximumNumberOfTheSameWord + @"$")); }
        }

        public Boolean IsMinimumNumberOfGroups
        {
            get { return (Regex.IsMatch(Name, @"^" + MinimumNumberOfGroups + @"$")); }
        }

        public Boolean IsMaximumNumberOfGroups
        {
            get { return (Regex.IsMatch(Name, @"^" + MaximumNumberOfGroups + @"$")); }
        }

        public Boolean IsPointsPerWord
        {
            get { return (Regex.IsMatch(Name, @"^" + PointsPerWord + @"$")); }
        }

        public Boolean IsIntersecting
        {
            get { return (Regex.IsMatch(Name, @"^" + IntersectingPointsPerLetter + @"$")); }
        }

        public Boolean IsNonIntersecting
        {
            get { return (Regex.IsMatch(Name, @"^" + NonIntersectingPointsPerLetter + @"$")); }
        }*/
        #endregion      

        #region constructors
        public ConfigurationFileItem(String originalItemData)
        {
            OriginalItem = originalItemData;
            Valid = false;
            Name = null;
            KeyValue = null;
        }
        #endregion

        #region parsing
        public static Boolean TryParse(List<String> configurationFileGroup, out CfgGroup aconfigurationFileGroup)
        {
            Errors = new List<String>();
            aconfigurationFileGroup = new CfgGroup();

            String ConfigurationLine = "";
            String SymbolMark = "NULL";

            foreach (String line in configurationFileGroup)
            {
                // Discard comment.
                if (line.Contains("//"))
                {
                    int index = line.IndexOf("//");
                    ConfigurationLine = line.Remove(index);
                }
                else
                    ConfigurationLine = line;
                ConfigurationLine = ConfigurationLine.Trim();
                
                // Use Configuration  object to store name and keyvalue
                ConfigurationFileItem aConfigurationFileItem = new ConfigurationFileItem(ConfigurationLine);

                if (Regex.IsMatch(line, @"^\s*$"))
                    aConfigurationFileItem.Name = ConfigurationKeys.NO_CONFIGURATION_ITEM;
                else
                {
                    if (String.IsNullOrEmpty(ConfigurationLine))
                        continue;
                    // Process the log file block
                    if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.LOGFILE + @".*"))
                    {
                        SymbolMark = "FILE-OPEN";
                        aconfigurationFileGroup.CfgTitle = ConfigurationKeys.LOGFILE;
                    }

                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.LOGFILE_NAME + @".*") && SymbolMark == "FILE-OPEN")
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(ConfigurationLine, ConfigurationKeys.LOGFILE_NAME, out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aConfigurationFileItem.Name = ConfigurationKeys.LOGFILE_NAME;
                        aConfigurationFileItem.KeyValue = aKeyValue;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.END_LOGFILE + @".*"))
                        SymbolMark = "NULL";

                    // Process the letter sequences configuration(limits on the number os unique letter sequences)
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.SEQUENCES_IN_FILE + @".*"))
                    {
                        SymbolMark = "SEQUENCES-IN-FILE-OPEN";
                        aconfigurationFileGroup.CfgTitle = ConfigurationKeys.SEQUENCES_IN_FILE;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.MINIMUM_NUMBER_OF_UNIQUE_WORDS + @".*") && SymbolMark == "SEQUENCES-IN-FILE-OPEN")
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(ConfigurationLine, ConfigurationKeys.MINIMUM_NUMBER_OF_UNIQUE_WORDS, out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aConfigurationFileItem.Name = ConfigurationKeys.MINIMUM_NUMBER_OF_UNIQUE_WORDS;
                        aConfigurationFileItem.KeyValue = aKeyValue;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.MAXIMUM_NUMBER_OF_UNIQUE_WORDS + @".*") && SymbolMark == "SEQUENCES-IN-FILE-OPEN")
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(ConfigurationLine, ConfigurationKeys.MAXIMUM_NUMBER_OF_UNIQUE_WORDS, out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aConfigurationFileItem.Name = ConfigurationKeys.MAXIMUM_NUMBER_OF_UNIQUE_WORDS;
                        aConfigurationFileItem.KeyValue = aKeyValue;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.END_SEQUENCES_IN_FILE + @".*"))
                        SymbolMark = "NULL";

                    // Process the crozzle output configuration
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.CROZZLE_OUTPUT + @".*"))
                    {
                        SymbolMark = "CROZZLE-OUTPUT-OPEN";
                        aconfigurationFileGroup.CfgTitle = ConfigurationKeys.CROZZLE_OUTPUT;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.INVALID_CROZZLE_SCORE + @".*") && SymbolMark == "CROZZLE-OUTPUT-OPEN")
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(ConfigurationLine, ConfigurationKeys.INVALID_CROZZLE_SCORE, out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aConfigurationFileItem.Name = ConfigurationKeys.INVALID_CROZZLE_SCORE;
                        aConfigurationFileItem.KeyValue = aKeyValue;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.UPPERCASE + @".*") && SymbolMark == "CROZZLE-OUTPUT-OPEN")
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(ConfigurationLine, ConfigurationKeys.UPPERCASE, out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aConfigurationFileItem.Name = ConfigurationKeys.UPPERCASE;
                        aConfigurationFileItem.KeyValue = aKeyValue;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.STYLE + @".*") && SymbolMark == "CROZZLE-OUTPUT-OPEN")
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(ConfigurationLine, ConfigurationKeys.STYLE, out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aConfigurationFileItem.Name = ConfigurationKeys.STYLE;
                        aConfigurationFileItem.KeyValue = aKeyValue;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.BGCOLOUR_EMPTY_TD + @".*") && SymbolMark == "CROZZLE-OUTPUT-OPEN")
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(ConfigurationLine, ConfigurationKeys.BGCOLOUR_EMPTY_TD, out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aConfigurationFileItem.Name = ConfigurationKeys.BGCOLOUR_EMPTY_TD;
                        aConfigurationFileItem.KeyValue = aKeyValue;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.BGCOLOUR_NON_EMPTY_TD + @".*") && SymbolMark == "CROZZLE-OUTPUT-OPEN")
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(ConfigurationLine, ConfigurationKeys.BGCOLOUR_NON_EMPTY_TD, out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aConfigurationFileItem.Name = ConfigurationKeys.BGCOLOUR_NON_EMPTY_TD;
                        aConfigurationFileItem.KeyValue = aKeyValue;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.END_CROZZLE_OUTPUT + @".*"))
                    {
                        SymbolMark = "NULL";
                    }

                    // Process the limits on the size of the crozlle grid
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.CROZZLE_SIZE + @".*"))
                    {
                        SymbolMark = "CROZZLE-SIZE-OPEN";
                        aconfigurationFileGroup.CfgTitle = ConfigurationKeys.CROZZLE_SIZE;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.MINIMUM_NUMBER_OF_ROWS + @".*") && SymbolMark == "CROZZLE-SIZE-OPEN")
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(ConfigurationLine, ConfigurationKeys.MINIMUM_NUMBER_OF_ROWS, out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aConfigurationFileItem.Name = ConfigurationKeys.MINIMUM_NUMBER_OF_ROWS;
                        aConfigurationFileItem.KeyValue = aKeyValue;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.MAXIMUM_NUMBER_OF_ROWS + @".*") && SymbolMark == "CROZZLE-SIZE-OPEN")
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(ConfigurationLine, ConfigurationKeys.MAXIMUM_NUMBER_OF_ROWS, out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aConfigurationFileItem.Name = ConfigurationKeys.MAXIMUM_NUMBER_OF_ROWS;
                        aConfigurationFileItem.KeyValue = aKeyValue;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.MINIMUM_NUMBER_OF_COLUMNS + @".*") && SymbolMark == "CROZZLE-SIZE-OPEN")
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(ConfigurationLine, ConfigurationKeys.MINIMUM_NUMBER_OF_COLUMNS, out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aConfigurationFileItem.Name = ConfigurationKeys.MINIMUM_NUMBER_OF_COLUMNS;
                        aConfigurationFileItem.KeyValue = aKeyValue;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.MAXIMUM_NUMBER_OF_COLUMNS + @".*") && SymbolMark == "CROZZLE-SIZE-OPEN")
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(ConfigurationLine, ConfigurationKeys.MAXIMUM_NUMBER_OF_COLUMNS, out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aConfigurationFileItem.Name = ConfigurationKeys.MAXIMUM_NUMBER_OF_COLUMNS;
                        aConfigurationFileItem.KeyValue = aKeyValue;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.END_CROZZLE_OUTPUT + @".*"))
                    {
                        SymbolMark = "NULL";
                    }

                    // Process the sequence in crozzle
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.SEQUENCES_IN_CROZZLE + @".*"))
                    {
                        SymbolMark = "SEQUENCES-IN-CROZZLE-OPEN";
                        aconfigurationFileGroup.CfgTitle = ConfigurationKeys.SEQUENCES_IN_CROZZLE;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.MINIMUM_HORIZONTAL_WORDS + @".*") && SymbolMark == "SEQUENCES-IN-CROZZLE-OPEN")
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(ConfigurationLine, ConfigurationKeys.MINIMUM_HORIZONTAL_WORDS, out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aConfigurationFileItem.Name = ConfigurationKeys.MINIMUM_HORIZONTAL_WORDS;
                        aConfigurationFileItem.KeyValue = aKeyValue;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.MAXIMUM_VERTICAL_WORDS + @".*") && SymbolMark == "SEQUENCES-IN-CROZZLE-OPEN")
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(ConfigurationLine, ConfigurationKeys.MAXIMUM_VERTICAL_WORDS, out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aConfigurationFileItem.Name = ConfigurationKeys.MAXIMUM_VERTICAL_WORDS;
                        aConfigurationFileItem.KeyValue = aKeyValue;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.MAXIMUM_HORIZONTAL_WORDS + @".*") && SymbolMark == "SEQUENCES-IN-CROZZLE-OPEN")
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(ConfigurationLine, ConfigurationKeys.MAXIMUM_HORIZONTAL_WORDS, out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aConfigurationFileItem.Name = ConfigurationKeys.MAXIMUM_HORIZONTAL_WORDS;
                        aConfigurationFileItem.KeyValue = aKeyValue;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.MINIMUM_VERTICAL_WORDS + @".*") && SymbolMark == "SEQUENCES-IN-CROZZLE-OPEN")
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(ConfigurationLine, ConfigurationKeys.MINIMUM_VERTICAL_WORDS, out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aConfigurationFileItem.Name = ConfigurationKeys.MINIMUM_VERTICAL_WORDS;
                        aConfigurationFileItem.KeyValue = aKeyValue;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.END_SEQUENCES_IN_CROZZLE + @".*"))
                    {
                        SymbolMark = "NULL";
                    }

                    // Process the limits on the number of intersections
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.INTERSECTIONS_IN_SEQUENCES + @".*"))
                    {
                        SymbolMark = "INTERSECTIONS_IN_SEQUENCES-OPEN";
                        aconfigurationFileGroup.CfgTitle = ConfigurationKeys.INTERSECTIONS_IN_SEQUENCES;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.MINIMUM_INTERSECTIONS_IN_HORIZONTAL_WORDS + @".*") && SymbolMark == "INTERSECTIONS_IN_SEQUENCES-OPEN")
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(ConfigurationLine, ConfigurationKeys.MINIMUM_INTERSECTIONS_IN_HORIZONTAL_WORDS, out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aConfigurationFileItem.Name = ConfigurationKeys.MINIMUM_INTERSECTIONS_IN_HORIZONTAL_WORDS;
                        aConfigurationFileItem.KeyValue = aKeyValue;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.MAXIMUM_INTERSECTIONS_IN_HORIZONTAL_WORDS + @".*") && SymbolMark == "INTERSECTIONS_IN_SEQUENCES-OPEN")
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(ConfigurationLine, ConfigurationKeys.MAXIMUM_INTERSECTIONS_IN_HORIZONTAL_WORDS, out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aConfigurationFileItem.Name = ConfigurationKeys.MAXIMUM_INTERSECTIONS_IN_HORIZONTAL_WORDS;
                        aConfigurationFileItem.KeyValue = aKeyValue;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.MINIMUM_INTERSECTIONS_IN_VERTICAL_WORDS + @".*") && SymbolMark == "INTERSECTIONS_IN_SEQUENCES-OPEN")
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(ConfigurationLine, ConfigurationKeys.MINIMUM_INTERSECTIONS_IN_VERTICAL_WORDS, out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aConfigurationFileItem.Name = ConfigurationKeys.MINIMUM_INTERSECTIONS_IN_VERTICAL_WORDS;
                        aConfigurationFileItem.KeyValue = aKeyValue;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.MAXIMUM_INTERSECTIONS_IN_VERTICAL_WORDS + @".*") && SymbolMark == "INTERSECTIONS_IN_SEQUENCES-OPEN")
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(ConfigurationLine, ConfigurationKeys.MAXIMUM_INTERSECTIONS_IN_VERTICAL_WORDS, out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aConfigurationFileItem.Name = ConfigurationKeys.MAXIMUM_INTERSECTIONS_IN_VERTICAL_WORDS;
                        aConfigurationFileItem.KeyValue = aKeyValue;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.END_INTERSECTIONS_IN_SEQUENCES + @".*"))
                        SymbolMark = "NULL";

                    //process the duplicate sequence
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.DUPLICATE_SEQUENCES + @".*"))
                    {
                        SymbolMark = "DUPLICATE-SEQUENCES-OPEN";
                        aconfigurationFileGroup.CfgTitle = ConfigurationKeys.DUPLICATE_SEQUENCES;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.MINIMUM_NUMBER_OF_THE_SAME_WORD + @".*") && SymbolMark == "DUPLICATE-SEQUENCES-OPEN")
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(ConfigurationLine, ConfigurationKeys.MINIMUM_NUMBER_OF_THE_SAME_WORD, out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aConfigurationFileItem.Name = ConfigurationKeys.MINIMUM_NUMBER_OF_THE_SAME_WORD;
                        aConfigurationFileItem.KeyValue = aKeyValue;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.MAXIMUM_NUMBER_OF_THE_SAME_WORD + @".*") && SymbolMark == "DUPLICATE-SEQUENCES-OPEN")
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(ConfigurationLine, ConfigurationKeys.MAXIMUM_NUMBER_OF_THE_SAME_WORD, out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aConfigurationFileItem.Name = ConfigurationKeys.MAXIMUM_NUMBER_OF_THE_SAME_WORD;
                        aConfigurationFileItem.KeyValue = aKeyValue;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.END_DUPLICATE_SEQUENCES + @".*"))
                    {
                        SymbolMark = "NULL";
                    }

                    //process the valid group
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.VALID_GROUPS + @".*"))
                    {
                        SymbolMark = "VALID-GROUPS-OPEN";
                        aconfigurationFileGroup.CfgTitle = ConfigurationKeys.VALID_GROUPS;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.MAXIMUM_NUMBER_OF_GROUPS + @".*") && SymbolMark == "VALID-GROUPS-OPEN")
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(ConfigurationLine, ConfigurationKeys.MAXIMUM_NUMBER_OF_GROUPS, out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aConfigurationFileItem.Name = ConfigurationKeys.MAXIMUM_NUMBER_OF_GROUPS;
                        aConfigurationFileItem.KeyValue = aKeyValue;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.MINIMUM_NUMBER_OF_GROUPS + @".*") && SymbolMark == "VALID-GROUPS-OPEN")
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(ConfigurationLine, ConfigurationKeys.MINIMUM_NUMBER_OF_GROUPS, out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aConfigurationFileItem.Name = ConfigurationKeys.MINIMUM_NUMBER_OF_GROUPS;
                        aConfigurationFileItem.KeyValue = aKeyValue;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.END_VALID_GROUPS + @".*"))
                    {
                        SymbolMark = "NULL";
                    }

                    //process the intersection points
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.INTERSECTING_POINTS + @".*"))
                    {
                        SymbolMark = "INTERSECTING-POINTS-OPEN";
                        aconfigurationFileGroup.CfgTitle = ConfigurationKeys.INTERSECTING_POINTS;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + AtoZ + @".*") && SymbolMark == "INTERSECTING-POINTS-OPEN")
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(ConfigurationLine, AtoZ, out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aConfigurationFileItem.Name = aKeyValue.Key;
                        aConfigurationFileItem.KeyValue = aKeyValue;
                        // If the score of the word is not a number
                        if (aConfigurationFileItem.KeyValue.Value != null && Regex.IsMatch(aConfigurationFileItem.KeyValue.Value, Configuration.allowedCharacters))
                        {
                            Errors.Add(String.Format(KeyValueErrors.InvalidValueError, ConfigurationLine));
                            aConfigurationFileItem.InvalidTitle = ConfigurationKeys.INTERSECTING_POINTS;
                        }
                    }
                    // If the pre-defined intersecting character is not a single character from A-Z
                    else if (Regex.IsMatch(ConfigurationLine.Split('=')[0], Configuration.allowedCharacters) && SymbolMark == "INTERSECTING-POINTS-OPEN")
                    {
                            Errors.Add(String.Format(KeyValueErrors.InvalidKeyError, ConfigurationLine));
                            aConfigurationFileItem.InvalidTitle = ConfigurationKeys.INTERSECTING_POINTS;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.END_INTERSECTION_POINTS + @".*"))
                    {
                        SymbolMark = "NULL";
                    }

                    //Process the non-intersection points
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.NON_INTERSECTING_POINTS + @".*"))
                    {
                        SymbolMark = "NON-INTERSECTING-POINTS-OPEN";
                        aconfigurationFileGroup.CfgTitle = ConfigurationKeys.NON_INTERSECTING_POINTS;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + AtoZ + @".*") && SymbolMark == "NON-INTERSECTING-POINTS-OPEN")
                    {
                        KeyValue aKeyValue;
                        if (!KeyValue.TryParse(ConfigurationLine, AtoZ, out aKeyValue))
                            Errors.AddRange(KeyValue.Errors);
                        aConfigurationFileItem.Name = aKeyValue.Key;
                        aConfigurationFileItem.KeyValue = aKeyValue;
                    }
                    else if (Regex.IsMatch(ConfigurationLine, @"^" + ConfigurationKeys.END_NON_INTERSECTING_POINTS + @".*"))
                    {
                        SymbolMark = "NULL";
                    }                  
                }
                aConfigurationFileItem.Valid = Errors.Count == 0;
                if (aConfigurationFileItem.Name != null)
                    aconfigurationFileGroup.Lines.Add(aConfigurationFileItem);
            }

            // Check the validation
            Boolean Valid = true;
            foreach (ConfigurationFileItem line in aconfigurationFileGroup.Lines)
            {
                if (line.Valid == false)
                    Valid = false;
            }
            return (Valid);
        }
    }
        #endregion
}