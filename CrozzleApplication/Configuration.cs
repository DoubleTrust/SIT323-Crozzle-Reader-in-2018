using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace CrozzleApplication
{
    class Configuration
    {
        #region constants
        public static String allowedCharacters = @"^[a-zA-Z]+$";
        public static String allowedBooleans = @"^(true|false)$";
        private static readonly Char[] PointSeparators = new Char[] { ',' };
        #endregion

        #region properties - errors
        public static List<String> Errors { get; set; }

        public String FileErrorsTXT
        {
            get
            {
                int errorNumber = 1;
                String errors = "START PROCESSING FILE: " + ConfigurationFileName + "\r\n";

                foreach (String error in Configuration.Errors)
                    errors += "error " + errorNumber++ + ": " + error + "\r\n";
                errors += "END PROCESSING FILE: " + ConfigurationFileName + "\r\n";

                return (errors);
            }
        }

        public String FileErrorsHTML
        {
            get
            {
                int errorNumber = 1;
                String errors = "<p style=\"font-weight:bold\">START PROCESSING FILE: " + ConfigurationFileName + "</p>";

                foreach (String error in Configuration.Errors)
                    errors += "<p>error " + errorNumber++ + ": " + error + "</p>";
                errors += "<p style=\"font-weight:bold\">END PROCESSING FILE: " + ConfigurationFileName + "</p>";

                return (errors);
            }
        }
        #endregion

        #region properties - configuration file validity
        public Boolean Valid { get; set; } = false;
        #endregion

        #region properties - file names
        public String ConfigurationPath { get; set; }
        public String ConfigurationFileName { get; set; }
        public String ConfigurationDirectoryName { get; set; }
        public String LogFileName { get; set; }
        #endregion

        #region properties - word list configurations
        // Limits on the size of a word list.
        public int MinimumNumberOfUniqueWords { get; set; }
        public int MaximumNumberOfUniqueWords { get; set; }
        #endregion

        #region properties - crozzle output configurations
        public String InvalidCrozzleScore { get; set; } = "";
        public Boolean Uppercase { get; set; } = true;
        public String Style { get; set; } = @"<style></style>";
        public String BGcolourEmptyTD { get; set; } = @"#ffffff";
        public String BGcolourNonEmptyTD { get; set; } = @"#ffffff";
        #endregion

        #region properties - configurations keys
        private static Boolean[] ActualIntersectingKeys { get; set; }
        private static Boolean[] ActualNonIntersectingKeys { get; set; }
        private static List<string> ActualKeys { get; set; }
        private static readonly List<string> ExpectedKeys = new List<string>()
        {
            ConfigurationKeys.LOGFILE + ConfigurationKeys.LOGFILE_NAME,
            ConfigurationKeys.SEQUENCES_IN_FILE + ConfigurationKeys.MINIMUM_NUMBER_OF_UNIQUE_WORDS,
            ConfigurationKeys.SEQUENCES_IN_FILE + ConfigurationKeys.MAXIMUM_NUMBER_OF_UNIQUE_WORDS,
            ConfigurationKeys.CROZZLE_OUTPUT + ConfigurationKeys.INVALID_CROZZLE_SCORE,
            ConfigurationKeys.CROZZLE_OUTPUT+ ConfigurationKeys.UPPERCASE,
            ConfigurationKeys.CROZZLE_OUTPUT + ConfigurationKeys.STYLE,
            ConfigurationKeys.CROZZLE_OUTPUT + ConfigurationKeys.BGCOLOUR_EMPTY_TD,
            ConfigurationKeys.CROZZLE_OUTPUT + ConfigurationKeys.BGCOLOUR_NON_EMPTY_TD,
            ConfigurationKeys.CROZZLE_SIZE + ConfigurationKeys.MINIMUM_NUMBER_OF_ROWS,
            ConfigurationKeys.CROZZLE_SIZE + ConfigurationKeys.MAXIMUM_NUMBER_OF_ROWS,
            ConfigurationKeys.CROZZLE_SIZE + ConfigurationKeys.MINIMUM_NUMBER_OF_COLUMNS,
            ConfigurationKeys.CROZZLE_SIZE + ConfigurationKeys.MAXIMUM_NUMBER_OF_COLUMNS,
            ConfigurationKeys.SEQUENCES_IN_CROZZLE + ConfigurationKeys.MINIMUM_HORIZONTAL_WORDS,
            ConfigurationKeys.SEQUENCES_IN_CROZZLE + ConfigurationKeys.MAXIMUM_HORIZONTAL_WORDS,
            ConfigurationKeys.SEQUENCES_IN_CROZZLE + ConfigurationKeys.MINIMUM_VERTICAL_WORDS,
            ConfigurationKeys.SEQUENCES_IN_CROZZLE + ConfigurationKeys.MAXIMUM_VERTICAL_WORDS,
            ConfigurationKeys.INTERSECTIONS_IN_SEQUENCES + ConfigurationKeys.MINIMUM_INTERSECTIONS_IN_HORIZONTAL_WORDS,
            ConfigurationKeys.INTERSECTIONS_IN_SEQUENCES + ConfigurationKeys.MAXIMUM_INTERSECTIONS_IN_HORIZONTAL_WORDS,
            ConfigurationKeys.INTERSECTIONS_IN_SEQUENCES + ConfigurationKeys.MAXIMUM_INTERSECTIONS_IN_VERTICAL_WORDS,
            ConfigurationKeys.INTERSECTIONS_IN_SEQUENCES + ConfigurationKeys.MAXIMUM_INTERSECTIONS_IN_VERTICAL_WORDS,
            ConfigurationKeys.DUPLICATE_SEQUENCES + ConfigurationKeys.MINIMUM_NUMBER_OF_THE_SAME_WORD,
            ConfigurationKeys.DUPLICATE_SEQUENCES + ConfigurationKeys.MAXIMUM_NUMBER_OF_THE_SAME_WORD,
            ConfigurationKeys.VALID_GROUPS + ConfigurationKeys.MINIMUM_NUMBER_OF_GROUPS,
            ConfigurationKeys.VALID_GROUPS + ConfigurationKeys.MAXIMUM_NUMBER_OF_GROUPS,
        };
        #endregion

        #region properties - crozzle configurations
        // Limits on the size of the crozzle grid.
        public int MinimumNumberOfRows { get; set; }
        public int MaximumNumberOfRows { get; set; }
        public int MinimumNumberOfColumns { get; set; }
        public int MaximumNumberOfColumns { get; set; }

        // Limits on the number of horizontal and vertical words in a crozzle.
        public int MinimumHorizontalWords { get; set; }
        public int MaximumHorizontalWords { get; set; }
        public int MinimumVerticalWords { get; set; }
        public int MaximumVerticalWords { get; set; }

        // Limits on the number of 
        // intersecting vertical words for each horizontal word, and
        // intersecting horizontal words for each vertical word.
        public int MinimumIntersectionsInHorizontalWords { get; set; }
        public int MaximumIntersectionsInHorizontalWords { get; set; }
        public int MinimumIntersectionsInVerticalWords { get; set; }
        public int MaximumIntersectionsInVerticalWords { get; set; }

        // Limits on duplicate words in the crozzle.
        public int MinimumNumberOfTheSameWord { get; set; }
        public int MaximumNumberOfTheSameWord { get; set; }

        // Limits on the number of valid word groups.
        public int MinimumNumberOfGroups { get; set; }
        public int MaximumNumberOfGroups { get; set; }
        #endregion

        #region properties - scoring configurations
        // The number of points per word within the crozzle.
        public int PointsPerWord { get; set; }

        // Points per letter that is at the intersection of a horizontal and vertical word within the crozzle.
        public int[] IntersectingPointsPerLetter { get; set; } = new int[26];

        // Points per letter that is not at the intersection of a horizontal and vertical word within the crozzle.
        public int[] NonIntersectingPointsPerLetter { get; set; } = new int[26];
        #endregion

        #region constructors
        public Configuration(String path)
        {
            ConfigurationPath = path;
            ConfigurationFileName = Path.GetFileName(path);
            ConfigurationDirectoryName = Path.GetDirectoryName(path);
        }
        #endregion

        #region parsing
        public static Boolean TryParse(String path, out Configuration aConfiguration)
        {
            Errors = new List<String>();
            ActualIntersectingKeys = new Boolean[26];
            ActualNonIntersectingKeys = new Boolean[26];
            ActualKeys = new List<string>();
            aConfiguration = new Configuration(path);

            if (aConfiguration.ConfigurationFileName.IndexOfAny(Path.GetInvalidFileNameChars()) > -1)
                Errors.Add(String.Format(ConfigurationErrors.FilenameError, path));
            else
            {
                StreamReader fileIn = new StreamReader(path);

                // Validate file.
                while (!fileIn.EndOfStream)
                {
                    // Create a Block to store a group of data
                    List<String> Block = new List<String>();
                    // Read a line
                    String line = fileIn.ReadLine();
                    while (!String.IsNullOrEmpty(line))
                    {
                        Block.Add(line);
                        line = fileIn.ReadLine();
                    }
                    // Parse a configuration item.
                    CfgGroup aConfigurationFileGroup;
                    if (ConfigurationFileItem.TryParse(Block, out aConfigurationFileGroup))
                    {
                        String title = aConfigurationFileGroup.CfgTitle;

                        for (int i = 0; i < aConfigurationFileGroup.Lines.Count; i++)
                        {
                            if (aConfigurationFileGroup.Lines[i].KeyValue != null && ActualKeys.Contains(title + aConfigurationFileGroup.Lines[i].KeyValue.Key))
                            {
                                Errors.Add(String.Format(ConfigurationErrors.DuplicateKeyError, title + aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue));
                            }
                            else
                            {
                                // Record that this key has been found.
                                if (aConfigurationFileGroup.Lines[i].KeyValue != null && title != ConfigurationKeys.NON_INTERSECTING_POINTS && title != ConfigurationKeys.INTERSECTING_POINTS)
                                    ActualKeys.Add(title + aConfigurationFileGroup.Lines[i].KeyValue.Key);

                                // Process the log file group
                                if (title == ConfigurationKeys.LOGFILE)
                                {
                                    if (aConfigurationFileGroup.Lines[i].Name == ConfigurationKeys.LOGFILE_NAME)
                                    {
                                        String logFilePath = aConfigurationFileGroup.Lines[i].KeyValue.Value.Trim('"');
                                        if (logFilePath == null || logFilePath == "")
                                        {
                                            //ConfigurationFileItemErrors.resx 加了一行, 不知道.resx的报错语句写对了没
                                            Errors.Add(ConfigurationFileItemErrors.logFilePathMissing);
                                            aConfiguration.LogFileName = "log.txt";
                                        }
                                        else
                                        {
                                            aConfiguration.LogFileName = aConfigurationFileGroup.Lines[i].KeyValue.Value.Trim();
                                            if (Validator.IsDelimited(aConfiguration.LogFileName, Crozzle.StringDelimiters))
                                            {
                                                aConfiguration.LogFileName = aConfiguration.LogFileName.Trim(Crozzle.StringDelimiters);
                                                if (!Validator.IsFilename(aConfiguration.LogFileName))
                                                    Errors.Add(String.Format(ConfigurationErrors.ValueError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                            }
                                            else
                                                Errors.Add(String.Format(ConfigurationErrors.ValueError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                    }

                                }
                                // Process the sequence in file group
                                else if (title == ConfigurationKeys.SEQUENCES_IN_FILE)
                                {
                                    if (aConfigurationFileGroup.Lines[i].Name == ConfigurationKeys.MAXIMUM_NUMBER_OF_UNIQUE_WORDS)
                                    {
                                        int maximum;
                                        if (Validator.IsInt32(aConfigurationFileGroup.Lines[i].KeyValue.Value.Trim(), out maximum))
                                        {
                                            aConfiguration.MaximumNumberOfUniqueWords = maximum;
                                            if (!Validator.TryRange(maximum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }

                                    else if (aConfigurationFileGroup.Lines[i].Name == ConfigurationKeys.MINIMUM_NUMBER_OF_UNIQUE_WORDS)
                                    {
                                        int minimum;
                                        if (Validator.IsInt32(aConfigurationFileGroup.Lines[i].KeyValue.Value.Trim(), out minimum))
                                        {
                                            aConfiguration.MinimumNumberOfUniqueWords = minimum;
                                            if (!Validator.TryRange(minimum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                }
                                // Process the crozzle output group
                                else if (title == ConfigurationKeys.CROZZLE_OUTPUT)
                                {
                                    if (aConfigurationFileGroup.Lines[i].Name == ConfigurationKeys.INVALID_CROZZLE_SCORE)
                                    {
                                        // Get the value representing an invalid score.
                                        aConfiguration.InvalidCrozzleScore = aConfigurationFileGroup.Lines[i].KeyValue.Value.Trim();
                                        if (Validator.IsDelimited(aConfiguration.InvalidCrozzleScore, Crozzle.StringDelimiters))
                                            aConfiguration.InvalidCrozzleScore = aConfiguration.InvalidCrozzleScore.Trim(Crozzle.StringDelimiters);
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                    else if (aConfigurationFileGroup.Lines[i].Name == ConfigurationKeys.UPPERCASE)
                                    {
                                        // Get the Boolean value that determines whether to display the crozzle letters in uppercase or lowercase.
                                        Boolean uppercase = true;
                                        if (!Validator.IsMatch(aConfigurationFileGroup.Lines[i].KeyValue.Value, allowedBooleans))
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        aConfiguration.Uppercase = uppercase;
                                    }
                                    else if (aConfigurationFileGroup.Lines[i].Name == ConfigurationKeys.STYLE)
                                    {
                                        // Get the value of the HTML style to display the crozzle in an HTML table.
                                        aConfiguration.Style = aConfigurationFileGroup.Lines[i].KeyValue.Value.Trim();
                                        if (Validator.IsDelimited(aConfiguration.Style, Crozzle.StringDelimiters))
                                        {
                                            aConfiguration.Style = aConfiguration.Style.Trim(Crozzle.StringDelimiters);
                                            if (!Validator.IsStyleTag(aConfiguration.Style))
                                                Errors.Add(String.Format(ConfigurationErrors.StyleError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));

                                    }
                                    else if (aConfigurationFileGroup.Lines[i].Name == ConfigurationKeys.BGCOLOUR_EMPTY_TD)
                                    {
                                        // Get the value of the background colour for an empty TD (HTML table data).
                                        aConfiguration.BGcolourEmptyTD = aConfigurationFileGroup.Lines[i].KeyValue.Value.Trim();
                                        if (!Validator.IsHexColourCode(aConfiguration.BGcolourEmptyTD))
                                            Errors.Add(String.Format(ConfigurationErrors.ColourError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));

                                    }
                                    else if (aConfigurationFileGroup.Lines[i].Name == ConfigurationKeys.BGCOLOUR_NON_EMPTY_TD)
                                    {
                                        // Get the value of the background colour for a non empty TD (HTML table data).
                                        aConfiguration.BGcolourNonEmptyTD = aConfigurationFileGroup.Lines[i].KeyValue.Value.Trim();
                                        if (!Validator.IsHexColourCode(aConfiguration.BGcolourNonEmptyTD))
                                            Errors.Add(String.Format(ConfigurationErrors.ColourError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                }
                                // Process the crozzle size
                                else if (title == ConfigurationKeys.CROZZLE_SIZE)
                                {
                                    if (aConfigurationFileGroup.Lines[i].Name == ConfigurationKeys.MINIMUM_NUMBER_OF_ROWS)
                                    {
                                        int minimum;
                                        if (Validator.IsInt32(aConfigurationFileGroup.Lines[i].KeyValue.Value.Trim(), out minimum))
                                        {
                                            aConfiguration.MinimumNumberOfRows = minimum;
                                            if (!Validator.TryRange(minimum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                    else if (aConfigurationFileGroup.Lines[i].Name == ConfigurationKeys.MAXIMUM_NUMBER_OF_ROWS)
                                    {
                                        int maximum;
                                        if (Validator.IsInt32(aConfigurationFileGroup.Lines[i].KeyValue.Value.Trim(), out maximum))
                                        {
                                            aConfiguration.MaximumNumberOfRows = maximum;
                                            if (!Validator.TryRange(maximum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));

                                    }
                                    else if (aConfigurationFileGroup.Lines[i].Name == ConfigurationKeys.MINIMUM_NUMBER_OF_COLUMNS)
                                    {
                                        // Get the value of the minimum number of columns per crozzle.
                                        int minimum;
                                        if (Validator.IsInt32(aConfigurationFileGroup.Lines[i].KeyValue.Value.Trim(), out minimum))
                                        {
                                            aConfiguration.MinimumNumberOfColumns = minimum;
                                            if (!Validator.TryRange(minimum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));

                                    }
                                    else if (aConfigurationFileGroup.Lines[i].Name == ConfigurationKeys.MAXIMUM_NUMBER_OF_COLUMNS)
                                    {
                                        // Get the value of the maximum number of columns per crozzle.
                                        int maximum;
                                        if (Validator.IsInt32(aConfigurationFileGroup.Lines[i].KeyValue.Value.Trim(), out maximum))
                                        {
                                            aConfiguration.MaximumNumberOfColumns = maximum;
                                            if (!Validator.TryRange(maximum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));

                                    }
                                }
                                // Process teh sequences in crozzle
                                else if (title == ConfigurationKeys.SEQUENCES_IN_CROZZLE)
                                {
                                    if (aConfigurationFileGroup.Lines[i].Name == ConfigurationKeys.MINIMUM_HORIZONTAL_WORDS)
                                    {
                                        // Get the value of the minimum number of horizontal words in a crozzle.
                                        int minimum;
                                        if (Validator.IsInt32(aConfigurationFileGroup.Lines[i].KeyValue.Value.Trim(), out minimum))
                                        {
                                            aConfiguration.MinimumHorizontalWords = minimum;
                                            if (!Validator.TryRange(minimum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));

                                    }
                                    else if (aConfigurationFileGroup.Lines[i].Name == ConfigurationKeys.MAXIMUM_HORIZONTAL_WORDS)
                                    {
                                        // Get the value of the maximum number of horizontal words in a crozzle.
                                        int maximum;
                                        if (Validator.IsInt32(aConfigurationFileGroup.Lines[i].KeyValue.Value.Trim(), out maximum))
                                        {
                                            aConfiguration.MaximumHorizontalWords = maximum;
                                            if (!Validator.TryRange(maximum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));

                                    }
                                    else if (aConfigurationFileGroup.Lines[i].Name == ConfigurationKeys.MINIMUM_VERTICAL_WORDS)
                                    {
                                        // Get the value of the minimum number of vertical words in a crozzle.
                                        int minimum;
                                        if (Validator.IsInt32(aConfigurationFileGroup.Lines[i].KeyValue.Value.Trim(), out minimum))
                                        {
                                            aConfiguration.MinimumVerticalWords = minimum;
                                            if (!Validator.TryRange(minimum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                    else if (aConfigurationFileGroup.Lines[i].Name == ConfigurationKeys.MAXIMUM_VERTICAL_WORDS)
                                    {
                                        // Get the value of the maximum number of vertical words in a crozzle.
                                        int maximum;
                                        if (Validator.IsInt32(aConfigurationFileGroup.Lines[i].KeyValue.Value.Trim(), out maximum))
                                        {
                                            aConfiguration.MaximumVerticalWords = maximum;
                                            if (!Validator.TryRange(maximum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));

                                    }

                                }
                                // Process the intersections in sequences
                                else if (title == ConfigurationKeys.INTERSECTIONS_IN_SEQUENCES)
                                {
                                    if (aConfigurationFileGroup.Lines[i].Name == ConfigurationKeys.MINIMUM_INTERSECTIONS_IN_HORIZONTAL_WORDS)
                                    {
                                        int minimum;
                                        if (Validator.IsInt32(aConfigurationFileGroup.Lines[i].KeyValue.Value.Trim(), out minimum))
                                        {
                                            aConfiguration.MinimumIntersectionsInHorizontalWords = minimum;
                                            if (!Validator.TryRange(minimum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));

                                    }
                                    else if (aConfigurationFileGroup.Lines[i].Name == ConfigurationKeys.MAXIMUM_INTERSECTIONS_IN_HORIZONTAL_WORDS)
                                    {
                                        // Get the value of the maximum number of the intersections in a horizontal word.
                                        int maximum;
                                        if (Validator.IsInt32(aConfigurationFileGroup.Lines[i].KeyValue.Value.Trim(), out maximum))
                                        {
                                            aConfiguration.MaximumIntersectionsInHorizontalWords = maximum;
                                            if (!Validator.TryRange(maximum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));

                                    }
                                    else if (aConfigurationFileGroup.Lines[i].Name == ConfigurationKeys.MINIMUM_INTERSECTIONS_IN_VERTICAL_WORDS)
                                    {
                                        // Get the value of the minimum number of the intersections in a vertical word.
                                        int minimum;
                                        if (Validator.IsInt32(aConfigurationFileGroup.Lines[i].KeyValue.Value.Trim(), out minimum))
                                        {
                                            aConfiguration.MinimumIntersectionsInVerticalWords = minimum;
                                            if (!Validator.TryRange(minimum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));

                                    }
                                    else if (aConfigurationFileGroup.Lines[i].Name == ConfigurationKeys.MAXIMUM_INTERSECTIONS_IN_VERTICAL_WORDS)
                                    {
                                        // Get the value of the maximum number of the intersections in a vertical word.
                                        int maximum;
                                        if (Validator.IsInt32(aConfigurationFileGroup.Lines[i].KeyValue.Value.Trim(), out maximum))
                                        {
                                            aConfiguration.MaximumIntersectionsInVerticalWords = maximum;
                                            if (!Validator.TryRange(maximum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));

                                    }
                                }
                                // Process the duplicate sequences
                                else if (title == ConfigurationKeys.DUPLICATE_SEQUENCES)
                                {
                                    if (aConfigurationFileGroup.Lines[i].Name == ConfigurationKeys.MINIMUM_NUMBER_OF_THE_SAME_WORD)
                                    {
                                        // Get the value of the minimum number of the same word per crozzle limit.
                                        int minimum;
                                        if (Validator.IsInt32(aConfigurationFileGroup.Lines[i].KeyValue.Value.Trim(), out minimum))
                                        {
                                            aConfiguration.MinimumNumberOfTheSameWord = minimum;
                                            if (!Validator.TryRange(minimum, 0, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                    else if (aConfigurationFileGroup.Lines[i].Name == ConfigurationKeys.MAXIMUM_NUMBER_OF_THE_SAME_WORD)
                                    {
                                        // Get the value of the maximum number of the same word per crozzle limit.
                                        int maximum;
                                        if (Validator.IsInt32(aConfigurationFileGroup.Lines[i].KeyValue.Value.Trim(), out maximum))
                                        {
                                            aConfiguration.MaximumNumberOfTheSameWord = maximum;
                                            if (!Validator.TryRange(maximum, 0, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));

                                    }
                                }
                                // Process the valid group
                                else if (title == ConfigurationKeys.VALID_GROUPS)
                                {
                                    if (aConfigurationFileGroup.Lines[i].Name == ConfigurationKeys.MINIMUM_NUMBER_OF_GROUPS)
                                    {
                                        // Get the value of the minimum number of groups per crozzle limit.
                                        int minimum;
                                        if (Validator.IsInt32(aConfigurationFileGroup.Lines[i].KeyValue.Value.Trim(), out minimum))
                                        {
                                            aConfiguration.MinimumNumberOfGroups = minimum;
                                            if (!Validator.TryRange(minimum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));

                                    }
                                    else if (aConfigurationFileGroup.Lines[i].Name == ConfigurationKeys.MAXIMUM_NUMBER_OF_GROUPS)
                                    {
                                        // Get the value of the maximum number of groups per crozzle limit.
                                        int maximum;
                                        if (Validator.IsInt32(aConfigurationFileGroup.Lines[i].KeyValue.Value.Trim(), out maximum))
                                        {
                                            aConfiguration.MaximumNumberOfGroups = maximum;
                                            if (!Validator.TryRange(maximum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));

                                    }
                                }
                                // Process the intersecting points
                                else if (title == ConfigurationKeys.INTERSECTING_POINTS)
                                {
                                    if (aConfigurationFileGroup.Lines[i].Name.Length != 1)
                                    {
                                        Errors.Add(String.Format(ConfigurationErrors.ValueError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                    else
                                    {
                                        int points;
                                        if (Validator.IsInt32(aConfigurationFileGroup.Lines[i].KeyValue.Value.Trim(), out points))
                                        {
                                            int index = (int)aConfigurationFileGroup.Lines[i].KeyValue.Key[0] - (int)'A';
                                            aConfiguration.IntersectingPointsPerLetter[index] = points;
                                            ActualIntersectingKeys[index] = true;
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                }
                                // Processor the non-intersecting points
                                else if (title == ConfigurationKeys.NON_INTERSECTING_POINTS)
                                {
                                    if (aConfigurationFileGroup.Lines[i].Name.Length != 1)
                                    {
                                        Errors.Add(String.Format(ConfigurationErrors.ValueError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                    else
                                    {
                                        int points;
                                        if (Validator.IsInt32(aConfigurationFileGroup.Lines[i].KeyValue.Value.Trim(), out points))
                                        {
                                            int index = (int)aConfigurationFileGroup.Lines[i].KeyValue.Key[0] - (int)'A';
                                            aConfiguration.NonIntersectingPointsPerLetter[index] = points;
                                            ActualNonIntersectingKeys[index] = true;
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aConfigurationFileGroup.Lines[i].KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Errors.AddRange(ConfigurationFileItem.Errors);
                        // Validate the rest characters
                        for (int i = 0; i < aConfigurationFileGroup.Lines.Count; i++)
                        {
                            int points;
                            if (aConfigurationFileGroup.Lines[i].KeyValue.Value != null)
                            {
                                if (Validator.IsInt32(aConfigurationFileGroup.Lines[i].KeyValue.Value.Trim(), out points))
                                {
                                    int index = (int)aConfigurationFileGroup.Lines[i].KeyValue.Key[0] - (int)'A';
                                    aConfiguration.IntersectingPointsPerLetter[index] = points;
                                    ActualIntersectingKeys[index] = true;
                                }
                            }
                        }
                    }                    
                }
            
                 // Close files.
                fileIn.Close();

                 // Check which keys are missing from the configuration file.
                 foreach (string expectedKey in ExpectedKeys)
                     if (!ActualKeys.Contains(expectedKey))
                         Errors.Add(String.Format(ConfigurationErrors.MissingKeyError, expectedKey));
                 for (char ch = 'A'; ch <= 'Z'; ch++)
                     if (!ActualIntersectingKeys[(int)ch - (int)'A'])
                         Errors.Add(String.Format(ConfigurationErrors.MissingIntersectionKeyError, ch.ToString()));
                 for (char ch = 'A'; ch <= 'Z'; ch++)
                     if (!ActualNonIntersectingKeys[(int)ch - (int)'A'])
                         Errors.Add(String.Format(ConfigurationErrors.MissingNonIntersectionKeyError, ch.ToString()));

                // Check that minimum values are <= to their maximmum counterpart values.
                if (ActualKeys.Contains("SEQUENCES-IN-FILEMINIMUM") && ActualKeys.Contains("SEQUENCES-IN-FILEMAXIMUM"))
                    if (aConfiguration.MinimumNumberOfUniqueWords > aConfiguration.MaximumNumberOfUniqueWords)
                        Errors.Add(String.Format(ConfigurationErrors.MinGreaterThanMaxError, "MINIMUM_NUMBER_OF_UNIQUE_WORDS",
                            aConfiguration.MinimumNumberOfUniqueWords, aConfiguration.MaximumNumberOfUniqueWords));
                if (ActualKeys.Contains("CROZZLE-SIZEMINIMUM-ROWS") && ActualKeys.Contains("CROZZLE-SIZEMAXIMUM-ROWS"))
                    if (aConfiguration.MinimumNumberOfRows > aConfiguration.MaximumNumberOfRows)
                        Errors.Add(String.Format(ConfigurationErrors.MinGreaterThanMaxError, "MINIMUM_NUMBER_OF_ROWS",
                            aConfiguration.MinimumNumberOfRows, aConfiguration.MaximumNumberOfRows));
                if (ActualKeys.Contains("CROZZLE-SIZEMINIMUM-COLUMNS") && ActualKeys.Contains("CROZZLE-SIZEMAXIMUM-COLUMNS"))
                    if (aConfiguration.MinimumNumberOfColumns > aConfiguration.MaximumNumberOfColumns)
                        Errors.Add(String.Format(ConfigurationErrors.MinGreaterThanMaxError, "MINIMUM_NUMBER_OF_COLUMNS",
                            aConfiguration.MinimumNumberOfColumns, aConfiguration.MaximumNumberOfColumns));
                if (ActualKeys.Contains("SEQUENCES-IN-CROZZLEMINIMUM-HORIZONTAL") && ActualKeys.Contains("SEQUENCES-IN-CROZZLEMAXIMUM-HORIZONTAL"))
                    if (aConfiguration.MinimumHorizontalWords > aConfiguration.MaximumHorizontalWords)
                        Errors.Add(String.Format(ConfigurationErrors.MinGreaterThanMaxError, "MINIMUM_HORIZONTAL_WORDS",
                            aConfiguration.MinimumHorizontalWords, aConfiguration.MaximumHorizontalWords));
                if (ActualKeys.Contains("SEQUENCES-IN-CROZZLEMINIMUM-VERTICAL") && ActualKeys.Contains("SEQUENCES-IN-CROZZLEMAXIMUM-VERTICAL"))
                    if (aConfiguration.MinimumVerticalWords > aConfiguration.MaximumVerticalWords)
                        Errors.Add(String.Format(ConfigurationErrors.MinGreaterThanMaxError, "MINIMUM_VERTICAL_WORDS",
                            aConfiguration.MinimumVerticalWords, aConfiguration.MaximumVerticalWords));
                if (ActualKeys.Contains("INTERSECTIONS-IN-SEQUENCESMINIMUM-HORIZONTAL") && ActualKeys.Contains("INTERSECTIONS-IN-SEQUENCESMAXIMUM-HORIZONTAL"))
                    if (aConfiguration.MinimumIntersectionsInHorizontalWords > aConfiguration.MaximumIntersectionsInHorizontalWords)
                        Errors.Add(String.Format(ConfigurationErrors.MinGreaterThanMaxError, "MINIMUM_INTERSECTIONS_IN_HORIZONTAL_WORDS",
                            aConfiguration.MinimumIntersectionsInHorizontalWords, aConfiguration.MaximumIntersectionsInHorizontalWords));
                if (ActualKeys.Contains("INTERSECTIONS-IN-SEQUENCESMINIMUM-VERTICAL") && ActualKeys.Contains("INTERSECTIONS-IN-SEQUENCESMAXIMUM-VERTICAL"))
                    if (aConfiguration.MinimumIntersectionsInVerticalWords > aConfiguration.MaximumIntersectionsInVerticalWords)
                        Errors.Add(String.Format(ConfigurationErrors.MinGreaterThanMaxError, "MIIMUM_INTERSECTIONS_IN_VERTICAL_WORDS",
                            aConfiguration.MinimumIntersectionsInVerticalWords, aConfiguration.MaximumIntersectionsInVerticalWords));
                if (ActualKeys.Contains("DUPLICATE-SEQUENCESMINIMUM") && ActualKeys.Contains("DUPLICATE-SEQUENCESMAXIMUM"))
                    if (aConfiguration.MinimumNumberOfTheSameWord > aConfiguration.MaximumNumberOfTheSameWord)
                        Errors.Add(String.Format(ConfigurationErrors.MinGreaterThanMaxError, "MINIMUM_NUMBER_OF_THE_SAME_WORD",
                            aConfiguration.MinimumNumberOfTheSameWord, aConfiguration.MaximumNumberOfTheSameWord));
                if (ActualKeys.Contains("VALID-GROUPSMAXIMUM") && ActualKeys.Contains("VALID-GROUPSMINIMUM"))
                    if (aConfiguration.MinimumNumberOfGroups > aConfiguration.MaximumNumberOfGroups)
                        Errors.Add(String.Format(ConfigurationErrors.MinGreaterThanMaxError, "MINIMUM_NUMBER_OF_GROUPS",
                            aConfiguration.MinimumNumberOfGroups, aConfiguration.MaximumNumberOfGroups));
            }

            // Store validity.
            aConfiguration.Valid = Errors.Count == 0;
            return (aConfiguration.Valid);
        }
        #endregion
    }
}