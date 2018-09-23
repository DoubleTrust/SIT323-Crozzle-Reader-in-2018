using System;
using System.Collections.Generic;
using System.Linq;

namespace CrozzleApplication
{
    class CrozzleSequences
    {
        #region properties - errors
        public List<String> ErrorMessages { get; set; }
        #endregion

        #region properties
        private List<WordData> Sequences { get; set; }
        private List<WordData> HorizontalSequences { get; set; }
        private List<WordData> VerticalSequences { get; set; }
        public Configuration Configuration { get; set; }

        public int DuplicateWordsCount { get; set; }

        public int Count
        {
            get { return (HorizontalSequences.Count + VerticalSequences.Count); }
        }

        public Boolean ErrorsDetected
        {
            get { return (ErrorMessages.Count > 0); }
        }
        #endregion

        #region constructors
        public CrozzleSequences(List<String[]> crozzleRows, List<String[]> crozzleColumns, Configuration aConfiguration)
        {
            Sequences = new List<WordData>();
            HorizontalSequences = new List<WordData>();
            VerticalSequences = new List<WordData>();
            ErrorMessages = new List<string>();
            Configuration = aConfiguration;
            DuplicateWordsCount = 0;

            this.AddHorizontalSequences(crozzleRows);
            this.AddVerticalSequences(crozzleColumns);
        }
        #endregion

        #region identify sequences
        private void AddHorizontalSequences(List<String[]> crozzleRows)
        {
            int rowNumber = 0;
            int columnIndex;
            String row;
            foreach (String[] crozzleRow in crozzleRows)
            {
                rowNumber++;
                columnIndex = 0;

                // Place all letters into one string, so that we can split it later.
                row = "";
                foreach (String letter in crozzleRow)
                    row = row + letter;

                // Use split to collect all sequences of letters.
                String[] letterSequences = row.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Collect and store data about each letter sequence of length > 1, as a sequence of one letter is not a word.
                foreach (String sequence in letterSequences)
                {
                    if (sequence.Length > 1)
                    {
                        // Set column values.
                        int columnNumber = row.IndexOf(sequence, columnIndex) + 1;

                        //////// check for duplicate word
                        //////WordData duplicate = Sequences.Find(x => x.Letters.Equals(sequence));
                        //////if (duplicate != null)
                        //////    ErrorMessages.Add("\"" + sequence + "\" at (" + rowNumber + ", " + columnNumber + ") already exists in the crozzle at (" + duplicate.Location.Row + ", " + duplicate.Location.Column + ")");


                        //////// Check that duplicate words are within limits.
                        //////List<WordData> duplicates = Sequences.FindAll(x => x.Letters.Equals(sequence));
                        //////if (duplicates.Count < Configuration.MinimumNumberOfTheSameWord)
                        //////    ErrorMessages.Add("\"" + sequence + "\" at (" + rowNumber + ", " + columnNumber + ") exists in the crozzle " + duplicates.Count +
                        //////        " times, which is more than the limit of " + Configuration.MinimumNumberOfTheSameWord);
                        //////if (duplicates.Count > Configuration.MaximumNumberOfTheSameWord)
                        //////    ErrorMessages.Add("\"" + sequence + "\" at (" + rowNumber + ", " + columnNumber + ") exists in the crozzle " + duplicates.Count +
                        //////        " times, which is more than the limit of " + Configuration.MaximumNumberOfTheSameWord);

                        // Collect data about the word, and 
                        // update the index for the next substring search.
                        WordData word = new WordData(WordData.OrientationRow, rowNumber, row.IndexOf(sequence, columnIndex) + 1, sequence);
                        columnIndex = word.Location.Column - 1 + sequence.Length;

                        // Store data about the word.
                        Sequences.Add(word);
                        HorizontalSequences.Add(word);
                    }
                }
            }
        }

        private void AddVerticalSequences(List<String[]> crozzleColumns)
        {
            int columnNumber = 0;
            int rowIndex;
            String column;
            foreach (String[] crozzleColumn in crozzleColumns)
            {
                columnNumber++;
                rowIndex = 0;

                // Place all letters into one string, so that we can split it later.
                column = "";
                foreach (String letter in crozzleColumn)
                    column = column + letter;

                // Use split to collect all sequences of letters.
                String[] letterSequences = column.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Collect and store data about each letter sequence of length > 1, as a sequence of one letter is not a word.
                foreach (String sequence in letterSequences)
                {
                    if (sequence.Length > 1)
                    {
                        // Set row values.
                        int rowNumber = column.IndexOf(sequence, rowIndex) + 1;

                        ////////// Check that duplicate words are within limits.
                        ////////List<WordData> duplicates = Sequences.FindAll(x => x.Letters.Equals(sequence));
                        ////////if (duplicates.Count < Configuration.MinimumNumberOfTheSameWord)
                        ////////    ErrorMessages.Add("\"" + sequence + "\" at (" + rowNumber + ", " + columnNumber + ") exists in the crozzle " + duplicates.Count + 
                        ////////        " times, which is more than the limit of " + Configuration.MinimumNumberOfTheSameWord);
                        ////////if (duplicates.Count > Configuration.MaximumNumberOfTheSameWord)
                        ////////    ErrorMessages.Add("\"" + sequence + "\" at (" + rowNumber + ", " + columnNumber + ") exists in the crozzle " + duplicates.Count +
                        ////////        " times, which is more than the limit of " + Configuration.MaximumNumberOfTheSameWord);

                        // Collect data about the word, and 
                        // update the index for the next substring search.
                        WordData word = new WordData(WordData.OrientationColumn, rowNumber, columnNumber, sequence);
                        rowIndex = word.Location.Row - 1 + sequence.Length;

                        // Store data about the word.
                        Sequences.Add(word);
                        VerticalSequences.Add(word);
                    }
                }
            }
        }
        #endregion
        
        #region intersecting letters
        public List<Char> GetIntersectingLetters()
        {
            List<Char> intersectingLetters = new List<Char>();

            foreach (WordData horizontalSequence in HorizontalSequences)
                intersectingLetters.AddRange(GetIntersectingLetters(horizontalSequence));
            return (intersectingLetters);
        }

        private List<Char> GetIntersectingLetters(WordData horizontalWord)
        {
            List<Char> intersectingLetters = new List<Char>();

            foreach (WordData verticalSequence in VerticalSequences)
            {
                if (verticalSequence.Location.Row == horizontalWord.Location.Row)
                {
                    if (verticalSequence.Location.Column >= horizontalWord.Location.Column &&
                        verticalSequence.Location.Column < horizontalWord.Location.Column + horizontalWord.Letters.Length)
                        intersectingLetters.Add(verticalSequence.Letters[0]);
                }
                else if (verticalSequence.Location.Row < horizontalWord.Location.Row)
                {
                    if (verticalSequence.Location.Column >= horizontalWord.Location.Column &&
                        verticalSequence.Location.Column < horizontalWord.Location.Column + horizontalWord.Letters.Length &&
                        verticalSequence.Location.Row + verticalSequence.Letters.Length > horizontalWord.Location.Row)
                        intersectingLetters.Add(verticalSequence.Letters[horizontalWord.Location.Row - verticalSequence.Location.Row]);
                }
            }
            return (intersectingLetters);
        }
        #endregion

        #region missing words
        public void FindMissingWords(WordList wordList)
        {
            foreach (WordData sequence in Sequences)
                if (!wordList.Contains(sequence.Letters))
                    ErrorMessages.Add(String.Format(CrozzleErrors.MissingFromWordlistError, sequence.Letters, sequence.Location.Row, sequence.Location.Column));
        }
        #endregion

        #region check duplicate words
        public void CheckDuplicateWords(int lowerLimit, int upperLimit)
        {
            // Create unique sequences.
            List<string> uniqueSequences = new List<string>();
            foreach (WordData sequence in Sequences)
                if (!uniqueSequences.Contains(sequence.Letters))
                    uniqueSequences.Add(sequence.Letters);

            // Check the number of occurances.
            foreach (String letters in uniqueSequences)
            {
                List<WordData> duplicates = Sequences.FindAll(x => x.Letters.Equals(letters));

                if (duplicates.Count > 1)
                {
                    int originalErrorMessage = ErrorMessages.Count;

                    if (duplicates.Count < Configuration.MinimumNumberOfTheSameWord || duplicates.Count > Configuration.MaximumNumberOfTheSameWord)
                        ErrorMessages.Add(String.Format(CrozzleErrors.DuplicateWordCountError, 
                            letters, duplicates.Count, Configuration.MinimumNumberOfTheSameWord, Configuration.MaximumNumberOfTheSameWord));
                    // For unit test only
                    if (ErrorMessages.Count != originalErrorMessage)
                        DuplicateWordsCount++; 
                }
            }
        }
        #endregion

        #region check intersections
        public void CheckHorizontalIntersections(int lowerLimit, int upperLimit)
        {
            foreach (WordData sequence in Sequences)
            {
                if (sequence.IsHorizontal)
                {
                    int verticalIntersections = GetVerticalIntersectingWords(sequence).Count;
                    if (verticalIntersections < lowerLimit || verticalIntersections > upperLimit)
                        ErrorMessages.Add(String.Format(CrozzleErrors.VerticalIntersectionsError, 
                            sequence.Letters, verticalIntersections, lowerLimit, upperLimit));
                }
            }
        }

        public void CheckVerticalIntersections(int lowerLimit, int upperLimit)
        {
            foreach (WordData sequence in Sequences)
            {
                if (sequence.IsVertical)
                {
                    int horizontalIntersections = GetHorizontalIntersectingWords(sequence).Count;
                    if (horizontalIntersections < lowerLimit || horizontalIntersections > upperLimit)
                        ErrorMessages.Add(String.Format(CrozzleErrors.HorizontalIntersectionsError,
                            sequence.Letters, horizontalIntersections, lowerLimit, upperLimit));
                }
            }
        }

        private List<WordData> GetVerticalIntersectingWords(WordData horizontalWord)
        {
            List<WordData> verticalWords = new List<WordData>();

            foreach (WordData verticalSequence in VerticalSequences)
            {
                if (verticalSequence.Location.Row == horizontalWord.Location.Row)
                {
                    if (verticalSequence.Location.Column >= horizontalWord.Location.Column &&
                        verticalSequence.Location.Column < horizontalWord.Location.Column + horizontalWord.Letters.Length)
                        verticalWords.Add(horizontalWord);
                }
                else if (verticalSequence.Location.Row < horizontalWord.Location.Row)
                {
                    if (verticalSequence.Location.Column >= horizontalWord.Location.Column &&
                        verticalSequence.Location.Column < horizontalWord.Location.Column + horizontalWord.Letters.Length &&
                        verticalSequence.Location.Row + verticalSequence.Letters.Length > horizontalWord.Location.Row)
                            verticalWords.Add(horizontalWord);
                }
            }
            return (verticalWords);
        }

        private List<WordData> GetHorizontalIntersectingWords(WordData verticalWord)
        {
            List<WordData> horizontalWords = new List<WordData>();

            foreach (WordData horizontalSequence in HorizontalSequences)
            {
                if (horizontalSequence.Location.Column == verticalWord.Location.Column)
                {
                    if (horizontalSequence.Location.Row >= verticalWord.Location.Row &&
                        horizontalSequence.Location.Row < verticalWord.Location.Row + verticalWord.Letters.Length)
                        horizontalWords.Add(horizontalSequence);
                }
                else if (horizontalSequence.Location.Column < verticalWord.Location.Column)
                {
                    if (horizontalSequence.Location.Row >= verticalWord.Location.Row &&
                        horizontalSequence.Location.Row < verticalWord.Location.Row + verticalWord.Letters.Length &&
                        horizontalSequence.Location.Column + horizontalSequence.Letters.Length > verticalWord.Location.Column)
                        horizontalWords.Add(horizontalSequence);
                }
            }
            return (horizontalWords);
        }
        #endregion

        #region check touching words
        public void CheckTouchingWords()
        {
            CheckTouchingHorizontalWords();
            CheckTouchingVerticalWords();          
        }
        
        private void CheckTouchingHorizontalWords()
        {
            WordData sequence1;
            WordData sequence2;

            for (int i = 0; i < HorizontalSequences.Count; i++)
            {
                sequence1 = HorizontalSequences[i];

                for (int j = i + 1; j < HorizontalSequences.Count; j++)
                {
                    sequence2 = HorizontalSequences[j];

                    if (sequence1.Letters.Equals(sequence2.Letters, StringComparison.Ordinal))
                        continue;

                    if (sequence2.Location.Row >= sequence1.Location.Row - 1 && sequence2.Location.Row <= sequence1.Location.Row + 1)
                    {
                        if (sequence2.Location.Column < sequence1.Location.Column - 1 && sequence2.Location.Column + sequence2.Letters.Length >= sequence1.Location.Column)
                            ErrorMessages.Add("the horizontal word \"" + sequence1.Letters + "\" on row " + sequence1.Location.Row + " is touching the horizontal word \"" + sequence2.Letters + "\" on row " + sequence2.Location.Row);
                        else if (sequence2.Location.Column >= sequence1.Location.Column - 1 && sequence2.Location.Column <= sequence1.Location.Column + sequence1.Letters.Length)
                            ErrorMessages.Add("the horizontal word \"" + sequence1.Letters + "\" on row " + sequence1.Location.Row + " is touching the horizontal word \"" + sequence2.Letters + "\" on row " + sequence2.Location.Row);
                    }
                }
            }
        }

        private void CheckTouchingVerticalWords()
        {
            WordData sequence1;
            WordData sequence2;

            for (int i = 0; i < VerticalSequences.Count; i++)
            {
                sequence1 = VerticalSequences[i];

                for (int j = i + 1; j < VerticalSequences.Count; j++)
                {
                    sequence2 = VerticalSequences[j];

                    if (sequence1.Letters.Equals(sequence2.Letters, StringComparison.Ordinal))
                        continue;

                    if (sequence2.Location.Column >= sequence1.Location.Column - 1 && sequence2.Location.Column <= sequence1.Location.Column + 1)
                    {
                        if (sequence2.Location.Row < sequence1.Location.Row - 1 && sequence2.Location.Row + sequence2.Letters.Length >= sequence1.Location.Row)
                            ErrorMessages.Add("the vertical word \"" + sequence1.Letters + "\" on column " + sequence1.Location.Column + " is touching the vertical word \"" + sequence2.Letters + "\" on column " + sequence2.Location.Column);
                        else if (sequence2.Location.Row >= sequence1.Location.Row - 1 && sequence2.Location.Row <= sequence1.Location.Row + sequence1.Letters.Length)
                            ErrorMessages.Add("the vertical word \"" + sequence1.Letters + "\" on column " + sequence1.Location.Column + " is touching the vertical word \"" + sequence2.Letters + "\" on column " + sequence2.Location.Column);
                    }
                }
            }
        }
        #endregion

        #region word-group connectivity
        public void CheckConnectivity(int lowerLimit, int upperLimit, List<String[]> crozzleRows, List<String[]> crozzleColumns)
        {
            CrozzleMap map = new CrozzleMap(crozzleRows, crozzleColumns);
            int count = map.GroupCount();

            // Check whether the number of groups is within the limit.
            if (count < lowerLimit || count > upperLimit)
                ErrorMessages.Add(String.Format(CrozzleErrors.ConnectivityError, count, lowerLimit, upperLimit));
        }
        #endregion
    }
}