using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IOStreams.Tests
{
	[TestClass]
	public class UnitTests
	{
		private const string ResourseFileName = @"Resources\Planets.xlsx";
		private const string EncodedFileName  = @"Resources\german_ISO-8859-1.txt";

		private void CheckFileIsClosed(string fileName)
		{
			try
			{
				using (var f = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) {}
			}
			catch (IOException)
			{
				Assert.Fail("Source stream is not closed! Please use 'using' statement.");
			}
		}


		[TestMethod]
		public void ReadPlanetInfoFromXlsx_Should_Parse_Excel_File()
		{
			var expected = new[] {
					 new PlanetInfo() { Name = "Jupiter", MeanRadius = 69911.00 },
					 new PlanetInfo() { Name = "Saturn",  MeanRadius = 58232.00 },
					 new PlanetInfo() { Name = "Uranus",  MeanRadius = 25362.00 },
					 new PlanetInfo() { Name = "Neptune", MeanRadius = 24622.00 },
					 new PlanetInfo() { Name = "Earth",   MeanRadius =  6371.00 },
					 new PlanetInfo() { Name = "Venus",   MeanRadius =  6051.80 },
					 new PlanetInfo() { Name = "Mars",    MeanRadius =  3390.00 },
					 new PlanetInfo() { Name = "Mercury", MeanRadius =  2439.70 },

				};
			var actual = TestTasks.ReadPlanetInfoFromXlsx(ResourseFileName).ToArray();

			Assert.IsTrue(expected.SequenceEqual(actual));

			CheckFileIsClosed(ResourseFileName);
		}


		[TestMethod]
		public void CalculateHash_Should_Return_Valid_Hash_Value()
		{
			var testData = new Dictionary<string, string> {
				{"MD5",                                 "82E3C45273D90BC76489F194D1FA5CE1"},
				{"System.Security.Cryptography.MD5",    "82E3C45273D90BC76489F194D1FA5CE1"},

				{"SHA",                                 "30535A22D7995613F8613DA379ED0C89F8D7A280"},
				{"SHA1",                                "30535A22D7995613F8613DA379ED0C89F8D7A280"},
				{"System.Security.Cryptography.SHA1",   "30535A22D7995613F8613DA379ED0C89F8D7A280"},

				{"RIPEMD160",                              "CC65C25BFD614A70D76C7EC9B951B55828C12643"},
				{"RIPEMD-160",                             "CC65C25BFD614A70D76C7EC9B951B55828C12643"},
				{"System.Security.Cryptography.RIPEMD160", "CC65C25BFD614A70D76C7EC9B951B55828C12643"},

				{"SHA256",                              "62974B0251BA38179EE7D692A874694C67999B29EDC5CA068DA86626D160135F"},
				{"SHA-256",                             "62974B0251BA38179EE7D692A874694C67999B29EDC5CA068DA86626D160135F"},
				{"System.Security.Cryptography.SHA256", "62974B0251BA38179EE7D692A874694C67999B29EDC5CA068DA86626D160135F"},

				{"SHA384",                              "43ED7BCA7751DD7FFFF6D1BF528F917E75580A9CB0669A43AA01B943A30F2C36CAF672D8F42FD2EC7BD622FBE72F4D67"},
				{"SHA-384",                             "43ED7BCA7751DD7FFFF6D1BF528F917E75580A9CB0669A43AA01B943A30F2C36CAF672D8F42FD2EC7BD622FBE72F4D67"},
				{"System.Security.Cryptography.SHA384", "43ED7BCA7751DD7FFFF6D1BF528F917E75580A9CB0669A43AA01B943A30F2C36CAF672D8F42FD2EC7BD622FBE72F4D67"},
				
				{"SHA512",                              "6670401F8BE30A3EA179042C8F17773339EA0E0B7FAE671799D5460A6AE4BCC9A824C08317268B0A92A2A4846FD9D3D858297EAB63F549DE8154DE7A1557E8B2"},
				{"SHA-512",                             "6670401F8BE30A3EA179042C8F17773339EA0E0B7FAE671799D5460A6AE4BCC9A824C08317268B0A92A2A4846FD9D3D858297EAB63F549DE8154DE7A1557E8B2"},
				{"System.Security.Cryptography.SHA512", "6670401F8BE30A3EA179042C8F17773339EA0E0B7FAE671799D5460A6AE4BCC9A824C08317268B0A92A2A4846FD9D3D858297EAB63F549DE8154DE7A1557E8B2"}
			};

			using (var stream = File.OpenRead(ResourseFileName))
			{
				foreach (var data in testData)
				{
					stream.Position = 0;
					var actual = stream.CalculateHash(data.Key);
					var expected = data.Value;
					Assert.AreEqual(expected, actual, "Error calculation hash "+data.Key);
				}
			}
		}


		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void CalculateHash_Should_Raise_an_Exeption_if_Hash_algorithm_is_not_supported()
		{
			Stream.Null.CalculateHash("unrecognized");
		}

		[TestMethod]
		public void DecompressStream_Should_Extract_Data_Correctly()
		{
			var testData = new Dictionary<string, DecompressionMethods> {
					{ ResourseFileName,            DecompressionMethods.None    },
					{ ResourseFileName+".deflate", DecompressionMethods.Deflate },
					{ ResourseFileName+".gzip",    DecompressionMethods.GZip    }
				};

			var expected = File.ReadAllBytes(ResourseFileName);

			foreach (var data in testData)
			{
				using (var stream = TestTasks.DecompressStream(data.Key, data.Value)) 
				{
					using (var memStream = new MemoryStream())
					{
						stream.CopyTo(memStream);
						Assert.IsTrue(expected.SequenceEqual(memStream.ToArray()), "DecompressStream failed for " + data.Value);
					}
				}
				CheckFileIsClosed(data.Key);
			}
		}


		[TestMethod]
		public void ReadEncodedText_Shoud_Convert_Text_using_Specifyed_Encoding() 
		{
			var expected =
			"Deutschland ist ein Bundesstaat in Mitteleuropa. Gemäß seiner Verfassung ist Deutschland eine föderal organisierte Republik, die aus den 16 deutschen Ländern gebildet wird. " +
			"Die Bundesrepublik Deutschland ist ein freiheitlich-demokratischer und sozialer Rechtsstaat und stellt die jüngste Ausprägung des deutschen Nationalstaates dar. Bundeshauptstadt ist Berlin." +
			"Neun europäische Nachbarstaaten grenzen an die Bundesrepublik, naturräumlich zudem im Norden die Gewässer der Nord- und Ostsee und im Süden das Bergland der Alpen. " +
			"Sie liegt in der gemäßigten Klimazone und zählt mit rund 82 Millionen Einwohnern zu den dicht besiedelten Flächenländern." +
			"Deutschland ist Gründungsmitglied der Europäischen Union sowie deren bevölkerungsreichstes Land und bildet mit 16 anderen EU-Mitgliedstaaten eine Währungsunion, die Eurozone. " +
			"Es ist Mitglied der Vereinten Nationen, der OECD, der NATO, der G8 und der G20." +
			"Gemessen am nominalen Bruttoinlandsprodukt ist Deutschland die größte Volkswirtschaft Europas und viertgrößte der Welt. " +
			"Im Jahr 2011 war es die drittgrößte Export- und Importnation.[10] Der Index für menschliche Entwicklung zählt Deutschland zu den sehr hoch entwickelten Staaten";

			var actual = TestTasks.ReadEncodedText(EncodedFileName, "ISO-8859-1");

			Assert.AreEqual(expected, actual);

			CheckFileIsClosed(EncodedFileName);
		}


	}
}
