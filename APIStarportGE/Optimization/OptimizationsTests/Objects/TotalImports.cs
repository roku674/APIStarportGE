
//Created by Alexander Fields 

namespace OptimizedPaymentsTests.Objects
{
    public class TotalImports
    {
        /// <summary>
        /// Default
        /// </summary>
        public TotalImports()
        {
        }

        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="vendor"></param>
        /// <param name="clientId"></param>
        /// <param name="fileDescription"></param>
        /// <param name="uniqueValueByClient"></param>
        /// <param name="blobContainer"></param>
        /// <param name="folderPath"></param>
        /// <param name="drivePath"></param>
        /// <param name="fileCannotContain"></param>
        /// <param name="emptyFileSize"></param>
        /// <param name="dataLayerTables"></param>
        /// <param name="fileId"></param>
        /// <param name="fileName"></param>
        /// <param name="importDate"></param>
        /// <param name="fileRuleId"></param>
        /// <param name="oPCNotes"></param>
        /// <param name="fileDownloadId"></param>
        /// <param name="sftpId"></param>
        /// <param name="modifiedFileName"></param>
        /// <param name="downloadDate"></param>
        /// <param name="fileSize"></param>
        /// <param name="fileBytes"></param>
        public TotalImports(int itemId, string vendor, int clientId, string fileDescription, string uniqueValueByClient, string blobContainer, string folderPath, string drivePath, string fileCannotContain, int emptyFileSize, string dataLayerTables, int fileId, string fileName, System.DateTime importDate, int fileRuleId, string oPCNotes, int fileDownloadId, int sftpId, string modifiedFileName, System.DateTime downloadDate, int fileSize, byte[] fileBytes)
        {
            ItemId = itemId;
            Vendor = vendor;
            ClientId = clientId;
            FileDescription = fileDescription;
            UniqueValueByClient = uniqueValueByClient;
            BlobContainer = blobContainer;
            FolderPath = folderPath;
            DrivePath = drivePath;
            FileCannotContain = fileCannotContain;
            EmptyFileSize = emptyFileSize;
            DataLayerTables = dataLayerTables;
            FileId = fileId;
            FileName = fileName;
            ImportDate = importDate;
            FileRuleId = fileRuleId;
            OPCNotes = oPCNotes;
            FileDownloadId = fileDownloadId;
            SftpId = sftpId;
            ModifiedFileName = modifiedFileName;
            DownloadDate = downloadDate;
            FileSize = fileSize;
            FileBytes = fileBytes;
        }

        public int ItemId { get; set; }
        public string Vendor { get; set; }
        public int ClientId { get; set; }
        public string FileDescription { get; set; }
        public string UniqueValueByClient { get; set; }
        public string BlobContainer { get; set; }
        public string FolderPath { get; set; }
        public string DrivePath { get; set; }
        public string FileCannotContain { get; set; }
        public int EmptyFileSize { get; set; }
        public string DataLayerTables { get; set; }
        public int FileId { get; set; }
        public string FileName { get; set; }
        public System.DateTime ImportDate { get; set; }
        public int FileRuleId { get; set; }
        public string OPCNotes { get; set; }
        public int FileDownloadId { get; set; }
        public int SftpId { get; set; }
        public string ModifiedFileName { get; set; }
        public System.DateTime DownloadDate { get; set; }
        public int FileSize { get; set; }
        public byte[] FileBytes { get; set; }
    }
}