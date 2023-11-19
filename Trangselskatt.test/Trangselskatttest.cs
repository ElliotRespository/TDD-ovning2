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
        [TestCase("2023-05-31 06:15", "Total avgiften �r 8 kr")]
        [TestCase("2023-05-31 07:45", "Total avgiften �r 18 kr")]
        [TestCase("2023-05-31 15:15", "Total avgiften �r 13 kr")]
        [TestCase("2023-05-31 18:45", "Total avgiften �r 0 kr")]
        public void TestaR�knaTotalBeloppEnskildTid(string datumTid, string forvantatResultat)
        {
            // Arrange
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            // Act
            Metoder.R�knaTotalBelopp(datumTid);
            var resultat = stringWriter.ToString().Trim();

            // Assert
            Assert.That(resultat, Is.EqualTo(forvantatResultat));
        }


        [Test]
        public void TestaR�knaTotalBeloppFleraTider()
        {
            // Arrange
            var input = "2023-05-31 06:15, 2023-05-31 07:45, 2023-05-31 15:15, 2023-05-31 18:45";
            var forvantatResultat = "Total avgiften �r 39 kr";
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            // Act
            Metoder.R�knaTotalBelopp(input);
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
        public void TestaBer�knaDagligaAvgifter()
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
            var resultat = Metoder.Ber�knaDagligaAvgifter(tidpunkter);

            // Assert
            Assert.That(resultat, Is.EqualTo(forvantatResultat));

        }

        [Test]
        public void TestaR�knaTotalBeloppP�AvgiftsfriaDagar()
        {
            var testData = new List<(string Datum, string F�rv�ntatResultat)>
            {
             ("2023-06-03", "Total avgiften �r 0 kr"), // L�rdag
             ("2023-06-04", "Total avgiften �r 0 kr"), // S�ndag
             ("2023-07-01", "Total avgiften �r 0 kr")  // Juli
            };

            foreach (var (datum, f�rv�ntatResultat) in testData)
            {
                // Arrange
                var stringWriter = new StringWriter();
                Console.SetOut(stringWriter);

                // Act
                Metoder.R�knaTotalBelopp(datum);
                var resultat = stringWriter.ToString().Trim();

                // Assert
                Assert.That(resultat, Is.EqualTo(f�rv�ntatResultat), $"Test misslyckades f�r datum: {datum}");

            }
        }

        [Test]
        public void TestaR�knaTotalBeloppMedFleraPassagerInom60Minuter()
        {
            //Arrange
            var input = "2023-05-31 06:20, 2023-05-31 06:45, 2023-05-31 07:10"; // Avgifter: 8 kr, 13 kr, 18 kr
            var forvantatResultat = "Total avgiften �r 18 kr"; // H�gsta avgift inom f�rsta 60 minuter
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            //Act
            Metoder.R�knaTotalBelopp(input);
            var resultat = stringWriter.ToString().Trim();

            //Assert
            Assert.That(resultat, Is.EqualTo(forvantatResultat));

        }


    }
}
