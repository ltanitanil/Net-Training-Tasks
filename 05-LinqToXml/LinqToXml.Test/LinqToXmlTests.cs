using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;

namespace LinqToXml.Test
{
    [TestClass]
    public class LinqToXmlTests
    {
        [TestMethod]
        [TestCategory("LinqToXml.CreateHierarchyTest")]
        public void CreateHierarchyTest()
        {
            Assert.AreEqual(LinqToXmlResources.CreateHierarchyResultFile, LinqToXml.CreateHierarchy(LinqToXmlResources.CreateHierarchySourceFile));
        }

        [TestMethod]
        [TestCategory("LinqToXml.GetPurchaseOrders")]
        public void GetPurchaseOrdersTest()
        {
            Assert.AreEqual("99505,99607", LinqToXml.GetPurchaseOrders(LinqToXmlResources.PurchaseOrdersSourceFile));
        }

        [TestMethod]
        [TestCategory("LinqToXml.ReadCustomersFromCsv")]
        public void ReadCustomersFromCsvTest()
        {
            Assert.AreEqual(LinqToXmlResources.XmlFromCsvResultFile, LinqToXml.ReadCustomersFromCsv(LinqToXmlResources.XmlFromCsvSourceFile));
        }

        [TestMethod]
        [TestCategory("LinqToXml.GetConcatenationString")]
        public void GetConcatenationStringTest()
        {
            Assert.AreEqual(LinqToXmlResources.ConcatenationStringResult, LinqToXml.GetConcatenationString(LinqToXmlResources.ConcatenationStringSource));
        }

        [TestMethod]
        [TestCategory("LinqToXml.ReplaceAllCustomersWithContacts")]
        public void ReplaceAllCustomersWithContactsTest()
        {
            Assert.AreEqual(LinqToXmlResources.ReplaceCustomersWithContactsResult, LinqToXml.ReplaceAllCustomersWithContacts(LinqToXmlResources.ReplaceCustomersWithContactsSource));
        }

        [TestMethod]
        [TestCategory("LinqToXml.FindChannelsIds")]
        public void FindChannelsIdsTest()
        {
            Assert.IsTrue(new int[] { 7 }.SequenceEqual(LinqToXml.FindChannelsIds(LinqToXmlResources.FindAllChannelsIdsSource)));
        }

        [TestMethod]
        [TestCategory("LinqToXml.SortCustomers")]
        public void SortCustomersTest()
        {
            Assert.AreEqual(LinqToXmlResources.GeneralCustomersResultFile, LinqToXml.SortCustomers(LinqToXmlResources.GeneralCustomersSourceFile));
        }

        [TestMethod]
        [TestCategory("LinqToXml.GetFlattenString")]
        public void GetFlattenStringTest()
        {
            Assert.AreEqual(1510, LinqToXml.GetOrdersValue(LinqToXmlResources.GeneralOrdersFileSource));
        }
    }
}
