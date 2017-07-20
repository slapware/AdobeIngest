using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HCPACS4.FileIO
{
    public class HCPFileInfo
    {
        public int DropFolderId { get; set; }
        public string DropFolderAlias { get; set; }
        public string FileName { get; set; }
        public string FullName { get; set; }
        public string FileArchivePath { get; set; }
        public DateTime ModifiedDate { get; set; }
        public long FileSize { get; set; }

        public static Regex REGEX_ISBN13 = new Regex(@"^(?'isbn13'97\d{11})\.", RegexOptions.IgnoreCase);
        public string Isbn13
        {
            get
            {
                Match m = REGEX_ISBN13.Match(this.FileName);
                if (m.Success)
                    return m.Groups["isbn13"].Value;
                else
                    return "NotIsbn13";
            }
        }

        public string FileExtension
        {
            get
            {
                if (this.FileName.Contains("."))
                    return this.FileName.Substring(this.FileName.LastIndexOf('.'));
                else
                    return null;
            }
        }
    }

}
