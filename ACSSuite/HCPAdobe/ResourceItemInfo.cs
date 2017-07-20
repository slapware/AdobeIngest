using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HCPACS4
{
    class ResourceItemInfo
    {
        public string resource { get; set; }
        public int resourceItem { get; set; }
        public ResourceItemInfoMetadata metadata { get; set; }
        public string src { get; set; }
        public string downloadType { get; set; }
        
    }



 //<resourceItemInfo>
 //   <resource>urn:uuid:000157e8-6253-492a-b3b3-5e5e0a55878c</resource>
 //   <resourceItem>0</resourceItem>
 //   <metadata>
 //     <dc:title xmlns:dc="http://purl.org/dc/elements/1.1/">Cross Your Heart, Connie Pickles</dc:title>
 //     <dc:creator xmlns:dc="http://purl.org/dc/elements/1.1/">Sabine Durrant</dc:creator>
 //     <dc:format xmlns:dc="http://purl.org/dc/elements/1.1/">application/epub+zip</dc:format>
 //     <dc:publisher xmlns:dc="http://purl.org/dc/elements/1.1/">HarperCollins</dc:publisher>
 //     <dc:language xmlns:dc="http://purl.org/dc/elements/1.1/">en</dc:language>
 //     <dc:identifier xmlns:dc="http://purl.org/dc/elements/1.1/">9780061880711</dc:identifier>
 //   </metadata>
 //   <src>http://xerxes.harpercollins.com/media/000157e8-6253-492a-b3b3-5e5e0a55878c.epub</src>
 //   <downloadType>simple</downloadType>
 //   <licenseToken>
 //     <resource>urn:uuid:000157e8-6253-492a-b3b3-5e5e0a55878c</resource>
 //     <permissions>
 //       <display/>
 //       <print>
 //         <maxResolution>300.0</maxResolution>
 //         <count initial="10" max="0" incrementInterval="604800"/>
 //       </print>
 //     </permissions>
 //   </licenseToken>
 // </resourceItemInfo>
}
