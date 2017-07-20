using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCPACS4
{
    /// <summary>
    /// POCO to define object returned by MapIsbnToACS4Resource()
    /// </summary>
    public class ACS4CatalogResourceMapItem
    {
        const string GALLEY_PUBLISHER = "Harper Collins eGalley - NOT for re-sale";

        public string Guid { get; set; }
        public string Publisher { get; set; }

        public bool IsGalley
        {
            get
            {
                return String.Equals(this.Publisher, GALLEY_PUBLISHER);
            }

        }

    }
}
