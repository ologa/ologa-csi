namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_Created_and_LastUpdated_Users_to_Original : DbMigration
    {
        public override void Up()
        {
            //Update CreatedUserID for Child
            Sql(@"UPDATE obj
                SET obj.CreatedUserID = ufirst.UserID
                FROM [Child] obj
                JOIN [User] u ON u.UserID = obj.CreatedUserID
                JOIN [User] ufirst ON u.UserName = ufirst.UserName
                WHERE ufirst.UserID IN 
                ( SELECT obj.UserID FROM 
                    (   SELECT row_number() OVER (PARTITION BY UserName ORDER BY UserID ASC) AS numeroLinha ,UserID ,UserName
		                FROM [User] WHERE UserName IN ( Select UserName FROM [User] GROUP BY UserName HAVING count(User_guid) > 2 ) 
                    ) obj WHERE obj.numeroLinha=1
                )");

            //Update LastUpdatedUserID for Child
            Sql(@"UPDATE obj
                SET obj.LastUpdatedUserID = ufirst.UserID
                FROM [Child] obj
                JOIN [User] u ON u.UserID = obj.LastUpdatedUserID
                JOIN [User] ufirst ON u.UserName = ufirst.UserName
                WHERE ufirst.UserID IN 
                ( SELECT obj.UserID FROM 
                    (   SELECT row_number() OVER (PARTITION BY UserName ORDER BY UserID ASC) AS numeroLinha ,UserID ,UserName
		                FROM [User] WHERE UserName IN ( Select UserName FROM [User] GROUP BY UserName HAVING count(User_guid) > 2 ) 
                    ) obj WHERE obj.numeroLinha=1
                )");

            //Update CreatedUserID for Adult
            Sql(@"UPDATE obj
                SET obj.CreatedUserID = ufirst.UserID
                FROM [Adult] obj
                JOIN [User] u ON u.UserID = obj.CreatedUserID
                JOIN [User] ufirst ON u.UserName = ufirst.UserName
                WHERE ufirst.UserID IN 
                ( SELECT obj.UserID FROM 
                    (   SELECT row_number() OVER (PARTITION BY UserName ORDER BY UserID ASC) AS numeroLinha ,UserID ,UserName
		                FROM [User] WHERE UserName IN ( Select UserName FROM [User] GROUP BY UserName HAVING count(User_guid) > 2 ) 
                    ) obj WHERE obj.numeroLinha=1
                )");

            //Update LastUpdatedUserID for Adult
            Sql(@"UPDATE obj
                SET obj.LastUpdatedUserID = ufirst.UserID
                FROM [Adult] obj
                JOIN [User] u ON u.UserID = obj.LastUpdatedUserID
                JOIN [User] ufirst ON u.UserName = ufirst.UserName
                WHERE ufirst.UserID IN 
                ( SELECT obj.UserID FROM 
                    (   SELECT row_number() OVER (PARTITION BY UserName ORDER BY UserID ASC) AS numeroLinha ,UserID ,UserName
		                FROM [User] WHERE UserName IN ( Select UserName FROM [User] GROUP BY UserName HAVING count(User_guid) > 2 ) 
                    ) obj WHERE obj.numeroLinha=1
                )");

            //Update CreatedUserID for ChildStatusHistory
            Sql(@"UPDATE obj
                SET obj.CreatedUserID = ufirst.UserID
                FROM [ChildStatusHistory] obj
                JOIN [User] u ON u.UserID = obj.CreatedUserID
                JOIN [User] ufirst ON u.UserName = ufirst.UserName
                WHERE ufirst.UserID IN 
                ( SELECT obj.UserID FROM 
                    (   SELECT row_number() OVER (PARTITION BY UserName ORDER BY UserID ASC) AS numeroLinha ,UserID ,UserName
		                FROM [User] WHERE UserName IN ( Select UserName FROM [User] GROUP BY UserName HAVING count(User_guid) > 2 ) 
                    ) obj WHERE obj.numeroLinha=1
                )");

            //Update LastUpdatedUserID for ChildStatusHistory
            Sql(@"UPDATE obj
                SET obj.LastUpdatedUserID = ufirst.UserID
                FROM [ChildStatusHistory] obj
                JOIN [User] u ON u.UserID = obj.LastUpdatedUserID
                JOIN [User] ufirst ON u.UserName = ufirst.UserName
                WHERE ufirst.UserID IN 
                ( SELECT obj.UserID FROM 
                    (   SELECT row_number() OVER (PARTITION BY UserName ORDER BY UserID ASC) AS numeroLinha ,UserID ,UserName
		                FROM [User] WHERE UserName IN ( Select UserName FROM [User] GROUP BY UserName HAVING count(User_guid) > 2 ) 
                    ) obj WHERE obj.numeroLinha=1
                )");


            //Update UserID for HIVStatus
            Sql(@"UPDATE obj
                SET obj.UserID = ufirst.UserID
                FROM [HIVStatus] obj
                JOIN [User] u ON u.UserID = obj.UserID
                JOIN [User] ufirst ON u.UserName = ufirst.UserName
                WHERE ufirst.UserID IN 
                ( SELECT obj.UserID FROM 
                    (   SELECT row_number() OVER (PARTITION BY UserName ORDER BY UserID ASC) AS numeroLinha ,UserID ,UserName
		                FROM [User] WHERE UserName IN ( Select UserName FROM [User] GROUP BY UserName HAVING count(User_guid) > 2 ) 
                    ) obj WHERE obj.numeroLinha=1
                )");

            //Update CreatedUserID for Household
            Sql(@"UPDATE obj
                SET obj.CreatedUserID = ufirst.UserID
                FROM [Household] obj
                JOIN [User] u ON u.UserID = obj.CreatedUserID
                JOIN [User] ufirst ON u.UserName = ufirst.UserName
                WHERE ufirst.UserID IN 
                ( SELECT obj.UserID FROM 
                    (   SELECT row_number() OVER (PARTITION BY UserName ORDER BY UserID ASC) AS numeroLinha ,UserID ,UserName
		                FROM [User] WHERE UserName IN ( Select UserName FROM [User] GROUP BY UserName HAVING count(User_guid) > 2 ) 
                    ) obj WHERE obj.numeroLinha=1
                )");

            //Update LastUpdatedUserID for Household
            Sql(@"UPDATE obj
                SET obj.LastUpdatedUserID = ufirst.UserID
                FROM [Household] obj
                JOIN [User] u ON u.UserID = obj.LastUpdatedUserID
                JOIN [User] ufirst ON u.UserName = ufirst.UserName
                WHERE ufirst.UserID IN 
                ( SELECT obj.UserID FROM 
                    (   SELECT row_number() OVER (PARTITION BY UserName ORDER BY UserID ASC) AS numeroLinha ,UserID ,UserName
		                FROM [User] WHERE UserName IN ( Select UserName FROM [User] GROUP BY UserName HAVING count(User_guid) > 2 ) 
                    ) obj WHERE obj.numeroLinha=1
                )");

            //Update CreatedUserID for CSIDomain
            Sql(@"UPDATE obj
                SET obj.CreatedUserID = ufirst.UserID
                FROM [CSIDomain] obj
                JOIN [User] u ON u.UserID = obj.CreatedUserID
                JOIN [User] ufirst ON u.UserName = ufirst.UserName
                WHERE ufirst.UserID IN 
                ( SELECT obj.UserID FROM 
                    (   SELECT row_number() OVER (PARTITION BY UserName ORDER BY UserID ASC) AS numeroLinha ,UserID ,UserName
		                FROM [User] WHERE UserName IN ( Select UserName FROM [User] GROUP BY UserName HAVING count(User_guid) > 2 ) 
                    ) obj WHERE obj.numeroLinha=1
                )");

            //Update LastUpdatedUserID for CSIDomain
            Sql(@"UPDATE obj
                SET obj.LastUpdatedUserID = ufirst.UserID
                FROM [CSIDomain] obj
                JOIN [User] u ON u.UserID = obj.LastUpdatedUserID
                JOIN [User] ufirst ON u.UserName = ufirst.UserName
                WHERE ufirst.UserID IN 
                ( SELECT obj.UserID FROM 
                    (   SELECT row_number() OVER (PARTITION BY UserName ORDER BY UserID ASC) AS numeroLinha ,UserID ,UserName
		                FROM [User] WHERE UserName IN ( Select UserName FROM [User] GROUP BY UserName HAVING count(User_guid) > 2 ) 
                    ) obj WHERE obj.numeroLinha=1
                )");


            //Update CreatedUserID for CSI
            Sql(@"UPDATE obj
                SET obj.CreatedUserID = ufirst.UserID
                FROM [CSI] obj
                JOIN [User] u ON u.UserID = obj.CreatedUserID
                JOIN [User] ufirst ON u.UserName = ufirst.UserName
                WHERE ufirst.UserID IN 
                ( SELECT obj.UserID FROM 
                    (   SELECT row_number() OVER (PARTITION BY UserName ORDER BY UserID ASC) AS numeroLinha ,UserID ,UserName
		                FROM [User] WHERE UserName IN ( Select UserName FROM [User] GROUP BY UserName HAVING count(User_guid) > 2 ) 
                    ) obj WHERE obj.numeroLinha=1
                )");

            //Update LastUpdatedUserID for CSI
            Sql(@"UPDATE obj
                SET obj.LastUpdatedUserID = ufirst.UserID
                FROM [CSI] obj
                JOIN [User] u ON u.UserID = obj.LastUpdatedUserID
                JOIN [User] ufirst ON u.UserName = ufirst.UserName
                WHERE ufirst.UserID IN 
                ( SELECT obj.UserID FROM 
                    (   SELECT row_number() OVER (PARTITION BY UserName ORDER BY UserID ASC) AS numeroLinha ,UserID ,UserName
		                FROM [User] WHERE UserName IN ( Select UserName FROM [User] GROUP BY UserName HAVING count(User_guid) > 2 ) 
                    ) obj WHERE obj.numeroLinha=1
                )");


            //Update CreatedUserID for CSIDomainScore
            Sql(@"UPDATE obj
                SET obj.CreatedUserID = ufirst.UserID
                FROM [CSIDomainScore] obj
                JOIN [User] u ON u.UserID = obj.CreatedUserID
                JOIN [User] ufirst ON u.UserName = ufirst.UserName
                WHERE ufirst.UserID IN 
                ( SELECT obj.UserID FROM 
                    (   SELECT row_number() OVER (PARTITION BY UserName ORDER BY UserID ASC) AS numeroLinha ,UserID ,UserName
		                FROM [User] WHERE UserName IN ( Select UserName FROM [User] GROUP BY UserName HAVING count(User_guid) > 2 ) 
                    ) obj WHERE obj.numeroLinha=1
                )");

            //Update LastUpdatedUserID for CSIDomainScore
            Sql(@"UPDATE obj
                SET obj.LastUpdatedUserID = ufirst.UserID
                FROM [CSIDomainScore] obj
                JOIN [User] u ON u.UserID = obj.LastUpdatedUserID
                JOIN [User] ufirst ON u.UserName = ufirst.UserName
                WHERE ufirst.UserID IN 
                ( SELECT obj.UserID FROM 
                    (   SELECT row_number() OVER (PARTITION BY UserName ORDER BY UserID ASC) AS numeroLinha ,UserID ,UserName
		                FROM [User] WHERE UserName IN ( Select UserName FROM [User] GROUP BY UserName HAVING count(User_guid) > 2 ) 
                    ) obj WHERE obj.numeroLinha=1
                )");


            //Update CreatedUserID for CarePlan
            Sql(@"UPDATE obj
                SET obj.CreatedUserID = ufirst.UserID
                FROM [CarePlan] obj
                JOIN [User] u ON u.UserID = obj.CreatedUserID
                JOIN [User] ufirst ON u.UserName = ufirst.UserName
                WHERE ufirst.UserID IN 
                ( SELECT obj.UserID FROM 
                    (   SELECT row_number() OVER (PARTITION BY UserName ORDER BY UserID ASC) AS numeroLinha ,UserID ,UserName
		                FROM [User] WHERE UserName IN ( Select UserName FROM [User] GROUP BY UserName HAVING count(User_guid) > 2 ) 
                    ) obj WHERE obj.numeroLinha=1
                )");

            //Update LastUpdatedUserID for CarePlan
            Sql(@"UPDATE obj
                SET obj.LastUpdatedUserID = ufirst.UserID
                FROM [CarePlan] obj
                JOIN [User] u ON u.UserID = obj.LastUpdatedUserID
                JOIN [User] ufirst ON u.UserName = ufirst.UserName
                WHERE ufirst.UserID IN 
                ( SELECT obj.UserID FROM 
                    (   SELECT row_number() OVER (PARTITION BY UserName ORDER BY UserID ASC) AS numeroLinha ,UserID ,UserName
		                FROM [User] WHERE UserName IN ( Select UserName FROM [User] GROUP BY UserName HAVING count(User_guid) > 2 ) 
                    ) obj WHERE obj.numeroLinha=1
                )");


            //Update CreatedUserID for RoutineVisit
            Sql(@"UPDATE obj
                SET obj.CreatedUserID = ufirst.UserID
                FROM [RoutineVisit] obj
                JOIN [User] u ON u.UserID = obj.CreatedUserID
                JOIN [User] ufirst ON u.UserName = ufirst.UserName
                WHERE ufirst.UserID IN 
                ( SELECT obj.UserID FROM 
                    (   SELECT row_number() OVER (PARTITION BY UserName ORDER BY UserID ASC) AS numeroLinha ,UserID ,UserName
		                FROM [User] WHERE UserName IN ( Select UserName FROM [User] GROUP BY UserName HAVING count(User_guid) > 2 ) 
                    ) obj WHERE obj.numeroLinha=1
                )");

            //Update LastUpdatedUserID for RoutineVisit
            Sql(@"UPDATE obj
                SET obj.LastUpdatedUserID = ufirst.UserID
                FROM [RoutineVisit] obj
                JOIN [User] u ON u.UserID = obj.LastUpdatedUserID
                JOIN [User] ufirst ON u.UserName = ufirst.UserName
                WHERE ufirst.UserID IN 
                ( SELECT obj.UserID FROM 
                    (   SELECT row_number() OVER (PARTITION BY UserName ORDER BY UserID ASC) AS numeroLinha ,UserID ,UserName
		                FROM [User] WHERE UserName IN ( Select UserName FROM [User] GROUP BY UserName HAVING count(User_guid) > 2 ) 
                    ) obj WHERE obj.numeroLinha=1
                )");


            //Update CreatedUserID for ReferenceService
            Sql(@"UPDATE obj
                SET obj.CreatedUserID = ufirst.UserID
                FROM [ReferenceService] obj
                JOIN [User] u ON u.UserID = obj.CreatedUserID
                JOIN [User] ufirst ON u.UserName = ufirst.UserName
                WHERE ufirst.UserID IN 
                ( SELECT obj.UserID FROM 
                    (   SELECT row_number() OVER (PARTITION BY UserName ORDER BY UserID ASC) AS numeroLinha ,UserID ,UserName
		                FROM [User] WHERE UserName IN ( Select UserName FROM [User] GROUP BY UserName HAVING count(User_guid) > 2 ) 
                    ) obj WHERE obj.numeroLinha=1
                )");

            //Update LastUpdatedUserID for ReferenceService
            Sql(@"UPDATE obj
                SET obj.LastUpdatedUserID = ufirst.UserID
                FROM [ReferenceService] obj
                JOIN [User] u ON u.UserID = obj.LastUpdatedUserID
                JOIN [User] ufirst ON u.UserName = ufirst.UserName
                WHERE ufirst.UserID IN 
                ( SELECT obj.UserID FROM 
                    (   SELECT row_number() OVER (PARTITION BY UserName ORDER BY UserID ASC) AS numeroLinha ,UserID ,UserName
		                FROM [User] WHERE UserName IN ( Select UserName FROM [User] GROUP BY UserName HAVING count(User_guid) > 2 ) 
                    ) obj WHERE obj.numeroLinha=1
                )");

            //Delete Duplicated Users
            Sql(@"DELETE obj FROM [User] obj
                WHERE obj.UserID IN 
                ( SELECT obj.UserID FROM 
                    (   SELECT row_number() OVER (PARTITION BY UserName ORDER BY UserID ASC) AS numeroLinha ,UserID ,UserName
		                FROM [User] WHERE UserName IN ( Select UserName FROM [User] GROUP BY UserName HAVING count(User_guid) > 2 ) 
                    ) obj WHERE obj.numeroLinha>1
                )");


        }
        
        public override void Down()
        {
        }
    }
}
