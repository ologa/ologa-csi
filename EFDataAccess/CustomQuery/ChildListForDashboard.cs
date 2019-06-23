using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccess.CustomQuery
{
    public class ChildListForDashboard
    {
        public List<CSIDataChild> getChildList(int? partnerId, int? age0, int? age1, int? active, string gender) 
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();

            String query = @"SELECT CSI.CSIID AS CSIID, Child.ChildID AS ChildID, [CSIDomainScore].QuestionID,[CSIDomainScore].AnswerID, stype.Color as AnswerColor,
                        LTRIM(RTRIM(Child.FirstName)) + ' ' + LTRIM(RTRIM(Child.LastName)) AS FullName, IndexDate, Gender, DateOfBirth, 
	                        FLOOR(DATEDIFF(dd, DateOfBirth, getdate()) / 365.25) AS Age, [Partner].Name AS [Partner],
	                        CASE WHEN
	                        (SELECT TOP 1 ChildStatusID FROM [ChildStatusHistory] WHERE ChildID = [Child].[ChildID] AND 
								[EffectiveDate] >= [CSI].IndexDate ORDER BY [EffectiveDate] DESC, [ChildStatusHistoryID] DESC)= 4
								THEN 'GRADUATED'
								ELSE 'NOT GRADUATED' END AS IsGraduated 
	                        FROM [CSIDomainScore] INNER JOIN [CSIDomain] ON [CSIDomainScore].[CSIDomainID] = [CSIDomain].[CSIDomainID] 
	                        INNER JOIN [CSI] ON [CSIDomain].[CSIID] = [CSI].[CSIID] 
	                        INNER JOIN [Child] ON [CSI].[ChildID] = [Child].[ChildID]
	                        left join Answer a on a.AnswerID =  [CSIDomainScore].AnswerID
	                        inner join ScoreType stype on stype.ScoreTypeID = a.ScoreID
	                        LEFT JOIN [ChildPartner] ON [Child].[ChildID] = [ChildPartner].[ChildID] 
	                        LEFT JOIN [Partner] ON [ChildPartner].[PartnerID] = [Partner].[PartnerID] 
	                        WHERE IndexDate = (SELECT MAX(IndexDate) FROM CSI WHERE ChildID = Child.ChildID) ";

            if (partnerId > 0)
            {
                query = query + " AND [Partner].[PartnerID] = " + partnerId;
            }

            if(age0 >= 0 && age1> age0)
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

            string orderBy = " ORDER BY Child.LastName ASC,CSI.CSIID, IndexDate ASC";

            return dbContext.Database.SqlQuery<CSIDataChild>(query + orderBy).ToList();
        }
         
    }

    public class CSIDataChild
    {
        public int CSIID { get; set; }
        public int ChildID { get; set; }
        public int QuestionID { get; set; }
        public int? AnswerID { get; set; }
        public string AnswerColor { get; set; }
        public string FullName { get; set; }
        public DateTime IndexDate { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public decimal Age { get; set; }
        public string Partner { get; set; }
        public string IsGraduated { get; set; }
    }
}
