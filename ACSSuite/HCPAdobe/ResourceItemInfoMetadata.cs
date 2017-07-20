using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HCPACS4
{
    class ResourceItemInfoMetadata
    {
        public string title { get; set; }
        public string creator { get; set; }
        public string format { get; set; }
        public string publisher { get; set; }
        public string language { get; set; }
        public string identifier { get; set; }
    }


    //   <metadata>
    //     <dc:title xmlns:dc="http://purl.org/dc/elements/1.1/">Cross Your Heart, Connie Pickles</dc:title>
    //     <dc:creator xmlns:dc="http://purl.org/dc/elements/1.1/">Sabine Durrant</dc:creator>
    //     <dc:format xmlns:dc="http://purl.org/dc/elements/1.1/">application/epub+zip</dc:format>
    //     <dc:publisher xmlns:dc="http://purl.org/dc/elements/1.1/">HarperCollins</dc:publisher>
    //     <dc:language xmlns:dc="http://purl.org/dc/elements/1.1/">en</dc:language>
    //     <dc:identifier xmlns:dc="http://purl.org/dc/elements/1.1/">9780061880711</dc:identifier>
    //   </metadata>
}
