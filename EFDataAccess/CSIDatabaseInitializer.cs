using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccess
{
    public class CSIDatabaseInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        /*
       protected override void Seed(CSIContext context)
       {
           
           var sqlFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.sql").OrderBy(x => x);
           foreach(string file in sqlFiles)
           {
               StreamReader readfilequery = new StreamReader(file);
               string query = "";
               string line = readfilequery.ReadLine();
               while (line != null)
               {
                   if (line.IndexOf("GO") == -1)
                   {
                       query = query + " " + line;
                   }
                   else
                   {
                       context.Database.ExecuteSqlCommand(query);                
                       query = null;
                   }
                   line = readfilequery.ReadLine();
               }
               //context.Database.ExecuteSqlCommand(File.ReadAllText(file));
           }
           base.Seed(context);
       }*/
    }
}
