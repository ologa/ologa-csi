namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InsertUndeterminedTypeForBeneficiaryAndHouseholdNewMandatoryFields : DbMigration
    {
        public override void Up()
        {
            Sql(@"INSERT INTO [SimpleEntity] ([Type],[Code],[Description],[CreatedDate],
                [LastUpdatedDate],[CreatedUserID],[LastUpdatedUserID],[State],[SyncState],[SimpleEntityGuid])
                VALUES('fam-head-type','99','Indeterminado', GETDATE(), GETDATE(),1,1,0,0, newid())");
            Sql(@"INSERT INTO [SimpleEntity] ([Type],[Code],[Description],[CreatedDate],
                [LastUpdatedDate],[CreatedUserID],[LastUpdatedUserID],[State],[SyncState],[SimpleEntityGuid])
                VALUES('fam-origin-ref-type','99','Indeterminado', GETDATE(), GETDATE(),1,1,0,0, newid())");
            Sql(@"INSERT INTO [SimpleEntity] ([Type],[Code],[Description],[CreatedDate],
                [LastUpdatedDate],[CreatedUserID],[LastUpdatedUserID],[State],[SyncState],[SimpleEntityGuid])
                VALUES('degree-of-kinship','99','Indeterminado', GETDATE(), GETDATE(),1,1,0,0, newid())");
            Sql(@"INSERT INTO [OVCType] ([Description], [ovctype_guid]) VALUES('Indeterminado', newid())");

            Sql(@"Update Child Set KinshipToFamilyHeadID = (Select SimpleEntityID from SimpleEntity where Type = 'degree-of-kinship' and Code = '99') Where KinshipToFamilyHeadID is NULL");
            Sql(@"Update Child Set OVCTypeID = (Select OVCTypeID from OVCType where Description = 'Indeterminado') Where OVCTypeID is NULL");
            Sql(@"Update Adult Set KinshipToFamilyHeadID = (Select SimpleEntityID from SimpleEntity where Type = 'degree-of-kinship' and Code = '99') Where KinshipToFamilyHeadID is NULL");
            Sql(@"Update HouseHold Set FamilyHeadId = (Select SimpleEntityID from SimpleEntity where Type = 'fam-head-type' and Code = '99') Where FamilyHeadId is NULL");
            Sql(@"Update HouseHold Set FamilyOriginRefId = (Select SimpleEntityID from SimpleEntity where Type = 'fam-origin-ref-type' and Code = '99') Where FamilyOriginRefId is NULL");
        }
        
        public override void Down()
        {
        }
    }
}
