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
using System.Text;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;

namespace Adobe.Adept
{
    public class Utils
    {
        private static long initTime = GetCurrentTimeMillis();
        private static long counter = 0;

        /// <summary>
        /// Returns Epoch time in milliseconds
        /// </summary>
        /// <returns>Epoch time in milliseconds</returns>
        public static long GetCurrentTimeMillis()
        {
            return (DateTime.UtcNow.Ticks - 621355968000000000) / 10000;
        }

        /// <summary>
        /// Constructs shared secret for built-in distributor from the password
        /// </summary>
        /// <param name="builtInDistributorPassword">password</param>
        /// <returns>Shared secret as array of bytes</returns>
        public static byte[] GetBuiltInDistributorSharedSecret(string builtInDistributorPassword)
        {
            SHA1 hash = SHA1.Create();
            return hash.ComputeHash(Encoding.ASCII.GetBytes(builtInDistributorPassword));
        }

        /// <summary>
        /// Creates unique sequence of bytes
        /// </summary>
        /// <remarks>Note that the method is synchronized.</remarks>
        /// <returns>unique sequence of bytes</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static byte[] CreateNonce()
        {
            byte[] nonce = new byte[16];
		    nonce[0] = (byte) (initTime >> 56);
		    nonce[1] = (byte) (initTime >> 48);
		    nonce[2] = (byte) (initTime >> 40);
		    nonce[3] = (byte) (initTime >> 32);
		    nonce[4] = (byte) (initTime >> 24);
		    nonce[5] = (byte) (initTime >> 16);
		    nonce[6] = (byte) (initTime >> 8);
		    nonce[7] = (byte) initTime;
            
            counter++;
		    nonce[8] = (byte) (counter >> 56);
		    nonce[9] = (byte) (counter >> 48);
		    nonce[10] = (byte) (counter >> 40);
		    nonce[11] = (byte) (counter >> 32);
		    nonce[12] = (byte) (counter >> 24);
		    nonce[13] = (byte) (counter >> 16);
		    nonce[14] = (byte) (counter >> 8);
		    nonce[15] = (byte) counter;

            return nonce;
        }

        /// <summary>
        /// Default request expiration time is 15 minutes
        /// </summary>
        /// <returns>15 minutes from now</returns>
        public static DateTime GetDefaultExpirationTime()
        {
            return DateTime.Now.AddMinutes(15);
        }

        /// <summary>
        /// Calculates secure hash of sequence of bytes
        /// </summary>
        /// <param name="bytesToSign">bytes to be signed</param>
        /// <param name="sharedSecret">secret to be used</param>
        /// <returns>digest</returns>
        public static byte[] HmacBytes(byte[] bytesToSign, byte[] sharedSecret)
        {
            return HmacBytes(bytesToSign, 0, bytesToSign.Length, sharedSecret);
        }

        /// <summary>
        /// Calculates secure hash of sequence of bytes
        /// </summary>
        /// <param name="bytesToSign">bytes to be signed</param>
        /// <param name="offset">offset to start with</param>
        /// <param name="count">amount of bytes to be signed</param>
        /// <param name="sharedSecret">secret to be used</param>
        /// <returns>digest</returns>
        public static byte[] HmacBytes(byte[] bytesToSign, int offset, int count, byte[] sharedSecret)
        {
            // Create hash
            HMACSHA1 hmac = new HMACSHA1(sharedSecret);

            //Compute and return digest
            return hmac.ComputeHash(bytesToSign, offset, count);
        }

        /// <summary>
        /// Converts byte array to a hexadecimal string of nibbles. 
        /// All nibbles 'a' to 'f' are in lowercase.
        /// </summary>
        /// <param name="bytes">Array of bytes to be converted</param>
        /// <returns>hexadecimal string without any prefix</returns>
        public static string ByteArrayToHexString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                sb.AppendFormat("{0:x2}", b);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets W3C DTF string for the given time
        /// </summary>
        /// <param name="time">time to convert</param>
        /// <returns>W3C DTF string for the given time</returns>
        public static string DateTimeToW3CString(DateTime time)
        {
            return time.ToUniversalTime().ToString("u").Replace(" ", "T");
        }

        /// <summary>
        ///  Calculates HMAC-SHA1 digest and returns it as a hexadecimal string.
        /// </summary>
        /// <param name="queryParams">URL parameters to be signed</param>
        /// <param name="sharedSecret">shared secret as an array of bytes</param>
        /// <returns>HMAC-SHA1 digest as a hexadecimal string that should be added to the URL as the value of auth parameter</returns>
        public static string CreateUrlAuthValue(string queryParams, byte[] sharedSecret)
        {
            // Convert unicode string to sequence of bytes
            byte[] bytesToSign = Encoding.ASCII.GetBytes(queryParams);

            // Compute digest
            byte[] digest = Utils.HmacBytes(bytesToSign, sharedSecret);

            // Return hash as a hexadecimal string
            return Utils.ByteArrayToHexString(digest);
        }


        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }
    }
}