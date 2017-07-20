using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HCPACS4
{
    public class ACS4Catalog
    {
        public string resource { get; set; }
        public string src { get; set; }
        public string title { get; set; }
        public string creator { get; set; }
        public string format { get; set; }
        public string identifier { get; set; }
        public string isbn { get; set; }
        public string publisher { get; set; }
        public string description { get; set; }

        static Regex _rxIsbn13 = new Regex(@"97\d{11}", RegexOptions.IgnoreCase);

        public bool IsValidIsbn
        {
            get
            {
                if (String.IsNullOrEmpty(this.isbn))
                    return false;

                Match m = _rxIsbn13.Match(this.isbn);
                return m.Success;
            }
        }
    }
}
