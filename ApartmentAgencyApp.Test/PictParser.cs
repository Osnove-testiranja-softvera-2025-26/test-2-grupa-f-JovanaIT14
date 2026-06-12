using ApartmentAgencyApp.Models;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentAgencyApp.Test
{
    public class PictParser
    {
        public static IEnumerable GetTestCases(string filename)
        {
            string path = $@"{AppDomain.CurrentDomain.BaseDirectory}\{filename}";
            string[] lines = File.ReadAllLines(path);

            List<TestCaseData> testCasesData = new List<TestCaseData>();

            foreach (string line in lines)
            {
                string[] values = line.Split(null);
                double distanceFromBeach = double.Parse(values[0]);
                int percentOfPositiveReviews = int.Parse(values[1]);
                ApartmentType apartmentType = (ApartmentType)Enum.Parse(typeof(ApartmentType), values[2]);
                bool renovatedInTheLastYear = bool.Parse(values[3]);
                ApartmentRank expectedResult = (ApartmentRank)Enum.Parse(typeof(ApartmentRank), values[4]);

                testCasesData.Add(new TestCaseData(distanceFromBeach, apartmentType, percentOfPositiveReviews, renovatedInTheLastYear, expectedResult));


            }
            return testCasesData;
        }
    }
}
