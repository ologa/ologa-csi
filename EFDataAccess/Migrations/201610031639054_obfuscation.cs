namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class obfuscation : DbMigration
    {
        public override void Up()
        {
//            var CreateLogin = @"USE master
//                            GO
// 
//                            CREATE LOGIN [AppLogin]
//                                WITH PASSWORD = 'VPPassword',
//                                DEFAULT_DATABASE = [master],
//                                CHECK_POLICY = OFF,
//                                CHECK_EXPIRATION = OFF
//                            GO
//
//                            CREATE LOGIN [AdminLogin]
//                                WITH PASSWORD = 'VPPassword',
//                                DEFAULT_DATABASE = [master],
//                                CHECK_POLICY = OFF,
//                                CHECK_EXPIRATION = OFF
//                            GO";
//            Sql(CreateLogin);

//            var createUser = @"CREATE USER AppReader FOR LOGIN AppLogin WITH DEFAULT_SCHEMA = dbo
//                                GO
// 
//                                ALTER ROLE db_datareader ADD MEMBER AppReader
//                                GO
// 
//                                CREATE USER AppOwner FOR LOGIN AdminLogin WITH DEFAULT_SCHEMA = dbo
//                                GO
// 
//                                ALTER ROLE db_owner ADD MEMBER AppOwner
//                                GO";
//            Sql(createUser);

//            var createMasks = @"
//                                DECLARE @ProductMajorVersion INT
//                                DECLARE @Edition VARCHAR(255)
//
//                                SELECT
//	                                @ProductMajorVersion =	CONVERT(INT,SERVERPROPERTY('ProductMajorVersion')),
//	                                @Edition = CONVERT(VARCHAR, SERVERPROPERTY('edition'))
//
//                                IF @ProductMajorVersion >= '13' AND  LEFT(@Edition,15)  <> 'Express Edition'
//                                BEGIN
//	                                ALTER TABLE dbo.Child ALTER COLUMN FirstName ADD MASKED WITH (FUNCTION = 'default()')
//                                    ALTER TABLE dbo.Child ALTER COLUMN LastName ADD MASKED WITH (FUNCTION = 'default()')
//                                    ALTER TABLE dbo.Adult ALTER COLUMN FirstName ADD MASKED WITH (FUNCTION = 'default()')
//                                    ALTER TABLE dbo.Adult ALTER COLUMN LastName ADD MASKED WITH (FUNCTION = 'default()')
//                                    ALTER TABLE dbo.HouseHold ALTER COLUMN HouseholdName ADD MASKED WITH (FUNCTION = 'default()')
//                                END
//
//                                GO
//                            ";

//            Sql(createMasks);
        }
        
        public override void Down()
        {
        }
    }
}
