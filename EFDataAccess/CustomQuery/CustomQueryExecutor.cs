using EFDataAccess.CustomQueryObject;
using EFDataAccess.UOW;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPPS.CSI.Domain;

namespace EFDataAccess.CustomQuery
{
    public class CustomQueryExecutor
    {
        private readonly ApplicationDbContext context;

        public CustomQueryExecutor(UnitOfWork unitOfWork)
        {
            this.context = unitOfWork.DbContext;
        }

        public int ExecuteSqlCommand(string sql)
        {
            int noOfRowAffected = this.context.Database.ExecuteSqlCommand(sql);
            return noOfRowAffected;
        }

        public DataTable GetCSITrendReportData(int partnerId, int domainId = 0, int locationId = 0, double cutPointInitial = 0.0, double cutPointFinal = 0.0, string gender = "All")
        {
            var sqlCte = string.Format(@"

Declare @partnerID int, @cutPointInitial decimal(5,2), @cutPointFinal decimal(5,2), @maxValue int, @domainID int

SELECT 
	@maxValue = Max(Score)
FROM 	
	ScoreType S 

SET @partnerID = {0};
SET @cutPointInitial = CONVERT(DECIMAL(5,2),{1});
SET @cutPointFinal = CONVERT(DECIMAL(5,2),{2});
SET @domainID = {3};

WITH NormalisedScores (AnswerID, NormalisedScore, Score)
AS
(
	SELECT
		A.AnswerID,
		CAST(100 * (CAST(S.Score AS Decimal) / CAST(((MinMaxScores.MaxScore - MinMaxScores.MinScore) + 1) AS Decimal)) AS INT) AS NormalisedScore,
        S.Score
	FROM
		Answer A
		JOIN ScoreType S ON S.ScoreTypeID = A.ScoreID
		JOIN
		(
			SELECT
				Q.QuestionID, 
				MIN(S.Score) AS MinScore,
				MAX(S.Score) AS MaxScore 
			FROM 
				Question Q
				JOIN Answer A ON A.QuestionID = Q.QuestionID
				JOIN ScoreType S ON S.ScoreTypeID = A.ScoreID
			GROUP BY
				Q.QuestionID
		) MinMaxScores ON MinMaxScores.QuestionID = A.QuestionID
        WHERE
             A.ExcludeFromReport = 0
),

CSIScores (ChildId, DomainId, FirstCSIAverageScore, LastCSIAverageScore)
AS
(
	SELECT
		FirstCSI.ChildID,
		FirstCSI.DomainID,
		FirstCSI.AverageScore AS FirstCSIAverageScore,
		LastCSI.AverageScore AS LastCSIAverageScore
	FROM
		(
			SELECT
				C.ChildID,
				CD.DomainID,
				1 AS Ordinal,
				ROUND(AVG(CAST(NormalisedScore AS FLOAT)),2) AS AverageScore
			FROM
				CSI C
				JOIN Child CH ON CH.ChildID = C.ChildID
				JOIN ChildPartner CP ON CP.ChildID = CH.ChildID
				JOIN CSIDomain CD ON CD.CSIID = C.CSIID
				JOIN CSIDomainScore CDS ON CDS.CSIDomainID = CD.CSIDomainID
				JOIN
				(
				SELECT
					CH.ChildId,
					MIN(IndexDate) AS CSIDate
				FROM
					CSI C
					JOIN Child CH ON CH.ChildID = C.ChildID
					JOIN ChildPartner CP ON CP.ChildID = CH.ChildID
				WHERE
					(@partnerID > 0  AND PartnerID = @partnerID) OR
                    (@partnerID = 0)
				GROUP BY
					CH.ChildID
				) FirstCSI ON FirstCSI.ChildID = C.ChildID
					AND FirstCSI.CSIDate = C.IndexDate
				JOIN NormalisedScores ON NormalisedScores.AnswerID = CDS.AnswerID
			WHERE
				(@partnerID > 0  AND CP.PartnerID = @partnerID) OR
                (@partnerID = 0)
			GROUP BY
				C.ChildID,
				CD.DomainID
		) FirstCSI
	JOIN
		(
			SELECT
				C.ChildID,
				CD.DomainID,
				2 AS Ordinal,
				ROUND(AVG(CAST(NormalisedScore AS FLOAT)),2) AS AverageScore
			FROM
				CSI C
				JOIN Child CH ON CH.ChildID = C.ChildID
				JOIN ChildPartner CP ON CP.ChildID = CH.ChildID
				JOIN CSIDomain CD ON CD.CSIID = C.CSIID
				JOIN CSIDomainScore CDS ON CDS.CSIDomainID = CD.CSIDomainID
				JOIN
				(
				SELECT
					CH.ChildId,
					MAX(IndexDate) AS CSIDate
				FROM
					CSI C
					JOIN Child CH ON CH.ChildID = C.ChildID
					JOIN ChildPartner CP ON CP.ChildID = CH.ChildID
				WHERE
					(@partnerID > 0  AND PartnerID = @partnerID) OR
                    (@partnerID = 0)
				GROUP BY
					CH.ChildID
				) LastCSI ON LastCSI.ChildID = C.ChildID
					AND LastCSI.CSIDate = C.IndexDate
				JOIN NormalisedScores ON NormalisedScores.AnswerID = CDS.AnswerID
			WHERE
				(@partnerID > 0  AND CP.PartnerID = @partnerID) OR
                (@partnerID = 0)				
			GROUP BY
				C.ChildID,
				CD.DomainID
            HAVING
                (@domainID > 0 AND CD.DomainID = @domainID AND @cutPointInitial >= CONVERT(DECIMAL(5,2),0) AND ROUND(AVG(CAST(NormalisedScores.Score AS FLOAT)),2) BETWEEN  @cutPointInitial AND @cutPointFinal) OR
                (@domainID > 0 AND CD.DomainID <> @domainID) OR
                (@domainID = 0 AND @cutPointInitial >= CONVERT(DECIMAL(5,2),0) AND ROUND(AVG(CAST(NormalisedScores.Score AS FLOAT)),2) BETWEEN  @cutPointInitial AND @cutPointFinal) OR
                (@cutPointInitial < CONVERT(DECIMAL(5,2),0))
		) LastCSI ON LastCSI.ChildID = FirstCSI.ChildID 
			AND LastCSI.DomainID = FirstCSI.DomainID
			AND LastCSI.Ordinal = 2
)", partnerId, cutPointInitial.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture), cutPointFinal.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture), domainId);

            var select = "SELECT C.ChildId, C.FirstName + ' ' + C.LastName AS [ChildName], C.Gender";
            var from = "FROM Child C";

            var where = "";

            if (gender.Equals("M"))
                where = " WHERE C.Gender = 'M' ";

            if (gender.Equals("F"))
                where = " WHERE C.Gender = 'F' ";

            if (locationId > 0)
                if (where.Length > 0)
                    where = where + "LocationID = '" + locationId + "'";
                else
                    where = "LocationID = '" + locationId + "'";           


            foreach (var domain in context.Domains)
            {
                var domainNameStripped = domain.Description.Replace(" ", string.Empty);

                select += string.Format(", [{0}].LastCSIAverageScore - [{0}].FirstCSIAverageScore AS [{0}]", domainNameStripped);
                from += string.Format(" JOIN CSIScores [{0}] ON [{0}].ChildId = C.ChildID AND [{0}].DomainID = {1}", domainNameStripped, domain.DomainID);
            }

            return ExecuteSql(sqlCte + select + from + where);
        }

        private DataTable ExecuteSql(string sql)
        {
            var connection = context.Database.Connection;
            DataSet dataSet = new DataSet();
            DbDataAdapter adapter;

            DbProviderFactory factory = DbProviderFactories.GetFactory(connection);
            ConnectionState initialState = connection.State;
            try
            {
                if (initialState != ConnectionState.Open)
                    connection.Open();  // open connection if not already open
                using (DbCommand cmd = connection.CreateCommand())
                {
                    adapter = factory.CreateDataAdapter();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    cmd.CommandTimeout = 300;
                    adapter.SelectCommand = cmd;
                    adapter.Fill(dataSet);
                }
            }
            finally
            {
                if (initialState != ConnectionState.Open)
                    connection.Close(); // only close connection if not initially open
            }

            if (dataSet.Tables.Count > 0)
                return dataSet.Tables[0];
            else
                // Not sure if this is the correct thing to do. Need to confirm
                return new DataTable(); 
        }

        private DataSet ExecuteSql(DataSet dataSet, string sql, string tableName)
        {
            var connection = context.Database.Connection;
            
            DbDataAdapter adapter;

            DbProviderFactory factory = DbProviderFactories.GetFactory(connection);
            ConnectionState initialState = connection.State;
            try
            {
                if (initialState != ConnectionState.Open)
                    connection.Open();  // open connection if not already open
                using (DbCommand cmd = connection.CreateCommand())
                {
                    adapter = factory.CreateDataAdapter();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    cmd.CommandTimeout = 300;
                    adapter.SelectCommand = cmd;
                    adapter.Fill(dataSet,tableName);
                }
            }
            finally
            {
                if (initialState != ConnectionState.Open)
                    connection.Close(); // only close connection if not initially open
            }

            return dataSet;
        }

        public DataTable GetCSICurrentStatusReportData(int partnerId, int domainId = 0, int locationId = 0, double cutPointInitial = 0.0, double cutPointFinal = 0.0, string gender = "All")
        {

            var sqlCte = string.Format(@"

Declare @partnerID int, @cutPointInitial decimal(5,2), @cutPointFinal decimal(5,2), @maxValue int, @domainID int

SET @partnerID = {0};
SET @cutPointInitial = CONVERT(DECIMAL(5,2),{1});
SET @cutPointFinal = CONVERT(DECIMAL(5,2),{2});
SET @domainID = {3};

WITH ColorScores (AnswerID, Score)
            AS
            (
	            SELECT
		            A.AnswerID,
		            S.Score
	            FROM
		            Answer A
		            JOIN ScoreType S ON S.ScoreTypeID = A.ScoreID
                WHERE
                    A.ExcludeFromReport = 0		           
            ),

            CSIScores (ChildId, DomainId, LastCSIAverageColor)
            AS
            (
	            SELECT
		            LastCSI.ChildID,
		            LastCSI.DomainID,
		            S.Color AS LastCSIAverageColor
	            FROM
		            (
			            SELECT
				            C.ChildID,
				            CD.DomainID,
				            1 AS Ordinal,
				            ROUND(AVG(CAST(ColorScores.Score AS FLOAT)),2) AS AverageScore
			            FROM
				            CSI C
				            JOIN Child CH ON CH.ChildID = C.ChildID
				            JOIN ChildPartner CP ON CP.ChildID = CH.ChildID
				            JOIN CSIDomain CD ON CD.CSIID = C.CSIID
				            JOIN CSIDomainScore CDS ON CDS.CSIDomainID = CD.CSIDomainID
				            JOIN
				            (
				            SELECT
					            CH.ChildId,
					            MAX(IndexDate) AS CSIDate
				            FROM
					            CSI C
					            JOIN Child CH ON CH.ChildID = C.ChildID
					            JOIN ChildPartner CP ON CP.ChildID = CH.ChildID
				            WHERE
					            (@partnerID > 0  AND PartnerID = @partnerID) OR
                                (@partnerID = 0)
				            GROUP BY
					            CH.ChildID
				            ) LastCSI ON LastCSI.ChildID = C.ChildID
					            AND LastCSI.CSIDate = C.IndexDate
				            JOIN ColorScores ON ColorScores.AnswerID = CDS.AnswerID
			            WHERE
				            (@partnerID > 0  AND CP.PartnerID = @partnerID) OR
                            (@partnerID = 0)
			            GROUP BY
				            C.ChildID,
				            CD.DomainID
                        HAVING
                            (@domainID > 0 AND CD.DomainID = @domainID AND @cutPointInitial >= CONVERT(DECIMAL(5,2),0) AND ROUND(AVG(CAST(ColorScores.Score AS FLOAT)),2) BETWEEN @cutPointInitial AND @cutPointFinal) OR
                            (@domainID > 0 AND CD.DomainID <> @domainID) OR
                            (@domainID = 0 AND @cutPointInitial >= CONVERT(DECIMAL(5,2),0) AND ROUND(AVG(CAST(ColorScores.Score AS FLOAT)),2) BETWEEN @cutPointInitial AND @cutPointFinal) OR
                            (@cutPointInitial < CONVERT(DECIMAL(5,2),0))
		            ) LastCSI
                      JOIN CutPoint S ON (S.InitialValue <= AverageScore AND S.FinalValue >= AverageScore )

            )", partnerId, cutPointInitial.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture), cutPointFinal.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture), domainId);

                        var select = "SELECT C.ChildId, C.FirstName + ' ' + C.LastName AS [ChildName], C.Gender";
                        var from = "FROM Child C";
                        var where = "";

                        if (gender.Equals("M"))
                            where = " WHERE C.Gender = 'M' ";

                        if (gender.Equals("F"))
                            where = " WHERE C.Gender = 'F' ";

                        if (locationId > 0)
                            if (where.Length > 0)
                                where = where + "LocationID = '" + locationId + "'";
                            else
                                where = "LocationID = '" + locationId + "'";   

                        foreach (var domain in context.Domains)
                        {
                            var domainNameStripped = domain.Description.Replace(" ", string.Empty);

                            select += string.Format(", [{0}].LastCSIAverageColor AS [{0}]", domainNameStripped);
                            from += string.Format(" JOIN CSIScores [{0}] ON [{0}].ChildId = C.ChildID AND [{0}].DomainID = {1}", domainNameStripped, domain.DomainID);
                        }

           return ExecuteSql(sqlCte + select + from + where);
        }

        public DataTable GetCSITrendSummaryReportData()
        {
            var sqlCte = string.Format(@"WITH NormalisedScores (AnswerID, NormalisedScore)
AS
(
	SELECT
		A.AnswerID,
		CAST(100 * (CAST(S.Score AS Decimal) / CAST(((MinMaxScores.MaxScore - MinMaxScores.MinScore) + 1) AS Decimal)) AS INT) AS NormalisedScore
	FROM
		Answer A
		JOIN ScoreType S ON S.ScoreTypeID = A.ScoreID
		JOIN
		(
			SELECT
				Q.QuestionID, 
				MIN(S.Score) AS MinScore,
				MAX(S.Score) AS MaxScore 
			FROM 
				Question Q
				JOIN Answer A ON A.QuestionID = Q.QuestionID
				JOIN ScoreType S ON S.ScoreTypeID = A.ScoreID
			GROUP BY
				Q.QuestionID
		) MinMaxScores ON MinMaxScores.QuestionID = A.QuestionID
        WHERE
             A.ExcludeFromReport = 0
),

CSIScores (ChildId, DomainId, PartnerID, FirstCSIAverageScore, LastCSIAverageScore)
AS
(
	SELECT
		FirstCSI.ChildID,
		FirstCSI.DomainID,
        FirstCSI.PartnerID,
		FirstCSI.AverageScore AS FirstCSIAverageScore,
		LastCSI.AverageScore AS LastCSIAverageScore
	FROM
		(
			SELECT
				C.ChildID,
				CD.DomainID,
                CP.PartnerID,
				1 AS Ordinal,
				AVG(NormalisedScore) AS AverageScore
			FROM
				CSI C
				JOIN Child CH ON CH.ChildID = C.ChildID
				JOIN ChildPartner CP ON CP.ChildID = CH.ChildID
				JOIN CSIDomain CD ON CD.CSIID = C.CSIID
				JOIN CSIDomainScore CDS ON CDS.CSIDomainID = CD.CSIDomainID
				JOIN
				(
				SELECT
					CH.ChildId,
					MIN(IndexDate) AS CSIDate
				FROM
					CSI C
					JOIN Child CH ON CH.ChildID = C.ChildID
					JOIN ChildPartner CP ON CP.ChildID = CH.ChildID
				GROUP BY
					CH.ChildID
				) FirstCSI ON FirstCSI.ChildID = C.ChildID
					AND FirstCSI.CSIDate = C.IndexDate
				JOIN NormalisedScores ON NormalisedScores.AnswerID = CDS.AnswerID				
			GROUP BY
				C.ChildID,
				CD.DomainID,
                CP.PartnerID
		) FirstCSI
	JOIN
		(
			SELECT
				C.ChildID,
				CD.DomainID,
                CP.PartnerID,
				2 AS Ordinal,
				AVG(NormalisedScore) AS AverageScore
			FROM
				CSI C
				JOIN Child CH ON CH.ChildID = C.ChildID
				JOIN ChildPartner CP ON CP.ChildID = CH.ChildID
				JOIN CSIDomain CD ON CD.CSIID = C.CSIID
				JOIN CSIDomainScore CDS ON CDS.CSIDomainID = CD.CSIDomainID
				JOIN
				(
				SELECT
					CH.ChildId,
					MAX(IndexDate) AS CSIDate
				FROM
					CSI C
					JOIN Child CH ON CH.ChildID = C.ChildID
					JOIN ChildPartner CP ON CP.ChildID = CH.ChildID				
				GROUP BY
					CH.ChildID
				) LastCSI ON LastCSI.ChildID = C.ChildID
					AND LastCSI.CSIDate = C.IndexDate
				JOIN NormalisedScores ON NormalisedScores.AnswerID = CDS.AnswerID							
			GROUP BY
				C.ChildID,
				CD.DomainID,
                CP.PartnerID
		) LastCSI ON LastCSI.ChildID = FirstCSI.ChildID 
			AND LastCSI.DomainID = FirstCSI.DomainID
			AND LastCSI.Ordinal = 2
),
CSIScoresByPartner(PartnerId, DomainId,Pos, Neg, No)
AS
    (
        SELECT  
            PartnerId,
            DomainId,
            SUM(CASE WHEN LastCSIAverageScore > FirstCSIAverageScore THEN 1 ELSE 0 END) AS [Pos],
            SUM(CASE WHEN LastCSIAverageScore = FirstCSIAverageScore THEN 1 ELSE 0 END) AS [No],
            SUM(CASE WHEN LastCSIAverageScore < FirstCSIAverageScore THEN 1 ELSE 0 END) AS [Neg]
        FROM
            CSIScores
        GROUP BY
            PartnerID,
            DomainID
            
)");

            var select = "SELECT P.PartnerID, P.Name AS [PartnerName], TotalChildByPartner.totalChild AS [TotalOVC]";
            var from = "FROM Partner P";

            var totalChildPerPartner = @" JOIN (SELECT 
                                                CP.PartnerID, COUNT(*) as totalChild
                                           FROM
                                                ChildPartner CP
                                           GROUP BY
                                                CP.PartnerID) TotalChildByPartner ON TotalChildByPartner.PartnerID = P.PartnerID";
            from += totalChildPerPartner;

            String [] categories = { "POS", "NO", "NEG" };

            foreach (var domain in context.Domains)
            {
                var domainNameStripped = domain.Description.Replace(" ", string.Empty);

                foreach (var cat in categories)
                    select += string.Format(", [{0}].[{1}] AS [{2}]", domainNameStripped, cat, domainNameStripped + cat);

                from += string.Format(" JOIN CSIScoresByPartner [{0}] ON [{0}].PartnerID = P.PartnerID AND [{0}].DomainID = {1}", domainNameStripped, domain.DomainID);
            }

            return ExecuteSql(sqlCte + select + from);
        }

        public DataSet GetCSIScorePieReport()
        {
            DataSet result = new DataSet();

            var sqlCte = string.Format(@"WITH AnswerScores (AnswerID, ScoreTypeID, Score)
            AS
            (
	            SELECT
		            A.AnswerID,
                    S.ScoreTypeID,
		            S.Score
	            FROM
		            Answer A
		            JOIN ScoreType S ON S.ScoreTypeID = A.ScoreID
                WHERE
                     A.ExcludeFromReport = 0		           
            ),

            CSIScores (DomainID, Color, CutPointID, Score, Total)
            AS
            (
	            SELECT
		            FirstCSI.DomainID,
		            S.Color,
                    S.CutPointID,
                    ROUND(AVG(FirstCSI.AverageScore),2) AS Score,
                    COUNT(*) AS Total
	            FROM
		            (
			            SELECT
				            CH.ChildID,
				            CD.DomainID,
                            ROUND(AVG(CAST(AnswerScores.Score AS FLOAT)),2) AS AverageScore                            
			            FROM
				            CSI C
				            JOIN Child CH ON CH.ChildID = C.ChildID
				            JOIN ChildPartner CP ON CP.ChildID = CH.ChildID
				            JOIN CSIDomain CD ON CD.CSIID = C.CSIID
				            JOIN CSIDomainScore CDS ON CDS.CSIDomainID = CD.CSIDomainID
				            JOIN
				            (
				            SELECT
					            CH.ChildId,
					            MIN(IndexDate) AS CSIDate
				            FROM
					            CSI C
					            JOIN Child CH ON CH.ChildID = C.ChildID
					            JOIN ChildPartner CP ON CP.ChildID = CH.ChildID
				            GROUP BY
					            CH.ChildID
				            ) FirstCSI ON FirstCSI.ChildID = C.ChildID
					            AND FirstCSI.CSIDate = C.IndexDate
				            JOIN AnswerScores ON AnswerScores.AnswerID = CDS.AnswerID
			            GROUP BY
				            CH.ChildID,
				            CD.DomainID
		            ) FirstCSI 
                    INNER JOIN CutPoint S ON (S.InitialValue <= FirstCSI.AverageScore AND S.FinalValue >= FirstCSI.AverageScore)
                    GROUP BY
                        FirstCSI.DomainID,
                        S.Color,
                        S.CutPointID              

            )");

            var select = "SELECT CSIScores.* ";
            var from = "FROM CSIScores";
         
             // Add firstCSI
            ExecuteSql(result,sqlCte + select + from,"FirstCSI");

            // Process LastCSI

            sqlCte = string.Format(@"WITH AnswerScores (AnswerID, ScoreTypeID, Score)
            AS
            (
	            SELECT
		            A.AnswerID,
                    S.ScoreTypeID,
		            S.Score
	            FROM
		            Answer A
		            JOIN ScoreType S ON S.ScoreTypeID = A.ScoreID
                WHERE
                     A.ExcludeFromReport = 0		           
            ),

            CSIScores (DomainID, Color, CutPointID, Score, Total)
            AS
            (
	            SELECT
		            LastCSI.DomainID,
		            S.Color,
                    S.CutPointID,
                    ROUND(AVG(LastCSI.AverageScore),2) AS Score,
                    COUNT(*) AS Total
	            FROM
		            (
			            SELECT
				            CH.ChildID,
				            CD.DomainID,
                            ROUND(AVG(CAST(AnswerScores.Score AS FLOAT)),2) AS AverageScore 
			            FROM
				            CSI C
				            JOIN Child CH ON CH.ChildID = C.ChildID
				            JOIN ChildPartner CP ON CP.ChildID = CH.ChildID
				            JOIN CSIDomain CD ON CD.CSIID = C.CSIID
				            JOIN CSIDomainScore CDS ON CDS.CSIDomainID = CD.CSIDomainID
				            JOIN
				            (
				            SELECT
					            CH.ChildId,
					            MAX(IndexDate) AS CSIDate
				            FROM
					            CSI C
					            JOIN Child CH ON CH.ChildID = C.ChildID
					            JOIN ChildPartner CP ON CP.ChildID = CH.ChildID
				            GROUP BY
					            CH.ChildID
				            ) LastCSI ON LastCSI.ChildID = C.ChildID
					            AND LastCSI.CSIDate = C.IndexDate
				            JOIN AnswerScores ON AnswerScores.AnswerID = CDS.AnswerID
			            GROUP BY
				            CH.ChildID,
				            CD.DomainID
		            ) LastCSI                 
                    INNER JOIN CutPoint S ON (S.InitialValue <= LastCSI.AverageScore AND S.FinalValue >= LastCSI.AverageScore)
                    GROUP BY
                        LastCSI.DomainID,
                        S.Color,
                        S.CutPointID 
            )");

            select = "SELECT CSIScores.* ";
            from = "FROM CSIScores";

            // Add LastCSI
            ExecuteSql(result,sqlCte + select + from,"LastCSI");

            return result;
        }

        public DataTable GetCSICurrentAndFirstStatusAggregateData()
        {

            var sqlCte = string.Format(@"WITH AnswerScores (AnswerID, Score)
            AS
            (
	            SELECT
		            A.AnswerID,
		            S.Score
	            FROM
		            Answer A
		            JOIN ScoreType S ON S.ScoreTypeID = A.ScoreID	
                WHERE
                     A.ExcludeFromReport = 0	           
            ),

            CSIScores ( DomainId, AverageScore, CSI)
            AS
            (
	            SELECT		           
		            LastCSI.DomainID,
		            LastCSI.AverageScore,
                    'LastCSI' as CSI
	            FROM
		            (
			            SELECT				            
				            CD.DomainID,
				            1 AS Ordinal,
				            CAST(ROUND(AVG(CAST(AnswerScores.Score as FLOAT)),2) as NUMERIC(36,2)) AS AverageScore
			            FROM
				            CSI C
				            JOIN Child CH ON CH.ChildID = C.ChildID
				            JOIN ChildPartner CP ON CP.ChildID = CH.ChildID
				            JOIN CSIDomain CD ON CD.CSIID = C.CSIID
				            JOIN CSIDomainScore CDS ON CDS.CSIDomainID = CD.CSIDomainID
				            JOIN
				            (
				            SELECT
					            CH.ChildId,
					            MAX(IndexDate) AS CSIDate
				            FROM
					            CSI C
					            JOIN Child CH ON CH.ChildID = C.ChildID
					            JOIN ChildPartner CP ON CP.ChildID = CH.ChildID
				            GROUP BY
					            CH.ChildID
				            ) LastCSI ON LastCSI.ChildID = C.ChildID
					            AND LastCSI.CSIDate = C.IndexDate
				            JOIN AnswerScores ON AnswerScores.AnswerID = CDS.AnswerID
			            GROUP BY				            
				            CD.DomainID
		            ) LastCSI  
                 UNION ALL
                SELECT
		            FirstCSI.DomainID,
		            FirstCSI.AverageScore,
                    'FirstCSI' as CSI
	            FROM
		            (
			            SELECT
				            CD.DomainID,
				            1 AS Ordinal,
				           CAST(ROUND(AVG(CAST(AnswerScores.Score as FLOAT)),2) as NUMERIC(36,2)) AS AverageScore
			            FROM
				            CSI C
				            JOIN Child CH ON CH.ChildID = C.ChildID
				            JOIN ChildPartner CP ON CP.ChildID = CH.ChildID
				            JOIN CSIDomain CD ON CD.CSIID = C.CSIID
				            JOIN CSIDomainScore CDS ON CDS.CSIDomainID = CD.CSIDomainID
				            JOIN
				            (
				            SELECT
					            CH.ChildId,
					            MIN(IndexDate) AS CSIDate
				            FROM
					            CSI C
					            JOIN Child CH ON CH.ChildID = C.ChildID
					            JOIN ChildPartner CP ON CP.ChildID = CH.ChildID
				            GROUP BY
					            CH.ChildID
				            ) FirstCSI ON FirstCSI.ChildID = C.ChildID
					            AND FirstCSI.CSIDate = C.IndexDate
				            JOIN AnswerScores ON AnswerScores.AnswerID = CDS.AnswerID
			            GROUP BY
				            CD.DomainID
		            ) FirstCSI                     

            )");

            var select = "SELECT q.CSI";
            var from = "from (select 'FirstCSI' as CSI union all select 'LastCSI' as CSI) q";
            var orderby = " order by q.CSI";

            foreach (var domain in context.Domains)
            {
                var domainNameStripped = domain.Description.Replace(" ", string.Empty);

                select += string.Format(", [{0}].AverageScore AS [{0}]", domainNameStripped);
                from += string.Format(" LEFT JOIN CSIScores [{0}] ON [{0}].DomainID = {1} AND [{0}].CSI =q.CSI ", domainNameStripped, domain.DomainID);
            }

            return ExecuteSql(sqlCte + select + from + orderby);
        }

        public DataTable GetCSIFirstStatusAggregateData()
        {

            var sqlCte = string.Format(@"WITH AnswerScores (AnswerID, Score)
            AS
            (
	            SELECT
		            A.AnswerID,
		            S.Score
	            FROM
		            Answer A
		            JOIN ScoreType S ON S.ScoreTypeID = A.ScoreID
                WHERE
                     A.ExcludeFromReport = 0		           
            ),

            CSIScores ( DomainId, AverageScore)
            AS
            (
	            SELECT
		            FirstCSI.DomainID,
		            FirstCSI.AverageScore
	            FROM
		            (
			            SELECT
				            CD.DomainID,
				            1 AS Ordinal,
				           AVG(AnswerScores.Score) AS AverageScore
			            FROM
				            CSI C
				            JOIN Child CH ON CH.ChildID = C.ChildID
				            JOIN ChildPartner CP ON CP.ChildID = CH.ChildID
				            JOIN CSIDomain CD ON CD.CSIID = C.CSIID
				            JOIN CSIDomainScore CDS ON CDS.CSIDomainID = CD.CSIDomainID
				            JOIN
				            (
				            SELECT
					            CH.ChildId,
					            MIN(IndexDate) AS CSIDate
				            FROM
					            CSI C
					            JOIN Child CH ON CH.ChildID = C.ChildID
					            JOIN ChildPartner CP ON CP.ChildID = CH.ChildID
				            GROUP BY
					            CH.ChildID
				            ) FirstCSI ON FirstCSI.ChildID = C.ChildID
					            AND FirstCSI.CSIDate = C.IndexDate
				            JOIN AnswerScores ON AnswerScores.AnswerID = CDS.AnswerID
			            GROUP BY
				            CD.DomainID
		            ) FirstCSI                     

            )");

            var select = "SELECT 1";
            var from = "";

            foreach (var domain in context.Domains)
            {
                var domainNameStripped = domain.Description.Replace(" ", string.Empty);

                select += string.Format(", [{0}].AverageScore AS [{0}]", domainNameStripped);
                from += string.Format(" JOIN CSIScores [{0}] ON [{0}].ChildId = C.ChildID AND [{0}].DomainID = {1}", domainNameStripped, domain.DomainID);
            }

            return ExecuteSql(sqlCte + select + from);
        }

        public DataTable GetAllCSITrendReport(DateTime fromDate, DateTime toDate)
        {

            var sqlCte = string.Format(@"WITH AnswerScores (AnswerID, Score)
            AS
            (
	            SELECT
		            A.AnswerID,
		            S.Score
	            FROM
		            Answer A
		            JOIN ScoreType S ON S.ScoreTypeID = A.ScoreID
                WHERE
                     A.ExcludeFromReport = 0		           
            ),

            CSIPhases (childID, indexDate, CSINumber)
            AS
            (
	           SELECT
		        c.childID,
		        C.indexDate,
		        ROW_NUMBER() OVER(PARTITION BY C.ChildID ORDER BY C.indexDate) AS CSINumber
		        FROM
		        CSI C
		        JOIN Child CH ON CH.ChildID = C.ChildID
		        WHERE
		        C.indexDate>= DATEADD(day,DATEDIFF(day,0,'{0}'),0) and C.indexDate <= DATEADD(day,DATEDIFF(day,0,'{1}'),0)		           
            ),

            CSIScores ( DomainId, AverageScore, CSINumber)
            AS
            (
	            SELECT		           
		            LastCSI.DomainID,
		            LastCSI.AverageScore,
                    LastCSI.CSINumber
	            FROM
		            (
			            SELECT				            
				            CD.DomainID,
                            CSIPhases.CSINumber,				            
                            CAST(ROUND(AVG(CAST(AnswerScores.Score as FLOAT)),2) as NUMERIC(36,2)) as AverageScore
			            FROM
				            CSI C
				            JOIN Child CH ON CH.ChildID = C.ChildID
				            JOIN ChildPartner CP ON CP.ChildID = CH.ChildID
				            JOIN CSIDomain CD ON CD.CSIID = C.CSIID
				            JOIN CSIDomainScore CDS ON CDS.CSIDomainID = CD.CSIDomainID				            
				            JOIN AnswerScores ON AnswerScores.AnswerID = CDS.AnswerID
                            JOIN CSIPhases ON CSIPhases.ChildID = C.ChildID AND CSIPhases.IndexDate = C.IndexDate
                        GROUP BY
                             CD.DomainID,
                             CSIPhases.CSINumber
			            
		            ) LastCSI                     

            )", fromDate.ToString("d", DateTimeFormatInfo.InvariantInfo), toDate.ToString("d", DateTimeFormatInfo.InvariantInfo));

            var select = "SELECT ('CSI ' + Cast (CSIGroup.CSINumber as varchar(10))) AS CSIPhase";
            var from = string.Format(@" FROM (SELECT DISTINCT CSINumber from CSIPhases                              
                                        ) CSIGroup");
            var orderBy = " ORDER BY CSIGroup.CSINumber";

            foreach (var domain in context.Domains)
            {
                var domainNameStripped = domain.Description.Replace(" ", string.Empty);

                select += string.Format(", (ISNULL([{0}].AverageScore,0)) AS [{0}]", domainNameStripped);
                from += string.Format(" LEFT JOIN CSIScores [{0}] ON [{0}].DomainID = {1} AND [{0}].CSINumber = CSIGroup.CSINumber ", domainNameStripped, domain.DomainID);
            }

            return ExecuteSql(sqlCte + select + from + orderBy );
        }

        public DataTable GetUserActivityReport(int UserID, DateTime fromDate, DateTime toDate)
        {
            var select = string.Format(@"SELECT 
	                            U.FirstName + ' ' + U.LastName AS [USERNAME],
	                            DATEDIFF(DAY, U.LastLoginDate, '{1}') AS [DAYSFROMLASTLOGIN],
	                            DATEDIFF(DAY, LASTCHILD.LASTDATECAPTURED, '{1}') AS [DAYSSINCELASTCAPTUREDCHILD],
	                            DATEDIFF(DAY, LASTCSI.LASTDATECAPTURED, '{1}') AS [DAYSSINCELASTCAPTUREDCSI],
	                            DATEDIFF(DAY, LASTCAREPLAN.LASTDATECAPTURED, '{1}') AS [DAYSSINCELASTCAPTUREDCAREPLAN]
                            FROM
	                            [User] U
	                            LEFT JOIN
	                            (
		                            SELECT 
			                            C.CreatedUserID,
			                            MAX(C.CreatedDate) AS [LASTDATECAPTURED]
		                            FROM
			                            Child C
		                            GROUP BY C.CreatedUserID
	                            ) LASTCHILD ON LASTCHILD.CreatedUserID = U.UserID
	                            LEFT JOIN
	                            (
		                            SELECT 
			                            CSI.CreatedUserID,
			                            MAX(CSI.CreatedDate) AS [LASTDATECAPTURED]
		                            FROM
			                            CSI 
		                            GROUP BY CSI.CreatedUserID
	                            ) LASTCSI ON LASTCSI.CreatedUserID = U.UserID
	                            LEFT JOIN
	                            (
		                            SELECT 
			                            CP.CreatedUserID,
			                            MAX(CP.CreatedDate) AS [LASTDATECAPTURED]
		                            FROM
			                            CarePlan CP 
		                            GROUP BY CP.CreatedUserID
	                            ) LASTCAREPLAN ON LASTCAREPLAN.CreatedUserID = U.UserID
                            WHERE
                                U.LastLoginDate >= DATEADD(day,DATEDIFF(day,0,'{0}'),0)
                                AND U.LastLoginDate <= DATEADD(day,DATEDIFF(day,0,'{1}'),0)", fromDate.ToString("d", DateTimeFormatInfo.InvariantInfo), toDate.ToString("d", DateTimeFormatInfo.InvariantInfo));

            var conditionWhere = string.Format(" AND U.UserID = {0}", UserID);

            if (UserID > 0)
                return ExecuteSql(select + conditionWhere);
            else
                return ExecuteSql(select);
        }

        public DataTable GetChildForGraduation()
        {
            var sqlCte = string.Format(@"WITH ChildCSISCore(ChildID, DomainID, QuestionID, AnswerID, Score)
                                        AS
                                        (
	                                        SELECT 
		                                        CH.ChildID,
		                                        CD.DomainID,
		                                        CDS.QuestionID,
		                                        CDS.AnswerID,
		                                        ST.Score
	                                        FROM
		                                        CSI C
		                                        JOIN Child CH ON CH.ChildID = C.ChildID
		                                        JOIN CSIDomain CD ON CD.CSIID = C.CSIID
		                                        JOIN CSIDomainScore CDS ON CDS.CSIDomainID = CD.CSIDomainID
		                                        JOIN
		                                        (
		                                        SELECT
			                                        CH.ChildId,
			                                        MAX(IndexDate) AS CSIDate
		                                        FROM
			                                        CSI C
			                                        JOIN Child CH ON CH.ChildID = C.ChildID
		                                        GROUP BY
			                                        CH.ChildID
		                                        ) LastCSI ON LastCSI.ChildID = C.ChildID
			                                        AND LastCSI.CSIDate = C.IndexDate
		                                        JOIN Answer ON Answer.AnswerID = CDS.AnswerID
		                                        JOIN ScoreType ST ON ST.ScoreTypeID = Answer.ScoreID
                                        ),

                                        ChildCSISCoreAverage(ChildID, DomainID, AverageScore)
                                        AS
                                        (
	                                        SELECT
		                                        CCS.ChildID,
		                                        CCS.DomainID,
		                                        CAST(ROUND(AVG(CAST(CCS.Score as FLOAT)),2) as NUMERIC(6,2)) AS AverageScore
	                                        FROM
		                                        ChildCSISCore CCS
		                                        JOIN Answer A ON A.AnswerID = CCS.AnswerID AND A.ExcludeFromReport = 0
		                                        LEFT JOIN QuestionGraduationCriterias EXCQ ON EXCQ.QuestionID = CCS.QuestionID
	                                        WHERE
		                                        EXCQ.QuestionID IS NULL
	                                        GROUP BY
		                                        CCS.ChildID,
		                                        CCS.DomainID	
                                        ),

                                        ChildQuestionLowScore(ChildID)
                                        AS
                                        (
	                                        SELECT
		                                        DISTINCT ChildLowScore.ChildID
	                                        FROM
	                                        (
		                                        SELECT 
			                                        CCS.ChildID
		                                        FROM 
			                                        ChildCSISCore CCS
			                                        JOIN QuestionCriteria QC on QC.QuestionID = CCS.QuestionID
		                                        WHERE
			                                        CCS.Score < QC.Score
		                                        UNION
		                                        SELECT
			                                        CCS.ChildID
		                                        FROM
			                                        ChildCSISCore CCS
			                                        JOIN Answer A ON A.AnswerID = CCS.AnswerID AND A.ExcludeFromReport = 0
		                                        WHERE
			                                        CCS.Score < (SELECT [MinScoreQuestion] FROM [GraduationCriteria])
	                                        ) ChildLowScore
                                        )");

            var select = "SELECT C.ChildId, C.FirstName + ' ' + C.LastName AS [ChildName]";
            var from = " FROM Child C";
            var where = " WHERE NOT EXISTS (SELECT NULL FROM ChildQuestionLowScore LS WHERE C.ChildID = LS.ChildID)";
            where += @" AND
							(SELECT TOP 1 [ChildStatusHistory].[ChildStatusID] 
								FROM [ChildStatusHistory] WHERE [ChildStatusHistory].ChildID = C.ChildID  AND CAST([EffectiveDate] as DATE) <= CAST(getdate() as DATE)  ORDER BY [EffectiveDate] DESC, [ChildStatusHistoryID] DESC) = 1";

            foreach (var domain in context.Domains)
            {
                var domainNameStripped = "Domain" + domain.DomainID.ToString();

                select += string.Format(", [{0}].AverageScore AS [{0}]", domainNameStripped);
                from += string.Format(" LEFT JOIN ChildCSISCoreAverage [{0}] ON [{0}].ChildId = C.ChildID AND [{0}].DomainID = {1}", domainNameStripped, domain.DomainID);
            }

            return ExecuteSql(sqlCte + select + from + where);
        }

        public DataTable GetOrgUnit()
        {
            var sql = string.Format(@"WITH OURCTE (Name, [OrgUnitID],[OrgUnitTypeID], [ParentOrgUnitId], EMPLEVEL)
                    AS (
                    SELECT 
                        Name, [OrgUnitID],[OrgUnitTypeID], [ParentOrgUnitId], 1 EMPLEVEL 
                    FROM 
                        OrgUnit
                    WHERE 
                        [ParentOrgUnitId] IS NULL
                    UNION ALL
                    SELECT 
                        E.Name, E.[OrgUnitID], E.OrgUnitTypeID, E.[ParentOrgUnitId], CTE.EMPLEVEL + 1
                    FROM 
                        OrgUnit E
                    INNER JOIN 
                        OURCTE CTE ON E.[ParentOrgUnitId] = CTE.[OrgUnitID]
                    WHERE 
                        E.[ParentOrgUnitId] IS NOT NULL)

                    SELECT 
                        OURCTE.*, Parent.Name ParentName, OU.[Description] UnitTypeName
                    FROM 
                        OURCTE
                    INNER JOIN 
                        OrgUnitType OU ON OU.OrgUnitTypeID = OURCTE.OrgUnitTypeID
                    LEFT JOIN 
                        OrgUnit Parent ON Parent.OrgUnitID = OURCTE.ParentOrgUnitId
                    ORDER BY EMPLEVEL");
            return ExecuteSql(sql);
        }

        public DataTable GetCareplanActions(int careplanID)
        {
            var sql = string.Format(@"WITH CSIANSWER_CTE (QuestionID, Code, Score)
                    AS
                    (
	                    SELECT 
		                    Q.QuestionID,
                            Q.Code,
		                    ST.Score
	                    FROM
		                    CarePlan CP
	                    INNER JOIN 
		                    CSI C ON C.CSIID = CP.CSIID
	                    INNER JOIN
		                    CSIDomain CD ON CD.CSIID = C.CSIID
	                    INNER JOIN
		                    CSIDomainScore CDS ON CDS.CSIDomainID = CD.CSIDomainID
	                    INNER JOIN 
		                    Answer A ON A.AnswerID = CDS.AnswerID
	                    INNER JOIN
		                    ScoreType ST ON ST.ScoreTypeID = A.ScoreID
	                    INNER JOIN
		                    Question Q ON Q.QuestionID = CDS.QuestionID
	                    WHERE
		                    CP.CarePlanID = {0}
                    )

                    SELECT 
	                    d.DomainID,
	                    d.[Description] SUPPORT_DOMAIN,
	                    cpss.CarePlanDomainSupportServiceID as ServiceID,
	                    cpss.[Description] AS SUPPORT_SERVICE,
	                    CTE.Code AS QUESTIONCODE,
	                    CTE.Score AS SCORE,
	                    t.[Description] AS SUPPORT_ACTION,
	                    r.[Description] AS SUPPORT_RESOURCE,
	                    r.isOrganization,
	                    t.ServiceProviderID AS PROVIDER,
	                    CASE WHEN r.IsOrganization = 1 THEN
		                    CASE WHEN t.ServiceProviderID IS null THEN
			                    'APOIO DIRECTO'
		                    ELSE
			                    sp.[Description]
		                    END
	                    ELSE
		                    NULL 
	                    END AS REFERENCIA
                    FROM 
	                    CarePlan c
                    INNER JOIN 
	                    CarePlanDomain cpd on c.CarePlanID = cpd.CarePlanID
                    INNER JOIN 
	                    Domain d on d.DomainID = cpd.DomainID
                    INNER JOIN 
	                    CarePlanDomainSupportService cpss on cpss.CarePlanDomainID = cpd.CarePlanDomainID
                    LEFT JOIN
	                    CSIANSWER_CTE CTE ON CTE.QuestionID = cpss.QuestionID
                    LEFT JOIN
	                    Tasks t on t.CarePlanDomainSupportServiceID = cpss.CarePlanDomainSupportServiceID
                    LEFT JOIN 
	                    Resources r on r.ResourceID = t.ResourceID
                    LEFT JOIN
	                    ServiceProvider sp on sp.ServiceProviderID = t.ServiceProviderID
                    WHERE
	                    c.CarePlanID = {0}
                    ORDER BY
	                    d.[Order]", careplanID);

            return ExecuteSql(sql);
        }

        public DataTable GetCSIsTotal(DateTime fromDate1, DateTime toDate1, DateTime fromDate2, DateTime toDate2)
        {
            var sqlCte = string.Format(@"

Declare @fromDate1 DATETIME, @toDate1 DATETIME, @fromDate2 DATETIME, @toDate2 DATETIME

SET @fromDate1 =CONVERT(datetime, '{0}');
SET @toDate1 = CONVERT(datetime,'{1}');
SET @fromDate2 = CONVERT(datetime,'{2}');
SET @toDate2 =CONVERT(datetime, '{3}');

WITH FirstLastCSITotal(FirstCSITotal, LastCSITotal)
AS
(
    SELECT
         COALESCE(FirstCSI.[total], 0) AS FirstCSITotal,
         COALESCE(LastCSI.[total],0) AS LastCSITotal 
    FROM
        (
            SELECT 
                COUNT(*) AS [total]
            FROM
                CSI C
            JOIN
				(
					SELECT
						C1.ChildID,
						MAX(C1.IndexDate) AS CSIDate
					FROM 
						CSI C1
					WHERE
						C1.IndexDate >= @fromDate1 and C1.IndexDate <= @toDate1
					GROUP BY
						C1.ChildID
				) LateCSI ON LateCSI.ChildID = C.ChildID AND LateCSI.CSIDate = C.IndexDate
            WHERE
                EXISTS ( SELECT * FROM CSIDomain cd WHERE cd.CSIID = c.CSIID) AND
                C.IndexDate >= @fromDate1 and C.IndexDate <= @toDate1
        ) FirstCSI
    ,
        (
            SELECT 
                COUNT(*) AS [total]
            FROM
                CSI C
                JOIN
				(
					SELECT
						C1.ChildID,
						MAX(C1.IndexDate) AS CSIDate
					FROM 
						CSI C1
					WHERE
						C1.IndexDate >= @fromDate2 and C1.IndexDate <= @toDate2
					GROUP BY
						C1.ChildID
				) LateCSI ON LateCSI.ChildID = C.ChildID AND LateCSI.CSIDate = C.IndexDate
            WHERE
                EXISTS ( SELECT * FROM CSIDomain cd WHERE cd.CSIID = c.CSIID) AND
                C.IndexDate >= @fromDate2 and C.IndexDate <= @toDate2
        ) LastCSI
),

RangeCSI(TotalChildren)
AS
(
    SELECT
         COALESCE(COUNT(*),0) AS TotalChildren
    FROM
        (
            SELECT 
                C.ChildID,
                C.CSIID
            FROM
                CSI C
            JOIN
				(
					SELECT
						C1.ChildID,
						MAX(C1.IndexDate) AS CSIDate
					FROM 
						CSI C1
					WHERE
						C1.IndexDate >= @fromDate1 and C1.IndexDate <= @toDate1
					GROUP BY
						C1.ChildID
				) LateCSI ON LateCSI.ChildID = C.ChildID AND LateCSI.CSIDate = C.IndexDate
            WHERE
                EXISTS ( SELECT * FROM CSIDomain cd WHERE cd.CSIID = c.CSIID) AND
                C.IndexDate >= @fromDate1 and C.IndexDate <= @toDate1
        ) FirstCSI
    JOIN
        (
            SELECT 
                C.ChildID,
                C.CSIID
            FROM
                CSI C
                JOIN
				(
					SELECT
						C1.ChildID,
						MAX(C1.IndexDate) AS CSIDate
					FROM 
						CSI C1
					WHERE
						C1.IndexDate >= @fromDate2 and C1.IndexDate <= @toDate2
					GROUP BY
						C1.ChildID
				) LateCSI ON LateCSI.ChildID = C.ChildID AND LateCSI.CSIDate = C.IndexDate
            WHERE
                EXISTS ( SELECT * FROM CSIDomain cd WHERE cd.CSIID = c.CSIID) AND
                C.IndexDate >= @fromDate2 and C.IndexDate <= @toDate2
        ) LastCSI ON LastCSI.ChildID = FirstCSI.ChildID
)", fromDate1.ToString("d", DateTimeFormatInfo.InvariantInfo), toDate1.ToString("d", DateTimeFormatInfo.InvariantInfo), fromDate2.ToString("d", DateTimeFormatInfo.InvariantInfo), toDate2.ToString("d", DateTimeFormatInfo.InvariantInfo));

            var select = "SELECT FLC.FirstCSITotal, FLC.LastCSITotal, RC.TotalChildren ";
            var from = "FROM FirstLastCSITotal FLC, RangeCSI RC ";

            return ExecuteSql(sqlCte + select + from);
        }

        public DataTable GetIndividualCSIProgressReport(DateTime fromDate1, DateTime toDate1, DateTime fromDate2, DateTime toDate2)
        {
            var sqlCte = string.Format(@"

Declare @fromDate1 DATETIME, @toDate1 DATETIME, @fromDate2 DATETIME, @toDate2 DATETIME

SET @fromDate1 =CONVERT(datetime, '{0}');
SET @toDate1 = CONVERT(datetime,'{1}');
SET @fromDate2 = CONVERT(datetime,'{2}');
SET @toDate2 =CONVERT(datetime, '{3}');

WITH AllowedScores (AnswerID, Score)
AS
(
	SELECT
		A.AnswerID,
        S.Score
	FROM
		Answer A
		JOIN ScoreType S ON S.ScoreTypeID = A.ScoreID		
        WHERE
             A.ExcludeFromReport = 0
    UNION
    SELECT 0 AS AnswerID, 0 AS Score
),

CSIScores (ChildId, DomainId, FirstCSIAverageScore, LastCSIAverageScore, ChangeScore)
AS
(
	SELECT
		FirstCSI.ChildID,
		FirstCSI.DomainID,
		FirstCSI.AverageScore AS FirstCSIAverageScore,
		LastCSI.AverageScore AS LastCSIAverageScore,
        CASE WHEN FirstCSI.AverageScore >0 THEN CAST(((LastCSI.AverageScore - FirstCSI.AverageScore)/ FirstCSI.AverageScore) * 100 AS INT) ELSE 0 END AS ChangeScore
	FROM
		(
			SELECT
				C.ChildID,
				CD.DomainID,
				1 AS Ordinal,
				ROUND(AVG(CAST(AllowedScores.Score AS FLOAT)),2) AS AverageScore
			FROM
				CSI C
				JOIN Child CH ON CH.ChildID = C.ChildID
				JOIN CSIDomain CD ON CD.CSIID = C.CSIID
				JOIN CSIDomainScore CDS ON CDS.CSIDomainID = CD.CSIDomainID
				JOIN AllowedScores ON AllowedScores.AnswerID = COALESCE(CDS.AnswerID,0) 
				JOIN
				(
					SELECT
						C1.ChildID,
						MAX(C1.IndexDate) AS CSIDate
					FROM 
						CSI C1
					WHERE
						C1.IndexDate >= @fromDate1 and C1.IndexDate <= @toDate1
					GROUP BY
						C1.ChildID
				) LateCSI ON LateCSI.ChildID = C.ChildID AND LateCSI.CSIDate = C.IndexDate
			WHERE
				C.IndexDate >= @fromDate1 and C.IndexDate <= @toDate1
			GROUP BY
				C.ChildID,
				CD.DomainID
		) FirstCSI
	JOIN
		(
			SELECT
				C.ChildID,
				CD.DomainID,
				2 AS Ordinal,
				ROUND(AVG(CAST(AllowedScores.Score AS FLOAT)),2) AS AverageScore
			FROM
				CSI C
				JOIN Child CH ON CH.ChildID = C.ChildID
				JOIN CSIDomain CD ON CD.CSIID = C.CSIID
				JOIN CSIDomainScore CDS ON CDS.CSIDomainID = CD.CSIDomainID
				JOIN AllowedScores ON AllowedScores.AnswerID = COALESCE(CDS.AnswerID,0) 
				JOIN
				(
					SELECT
						C1.ChildID,
						MAX(C1.IndexDate) AS CSIDate
					FROM 
						CSI C1
					WHERE
						C1.IndexDate >= @fromDate2 and C1.IndexDate <= @toDate2
					GROUP BY
						C1.ChildID
				) LateCSI ON LateCSI.ChildID = C.ChildID AND LateCSI.CSIDate = C.IndexDate
			WHERE
				C.IndexDate >= @fromDate2 and C.IndexDate <= @toDate2				
			GROUP BY
				C.ChildID,
				CD.DomainID
		) LastCSI ON LastCSI.ChildID = FirstCSI.ChildID 
			AND LastCSI.DomainID = FirstCSI.DomainID
)", fromDate1.ToString("d", DateTimeFormatInfo.InvariantInfo), toDate1.ToString("d", DateTimeFormatInfo.InvariantInfo), fromDate2.ToString("d",DateTimeFormatInfo.InvariantInfo), toDate2.ToString("d",DateTimeFormatInfo.InvariantInfo));

            var select = "SELECT C.ChildId, C.FirstName + ' ' + C.LastName AS [ChildName], C.Gender";
            var from = "FROM Child C";

            var where = "";

            foreach (var domain in context.Domains)
            {
                var domainNameStripped = domain.Description.Replace(" ", string.Empty);

                select += string.Format(", CAST([{0}].FirstCSIAverageScore AS VARCHAR) AS [{1}], CAST([{0}].LastCSIAverageScore AS VARCHAR) AS [{2}], CAST([{0}].ChangeScore AS VARCHAR) AS [{3}]", domainNameStripped, domainNameStripped + "First", domainNameStripped + "Last", domainNameStripped + "Change");
                from += string.Format(" JOIN CSIScores [{0}] ON [{0}].ChildId = C.ChildID AND [{0}].DomainID = {1}", domainNameStripped, domain.DomainID);
            }

            return ExecuteSql(sqlCte + select + from + where);
        }

        public DataTable GetGlobalCSIProgressReport(DateTime fromDate1, DateTime toDate1, DateTime fromDate2, DateTime toDate2)
        {
            var sqlCte = string.Format(@"

Declare @fromDate1 DATETIME, @toDate1 DATETIME, @fromDate2 DATETIME, @toDate2 DATETIME

SET @fromDate1 =CONVERT(datetime, '{0}');
SET @toDate1 = CONVERT(datetime,'{1}');
SET @fromDate2 = CONVERT(datetime,'{2}');
SET @toDate2 =CONVERT(datetime, '{3}');

WITH AllowedScores (AnswerID, Score)
AS
(
	SELECT
		A.AnswerID,
        S.Score
	FROM
		Answer A
		JOIN ScoreType S ON S.ScoreTypeID = A.ScoreID		
        WHERE
             A.ExcludeFromReport = 0
    UNION
    SELECT 0 AS AnswerID, 0 AS Score
),

RangeCSI(ChildID, CSIID1, CSIID2)
AS
(
    SELECT
         FirstCSI.ChildID,
         FirstCSI.CSIID,
         LastCSI.CSIID
    FROM
        (
            SELECT 
                C.ChildID,
                C.CSIID
            FROM
                CSI C
            JOIN
				(
					SELECT
						C1.ChildID,
						MAX(C1.IndexDate) AS CSIDate
					FROM 
						CSI C1
					WHERE
						C1.IndexDate >= @fromDate1 and C1.IndexDate <= @toDate1
					GROUP BY
						C1.ChildID
				) LateCSI ON LateCSI.ChildID = C.ChildID AND LateCSI.CSIDate = C.IndexDate
            WHERE
                C.IndexDate >= @fromDate1 and C.IndexDate <= @toDate1
        ) FirstCSI
    JOIN
        (
            SELECT 
                C.ChildID,
                C.CSIID
            FROM
                CSI C
                JOIN
				(
					SELECT
						C1.ChildID,
						MAX(C1.IndexDate) AS CSIDate
					FROM 
						CSI C1
					WHERE
						C1.IndexDate >= @fromDate2 and C1.IndexDate <= @toDate2
					GROUP BY
						C1.ChildID
				) LateCSI ON LateCSI.ChildID = C.ChildID AND LateCSI.CSIDate = C.IndexDate
            WHERE
                C.IndexDate >= @fromDate2 and C.IndexDate <= @toDate2
        ) LastCSI ON LastCSI.ChildID = FirstCSI.ChildID
),

CSIScores ([DomainID], CutPointID, [Description], FirstCSIPercentualScore, LastCSIPercentualScore, ChangePercentualScore)
AS
(
	SELECT
        COALESCE(FirstCSI.DomainID, LastCSI.DomainID) AS [DomainID],
		COALESCE(FirstCSI.CutPointID, LastCSI.CutPointID) AS CutPointID,
		COALESCE(FirstCSI.[Description], LastCSI.[Description]) AS [Description],
		CAST(COALESCE(FirstCSI.PercentualScore,0) AS NUMERIC(6,2)) AS FirstCSIPercentualScore,
		CAST(COALESCE(LastCSI.PercentualScore,0) AS NUMERIC(6,2)) AS LastCSIPercentualScore,
        CAST(COALESCE(LastCSI.PercentualScore,0) - COALESCE(FirstCSI.PercentualScore,0) AS NUMERIC(6,2)) AS ChangePercentualScore
	FROM
		(
            SELECT
                DomainID,
                S.CutPointID, 
				S.[Description],
                (CAST(COUNT(ChildID) AS DECIMAL) /  (SELECT COUNT(*) FROM RangeCSI) * 100)  AS PercentualScore
            FROM
                (

			        SELECT
                        CD.DomainID,
                        C.ChildID,
				        1 AS Ordinal,
                        ROUND(AVG(CAST(AllowedScores.Score AS FLOAT)),2) AS AverageScore
			        FROM
				        CSI C
				        JOIN Child CH ON CH.ChildID = C.ChildID
				        JOIN CSIDomain CD ON CD.CSIID = C.CSIID
				        JOIN CSIDomainScore CDS ON CDS.CSIDomainID = CD.CSIDomainID
				        JOIN AllowedScores ON AllowedScores.AnswerID = COALESCE(CDS.AnswerID,0)                
			        WHERE
				        EXISTS (SELECT * FROM RangeCSI RC WHERE RC.CSIID1 = C.CSIID)
			        GROUP BY
				       CD.DomainID,
				       C.ChildID
                ) InnerCSI
            JOIN
                CutPoint S ON (S.InitialValue <= AverageScore AND S.FinalValue >= AverageScore)
            GROUP BY
				S.CutPointID,
				S.[Description],
				InnerCSI.DomainID
		) FirstCSI
	FULL OUTER JOIN
		(
            SELECT
                DomainID,
                S.CutPointID, 
				S.[Description],
                (CAST(COUNT(ChildID) AS DECIMAL) /  (SELECT COUNT(*) FROM RangeCSI) * 100)  AS PercentualScore
            FROM
                (
			        SELECT
				        CD.DomainID,
                        C.ChildID,
				        1 AS Ordinal,
                        ROUND(AVG(CAST(AllowedScores.Score AS FLOAT)),2) AS AverageScore
			        FROM
				        CSI C
				        JOIN Child CH ON CH.ChildID = C.ChildID
				        JOIN CSIDomain CD ON CD.CSIID = C.CSIID
				        JOIN CSIDomainScore CDS ON CDS.CSIDomainID = CD.CSIDomainID
				        JOIN AllowedScores ON AllowedScores.AnswerID = COALESCE(CDS.AnswerID,0)
			        WHERE
				        EXISTS (SELECT * FROM RangeCSI RC WHERE RC.CSIID2 = C.CSIID)				
			        GROUP BY
				        CD.DomainID,
						C.ChildID
                ) InnerCSI
            JOIN
               CutPoint S ON (S.InitialValue <= AverageScore AND S.FinalValue >= AverageScore)
            GROUP BY
				S.CutPointID,
				S.[Description],
				InnerCSI.DomainID
		) LastCSI ON LastCSI.DomainID = FirstCSI.DomainID AND FirstCSI.CutPointID = LastCSI.CutPointID
)", fromDate1.ToString("d", DateTimeFormatInfo.InvariantInfo), toDate1.ToString("d", DateTimeFormatInfo.InvariantInfo), fromDate2.ToString("d",DateTimeFormatInfo.InvariantInfo), toDate2.ToString("d",DateTimeFormatInfo.InvariantInfo));

            var select = "SELECT S.CutPointID, S.[Description] AS CutPointDescription";
            var from = "FROM CutPoint S ";

            var where = "";

            foreach (var domain in context.Domains)
            {
                var domainNameStripped = domain.Description.Replace(" ", string.Empty);

                select += string.Format(", CAST([{0}].FirstCSIPercentualScore AS VARCHAR) + '%' AS [{1}], CAST([{0}].LastCSIPercentualScore AS VARCHAR) + '%' AS [{2}], CAST([{0}].ChangePercentualScore AS VARCHAR) + '%' AS [{3}]", domainNameStripped, domainNameStripped + "First", domainNameStripped + "Last", domainNameStripped + "Change");
                from += string.Format(" LEFT JOIN CSIScores [{0}] ON [{0}].CutPointID = S.CutPointID AND [{0}].DomainID = {1}", domainNameStripped, domain.DomainID);
            }

            return ExecuteSql(sqlCte + select + from + where);

        }

        public DataTable GetServiceTracking(int ReferenceTypeID, int parterID, DateTime fromDate, DateTime toDate, string [] StatusDesc )
        {
            var sql = string.Format(@"

Declare @ReferenceTypeID int, @partnerID int, @Completed varchar(50), @incompleted varchar(50)

SET @ReferenceTypeID = {0};
SET @partnerID = {1};
SET @incompleted = '{2}';
SET @completed = '{3}';

WITH CTE(MemberID, MemberName, Gender, ReferenceServiceID, ReferenceDate, PartnerName, [Status])
AS
(
	SELECT
		ActivistReferences.MemberID,
		activistReferences.MemberName,
		ActivistReferences.Gender,
		ActivistReferences.ReferenceServiceID,
		ActivistReferences.ReferenceDate,
		ActivistReferences.PartnerName,
		CASE WHEN RT.FieldType IS NULL THEN @completed ELSE
			(CASE WHEN ((RT.FieldType = 'CheckBox' AND R.[Value] = '1') OR (RT.FieldType <> 'CheckBox' AND R.[Value] <>'')) THEN @completed ELSE @incompleted END)
		END AS [Status]
	FROM
		(	SELECT
				C.ChildID AS MemberID, C.FirstName + ' ' + C.LastName AS MemberName, C.Gender, RS.ReferenceServiceID, RT.ReferenceTypeID, RS.ReferenceDate, P.[NAME] AS PartnerName 
			FROM
				Child C
			JOIN
				RoutineVisitMember RVM ON (RVM.AdultID IS NULL AND RVM.ChildID = C.ChildID)
			JOIN
				RoutineVisit RV ON RV.RoutineVisitID = RVM.RoutineVisitID
			JOIN
				HouseHold H ON H.HouseHoldID = RV.HouseholdID
			JOIN
				[Partner] P ON P.PartnerID = H.PartnerID
			JOIN 
				ReferenceService RS ON RS.RoutineVisitMemberID = RVM.RoutineVisitMemberID
			JOIN
				Reference R ON R.ReferenceServiceID = RS.ReferenceServiceID
			JOIN
				ReferenceType RT ON (RT.ReferenceTypeID = R.ReferenceTypeID)
			WHERE
				RT.ReferenceCategory = 'Activist'
				AND ((RT.FieldType = 'CheckBox' AND R.[Value] = '1') OR (RT.FieldType <> 'CheckBox' AND R.[Value] <>''))
				AND ((@ReferenceTypeID > 0 AND RT.ReferenceTypeID = @ReferenceTypeID) OR @ReferenceTypeID = 0)
				AND ( @partnerID = 0 OR (@partnerID > 0 AND P.PartnerID= @partnerID ))
				AND RS.ReferenceDate >= DATEADD(day,DATEDIFF(day,0,'{4}'),0) AND RS.ReferenceDate <= DATEADD(day,DATEDIFF(day,0,'{5}'),0)	
			UNION ALL
			SELECT
				AD.AdultId AS MemberID, AD.FirstName + ' ' + AD.LastName AS MemberName, AD.Gender, RS.ReferenceServiceID, RT.ReferenceTypeID, RS.ReferenceDate, P.[NAME] AS PartnerName 
			FROM
				Adult AD
			JOIN
				RoutineVisitMember RVM ON (RVM.ChildID IS NULL AND RVM.AdultID = AD.AdultId)
			JOIN
				RoutineVisit RV ON RV.RoutineVisitID = RVM.RoutineVisitID
			JOIN
				HouseHold H ON H.HouseHoldID = RV.HouseholdID
			JOIN
				[Partner] P ON P.PartnerID = H.PartnerID
			JOIN 
				ReferenceService RS ON RS.RoutineVisitMemberID = RVM.RoutineVisitMemberID
			JOIN
				Reference R ON R.ReferenceServiceID = RS.ReferenceServiceID
			JOIN
				ReferenceType RT ON (RT.ReferenceTypeID = R.ReferenceTypeID)
			WHERE
				RT.ReferenceCategory = 'Activist'
				AND ((RT.FieldType = 'CheckBox' AND R.[Value] = '1') OR (RT.FieldType <> 'CheckBox' AND R.[Value] <>''))
				AND ((@ReferenceTypeID > 0 AND RT.ReferenceTypeID = @ReferenceTypeID) OR @ReferenceTypeID = 0)
				AND ( @partnerID = 0 OR (@partnerID > 0 AND P.PartnerID= @partnerID ))
				AND RS.ReferenceDate >= DATEADD(day,DATEDIFF(day,0,'{4}'),0) AND RS.ReferenceDate <= DATEADD(day,DATEDIFF(day,0,'{5}'),0)	
		) ActivistReferences
	LEFT JOIN
		ReferenceType RT ON ( RT.OriginReferenceID = ActivistReferences.ReferenceTypeID)		
	LEFT JOIN
		Reference R  ON (R.ReferenceServiceID = ActivistReferences.ReferenceServiceID AND RT.ReferenceTypeID = R.ReferenceTypeID)
)


SELECT
	CTE.MemberID, CTE.MemberName, CTE.Gender, CTE.ReferenceServiceID, FORMAT(CTE.ReferenceDate, 'dd-MM-yyyy') As ReferenceDate, CTE.PartnerName,
	SUM(CASE WHEN CTE.[STATUS] = @completed THEN 1 ELSE 0 END) AS [COMPLETED],
	SUM(CASE WHEN CTE.[STATUS] = @completed THEN 0 ELSE 1 END) AS [INCOMPLETED],
	CASE WHEN SUM(CASE WHEN CTE.[STATUS] = @completed THEN 1 ELSE 0 END) > 0 AND SUM(CASE WHEN CTE.[STATUS] = @completed THEN 0 ELSE 1 END) = 0 THEN @completed ELSE @incompleted END AS [Status]
FROM
	CTE
GROUP BY
	CTE.MemberID, CTE.MemberName, CTE.Gender, CTE.ReferenceServiceID, FORMAT(CTE.ReferenceDate, 'dd-MM-yyyy'), PartnerName
ORDER BY
	CTE.MemberName ASC
", ReferenceTypeID, parterID, StatusDesc[0], StatusDesc[1], fromDate.ToString("d", DateTimeFormatInfo.InvariantInfo), toDate.ToString("d", DateTimeFormatInfo.InvariantInfo));

            return ExecuteSql(sql);
        }

        public List<HouseholdCQO> SearchHouseholds(string householdName, int partnerID)
        {
            var sql = @"SELECT 
	[UnionAll1].[HouseHoldID], 
    [UnionAll1].[HouseholdID1], 
    [UnionAll1].[OfficialHouseholdIdentifierNumber], 
    [UnionAll1].[HouseholdName], 
    [UnionAll1].[PrincipalChiefName], 
    [UnionAll1].[CreatedDate], 
    [UnionAll1].[RegistrationDate], 
    [UnionAll1].[Address], 
    [UnionAll1].[NeighborhoodName], 
    [UnionAll1].[Block], 
    [UnionAll1].[HouseNumber], 
    [UnionAll1].[HouseholdUniqueIdentifier], 
    [UnionAll1].[LastUpdatedDate], 
    [UnionAll1].[CreatedUserID], 
    [UnionAll1].[LastUpdatedUserID], 
    [UnionAll1].[OrgUnitID], 
    [UnionAll1].[PartnerID],
	COUNT([UnionAll1].[AdultId]) AS TotalAdult,  
	COUNT([UnionAll1].[ChildID]) AS TotalChild
FROM  (SELECT	 
        [Extent1].[HouseHoldID] AS [HouseHoldID], 
        [Extent1].[HouseHoldID] AS [HouseholdID1], 
        [Extent1].[OfficialHouseholdIdentifierNumber] AS [OfficialHouseholdIdentifierNumber], 
        [Extent1].[HouseholdName] AS [HouseholdName], 
        [Extent1].[PrincipalChiefName] AS [PrincipalChiefName], 
        [Extent1].[CreatedDate] AS [CreatedDate], 
        [Extent1].[RegistrationDate] AS [RegistrationDate], 
        [Extent1].[Address] AS [Address], 
        [Extent1].[NeighborhoodName] AS [NeighborhoodName], 
        [Extent1].[Block] AS [Block], 
        [Extent1].[HouseNumber] AS [HouseNumber], 
        [Extent1].[HouseholdUniqueIdentifier] AS [HouseholdUniqueIdentifier], 
        [Extent1].[LastUpdatedDate] AS [LastUpdatedDate], 
        [Extent1].[CreatedUserID] AS [CreatedUserID], 
        [Extent1].[LastUpdatedUserID] AS [LastUpdatedUserID], 
        [Extent1].[OrgUnitID] AS [OrgUnitID], 
        [Extent1].[PartnerID] AS [PartnerID],
		[Extent2].[AdultId] AS [AdultId],
		CAST(NULL AS int) AS [ChildID]
FROM
	HouseHold AS [Extent1]
LEFT OUTER JOIN [dbo].[Adult] AS [Extent2] ON [Extent1].[HouseHoldID] = [Extent2].[HouseholdID]
WHERE ([Extent1].[HouseholdName] LIKE @p0 ESCAPE N'~') AND (@p1 = 0 OR (@p1>0 AND [Extent1].[PartnerID] = @p1))   
UNION ALL
	SELECT
        [Extent3].[HouseHoldID] AS [HouseHoldID], 
        [Extent3].[HouseHoldID] AS [HouseholdID1], 
        [Extent3].[OfficialHouseholdIdentifierNumber] AS [OfficialHouseholdIdentifierNumber], 
        [Extent3].[HouseholdName] AS [HouseholdName], 
        [Extent3].[PrincipalChiefName] AS [PrincipalChiefName], 
        [Extent3].[CreatedDate] AS [CreatedDate], 
        [Extent3].[RegistrationDate] AS [RegistrationDate], 
        [Extent3].[Address] AS [Address], 
        [Extent3].[NeighborhoodName] AS [NeighborhoodName], 
        [Extent3].[Block] AS [Block], 
        [Extent3].[HouseNumber] AS [HouseNumber], 
        [Extent3].[HouseholdUniqueIdentifier] AS [HouseholdUniqueIdentifier], 
        [Extent3].[LastUpdatedDate] AS [LastUpdatedDate], 
        [Extent3].[CreatedUserID] AS [CreatedUserID], 
        [Extent3].[LastUpdatedUserID] AS [LastUpdatedUserID], 
        [Extent3].[OrgUnitID] AS [OrgUnitID], 
        [Extent3].[PartnerID] AS [PartnerID],
		CAST(NULL AS int) AS [AdultID],
		[Extent4].[ChildID] AS [ChildID]
		FROM  [dbo].[HouseHold] AS [Extent3]
        INNER JOIN [dbo].[Child] AS [Extent4] ON [Extent3].[HouseHoldID] = [Extent4].[HouseholdID]
        WHERE ([Extent3].[HouseholdName] LIKE @p0 ESCAPE N'~') AND (@p1 = 0 OR (@p1>0 AND [Extent3].[PartnerID] = @p1)) AND
			(SELECT TOP 1 [ChildStatusHistory].[ChildStatusID] 
								FROM [ChildStatusHistory] WHERE [ChildStatusHistory].ChildID = [Extent4].ChildID  AND CAST([EffectiveDate] as DATE) <= CAST(getdate() as DATE)  ORDER BY [EffectiveDate] DESC, [ChildStatusHistoryID] DESC) <> 6) AS [UnionAll1] 
	GROUP BY [UnionAll1].[HouseHoldID], 
    [UnionAll1].[HouseholdID1], 
    [UnionAll1].[OfficialHouseholdIdentifierNumber], 
    [UnionAll1].[HouseholdName], 
    [UnionAll1].[PrincipalChiefName], 
    [UnionAll1].[CreatedDate], 
    [UnionAll1].[RegistrationDate], 
    [UnionAll1].[Address], 
    [UnionAll1].[NeighborhoodName], 
    [UnionAll1].[Block], 
    [UnionAll1].[HouseNumber], 
    [UnionAll1].[HouseholdUniqueIdentifier], 
    [UnionAll1].[LastUpdatedDate], 
    [UnionAll1].[CreatedUserID], 
    [UnionAll1].[LastUpdatedUserID], 
    [UnionAll1].[OrgUnitID], 
    [UnionAll1].[PartnerID]
	ORDER BY [UnionAll1].[HouseholdID1] ASC";

            var results = context.Database.SqlQuery<HouseholdCQO>(sql, householdName + '%', partnerID).ToList();
            return results;
        }
    }
}
