using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HCPACS4
{
    public class Epub
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string DropFolderAlias { get; set; }
        public string FullName { get; set; }
        public string Isbn { get; set; }
        public string OpfFileName { get; set; }
        public bool HasOpf { get; set; }
        public string OpfIdentifier { get; set; }
        public string IsbnFromOpf { get; set; }
        public string IsbnFromFileName { get; set; }
        public string ACS4Guid { get; set; }
        public string ACS4Publisher { get; set; }
        public bool InvalidDocType { get; set; }
        public long FileSizeBytes { get; set; }
        public bool CorruptFile { get; set; }

 

        public bool IsGalley
        {
            get
            {
                return this.DropFolderAlias.ToLower().Contains("galley");
            }

        }

        public Epub(string fullName, string dropFolderAlias)
        {
            this.DropFolderAlias = dropFolderAlias;
            this.FullName = fullName;
            this.InvalidDocType = false;
            this.HasOpf = false;
            this.CorruptFile = false;
            this.Initialize();
        }


        private void Initialize()
        {
            this.AnalyzeEpub();
        }

        private void AnalyzeEpub()
        {
            this.FileSizeBytes = new FileInfo(this.FullName).Length;
            this.IsbnFromFileName = HCPUtils.GetIsbn13(this.FullName);
            this.Isbn = this.IsbnFromFileName;
            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(this.FullName))
                {
                    
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        // check for invalid DOCTYPE in any .xhtml file
                        if (entry.FullName.EndsWith(".xhtml") && !this.InvalidDocType)
                        {
                            using (var stream = entry.Open())
                            using (var reader = new StreamReader(stream))
                            {
                                string xhtml = reader.ReadToEnd();
                                if (xhtml.Contains("about:legacy-compat"))
                                    this.InvalidDocType = true;
                            }
                        }

                        // check .opf - primarily for isbn
                        if (entry.FullName.EndsWith(".opf"))
                        {
                            this.OpfFileName = entry.FullName;
                            this.HasOpf = true;

                            // get isbn from opf
                            using (var stream = entry.Open())
                            {
                                XDocument xmlOpf = XDocument.Load(stream);

                                XNamespace dc = "http://purl.org/dc/elements/1.1/";
                                XNamespace opf = "http://www.idpf.org/2007/opf";
                                this.OpfIdentifier = xmlOpf.Descendants(opf + "metadata").Elements(dc + "identifier").FirstOrDefault().Value;
                                
                                if (!String.IsNullOrEmpty(this.OpfIdentifier))
                                {
                                    // sometimes isbns are formatted with dashes
                                    string tmpIsbn = this.OpfIdentifier.Replace("-", "");
                                    this.IsbnFromOpf = HCPUtils.GetIsbn13(tmpIsbn);
                                }
                            }
                        }
                    }
                }

            }
            catch (System.IO.InvalidDataException exIO)
            {
                this.CorruptFile = true;
                log.Error(String.Format("IO Exception analyzing Epub {0} - {1}", this.FullName, exIO.StackTrace));
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Exception analyzing Epub {0} - {1}", this.FullName, ex.StackTrace));
            }
        }

        
        

        public string GetEpubAsBase64()
        {
            byte[] bytes = File.ReadAllBytes(this.FullName);
            return Convert.ToBase64String(bytes);
        }
    }

}
