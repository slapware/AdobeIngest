using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCPACS4.FileIO
{
    public class FileUtils
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }


        public static string SaveFile(string path, string fileName, byte[] data)
        {
            string localPath = path.TrimEnd('\\');
            string localFile = Path.Combine(path, fileName);
            localPath = Path.GetDirectoryName(localFile);

            try
            {
                if (!Directory.Exists(localPath))
                    Directory.CreateDirectory(localPath);

                File.WriteAllBytes(localFile, data);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Exception in SaveFile {0}", localFile), ex);
                localFile = String.Empty;
            }
            return localFile;
        }

        public static string SaveFile(string path, string fileName, Stream inputStream)
        {
            string localPath = path.TrimEnd('\\');
            string localFile = Path.Combine(path, fileName);
            localPath = Path.GetDirectoryName(localFile);

            try
            {
                if (!Directory.Exists(localPath))
                    Directory.CreateDirectory(localPath);

                using (FileStream outputStream = File.OpenWrite(localFile))
                {
                    inputStream.CopyTo(outputStream);
                }
                
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Exception in SaveFile {0}", localFile), ex);
                localFile = String.Empty;
            }
            return localFile;
        }


        public static string SaveToAzureBlob(byte[] data, string fileName)
        {
            Guid guid = Guid.NewGuid();

            string blobName = String.Format("{0}_{1}",guid, fileName);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(HCPACS4.AdobeConfig.AZURE_STORAGE_ACCT);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(HCPACS4.AdobeConfig.AZURE_BLOB_CONTAINER);

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
            blockBlob.UploadFromByteArray(data, 0, data.Length);

            return blockBlob.Uri.ToString();
        }

        public static string SaveToAzureBlob(string fileName)
        {
            Guid guid = Guid.NewGuid();
            string blobName = String.Format("{0}_{1}", guid, Path.GetFileName(fileName));
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(HCPACS4.AdobeConfig.AZURE_STORAGE_ACCT);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(HCPACS4.AdobeConfig.AZURE_BLOB_CONTAINER);

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
            blockBlob.UploadFromFile(fileName);

            return blockBlob.Uri.ToString();
        }
    }
}
