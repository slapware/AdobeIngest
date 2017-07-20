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
using System.Xml;
using Adobe.Adept;
using System.Security.Cryptography;
using System.IO;

namespace Adobe.Adept
{
    public class XmlUtils
    {
        public const string AdeptURI = "http://ns.adobe.com/adept";
        public const string XmlnsURI = "http://www.w3.org/2000/xmlns/";

        public const string HmacName = "hmac";
        public const string SignatureName = "signature";
        public const string NonceName = "nonce";
        public const string ExpirationName = "expiration";

        /// <summary>
        /// Adds automatically generated nonce and expiration elements to the XML message to be sent 
        /// to Adobe Content Server, adds the signature as hmac element.
        /// </summary>
        /// <param name="message">XML message to be sent to the Adobe Constent Server serialized to string</param>
        /// <param name="sharedSecret">Secret shared between Adobe Content Server distributor and operator</param>
        /// <returns>String representation of XML message with added nonce, expiration and hmac elements</returns>
        public static string GetSignedMessage(string message, byte[] sharedSecret)
        {
            return GetSignedMessage(message, Utils.CreateNonce(), Utils.GetDefaultExpirationTime(), sharedSecret);
        }

        /// <summary>
        /// Adds given nonce and expiration elements to the XML message to be sent 
        /// to Adobe Content Server, adds the signature as hmac element.
        /// </summary>
        /// <param name="message">Root element of the XML message to be sent to Adobe Content Server</param>
        /// <param name="nonce">unique sequence of bytes</param>
        /// <param name="expirationTime">Request experation time</param>
        /// <param name="sharedSecret">Secret shared between Adobe Content Server distributor and operator</param>
        /// <returns>String representation of XML message with added nonce, expiration and hmac elements</returns>
        public static string GetSignedMessage(string message, byte[] nonce, DateTime expirationTime, byte[] sharedSecret)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(message);
            SignMessage(doc.DocumentElement, nonce, expirationTime, sharedSecret);
            return doc.DocumentElement.OuterXml;
        }

        /// <summary>
        /// Adds automatically generated nonce and expiration elements to the XML message to be sent 
        /// to Adobe Content Server, adds the signature as hmac element.
        /// </summary>
        /// <param name="message">Root element of the XML message to be sent to Adobe Content Server</param>
        /// <param name="sharedSecret">Secret shared between Adobe Content Server distributor and operator</param>
        public static void SignMessage(XmlElement message, byte[] sharedSecret)
        {
            SignMessage(message, Utils.CreateNonce(), Utils.GetDefaultExpirationTime(), sharedSecret);
        }

        /// <summary>
        /// Adds given nonce and expiration elements to the XML message to be sent 
        /// to Adobe Content Server, adds the signature as hmac element.
        /// </summary>
        /// <param name="message">Root element of the XML message to be sent to Adobe Content Server</param>
        /// <param name="nonce">unique sequence of bytes</param>
        /// <param name="expirationTime">Request experation time</param>
        /// <param name="sharedSecret">Secret shared between Adobe Content Server distributor and operator</param>
        public static void SignMessage(XmlElement message, byte[] nonce, DateTime expirationTime, byte[] sharedSecret)
        {
            XmlElement child = message.OwnerDocument.CreateElement(ExpirationName, AdeptURI);
            //Converting DateTime to W3C DTF
            child.InnerText = Utils.DateTimeToW3CString(expirationTime);
            message.AppendChild(child);

            child = message.OwnerDocument.CreateElement(NonceName, AdeptURI);
            child.InnerText = Convert.ToBase64String(nonce);
            message.AppendChild(child);

            // Create hash
            HMACSHA1 hmac = new HMACSHA1(sharedSecret);

            //Create crypto stream
            CryptoStream cs = new CryptoStream(Stream.Null, hmac, CryptoStreamMode.Write);

            // Serializes the XML to crypto stream, digest is calculated on-the-fly
            new XmlDigester().Serialize(message, cs);

            //This flushes all the buffers
            cs.Close();

            child = message.OwnerDocument.CreateElement(HmacName, AdeptURI);
            child.InnerText = Convert.ToBase64String(hmac.Hash);
            message.AppendChild(child);
        }

    }
}
