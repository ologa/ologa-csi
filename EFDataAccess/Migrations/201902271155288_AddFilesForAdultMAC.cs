namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.IO;
    using System.Reflection;


    public partial class AddFilesForAdultMAC : DbMigration
    {
        public override void Up()
        {



            for (int i = 1; i <= 17; i++)
            {
                int FileID = 189 + i;
                var imagename = "Mdc_" + i + "";
                var imagevariable = "macPicture" + i + "";

                var outPutDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);

                //Project Development Path
                var developPath = Path.Combine(outPutDirectory, @"..\..\..\MAC\Resources\V3_Mdc_Images\" + imagename + ".jpg");
                string develop_Path = new Uri(developPath).LocalPath;

                //CBO deployment Path
                var cboPath = Path.Combine(outPutDirectory, @"..\Resources\V3_Mdc_Images\" + imagename + ".jpg");
                string cbo_Path = new Uri(cboPath).LocalPath;

                if (File.Exists(develop_Path))
                {
                    string sql = @"  
                                          INSERT INTO  [File] ([FileName] ,[Content]) 
                                          SELECT '" + imagename + "', BulkColumn FROM Openrowset(Bulk '" + develop_Path + "', Single_Blob) as " + imagevariable + "";

                    Sql(sql, false);

                }
                else
                {
                    string sql = @"  
                                          INSERT INTO[File] ([FileName],[Content])  
                                          SELECT '" + imagename + "', BulkColumn FROM Openrowset(Bulk '" + cbo_Path + "', Single_Blob) as " + imagevariable + "";
                    Sql(sql, false);

                }
            }
        }

        public override void Down()
        {
        }
    }
}
