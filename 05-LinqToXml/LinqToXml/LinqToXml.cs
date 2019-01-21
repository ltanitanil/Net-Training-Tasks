using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace LinqToXml
{
    public static class LinqToXml
    {
        /// <summary>
        /// Creates hierarchical data grouped by category
        /// </summary>
        /// <param name="xmlRepresentation">Xml representation (refer to CreateHierarchySourceFile.xml in Resources)</param>
        /// <returns>Xml representation (refer to CreateHierarchyResultFile.xml in Resources)</returns>
        public static string CreateHierarchy(string xmlRepresentation)
        {
            return new XElement("Root", XElement.Parse(xmlRepresentation).Elements("Data")
                                        .GroupBy(x => x.Element("Category").Value)
                                        .Select(x => new XElement("Group",
                                                    new XAttribute("ID", x.Key),
                                                    x.Select(y => new XElement("Data",
                                                                      y.Element("Quantity"),
                                                                      y.Element("Price")))))).ToString();
        }

        /// <summary>
        /// Get list of orders numbers (where shipping state is NY) from xml representation
        /// </summary>
        /// <param name="xmlRepresentation">Orders xml representation (refer to PurchaseOrdersSourceFile.xml in Resources)</param>
        /// <returns>Concatenated orders numbers</returns>
        /// <example>
        /// 99301,99189,99110
        /// </example>
        public static string GetPurchaseOrders(string xmlRepresentation)
        {
            XNamespace xNamespace = "http://www.adventure-works.com";
            var purchaseOrdersNumbers = XElement.Parse(xmlRepresentation).Descendants(xNamespace + "Address")
                .Where(x => x.Attribute(xNamespace + "Type").Value == "Shipping" && x.Element(xNamespace + "State").Value == "NY")
                .Select(x => x.Parent.Attribute(xNamespace + "PurchaseOrderNumber").Value);
            return string.Join(",", purchaseOrdersNumbers);
        }

        /// <summary>
        /// Reads csv representation and creates appropriate xml representation
        /// </summary>
        /// <param name="customers">Csv customers representation (refer to XmlFromCsvSourceFile.csv in Resources)</param>
        /// <returns>Xml customers representation (refer to XmlFromCsvResultFile.xml in Resources)</returns>
        public static string ReadCustomersFromCsv(string customers)
        {
            var a = customers.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                             .Select(x => x.Split(','));

            return new XElement("Root",
                     a.Select(x => new XElement("Customer",
                                     new XAttribute("CustomerID", x.ElementAt(0)),
                                     new XElement("CompanyName", x.ElementAt(1)),
                                     new XElement("ContactName", x.ElementAt(2)),
                                     new XElement("ContactTitle", x.ElementAt(3)),
                                     new XElement("Phone", x.ElementAt(4)),
                                     new XElement("FullAddress",
                                         new XElement("Address", x.ElementAt(5)),
                                         new XElement("City", x.ElementAt(6)),
                                         new XElement("Region", x.ElementAt(7)),
                                         new XElement("PostalCode", x.ElementAt(8)),
                                         new XElement("Country", x.ElementAt(9)))))).ToString();
        }

        /// <summary>
        /// Gets recursive concatenation of elements
        /// </summary>
        /// <param name="xmlRepresentation">Xml representation of document with Sentence, Word and Punctuation elements. (refer to ConcatenationStringSource.xml in Resources)</param>
        /// <returns>Concatenation of all this element values.</returns>
        public static string GetConcatenationString(string xmlRepresentation)
        {
            return string.Join("", XElement.Parse(xmlRepresentation).Elements("Sentence")
                .Select(x => string.Join("", x.Element("Part") != null
                    ? x.Element("Part").Elements().Select(y => y.Value)
                    : x.Elements().Select(y => y.Value))));
        }

        /// <summary>
        /// Replaces all "customer" elements with "contact" elements with the same childs
        /// </summary>
        /// <param name="xmlRepresentation">Xml representation with customers (refer to ReplaceCustomersWithContactsSource.xml in Resources)</param>
        /// <returns>Xml representation with contacts (refer to ReplaceCustomersWithContactsResult.xml in Resources)</returns>
        public static string ReplaceAllCustomersWithContacts(string xmlRepresentation)
        {
            return new XElement("Document", XElement.Parse(xmlRepresentation).Elements("customer")
                .Select(x => new XElement("contact", x.Element("name"), x.Element("lastname")))).ToString();
        }

        /// <summary>
        /// Finds all ids for channels with 2 or more subscribers and mark the "DELETE" comment
        /// </summary>
        /// <param name="xmlRepresentation">Xml representation with channels (refer to FindAllChannelsIdsSource.xml in Resources)</param>
        /// <returns>Sequence of channels ids</returns>
        public static IEnumerable<int> FindChannelsIds(string xmlRepresentation)
        {
            return XElement.Parse(xmlRepresentation).Elements("channel")
                .Where(x => x.Elements("subscriber").Count() > 1 && x.Nodes().OfType<XComment>().Any(y => y.Value == "DELETE"))
                .Select(x => int.Parse(x.Attribute("id").Value));
        }

        /// <summary>
        /// Sort customers in docement by Country and City
        /// </summary>
        /// <param name="xmlRepresentation">Customers xml representation (refer to GeneralCustomersSourceFile.xml in Resources)</param>
        /// <returns>Sorted customers representation (refer to GeneralCustomersResultFile.xml in Resources)</returns>
        public static string SortCustomers(string xmlRepresentation)
        {
            return new XElement("Root", XElement.Parse(xmlRepresentation).Elements("Customers")
                .OrderBy(x => x.Element("FullAddress").Element("Country").Value)
                .ThenBy(x => x.Element("FullAddress").Element("City").Value)).ToString();
        }

        /// <summary>
        /// Gets XElement flatten string representation to save memory
        /// </summary>
        /// <param name="xmlRepresentation">XElement object</param>
        /// <returns>Flatten string representation</returns>
        /// <example>
        ///     <root><element>something</element></root>
        /// </example>
        public static string GetFlattenString(XElement xmlRepresentation)
        {
            return xmlRepresentation.ToString(SaveOptions.DisableFormatting);
        }

        /// <summary>
        /// Gets total value of orders by calculating products value
        /// </summary>
        /// <param name="xmlRepresentation">Orders and products xml representation (refer to GeneralOrdersFileSource.xml in Resources)</param>
        /// <returns>Total purchase value</returns>
        public static int GetOrdersValue(string xmlRepresentation)
        {
            var products = XElement.Parse(xmlRepresentation).Element("products").Elements();
            return XElement.Parse(xmlRepresentation).Element("Orders").Descendants("product")
                .Select(x => int.Parse(products.FirstOrDefault(y => y.Attribute("Id").Value == x.Value).Attribute("Value").Value))
                .Sum();
        }
    }
}
