using System;
using System.Collections.Generic;

namespace CrozzleApplication
{
    class WordDataList
    {
        #region properties - errors
        public static List<String> Errors { get; set; }
        #endregion

        #region properties
        public List<TitleSequence> OriginalWordDataList { get; set; }
        public Boolean Valid { get; set; } = false;
        public List<WordData> AllWordData { get; set; }
        public List<WordData> HorizontalWordData { get; set; }
        public List<WordData> VerticalWordData { get; set; }
        #endregion

        #region properties - counting
        public int Count
        {
            get
            {
                return (AllWordData.Count);
            }
        }

        public int HorizontalCount
        {
            get
            {
                return (HorizontalWordData.Count);
            }
        }

        public int VerticalCount
        {
            get
            {
                return (VerticalWordData.Count);
            }
        }
        #endregion

        #region constructors
        public WordDataList(List<TitleSequence> originalList)
        {
            OriginalWordDataList = originalList;
            AllWordData = new List<WordData>();
            HorizontalWordData = new List<WordData>();
            VerticalWordData = new List<WordData>();
        }
        #endregion

        #region parsing
        public static Boolean TryParse(List<TitleSequence> originalWordDataList, Crozzle aCrozzle, out WordDataList aWordDataList)
        {
            List<WordData> aList = new List<WordData>();

            Errors = new List<String>();
            aWordDataList = new WordDataList(originalWordDataList);

            foreach (TitleSequence block in originalWordDataList)
            {
                WordData aWordData;
                if (WordData.TryParse(block, aCrozzle, out aWordData))
                    aWordDataList.Add(aWordData);
                else
                    Errors.AddRange(WordData.Errors);
            }
            aWordDataList.Valid = Errors.Count == 0;
            return (aWordDataList.Valid);
        }

        private void Add(WordData wordData)
        {
            AllWordData.Add(wordData);
            if(wordData.IsHorizontal)
                HorizontalWordData.Add(wordData);
            else
                VerticalWordData.Add(wordData);
        }
        #endregion
    }
}