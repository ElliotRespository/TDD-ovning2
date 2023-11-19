using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;
using Trangselskatt;

namespace Trangselskatt.test
{
    [TestFixture]
    public class TrangselskattTest
    {
        [TestCase("2023-05-31 06:15", "Total avgiften är 8 kr")]
        [TestCase("2023-05-31 07:45", "Total avgiften är 18 kr")]
        [TestCase("2023-05-31 15:15", "Total avgiften är 13 kr")]
        [TestCase("2023-05-31 18:45", "Total avgiften är 0 kr")]
        public void TestaRäknaTotalBeloppEnskildTid(string datumTid, string forvantatResultat)
        {
            // Arrange
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            // Act
            Metoder.RäknaTotalBelopp(datumTid);
            var resultat = stringWriter.ToString().Trim();

            // Assert
            Assert.That(resultat, Is.EqualTo(forvantatResultat));
        }


        [Test]
        public void TestaRäknaTotalBeloppFleraTider()
        {
            // Arrange
            var input = "2023-05-31 06:15, 2023-05-31 07:45, 2023-05-31 15:15, 2023-05-31 18:45";
            var forvantatResultat = "Total avgiften är 39 kr";
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            // Act
            Metoder.RäknaTotalBelopp(input);
            var resultat = stringWriter.ToString().Trim();

            // Assert
            Assert.That(resultat, Is.EqualTo(forvantatResultat));
        }


        [Test]
        public void TestaParseInputData()
        {
            //Arrange
            var input = "2023-05-31 06:15, felaktigt datum, 2023-05-31 07:45";
            var forvantadeTider = new List<DateTime>
            {
                new DateTime(2023, 5, 31, 6, 15, 0),
                new DateTime(2023, 5, 31, 7, 45, 0)
            };

            // Act
            var resultat = Metoder.ParseInputData(input);

            // Assert
            Assert.That(resultat, Is.EqualTo(forvantadeTider));
        }


        [Test]
        public void TestaBeräknaDagligaAvgifter()
        {
            //Arrange
            var tidpunkter = new List<DateTime>
            {
                new DateTime(2023, 5, 31, 6, 15, 0),
                new DateTime(2023, 5, 31, 7, 45, 0),
                new DateTime(2023, 5, 31, 15, 15, 0)
            };
            var forvantatResultat = new Dictionary<DateTime, int>
            {
                { new DateTime(2023, 5, 31), 39 }
            };

            // Act
            var resultat = Metoder.BeräknaDagligaAvgifter(tidpunkter);

            // Assert
            Assert.That(resultat, Is.EqualTo(forvantatResultat));

        }

        [Test]
        public void TestaRäknaTotalBeloppPåAvgiftsfriaDagar()
        {
            var testData = new List<(string Datum, string FörväntatResultat)>
            {
             ("2023-06-03", "Total avgiften är 0 kr"), // Lördag
             ("2023-06-04", "Total avgiften är 0 kr"), // Söndag
             ("2023-07-01", "Total avgiften är 0 kr")  // Juli
            };

            foreach (var (datum, förväntatResultat) in testData)
            {
                // Arrange
                var stringWriter = new StringWriter();
                Console.SetOut(stringWriter);

                // Act
                Metoder.RäknaTotalBelopp(datum);
                var resultat = stringWriter.ToString().Trim();

                // Assert
                Assert.That(resultat, Is.EqualTo(förväntatResultat), $"Test misslyckades för datum: {datum}");

            }
        }

        [Test]
        public void TestaRäknaTotalBeloppMedFleraPassagerInom60Minuter()
        {
            //Arrange
            var input = "2023-05-31 06:20, 2023-05-31 06:45, 2023-05-31 07:10"; // Avgifter: 8 kr, 13 kr, 18 kr
            var forvantatResultat = "Total avgiften är 18 kr"; // Högsta avgift inom första 60 minuter
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            //Act
            Metoder.RäknaTotalBelopp(input);
            var resultat = stringWriter.ToString().Trim();

            //Assert
            Assert.That(resultat, Is.EqualTo(forvantatResultat));

        }


    }
}
