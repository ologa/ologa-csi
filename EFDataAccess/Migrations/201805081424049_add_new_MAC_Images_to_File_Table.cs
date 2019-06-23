namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.IO;
    using System.Reflection;

    public partial class add_new_MAC_Images_to_File_Table : DbMigration
    {
        public override void Up()
        {
            for (var i = 1; i <= 33; i++)
            {
                var imagename = "new_MAC_" + i + "";
                var imagevariable = "macPicture" + i + "";

                var outPutDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                
                //Project Development Path
                var developPath = Path.Combine(outPutDirectory, @"..\..\..\MAC\Resources\v2_Mac_Images\" + imagename + ".jpg");
                string develop_Path = new Uri(developPath).LocalPath;

                //CBO deployment Path
                var cboPath = Path.Combine(outPutDirectory, @"..\Resources\v2_Mac_Images\" + imagename + ".jpg");
                string cbo_Path = new Uri(cboPath).LocalPath;
                
                
                if (File.Exists(develop_Path))
                {
                    string sql = "INSERT INTO  [File] ([FileName] ,[Content]) SELECT '" + imagename + "', BulkColumn FROM Openrowset(Bulk '" + develop_Path + "', Single_Blob) as " + imagevariable + "";
                    Sql(sql, false);
                }
                else
                {
                    string sql = "INSERT INTO  [File] ([FileName] ,[Content]) SELECT '" + imagename + "', BulkColumn FROM Openrowset(Bulk '" + cbo_Path + "', Single_Blob) as " + imagevariable + "";
                    Sql(sql, false);
                }
            }
        }

        public override void Down()
        {
            for (var i = 1; i <= 33; i++)
            {
                var imagename = "new_MAC_" + i + "";
                Sql(@"DELETE FROM  [File] WHERE FileName = '" + imagename + "'", false);
            }
        }
    }
}
