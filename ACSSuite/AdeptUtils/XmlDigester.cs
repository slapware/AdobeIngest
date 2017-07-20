/*
ADOBE SYSTEMS INCORPORATED
 Copyright 2009, Adobe Systems Incorporated
 All Rights Reserved.

NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the 
terms of the Adobe license agreement accompanying it.  If you have received this file from a 
source other than Adobe, then your use, modification, or distribution of it requires the prior 
written permission of Adobe.

*/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;


namespace Adobe.Adept
{
    public class XmlDigester
    {
        private const byte BEGIN_ELEMENT = 1;
        private const byte END_ATTRIBUTES = 2;
        private const byte END_ELEMENT = 3;
        private const byte TEXT_NODE = 4;
        private const byte ATTRIBUTE = 5;

        StringBuilder text;

        private class AttributeComparer : System.Collections.IComparer
        {
            public int Compare(Object x, Object y)
            {
                XmlAttribute lhs = (XmlAttribute)x;
                XmlAttribute rhs = (XmlAttribute)y;
                string ls = lhs.NamespaceURI;
                string rs = rhs.NamespaceURI;
                if (ls == rs)
                {
                    ls = lhs.LocalName;
                    rs = rhs.LocalName;
                }

                byte[] larr = Encoding.UTF8.GetBytes(ls);
                byte[] rarr = Encoding.UTF8.GetBytes(rs);

                int len = Math.Min(larr.Length, rarr.Length);
                for (int i = 0; i < len; i++)
                {
                    if (larr[i] < rarr[i])
                        return -1;

                    if (larr[i] > rarr[i])
                        return 1;
                }

                if (larr.Length < rarr.Length)
                    return -1;
                if (larr.Length > rarr.Length)
                    return 1;

                return 0;
            }
        }

        public XmlDigester()
        {
            text = new StringBuilder();
        }

        private void FlushText(Stream outStream)
        {
            // Right trim the text
            int i = text.Length - 1;
            while (i >= 0 && Char.IsWhiteSpace(text[i]))
                i--;
            int len = i + 1;

            // Now serialize
            if (len > 0)
            {
                int done = 0;
                do
                {
                    int remains = Math.Min(len - done, 0x7FFF);
                    Serialize(TEXT_NODE, outStream);
                    Serialize(text.ToString(done, remains), outStream);
                    done += remains;
                } while (done < len);
            }

            // Reset the buffer
            text.Length = 0;
        }

        private void Serialize(byte b, Stream outStream)
        {
            outStream.WriteByte(b);
        }

        private void Serialize(byte[] array, Stream outStream)
        {
            outStream.Write(array, 0, array.Length);
        }

        private void Serialize(string s, Stream outStream)
        {
            Serialize((byte)(s.Length >> 8), outStream);
            Serialize((byte)(s.Length & 0xFF), outStream);
            Serialize(Encoding.UTF8.GetBytes(s), outStream);
        }

        private void Serialize(XmlAttribute node, Stream outStream)
        {
            string ns = node.NamespaceURI;
            if (ns == null)
                ns = "";

            if (ns != XmlUtils.XmlnsURI)
            {
                Serialize(ATTRIBUTE, outStream);
                Serialize(node.NamespaceURI, outStream);
                Serialize(node.LocalName, outStream);
                Serialize(node.Value, outStream);
            }
        }

        private void Serialize(XmlCharacterData node, Stream outStream)
        {
            string val = (text.Length == 0 ? node.Value.TrimStart() : node.Value);
            text.Append(val);
        }

        public void Serialize(XmlElement node, Stream outStream)
        {
            // ignore signature and hmac elements
            if (node.NamespaceURI == XmlUtils.AdeptURI && (node.LocalName == XmlUtils.HmacName || node.LocalName == XmlUtils.SignatureName))
                return;

            FlushText(outStream);
            Serialize(BEGIN_ELEMENT, outStream);
            Serialize(node.NamespaceURI, outStream);
            Serialize(node.LocalName, outStream);

            XmlAttribute[] attrs = new XmlAttribute[node.Attributes.Count];
            node.Attributes.CopyTo(attrs, 0);
            Array.Sort(attrs, new AttributeComparer());
            foreach (XmlAttribute attr in attrs)
            {
                Serialize(attr, outStream);
            }
            Serialize(END_ATTRIBUTES, outStream);

            foreach (XmlNode child in node.ChildNodes)
            {
                if (child is XmlElement)
                    Serialize(child as XmlElement, outStream);
                else if (child is XmlCDataSection || child is XmlText)
                    Serialize(child as XmlCharacterData, outStream);
            }
            FlushText(outStream);
            Serialize(END_ELEMENT, outStream);
        }

        public byte[] Serialize(XmlElement node)
        {
            MemoryStream ms = new MemoryStream();
            Serialize(node, ms);
            return ms.ToArray();
        }
    }
}
