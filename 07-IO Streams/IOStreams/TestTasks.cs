using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace IOStreams
{

    public static class TestTasks
    {
        /// <summary>
        /// Parses Resourses\Planets.xlsx file and returns the planet data: 
        ///   Jupiter     69911.00
        ///   Saturn      58232.00
        ///   Uranus      25362.00
        ///    ...
        /// See Resourses\Planets.xlsx for details
        /// </summary>
        /// <param name="xlsxFileName">source file name</param>
        /// <returns>sequence of PlanetInfo</returns>
        public static IEnumerable<PlanetInfo> ReadPlanetInfoFromXlsx(string xlsxFileName)
        {
            XNamespace xNamespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

            //  /xl/sharedStrings.xml      - dictionary of all string values
            var xNames = ReadPlanetInfoFromXlsx1(xlsxFileName, "/xl/sharedStrings.xml").Descendants(xNamespace + "si");

            //  /xl/worksheets/sheet1.xml  - main worksheet
            var xMeanRadii = ReadPlanetInfoFromXlsx1(xlsxFileName, "/xl/worksheets/sheet1.xml").Descendants(xNamespace + "c")
                .Where(x => ((string)x.Attribute("r")).StartsWith("B"))
                                                      .Skip(1);

            return xNames.Zip(xMeanRadii, (x, y) => new PlanetInfo { Name = (string)x, MeanRadius = (double)y });
        }
        public static XElement ReadPlanetInfoFromXlsx1(string xlsxFileName, string uri)
        {
            using (Package package = Package.Open(xlsxFileName, FileMode.Open, FileAccess.Read))
            using (Stream stream = package.GetPart(new Uri(uri, UriKind.Relative)).GetStream())
                return XElement.Load(stream);
        }


        /// <summary>
        /// Calculates hash of stream using specifued algorithm
        /// </summary>
        /// <param name="stream">source stream</param>
        /// <param name="hashAlgorithmName">hash algorithm ("MD5","SHA1","SHA256" and other supported by .NET)</param>
        /// <returns></returns>
        public static string CalculateHash(this Stream stream, string hashAlgorithmName)
        {
            var algorithm = HashAlgorithm.Create(hashAlgorithmName);
            if (algorithm == null)
                throw new ArgumentException();
            return string.Join("", algorithm.ComputeHash(stream).Select(x => x.ToString("X2")));
        }


        /// <summary>
        /// Returns decompressed strem from file. 
        /// </summary>
        /// <param name="fileName">source file</param>
        /// <param name="method">method used for compression (none, deflate, gzip)</param>
        /// <returns>output stream</returns>
        public static Stream DecompressStream(string fileName, DecompressionMethods method)
        {
            Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            if (method == DecompressionMethods.GZip)
                return new GZipStream(stream, 0);
            if (method == DecompressionMethods.Deflate)
                return new DeflateStream(stream, 0);
            return stream;
        }


        /// <summary>
        /// Reads file content econded with non Unicode encoding
        /// </summary>
        /// <param name="fileName">source file name</param>
        /// <param name="encoding">encoding name</param>
        /// <returns>Unicoded file content</returns>
        public static string ReadEncodedText(string fileName, string encoding)
        {
            using (var file = new StreamReader(fileName, Encoding.GetEncoding(encoding)))
                return file.ReadToEnd();
        }
    }


    public class PlanetInfo : IEquatable<PlanetInfo>
    {
        public string Name { get; set; }
        public double MeanRadius { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", Name, MeanRadius);
        }

        public bool Equals(PlanetInfo other)
        {
            return Name.Equals(other.Name)
                && MeanRadius.Equals(other.MeanRadius);
        }
    }



}
