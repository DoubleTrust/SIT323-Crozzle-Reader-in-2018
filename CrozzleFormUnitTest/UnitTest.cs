using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CrozzleApplication;
namespace CrozzleFormUnitTest
{
    /* NOTE: All test files are put into "CrozzleFormUnitTest" project folder (as you can see on the right side) , We created a folder called "AssignmentFiles" and you can find those files inside it.*/
    [TestClass]
    [DeploymentItem("AssignmentFiles")]
    public class UnitTest
    {
        Configuration aConfiguration1, aConfiguration2, aConfiguration3;
        WordList aWordList1, aWordList2, aWordList3;
        Crozzle aCrozzle1, aCrozzle2, aCrozzle3;

        [TestMethod]
        public void Test_Validator_IsBoolean()
        {
            // Arrange
            String Test1 = "true";
            String Test2 = "false";
            Boolean Test1Result, Test2Result;

            // Act
            Validator.IsBoolean(Test1, out Test1Result);
            Validator.IsBoolean(Test2, out Test2Result);

            // Assert
            Assert.AreEqual(true, Test1Result, "error");
            Assert.AreEqual(false, Test2Result, "error");
        }

        [TestMethod]
        public void Test_Validator_IsInt32()
        {
            // Arrange
            String Test1 = "123";
            String Test2 = "aaa";
            String Test3 = "3.5";
            int Test1Result, Test2Result, Test3Result;

            // Act
            Validator.IsInt32(Test1, out Test1Result);
            Validator.IsInt32(Test2, out Test2Result);
            Validator.IsInt32(Test3, out Test3Result);

            // Assert
            Assert.AreEqual(123, Test1Result, "Error");
            Assert.AreEqual(0, Test2Result, "Error");
            Assert.AreEqual(0, Test3Result, "Error");
        }

        [TestMethod]
        public void Test_Validator_IsHexColourCode() {

            // Arrange
            String Test1 = "#123456";
            String Test2 = "#asdfwr";
            String Test3 = "#123e";
            String Test4 = "#1234567";

            // Act
            Boolean testResult1 = Validator.IsHexColourCode(Test1);
            Boolean testResult2 = Validator.IsHexColourCode(Test2);
            Boolean testResult3 = Validator.IsHexColourCode(Test3);
            Boolean testResult4 = Validator.IsHexColourCode(Test4);

            // Assert
            Assert.AreEqual(true, testResult1, "Error");
            Assert.AreEqual(false, testResult2, "Error");
            Assert.AreEqual(false, testResult3, "Error");
            Assert.AreEqual(false, testResult4, "Error");
        }

        [TestMethod]
        public void Test_KeyValue_TryParse()
        {
            // Arrange
            String Test1 = "MINIMUM-ROWS=5";
            String Test2 = "MAXIMUMROWS=500";
            String Test3 = "MAXIMUM-ROWS500";
            KeyValue aKeyValue;

            // Act
            Boolean Test1Result = KeyValue.TryParse(Test1, ConfigurationKeys.MINIMUM_NUMBER_OF_ROWS, out aKeyValue);
            Boolean Test2Result = KeyValue.TryParse(Test2, ConfigurationKeys.MAXIMUM_NUMBER_OF_ROWS, out aKeyValue);
            Boolean Test3Result = KeyValue.TryParse(Test3, ConfigurationKeys.MAXIMUM_NUMBER_OF_ROWS, out aKeyValue);

            // Assert
            Assert.AreEqual(true, Test1Result, "error");
            Assert.AreEqual(false, Test2Result, "error");
            Assert.AreEqual(false, Test3Result, "error");
        }

        [TestMethod]
        public void Test_CrozzleSequences_CheckDuplicateWords()
        {
            // Arrange
            setAttributes();
            CrozzleSequences aCrozzle1Sequences = new CrozzleSequences(aCrozzle1.getCrozzleRows(), aCrozzle1.getCrozzleColumns(), aCrozzle1.Configuration);
            CrozzleSequences aCrozzle2Sequences = new CrozzleSequences(aCrozzle2.getCrozzleRows(), aCrozzle2.getCrozzleColumns(), aCrozzle2.Configuration);
            CrozzleSequences aCrozzle3Sequences = new CrozzleSequences(aCrozzle3.getCrozzleRows(), aCrozzle3.getCrozzleColumns(), aCrozzle3.Configuration);

            // Act
            aCrozzle1Sequences.CheckDuplicateWords(aCrozzle1.Configuration.MinimumNumberOfTheSameWord, aCrozzle1.Configuration.MaximumNumberOfTheSameWord);
            aCrozzle2Sequences.CheckDuplicateWords(aCrozzle2.Configuration.MinimumNumberOfTheSameWord, aCrozzle2.Configuration.MaximumNumberOfTheSameWord);
            aCrozzle3Sequences.CheckDuplicateWords(aCrozzle3.Configuration.MinimumNumberOfTheSameWord, aCrozzle3.Configuration.MaximumNumberOfTheSameWord);

            // Assert
            Assert.AreEqual(0, aCrozzle1Sequences.DuplicateWordsCount, "error");
            Assert.AreEqual(3, aCrozzle2Sequences.DuplicateWordsCount, "error");
            Assert.AreEqual(0, aCrozzle3Sequences.DuplicateWordsCount, "error");
        }

        [TestMethod]
        public void Test_Crozzle_ToStrngHTML()
        {
            // Arrange
            setAttributes();
            String Test1 = @"<!DOCTYPE html><html><head><style> table, td { border: 1px solid black; border-collapse: collapse; } td { width:24px; height:18px; text-align: center; } </style><style>
                       .empty { background-color: " + "#777777" + @"; }
                       .nonempty { background-color: " + "#ffffff" + @"; }
                       </style>" + @"</head><body><table><tr><td class=""nonempty"">P</td><td class=""nonempty"">E</td><td class=""nonempty"">T</td><td class=""nonempty"">E</td><td class=""nonempty"">R</td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">R</td><td class=""nonempty"">O</td><td class=""nonempty"">N</td><td class=""nonempty"">A</td><td class=""nonempty"">L</td><td class=""nonempty"">D</td></tr><tr><td class=""nonempty"">A</td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">O</td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td></tr><tr><td class=""nonempty"">M</td><td class=""nonempty"">A</td><td class=""nonempty"">R</td><td class=""nonempty"">K</td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">G</td><td class=""nonempty"">R</td><td class=""nonempty"">A</td><td class=""nonempty"">H</td><td class=""nonempty"">A</td><td class=""nonempty"">M</td></tr><tr><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">O</td><td class=""empty""> </td><td class=""nonempty"">W</td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">E</td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">A</td><td class=""empty""> </td><td class=""empty""> </td></tr><tr><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">B</td><td class=""nonempty"">R</td><td class=""nonempty"">E</td><td class=""nonempty"">N</td><td class=""nonempty"">D</td><td class=""nonempty"">A</td><td class=""empty""> </td><td class=""nonempty"">R</td><td class=""empty""> </td><td class=""nonempty"">F</td><td class=""nonempty"">R</td><td class=""nonempty"">E</td><td class=""nonempty"">D</td></tr><tr><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">E</td><td class=""empty""> </td><td class=""nonempty"">N</td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">A</td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">R</td><td class=""empty""> </td><td class=""empty""> </td></tr><tr><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">R</td><td class=""empty""> </td><td class=""nonempty"">D</td><td class=""empty""> </td><td class=""nonempty"">G</td><td class=""empty""> </td><td class=""nonempty"">L</td><td class=""nonempty"">A</td><td class=""nonempty"">R</td><td class=""nonempty"">R</td><td class=""nonempty"">Y</td><td class=""empty""> </td><td class=""empty""> </td></tr><tr><td class=""nonempty"">B</td><td class=""nonempty"">E</td><td class=""nonempty"">T</td><td class=""nonempty"">T</td><td class=""nonempty"">Y</td><td class=""empty""> </td><td class=""nonempty"">A</td><td class=""empty""> </td><td class=""nonempty"">A</td><td class=""empty""> </td><td class=""nonempty"">O</td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td></tr><tr><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">O</td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">R</td><td class=""nonempty"">O</td><td class=""nonempty"">N</td><td class=""empty""> </td><td class=""nonempty"">S</td><td class=""nonempty"">U</td><td class=""nonempty"">S</td><td class=""nonempty"">A</td><td class=""nonempty"">N</td></tr><tr><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">M</td><td class=""nonempty"">A</td><td class=""nonempty"">R</td><td class=""nonempty"">Y</td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">E</td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">L</td><td class=""empty""> </td></tr></table><p>Crozzle file is valid.</p><p>Configuration file is valid.</p><p>Word list file is valid.</p><p></p></body></html>";

            String Test2 = @"<!DOCTYPE html><html><head><style> table, td { border: 1px solid black; border-collapse: collapse; } td { width:24px; height:18px; text-align: center; } </style><style>
                       .empty { background-color: #777777; }
                       .nonempty { background-color: #ffffff; }
                       </style>" + @"</head><body><table><tr><td class=""empty""> </td><td class=""nonempty"">R</td><td class=""nonempty"">O</td><td class=""nonempty"">B</td><td class=""nonempty"">E</td><td class=""nonempty"">R</td><td class=""nonempty"">T</td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">O</td><td class=""nonempty"">S</td><td class=""nonempty"">C</td><td class=""nonempty"">A</td><td class=""nonempty"">R</td></tr><tr><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">I</td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">A</td><td class=""nonempty"">L</td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">H</td><td class=""empty""> </td><td class=""nonempty"">O</td></tr><tr><td class=""empty""> </td><td class=""nonempty"">J</td><td class=""nonempty"">I</td><td class=""nonempty"">L</td><td class=""nonempty"">L</td><td class=""empty""> </td><td class=""nonempty"">A</td><td class=""nonempty"">L</td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">A</td><td class=""empty""> </td><td class=""nonempty"">S</td></tr><tr><td class=""empty""> </td><td class=""nonempty"">E</td><td class=""empty""> </td><td class=""nonempty"">L</td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">A</td><td class=""empty""> </td><td class=""nonempty"">G</td><td class=""nonempty"">E</td><td class=""nonempty"">O</td><td class=""nonempty"">R</td><td class=""nonempty"">G</td><td class=""nonempty"">E</td></tr><tr><td class=""empty""> </td><td class=""nonempty"">S</td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">B</td><td class=""empty""> </td><td class=""nonempty"">N</td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">L</td><td class=""empty""> </td><td class=""empty""> </td></tr><tr><td class=""empty""> </td><td class=""nonempty"">S</td><td class=""empty""> </td><td class=""nonempty"">M</td><td class=""nonempty"">A</td><td class=""nonempty"">R</td><td class=""nonempty"">Y</td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">R</td><td class=""nonempty"">O</td><td class=""nonempty"">S</td><td class=""nonempty"">E</td><td class=""empty""> </td><td class=""nonempty"">W</td></tr><tr><td class=""empty""> </td><td class=""nonempty"">I</td><td class=""empty""> </td><td class=""nonempty"">A</td><td class=""empty""> </td><td class=""nonempty"">E</td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">R</td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">S</td><td class=""empty""> </td><td class=""nonempty"">E</td></tr><tr><td class=""empty""> </td><td class=""nonempty"">C</td><td class=""empty""> </td><td class=""nonempty"">R</td><td class=""nonempty"">O</td><td class=""nonempty"">N</td><td class=""empty""> </td><td class=""nonempty"">R</td><td class=""nonempty"">O</td><td class=""nonempty"">N</td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">N</td></tr><tr><td class=""nonempty"">J</td><td class=""nonempty"">A</td><td class=""nonempty"">C</td><td class=""nonempty"">K</td><td class=""empty""> </td><td class=""nonempty"">D</td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">S</td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">F</td><td class=""nonempty"">R</td><td class=""nonempty"">E</td><td class=""nonempty"">D</td></tr><tr><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">A</td><td class=""nonempty"">N</td><td class=""nonempty"">G</td><td class=""nonempty"">E</td><td class=""nonempty"">L</td><td class=""nonempty"">A</td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">Y</td></tr></table><p>Crozzle file is valid.</p><p>Configuration file is valid.</p><p>Word list file is valid.</p><p></p></body></html>";

            String Test3 = @"<!DOCTYPE html><html><head><style> table, td { border: 1px solid black; border-collapse: collapse; } td { width:24px; height:18px; text-align: center; } </style><style>
                       .empty { background-color: 777777; }
                       .nonempty { background-color: #; }
                       </style>" + @"</head><body><table><tr><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td></tr><tr><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">C</td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">W</td></tr><tr><td class=""empty""> </td><td class=""nonempty"">J</td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">H</td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">E</td></tr><tr><td class=""empty""> </td><td class=""nonempty"">E</td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">H</td><td class=""empty""> </td><td class=""nonempty"">A</td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">N</td></tr><tr><td class=""empty""> </td><td class=""nonempty"">S</td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">A</td><td class=""empty""> </td><td class=""nonempty"">R</td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">D</td></tr><tr><td class=""empty""> </td><td class=""nonempty"">S</td><td class=""empty""> </td><td class=""nonempty"">M</td><td class=""empty""> </td><td class=""nonempty"">R</td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">R</td><td class=""empty""> </td><td class=""nonempty"">L</td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">Y</td></tr><tr><td class=""empty""> </td><td class=""nonempty"">I</td><td class=""empty""> </td><td class=""nonempty"">A</td><td class=""empty""> </td><td class=""nonempty"">O</td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">R</td><td class=""empty""> </td><td class=""nonempty"">E</td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td></tr><tr><td class=""empty""> </td><td class=""nonempty"">C</td><td class=""empty""> </td><td class=""nonempty"">R</td><td class=""empty""> </td><td class=""nonempty"">G</td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">Y</td><td class=""empty""> </td><td class=""nonempty"">S</td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td></tr><tr><td class=""empty""> </td><td class=""nonempty"">A</td><td class=""empty""> </td><td class=""nonempty"">K</td><td class=""empty""> </td><td class=""nonempty"">E</td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">A</td><td class=""nonempty"">L</td></tr><tr><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""nonempty"">R</td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td><td class=""empty""> </td></tr></table><p>Crozzle file is invalid.</p><p>Configuration file is invalid.</p><p>Word list file is invalid.</p><p></p></body></html>";

            // Act
            String Crozzle1Result = aCrozzle1.ToStringHTML();
            String Crozzle2Result = aCrozzle2.ToStringHTML();
            String Crozzle3Result = aCrozzle3.ToStringHTML();

            // Assert
            Assert.AreEqual(Test1, Crozzle1Result, "Error");
            Assert.AreEqual(Test2, Crozzle2Result, "Error");
            Assert.AreEqual(Test3, Crozzle3Result, "Error");

        }

        [TestMethod]
        public void Test_CrozzleMap_GroupCount()
        {
            // Arrange
            setAttributes();
            int Test1 = 1;
            int Test2 = 4;
            int Test3 = 7;

            // Act
            CrozzleMap map1 = new CrozzleMap(aCrozzle1.getCrozzleRows(), aCrozzle1.getCrozzleColumns());
            CrozzleMap map2 = new CrozzleMap(aCrozzle2.getCrozzleRows(), aCrozzle2.getCrozzleColumns());
            CrozzleMap map3 = new CrozzleMap(aCrozzle3.getCrozzleRows(), aCrozzle3.getCrozzleColumns());

            // Assert
            Assert.AreEqual(Test1, map1.GroupCount(), "Error");
            Assert.AreEqual(Test2, map2.GroupCount(), "Error");
            Assert.AreEqual(Test3, map3.GroupCount(), "Error");

        }

        // Import files for testing
        public void setAttributes()
        {
            String wordListPath1 = ".\\UploadedFiles\\.\\Test1.seq";
            String wordListPath2 = ".\\UploadedFiles\\.\\Test2.seq";
            String wordListPath3 = ".\\UploadedFiles\\.\\Test3.seq";
            String configurationPath1 = ".\\UploadedFiles\\.\\Test1.cfg";
            String configurationPath2 = ".\\UploadedFiles\\.\\Test2.cfg";
            String configurationPath3 = ".\\UploadedFiles\\.\\Test3.cfg";
            String crozzlePath1 = ".\\UploadedFiles\\Test1.czl";
            String crozzlePath2 = ".\\UploadedFiles\\Test2.czl";
            String crozzlePath3 = ".\\UploadedFiles\\Test3.czl";

            Configuration.TryParse(configurationPath1, out aConfiguration1);
            Configuration.TryParse(configurationPath2, out aConfiguration2);
            Configuration.TryParse(configurationPath3, out aConfiguration3);

            WordList.TryParse(wordListPath1, aConfiguration1, out aWordList1);
            WordList.TryParse(wordListPath2, aConfiguration2, out aWordList2);
            WordList.TryParse(wordListPath3, aConfiguration3, out aWordList3);

            Crozzle.TryParse(crozzlePath1, aConfiguration1, aWordList1, out aCrozzle1);
            Crozzle.TryParse(crozzlePath2, aConfiguration2, aWordList2, out aCrozzle2);
            Crozzle.TryParse(crozzlePath3, aConfiguration3, aWordList3, out aCrozzle3);
        }


    }
}
