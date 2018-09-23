using System;
using System.Collections.Generic;

namespace CrozzleApplication
{
    class Coordinate
    {
        #region properties - errors
        public static List<String> Errors { get; set; }
        #endregion

        #region properties
        private String[] OriginalCoordinate { get; set; }
        public Boolean Valid { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        #endregion

        #region constructors
        public Coordinate(String[] originalCoordinateData)
        {
            OriginalCoordinate = originalCoordinateData;
            Valid = false;
            Row = -1;
            Column = -1;
        }

        public Coordinate(int rowLocation, int columnLocation)
        {
            OriginalCoordinate = new String[] { rowLocation.ToString(), columnLocation.ToString() };
            Valid = rowLocation > 0 && columnLocation > 0;
            Row = rowLocation;
            Column = columnLocation;
        }
        #endregion

        #region parsing
        public static Boolean TryParse(String originalRowData, String originalColumnData, Crozzle aCrozzle, out Coordinate aCoordinate)
        {
            int anInteger;
            String[] originalCoordinate = new String[] { originalRowData, originalColumnData };

            Errors = new List<String>();
            aCoordinate = new Coordinate(originalCoordinate);

            // Check that the row value is an integer and in range.
            if (Validator.IsInt32(originalRowData, out anInteger))
            {
                aCoordinate.Row = anInteger;
                if (!Validator.TryRange(aCoordinate.Row, 1, aCrozzle.Rows))
                    Errors.Add(String.Format(CoordinateErrors.RowValueError, originalRowData, Validator.Errors[0]));
            }
            else
                Errors.Add(String.Format(CoordinateErrors.RowValueError, originalRowData, Validator.Errors[0]));

            // Check that the column value is an integer and in range.
            if (Validator.IsInt32(originalColumnData, out anInteger))
            {
                aCoordinate.Column = anInteger;
                if (!Validator.TryRange(aCoordinate.Column, 1, aCrozzle.Columns))
                    Errors.Add(String.Format(CoordinateErrors.ColumnValueError, originalColumnData, Validator.Errors[0]));
            }
            else
                Errors.Add(String.Format(CoordinateErrors.ColumnValueError, originalColumnData, Validator.Errors[0]));

            aCoordinate.Valid = Errors.Count == 0;
            return (aCoordinate.Valid);
        }        
        #endregion
    }
}