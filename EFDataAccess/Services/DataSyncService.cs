using EFDataAccess.Logging;
using EFDataAccess.Services;
using EFDataAccess.UOW;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using VPPS.CSI.Domain;

namespace EFDataAccess.Services
{
    public class DataSyncService : BaseService
    {
        //defined scope.
        public static string[] Scopes = { DriveService.Scope.Drive };
        protected ILogger logger = new Logger();
        private SiteService siteService = new SiteService(new UnitOfWork());
        //private Repository.IRepository<DataSyncInfo> DataSyncRepository => UnitOfWork.Repository<DataSyncInfo>();


        //private DataSyncService dataSyncService;

        //public DataSyncService(UnitOfWork uow) : base(uow)
        //{
        //    dataSyncService = new DataSyncService(uow);
        //}

        public class DataImportData
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public long? Size { get; set; }
        }

        //create Drive API service.
        public void GetService(string fileName, string CSVType)
        {
            if (!System.Configuration.ConfigurationManager.AppSettings["GoogleSync"].Equals("True"))
            {
                return;
            }

            //get Credentials from client_secret.json file 
            UserCredential credential;

            string credentialsPath = @"C:\client_secret.json";
            using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
            {
                String FolderPath = @"C:\";
                String FilePath = Path.Combine(FolderPath, "DriveServiceCredentials.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(FilePath, true)).Result;
            }

            //create Drive API service.
            DriveService service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "GoogleDriveRestAPI-v3",
            });

            service.HttpClient.Timeout = TimeSpan.FromMinutes(100);
            //Long Operations like file uploads might timeout. 100 is just precautionary value, 
            //can be set to any reasonable value depending on what you use your service for.

            Site site = siteService.findById(1);
            var siteName="";
            var csvName = "";

            if (CSVType == "MEReport")
            {
                //For MEReport set SiteName: M&EDataExported
                siteName = "M&EDataExported";
            }
            else
            {
                string today = DateTime.Now.ToString("yyyy.MM.dd");
                siteName = today + "_" + site.SiteName + "_" + CSVType;
                csvName = Path.GetFileName(fileName);
            }
            
            List<DataImportData> AllFoldersList = GetDriveFolders(service);

            
            if (AllFoldersList.Count > 0)
            {
                if (AllFoldersList.Any(i => i.Name == siteName))
                {
                    DataImportData existingFolder = AllFoldersList.Where(i => i.Name == siteName).FirstOrDefault();
                    var parentId = existingFolder.Id;
                    logger.Information("Importando " + csvName + " para a pasta da OCB: " + existingFolder.Name + " no Google Drive");
                    UploadBasicCSVs(existingFolder.Id, fileName, service);
                }
                else
                {
                    var folderID = CreateFolder(siteName, service);
                    DataImportData existingFolder = AllFoldersList.Where(i => i.Id == folderID).FirstOrDefault();
                    var parentId = existingFolder.Id;
                    logger.Information("A pasta da OCB: " + siteName + " foi criada no Google Drive com Sucesso!!!");
                    logger.Information("Importando " + csvName + " para a pasta da OCB: " + siteName + " no Google Drive");
                    UploadBasicCSVs(parentId, fileName, service);
                }
            }
            else
            {
                var folderID = CreateFolder(siteName, service);
                AllFoldersList = GetDriveFolders(service);
                DataImportData existingFolder = AllFoldersList.Where(i => i.Id == folderID).FirstOrDefault();
                var parentId = existingFolder.Id;
                logger.Information("A pasta da OCB: " + siteName + " foi criada no Google Drive com Sucesso!!!");
                logger.Information("Importando " + csvName + " para a pasta da OCB: " + siteName + " no Google Drive");
                UploadBasicCSVs(parentId, fileName, service);
            }

            //FileUploadInFolder(folderID, caminho, service);

            //return service;
        }

        private static string CreateFolder(string folderName, DriveService service)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = folderName,
                MimeType = "application/vnd.google-apps.folder"
            };
            var request = service.Files.Create(fileMetadata);
            request.Fields = "id";
            var file = request.Execute();

            return file.Id;
            //Console.WriteLine("Folder ID: " + file.Id);

        }


        public static List<DataImportData> GetDriveFolders(DriveService service)
        {
            List<DataImportData> FolderList = new List<DataImportData>();

            Google.Apis.Drive.v3.FilesResource.ListRequest request = service.Files.List();
            request.Q = "mimeType='application/vnd.google-apps.folder'";
            request.Fields = "files(id, name)";

            Google.Apis.Drive.v3.Data.FileList result = request.Execute();
            foreach (var file in result.Files)
            {
                DataImportData File = new DataImportData
                {
                    Id = file.Id,
                    //FileType = "Folder",
                    //ParentFileId = null,
                    Name = file.Name,
                    Size = file.Size
                };
                FolderList.Add(File);
            }
            return FolderList;
        }


        private static void UploadBasicCSVs(string folderID, string path, DriveService service)
        {
            var fileMetaData = new Google.Apis.Drive.v3.Data.File()
            {
                Name = Path.GetFileName(path),
                MimeType = "text/plain",
                Parents = new List<string>
                {
                    folderID
                }
            };

            //DataImportData File = new DataImportData
            //{
            //    Id = fileMetaData.Id,
            //    //FileType = "CSV",
            //    ParentFileId = parentId,
            //    Name = fileMetaData.Name,
            //    Size = fileMetaData.Size
            //};

            FilesResource.CreateMediaUpload request;
            using (var stream = new System.IO.FileStream(path, System.IO.FileMode.Open))
            {
                request = service.Files.Create(fileMetaData, stream, "text/plain");
                request.Fields = "id";
                request.Upload();
            }

            var file = request.ResponseBody;

            //Console.WriteLine("File ID: " + file.Id);

        }

        //public void Save(DataSyncInfo DataSyncInfo)
        //{
        //    AdultRepository.Add(Adult);
        //}

        //// file save to server path
        //private static void SaveStream(MemoryStream stream, string FilePath)
        //{
        //    using (System.IO.FileStream file = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite))
        //    {
        //        stream.WriteTo(file);
        //    }
        //}

    }
}
