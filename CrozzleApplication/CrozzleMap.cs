using System;
using System.Collections.Generic;

namespace CrozzleApplication
{
    class CrozzleMap
    {
        #region constants
        const int extraRows = 2;
        const int extraColumns = 2;
        #endregion

        #region properties
        private Boolean[,] Map { get; set; }
        #endregion

        #region constructors
        public CrozzleMap(List<String[]> crozzleRows, List<String[]> crozzleColumns)
        {
            // Create a 2D array of Boolean that is initialised to false.
            // For coding "neatness", it has an extra row at the top and one at the bottom, and
            // an extra column on the left and one on the right
            int numberOfRows = crozzleRows.Count + extraRows;
            int numberOfColumns = crozzleColumns.Count + extraColumns;
            Map = new Boolean[numberOfRows, numberOfColumns];

            // Store a true value in the map at the same location as each letter
            this.MapLetters(crozzleRows);
        }
        #endregion

        #region create the Boolean map
        private void MapLetters(List<String[]> crozzleRows)
        {
            int row;
            int column;

            row = 0;
            foreach (String[] letters in crozzleRows)
            {
                row++;
                column = 0;
                foreach (String letter in letters)
                {
                    column++;
                    if (letter[0] != ' ')
                    {
                        Map[row, column] = true;
                    }
                }
            }
        }
        #endregion

        #region count groups
        public int GroupCount()
        {
            // Make a copy to restore at teh end of this function.
            Boolean[,] mapCopy = (Boolean[,])Map.Clone();

            // Count the number of groups of connected words.
            int count = 0;

            while (this.ContainsGroup())
            {
                // remove a group (representing a group of words) from the map
                this.RemoveGroup();
                count++;
            }

            // Restore map.
            Map = mapCopy;

            return (count);
        }


        private Boolean ContainsGroup()
        {
            Boolean found = false;

            foreach (Boolean status in Map)
            {
                if (status)
                {
                    found = true;
                    break;
                }
            }
            return (found);
        }
        #endregion

        #region remove a group
        private void RemoveGroup()
        {
            // Remove a group of words from the map. If all words are connected as one group, 
            // the map ends up containing only false values

            // the start position can be the location of any letter
            Coordinate start = this.FindLocation();

            // the recursive call needs a List of Coordinates
            List<Coordinate> locations = new List<Coordinate>();
            locations.Add(start);

            // remove a group
            RemoveGroup(locations);
        }

        private Coordinate FindLocation()
        {
            Boolean found = false;
            int rowLocation = -1;
            int columnLocation = -1;

            for (int row = 0; row < Map.GetLength(0) && !found; row++)
            {
                for (int column = 0; column < Map.GetLength(1) && !found; column++)
                {
                    if (Map[row, column])
                    {
                        found = true;
                        rowLocation = row;
                        columnLocation = column;
                    }
                }
            }

            return (new Coordinate(rowLocation, columnLocation));
        }

        private void RemoveGroup(List<Coordinate> locations)
        {
            // Remove a group of words from the map. If all words are connected as one group, 
            // the map ends up containing only false values

            foreach (Coordinate location in locations)
            {
                // remove letter indicator from map
                Map[location.Row, location.Column] = false;

                // get the locations of letters that are "next" to the current letter
                List<Coordinate> adjacentLocations = GetAdjacentLocations(location);

                // recursively remove more of the group of words
                RemoveGroup(adjacentLocations);
            }
        }

        private List<Coordinate> GetAdjacentLocations(Coordinate location)
        {
            List<Coordinate> adjacentLocations = new List<Coordinate>();

            if (Map[location.Row, location.Column - 1] == true)
            {
                Coordinate loc = new Coordinate(location.Row, location.Column - 1);
                adjacentLocations.Add(loc);
            }

            if (Map[location.Row, location.Column + 1] == true)
            {
                Coordinate loc = new Coordinate(location.Row, location.Column + 1);
                adjacentLocations.Add(loc);
            }

            if (Map[location.Row - 1, location.Column] == true)
            {
                Coordinate loc = new Coordinate(location.Row - 1, location.Column);
                adjacentLocations.Add(loc);
            }

            if (Map[location.Row + 1, location.Column] == true)
            {
                Coordinate loc = new Coordinate(location.Row + 1, location.Column);
                adjacentLocations.Add(loc);
            }

            return (adjacentLocations);
        }
        #endregion
    }
}