using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CrozzleApplication
{
    public class Orientation
    {
        #region constants
        public const String Row = "HORIZONTAL-SEQUENCES";
        public const String Column = "VERTICAL-SEQUENCES";
        public static readonly String Pattern = String.Format("^({0}|{1})$", Row, Column);
        #endregion

        #region properties - errors
        public static List<String> Errors { get; set; }
        #endregion        

        #region properties
        public String OriginalDirection { get; set; }
        public Boolean Valid { get; set; }
        public String Direction { get; set; }
        #endregion        

        #region properties - testing
        public Boolean IsHorizontal
        {
            get { return (Direction.Equals(Row, StringComparison.Ordinal)); }
        }

        public Boolean IsVertical
        {
            get { return (Direction.Equals(Column, StringComparison.Ordinal)); }
        }
        #endregion

        #region constructors
        public Orientation(String originalOrientationData)
        {
            OriginalDirection = originalOrientationData;
            Valid = false;
            Direction = null;
        }
        #endregion

        #region parsing
        public static Boolean TryParse(String originalOrientationData, out Orientation anOrientation)
        {
            Errors = new List<String>();
            anOrientation = new Orientation(originalOrientationData);

            anOrientation.Valid = false;

            if (Regex.IsMatch(originalOrientationData, Pattern))
            {
                anOrientation.Direction = originalOrientationData;
                anOrientation.Valid = true;
            }
            else
                Errors.Add(String.Format(OrientationErrors.SymbolError, originalOrientationData));

            return (anOrientation.Valid);
        }
        #endregion
    }    
}
