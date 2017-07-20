using HCPACS4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HCPACS4
{
    public static class HCPUtils
    {
        public static readonly Regex REGEX_ISBN13 = new Regex(@"(?'isbn13'97\d{11})", RegexOptions.IgnoreCase & RegexOptions.Compiled);
        public static readonly Regex REGEX_ISBN13_FILENAME_NOEXTN = new Regex(@"^(?'isbn13'97\d{11})\.", RegexOptions.IgnoreCase & RegexOptions.Compiled);
        public static Regex REGEX_ISBN13_STRICT = new Regex(@"^(?'isbn13'97\d{11})$", RegexOptions.IgnoreCase & RegexOptions.Compiled);

        /// <summary>
        /// Get first matching part of string that matches isbn13 pattern
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetIsbn13(string input)
        {
            if (String.IsNullOrEmpty(input))
                return String.Empty;

            Match m = REGEX_ISBN13.Match(input);
            if (m.Success)
            {
                return m.Groups["isbn13"].Value;
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Checks if the FileName (without extension) matches expected pattern for isbn13 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool IsFileNamedByIsbn13(string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
                return false;

            Match m = REGEX_ISBN13_FILENAME_NOEXTN.Match(fileName);
            return m.Success;
        }

        
        public static bool IsValidIsbn13(string input)
        {
            return REGEX_ISBN13_STRICT.IsMatch(input);
        }

        public static List<Epub> GetEpubInfo(string dir, int maxNumEpubs, bool randomize)
        {
            IEnumerable<FileInfo> files = null;
            // get randomized set 
            if (randomize)
                files = new DirectoryInfo(dir)
                            .EnumerateFiles("*.epub", SearchOption.AllDirectories)
                            .OrderBy(x => Guid.NewGuid())
                            .Take(maxNumEpubs);
            else
                files = new DirectoryInfo(dir)
                            .EnumerateFiles("*.epub", SearchOption.AllDirectories)
                            .Take(maxNumEpubs);


            List<Epub> epubs = new List<Epub>();
            foreach (var f in files)
            {
                Epub epub = new Epub(f.FullName, "");
                epubs.Add(epub);
            }
            return epubs;
        }


        //public static IEnumerable<ACS4Catalog> ParseACS4Catalog(string filename)
        public static IEnumerable<ACS4Catalog> ParseACS4Catalog(string xml)
        {
            //var xml = XElement.Load(filename);
            var xmlcatalog = XElement.Parse(xml);
            XNamespace nsAdept = "http://ns.adobe.com/adept";
            XNamespace nsdc = "http://purl.org/dc/elements/1.1/";

            var catalog = from i in xmlcatalog.Elements(nsAdept + "resourceItemInfo")
                          select new ACS4Catalog
                          {
                              resource = i.Element(nsAdept + "resource").Value,
                              src = i.Element(nsAdept + "src").Value,
                              title = i.Descendants(nsAdept + "metadata").Elements(nsdc + "title").Any() ? i.Descendants(nsAdept + "metadata").Elements(nsdc + "title").FirstOrDefault().Value : null,
                              creator = i.Descendants(nsAdept + "metadata").Elements(nsdc + "creator").Any() ? i.Descendants(nsAdept + "metadata").Elements(nsdc + "creator").FirstOrDefault().Value : null,
                              format = i.Descendants(nsAdept + "metadata").Elements(nsdc + "format").Any() ? i.Descendants(nsAdept + "metadata").Elements(nsdc + "format").FirstOrDefault().Value : null,
                              identifier = i.Descendants(nsAdept + "metadata").Elements(nsdc + "identifier").Any() ? i.Descendants(nsAdept + "metadata").Elements(nsdc + "identifier").FirstOrDefault().Value : null,
                              publisher = i.Descendants(nsAdept + "metadata").Elements(nsdc + "publisher").Any() ? i.Descendants(nsAdept + "metadata").Elements(nsdc + "publisher").FirstOrDefault().Value : null,
                              // isbn has to be derived from identifier
                              isbn = i.Descendants(nsAdept + "metadata").Elements(nsdc + "identifier").Any() ? i.Descendants(nsAdept + "metadata").Elements(nsdc + "identifier").FirstOrDefault().Value.Replace("-", "").Replace(" ", "").Trim() : null
                          };
            return catalog;
        }

        public static Dictionary<string, ACS4CatalogResourceMapItem> MapIsbnToACS4Resource(IEnumerable<ACS4Catalog> acs4Catalog)
        {
            // generate isbn to acs4 guid map - there are just a couple of dupes, not too many to worry about
            var mapIsbnToAcs4Resource = acs4Catalog.Where(i => i.IsValidIsbn).GroupBy(i => i.isbn).ToDictionary(g => g.Key, g => new ACS4CatalogResourceMapItem { Guid = g.First().resource, Publisher = g.First().publisher });
            return mapIsbnToAcs4Resource;
        }



    }


}
