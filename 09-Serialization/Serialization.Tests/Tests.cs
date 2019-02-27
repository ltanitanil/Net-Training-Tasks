using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialization.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Serialization;

namespace Serialization.Tests
{

    [TestClass]
    public class Tests
    {

        [TestMethod]
        [TestCategory("GoogleSearchResult")]
        [DeploymentItem(@"Resources\GoogleSearchJson.txt")]
        public void GoogleSearchResult_Should_be_Parsed()
        {
            var rules = new Dictionary<Func<dynamic, object>, object>() {
                { x => x.Kind ,        "customsearch#search" },
                
                // ***********  Url   ****************************
                { x => x.Url.Type,     "application/json" },
                { x => x.Url.Template, "https://www.googleapis.com/customsearch/v1?q={searchTerms}&num={count?}&start={startIndex?}&lr={language?}&safe={safe?}&cx={cx?}&cref={cref?}&sort={sort?}&filter={filter?}&gl={gl?}&cr={cr?}&googlehost={googleHost?}&c2coff={disableCnTwTranslation?}&hq={hq?}&hl={hl?}&nsc={nsc?}&siteSearch={siteSearch?}&siteSearchFilter={siteSearchFilter?}&exactTerms={exactTerms?}&excludeTerms={excludeTerms?}&linkSite={linkSite?}&orTerms={orTerms?}&relatedSite={relatedSite?}&dateRestrict={dateRestrict?}&lowRange={lowRange?}&highRange={highRange?}&searchType={searchType}&fileType={fileType?}&rights={rights?}&imgSize={imgSize?}&imgType={imgType?}&imgColorType={imgColorType?}&imgDominantColor={imgDominantColor?}&alt=json"},

                // **********   Queries ****************************
                { x => x.Queries.NextPage[0].Title,           "Google Custom Search - flowers" },
                { x => x.Queries.NextPage[0].TotalResults,    10300000L },
                { x => x.Queries.NextPage[0].SearchTerms,     "flowers" },
                { x => x.Queries.NextPage[0].Count,           10 },
                { x => x.Queries.NextPage[0].StartIndex,      11 },
                { x => x.Queries.NextPage[0].InputEncoding,   "utf8" },
                { x => x.Queries.NextPage[0].OutputEncoding,  "utf8" },
                { x => x.Queries.NextPage[0].Cx,              "013036536707430787589:_pqjad5hr1a" },

                { x => x.Queries.PreviousPage,             null },

                { x => x.Queries.Request[0].Title,            "Google Custom Search - flowers" },
                { x => x.Queries.Request[0].TotalResults,     10300000L },
                { x => x.Queries.Request[0].SearchTerms,      "flowers" },
                { x => x.Queries.Request[0].Count,            10 },
                { x => x.Queries.Request[0].StartIndex,       1 },
                { x => x.Queries.Request[0].InputEncoding,    "utf8" },
                { x => x.Queries.Request[0].OutputEncoding,   "utf8" },
                { x => x.Queries.Request[0].Cx,               "013036536707430787589:_pqjad5hr1a" },


                // ************  Context  *************************
                { x => x.Context.Title,                    "Custom Search"},

                // ************ Items  ***************************
                { x => x.Items[0].Kind,                    "customsearch#result"},
                { x => x.Items[0].Title,                   "Flower - Wikipedia, the free encyclopedia"},
                { x => x.Items[0].HtmlTitle,               "<b>Flower</b> - Wikipedia, the free encyclopedia"},
                { x => x.Items[0].Link,                    "http://en.wikipedia.org/wiki/Flower"},
                { x => x.Items[0].DisplayLink,             "en.wikipedia.org"},
                { x => x.Items[0].Snippet,                 "A flower, sometimes known as a bloom or blossom, is the reproductive structure found in flowering plants (plants of the division Magnoliophyta, ..."},
                { x => x.Items[0].HtmlSnippet,             "A <b>flower</b>, sometimes known as a bloom or blossom, is the reproductive structure <br>  found in flowering plants (plants of the division Magnoliophyta, <b>... </b>"},
            };


            var serializer = new DataContractJsonSerializer(typeof(GoogleSearchResult));
            GoogleSearchResult actualObject = null;
            using (var stream = File.OpenRead("GoogleSearchJson.txt"))  {
                stream.Seek(Encoding.UTF8.GetPreamble().Length, SeekOrigin.Begin);  // HACK, skip Unicode preambule
                actualObject = serializer.ReadObject(stream) as GoogleSearchResult;
            }

            Assert.IsNotNull(actualObject, "GoogleSearchResult cannot be deserialized. Result is null");
            
            foreach (var rule in rules) {
                var actual = rule.Key(actualObject);
                var expected = rule.Value;
                Assert.AreEqual(expected, actual);
            }
            

        }



        [TestMethod]
        [TestCategory("Company serialization-deserialization")]
        public void Company_class_Should_be_Serializable() {

            var expected = new Company {
                    Name = "Noname company",
                };

            var data = Serialize(expected);
            var actual = Deserialize<Company>(data);

            Assert.AreEqual(expected.Name, actual.Name);
        }



        [TestMethod]
        [TestCategory("Company serialization-deserialization")]
        public void Company_class_Should_Serialize_the_Employee_List()
        {
            var expected = new Company
            {
                Name = "Noname company",
                Employee = new List<Employee>() {
                        new Manager { Name = "Peter", LastName = "Smith", Title = "Director", YearBonusRate = 50 },
                        new Worker  { Name ="John", LastName="Johnson", Title = "Worker", Salary = 80000 },
                     }
            };

            var data = Serialize(expected);
            var actual = Deserialize<Company>(data);

            Assert.AreEqual(expected.Employee[0].GetType(), actual.Employee[0].GetType());
            Assert.AreEqual(expected.Employee[1].GetType(), actual.Employee[1].GetType());
        }


        [TestMethod]
        [TestCategory("Company serialization-deserialization")]
        public void Company_class_Should_Serialize_the_Manager_property_as_Reference()
        {
            var manager = new Manager { Name = "Peter", LastName = "Smith", Title = "Director", YearBonusRate = 50 };

            var expected = new Company
            {
                Name = "Noname company",
                Employee = new List<Employee>() {
                        manager, 
                        new Worker { Name ="John", LastName="Johnson", Title = "Worker", Salary = 80000, Manager = manager },
                        new Worker { Name ="Michael", LastName="Worker", Title = "Worker", Salary = 85000, Manager = manager },
                     }
            };

            var data = Serialize(expected);
            var actual = Deserialize<Company>(data);

            Assert.AreSame(actual.Employee[1].Manager, actual.Employee[2].Manager);
        }



        [DataContract(Name = "Company", Namespace = "http://schemas.datacontract.org/2004/07/Serialization.Tasks")]
        public class DerivedCompany : Company
        {
            [DataMember] public string Address        { get; set; }
            [DataMember] public long   Capitalization { get; set; }
        }




        [TestMethod]
        [TestCategory("Company serialization-deserialization")]
        public void Company_class_Should_be_Serializable_with_Forward_Compatibility()
        {

            var expected = new DerivedCompany {
                Name = "Noname company",
                Address = "Somewhere",
                Capitalization = 100000000L
            };

            var data = Serialize(expected);
            
            var compatibleObj = Deserialize<Company>(data);
            var compatibleData = Serialize(compatibleObj);

            var actual = Deserialize<DerivedCompany>(compatibleData);

            Assert.AreEqual(expected.Name,           actual.Name);
            Assert.AreEqual(expected.Address,        actual.Address);
            Assert.AreEqual(expected.Capitalization, actual.Capitalization);
        }




        private static string Serialize<T>(T graph) 
        {
            var serializer = new DataContractSerializer(typeof(T));
            using (var stream = new MemoryStream()) {
                serializer.WriteObject(stream, graph);
                var result = Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
                Debug.WriteLine(result);
                return result;
            }
        }



        private static T Deserialize<T>(string data)
        {
            var serializer = new DataContractSerializer(typeof(T));
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(data))) {
                return (T)serializer.ReadObject(stream);
            }
        }



        [TestMethod]
        [DeploymentItem(@"Resources\CLR-ETW.man")]
        public void InstrumentationManifest_class_should_deserialized()
        {
            var serializer = new XmlSerializer(typeof(instrumentationManifest));

            using (var stream = File.OpenRead("CLR-ETW.man")) {
                var data = serializer.Deserialize(stream) as dynamic;

                Assert.IsNotNull(data);
                Assert.IsNotNull(data.Instrumentation);
                Assert.IsNotNull(data.Localization);
                Assert.IsNull(data.Metadata);
            }
           

        }



    }




}
