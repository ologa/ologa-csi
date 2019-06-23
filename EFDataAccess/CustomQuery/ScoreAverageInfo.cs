using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccess.CustomQuery
{
   public class ScoreAverageInfo
    {

       public List<ScoreAverage> ScoreAndAverage(int? partnerId, int? age0, int? age1, int? active, string gender)
       {
           ApplicationDbContext DbContext = new ApplicationDbContext();

           String query = @"SELECT
                                DomainChild.DomainID,
                                S.Color,
                                S.CutPointID,
                                ROUND(AVG(DomainChild.AverageScore),2) AS Score,
                                COUNT(*) AS Total
                                FROM
                                (	SELECT
		                                [Child].ChildID,
		                                [CSIDomain].DomainID,
		                                ROUND(AVG(CAST([SCORETYPE].Score AS FLOAT)),2) AS AverageScore

	                                From [CSIDomainScore] 
	                                INNER JOIN [CSIDomain] ON [CSIDomainScore].[CSIDomainID] = [CSIDomain].[CSIDomainID] 
	                                INNER JOIN [CSI] ON [CSIDomain].[CSIID] = [CSI].[CSIID] 
	                                INNER JOIN [Child] ON [CSI].[ChildID] = [Child].[ChildID]
	                                INNER JOIN [ANSWER] ON [CSIDomainScore].[AnswerID] = [ANSWER].[AnswerID]
	                                INNER JOIN [SCORETYPE] ON [ANSWER].[ScoreID] = [SCORETYPE].[ScoreTypeID]
	                                LEFT JOIN [ChildPartner] ON [Child].[ChildID] = [ChildPartner].[ChildID] 
	                                LEFT JOIN [Partner] ON [ChildPartner].[PartnerID] = [Partner].[PartnerID] 
	                                WHERE IndexDate = (SELECT MAX(IndexDate) 
					                                 FROM CSI WHERE ChildID = Child.ChildID) AND [CSIDomainScore].AnswerID is not null
                                    AND [ANSWER].[ExcludeFromReport] = 0
	                                ";

           if (partnerId > 0)
           {
               query = query + " AND [Partner].[PartnerID] = " + partnerId;
           }

           if (age0 >= 0 && age1 > age0)
           {
               query = query + string.Format(@" AND FLOOR(DATEDIFF(dd, DateOfBirth, getdate()) / 365.25) >= {0} AND 
                                                    FLOOR(DATEDIFF(dd, DateOfBirth, getdate()) / 365.25) <= {1} ", age0, age1);
           }

           if (active == 0)
           {
               query = query + @" AND (SELECT TOP 1 ChildStatusID FROM [ChildStatusHistory] WHERE ChildID = [Child].[ChildID] AND 
                    [EffectiveDate] <= getdate() ORDER BY [EffectiveDate] DESC, [ChildStatusHistoryID] DESC) = 1 ";
           }

           if (active == 1)
           {
               query = query + @" AND (SELECT TOP 1 ChildStatusID FROM [ChildStatusHistory] WHERE ChildID = [Child].[ChildID] 
                        AND [EffectiveDate] <= getdate() ORDER BY [EffectiveDate] DESC, [ChildStatusHistoryID] DESC) > 1";
           }

           if (gender == "Male")
           {
               query = query + " AND Gender = 'M'";
           }

           if (gender == "Female")
           {
               query = query + " AND Gender = 'F'";
           }

           string query2 = @" GROUP BY
	                                [Child].ChildID,
		                            [CSIDomain].DomainID
                                ) DomainChild
                                INNER JOIN CutPoint S ON (S.InitialValue <= AverageScore AND S.FinalValue >= AverageScore)
                                GROUP BY
                                [DomainChild].DomainID,
                                S.Color,
                                S.CutPointID
                                ORDER BY DomainID";

           return DbContext.Database.SqlQuery<ScoreAverage>(query + query2).ToList();
       }

    }

    public class ScoreAverage
    {
        public int DomainID { get; set; }
        public int Total { get; set; }
        public double Score { get; set; }
        public int ScoreID { get; set; }
        public string Color { get; set; }
    }
}
