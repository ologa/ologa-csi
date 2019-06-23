using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccess.CustomQuery
{
    public class ChildDomainReport
    {
        public List<ChildPartnerInfo> GetChildDomainReport(DateTime searchFrom, DateTime searchTo, Int32 activeOption, Boolean byGender, Boolean byAgeGroup, Int32 domainID)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();

            String query = @"SELECT [Partner].[PartnerID], [DomainID], [CSIDomainScore].[QuestionID], [ScoreID]";

            if (byGender)
            {
                query = query + @", [Gender] ";
            }

            if (byAgeGroup)
            {
                query = query + @", CASE WHEN (DATEDIFF(dd, [DateOfBirth], getdate())) < 1825 THEN '0 - 5'
                                    WHEN (DATEDIFF(dd, [DateOfBirth], getdate())) BETWEEN 1825 AND 4379 THEN '6 - 12' 
                                    WHEN (DATEDIFF(dd, [DateOfBirth], getdate())) > 4380 THEN '>12' END AS AgeGroup";
            }

            query = query + @", COUNT(*) AS NrChild 
                From [CSIDomainScore] 
					  INNER JOIN [CSIDomain] ON [CSIDomainScore].[CSIDomainID] = [CSIDomain].[CSIDomainID] 
					  INNER JOIN [CSI] C ON [CSIDomain].[CSIID] = C.[CSIID] 
					  INNER JOIN [Child] ON C.[ChildID] = [Child].[ChildID]
					  INNER JOIN [ANSWER] ON [CSIDomainScore].[AnswerID] = [ANSWER].[AnswerID]
					  INNER JOIN [SCORETYPE] ON [ANSWER].[ScoreID] = [SCORETYPE].[ScoreTypeID]
					  LEFT JOIN [ChildPartner] ON [Child].[ChildID] = [ChildPartner].[ChildID] 
					  LEFT JOIN [Partner] ON [ChildPartner].[PartnerID] = [Partner].[PartnerID]
                      WHERE [ANSWER].[ExcludeFromReport] = 0 AND [DomainID] = " + domainID + " AND C.[CSIID] = (SELECT MAX(CSIID) FROM [CSI] WHERE [ChildID] = C.[ChildID] AND [IndexDate] BETWEEN '" + searchFrom.ToString("yyyy-MM-dd HH:mm:ss") + "' AND '" + searchTo.ToString("yyyy-MM-dd HH:mm:ss") + "') ";

            if (activeOption == 1)
            {
                query = query + @" AND (SELECT TOP 1 ChildStatusID FROM [ChildStatusHistory] WHERE ChildID = C.[ChildID] AND 
                    [EffectiveDate] <= getdate() ORDER BY [EffectiveDate] DESC, [ChildStatusHistoryID] DESC) = 1 ";
            }

            if (activeOption == 2)
            {
                query = query + @"AND (SELECT TOP 1 ChildStatusID FROM [ChildStatusHistory] WHERE ChildID = C.[ChildID] 
                        AND [EffectiveDate] <= getdate() ORDER BY [EffectiveDate] DESC, [ChildStatusHistoryID] DESC) > 1";
            }

            query = query + @"GROUP BY [Partner].[PartnerID], [DomainID], [CSIDomainScore].[QuestionID], [ScoreID]";

            if (byGender)
            {
                query = query + @", [Gender] ";
            }

            if (byAgeGroup)
            {
                query = query + @", CASE WHEN (DATEDIFF(dd, [DateOfBirth], getdate())) < 1825 THEN '0 - 5' 
                                    WHEN (DATEDIFF(dd, [DateOfBirth], getdate())) BETWEEN 1825 AND 4379 THEN '6 - 12' 
                                    WHEN (DATEDIFF(dd, [DateOfBirth], getdate())) > 4380 THEN '>12' END";
            }

            query = query + " ORDER BY [Partner].[PartnerID], [ScoreID], [DomainID], [CSIDomainScore].[QuestionID] ";

            return dbContext.Database.SqlQuery<ChildPartnerInfo>(query).ToList();
        }

        public List<ChildQuestionReportInfo> FindChildQuestionReport(int questionID, int scoreID, Boolean sixMonths, int partnerID, int locationID, string gender)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();

            string strWhere = @"WHERE Question.QuestionID = '" + questionID + "' AND ScoreType.ScoreTypeID = '" + scoreID + "'";

            if (sixMonths)
            {
                strWhere = strWhere + @" AND CSI.IndexDate > DATEADD(month, -6, GETDATE()) ";
            }

            if (partnerID > 0)
            {
                strWhere = strWhere + " AND EXISTS (Select * FROM ChildPartner CP WHERE CP.ChildID = Child.ChildID AND CP.PartnerID = '" + partnerID + "')";
            }

            if (locationID > 0)
            {
                strWhere = strWhere + " AND Child.LocationID = '" + locationID + "'";
            }

            if (gender.Equals("M"))
                strWhere = strWhere + " AND Child.Gender = 'M' ";

            if (gender.Equals("F"))
                strWhere = strWhere + " AND Child.Gender = 'F' ";

            String query = @"SELECT LTRIM(RTRIM(FirstName)) + ' ' + LTRIM(RTRIM(LastName)) As FullName, 
			Child.ChildID AS ChildID, 
			DATEDIFF(YEAR, DateOFBirth, GETDATE()) AS Age, 
			DATENAME(d, CSI.IndexDate) + ' ' + DATENAME(m, CSI.IndexDate) + ' ' + DATENAME(YEAR, CSI.IndexDate) AS CSIDate,
            Domain.Description as Domain,
            Child.Gender
			FROM Child 
		    INNER JOIN CSI ON Child.ChildID = CSI.ChildID 
		    INNER JOIN CSIDomain ON CSI.CSIID = CSIDomain.CSIID
            INNER JOIN Domain ON Domain.DomainID = CSIDomain.DomainID
		    INNER JOIN CSIDomainScore ON CSIDomain.CSIDomainID = CSIDomainScore.CSIDomainID
		    INNER JOIN Question ON CSIDomainScore.QuestionID = Question.QuestionID
            INNER JOIN Answer ON CSIDomainScore.AnswerID = Answer.AnswerID
            INNER JOIN ScoreType ON ScoreType.ScoreTypeID = Answer.ScoreID " + strWhere;

            return dbContext.Database.SqlQuery<ChildQuestionReportInfo>(query).ToList();
        }

        public List<ChildStatusInfo> FindChildStatusReport(string childName, string gender, string age, int partnerID, int statusID)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();

            string strWhere = (childName.Length > 0) ? @"WHERE (Child.FirstName like '%" + childName + "%' or Child.LastName = '%" + childName + "%')" : @"WHERE 1=1 ";
            strWhere = gender.Equals("M") ? strWhere + " AND Child.Gender = 'M' " : gender.Equals("F") ? strWhere + " AND Child.Gender = 'F' " : strWhere;
            strWhere = (age == "") ? strWhere : strWhere + " AND DATEDIFF(YEAR, DateOFBirth, GETDATE()) = '" + age + "'";
            strWhere = (partnerID > 0) ? strWhere + " AND HouseHold.PartnerID = '" + partnerID + "'" : strWhere;
            strWhere = (statusID > 0) ? strWhere + " AND ChildStatusHistory.ChildStatusID = '" + statusID + "'" : strWhere;

            String query = @"SELECT LTRIM(RTRIM(FirstName))+' '+LTRIM(RTRIM(LastName)) As FullName, 
            Child.Gender As Gender,
			DATEDIFF(YEAR, DateOFBirth, GETDATE()) As Age,
            Partner.Name As PartnerName,
            ChildStatus.Description As ChildStatusDescription
			FROM Child
		    INNER JOIN HouseHold ON Child.HouseHoldID = HouseHold.HouseHoldID
            INNER JOIN Partner ON HouseHold.PartnerID = Partner.PartnerID
            INNER JOIN ChildStatusHistory ON Child.ChildID = ChildStatusHistory.ChildID
            INNER JOIN ChildStatus ON ChildStatusHistory.ChildStatusID = ChildStatus.StatusID " + strWhere;

            return dbContext.Database.SqlQuery<ChildStatusInfo>(query).ToList();
        }
    }

    public class ChildPartnerInfo
    {
        public int? PartnerID { get; set; }
        public int QuestionID { get; set; }
        public int ScoreID { get; set; }
        public int DomainID { get; set; }
        public int NrChild { get; set; }
        public string Gender { get; set; }
        public string AgeGroup { get; set; }
    }

    public class ChildProgressInfo
    {
        public int Count { get; set; }
    }

    public class ChildStatusInfo
    {
        public string FullName { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string PartnerName { get; set; }
        public string ChildStatusDescription { get; set; }
    }

    public class ChildQuestionReportInfo
    {
        public string FullName { get; set; }
        public int ChildID { get; set; }
        public int Age { get; set; }
        public string CSIDate { get; set; }
        public string Domain { get; set; }
        public string Gender { get; set; }
    }


}
