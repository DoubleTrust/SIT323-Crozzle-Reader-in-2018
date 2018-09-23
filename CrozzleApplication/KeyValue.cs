using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CrozzleApplication
{
    public class KeyValue
    {
        #region constants
        const int KeyValueLength = 2;
        #endregion

        #region properties - errors
        public static List<String> Errors { get; set; }
        #endregion

        #region properties
        public String OriginalKeyValue { get; set; }
        public Boolean Valid { get; set; } = false;
        public String Key { get; set; } = null;
        public String Value { get; set; } = null;
        #endregion

        #region constructors
        public KeyValue(String originalKeyValueData)
        {
            OriginalKeyValue = originalKeyValueData;
        }
        #endregion

        #region parsing
        public static Boolean TryParse(String originalKeyValueData, String keyPattern, out KeyValue aKeyValue)
        {
            const String EqualsSymbol = "=";

            Errors = new List<String>();
            aKeyValue = new KeyValue(originalKeyValueData);

            if (originalKeyValueData.Contains(EqualsSymbol))
            {
                String[] originalKeyValue = originalKeyValueData.Split(new String[] { EqualsSymbol }, KeyValueLength, StringSplitOptions.None);

                // Check that the original key-value pair data has correct length.
                if (originalKeyValue.Length != KeyValueLength)
                    Errors.Add(String.Format(KeyValueErrors.FieldCountError, originalKeyValue.Length, originalKeyValueData, KeyValueLength));

                if (originalKeyValue.Length > 0)
                {
                    // Check the key field.
                    if (Regex.IsMatch(originalKeyValue[0], keyPattern))
                        aKeyValue.Key = originalKeyValue[0];
                    else
                        Errors.Add(String.Format(KeyValueErrors.InvalidKeyError, originalKeyValueData));

                    // Check the value field.
                    if (originalKeyValue[1] == null)
                        Errors.Add(String.Format(KeyValueErrors.NullValueError, originalKeyValueData));
                    else if (originalKeyValue[1] == "")
                        Errors.Add(String.Format(KeyValueErrors.MissingValueError, originalKeyValueData));
                    else
                        aKeyValue.Value = originalKeyValue[1];
                }
            }
            else
                Errors.Add(String.Format(KeyValueErrors.MissingEqualsError, originalKeyValueData));

            aKeyValue.Valid = Errors.Count == 0;
            return (aKeyValue.Valid);
        }
        #endregion
    }
}