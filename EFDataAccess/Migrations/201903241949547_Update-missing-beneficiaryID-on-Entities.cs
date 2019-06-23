namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatemissingbeneficiaryIDonEntities : DbMigration
    {
        public override void Up()
        {
            // CSI
            Sql(@"UPDATE csi SET csi.BeneficiaryID = b.BeneficiaryID
            FROM CSI csi INNER JOIN 
            (select c.ChildID,BeneficiaryID from Child c 
            join Adult a on c.ChildID= a.ChildID 
            join Beneficiary b on b.AdultID=a.AdultId)b ON csi.ChildID = b.ChildID");

            // ReferenceService
            Sql(@"UPDATE r SET r.BeneficiaryID = b.BeneficiaryID
            FROM ReferenceService r INNER JOIN 
            (select c.ChildID,BeneficiaryID from Child c 
            join Adult a on c.ChildID= a.ChildID 
            join Beneficiary b on b.AdultID=a.AdultId)b ON r.ChildID = b.ChildID");

            // HIVStatus
            Sql(@"UPDATE h SET h.BeneficiaryID = b.BeneficiaryID, h.BeneficiaryGuid = b.Beneficiary_guid
            FROM HIVStatus h INNER JOIN 
            (select c.ChildID,BeneficiaryID ,Beneficiary_guid from Child c 
            join Adult a on c.ChildID= a.ChildID 
            join Beneficiary b on b.AdultID=a.AdultId)b ON h.ChildID = b.ChildID");

            // ChildStatusHistory
            Sql(@"UPDATE csh SET csh.BeneficiaryID = b.BeneficiaryID, csh.BeneficiaryGuid = b.Beneficiary_guid
            FROM ChildStatusHistory csh INNER JOIN 
            (select c.ChildID,BeneficiaryID,Beneficiary_guid from Child c 
            join Adult a on c.ChildID= a.ChildID 
            join Beneficiary b on b.AdultID=a.AdultId)b ON csh.ChildID = b.ChildID");

            // RoutineVisitMember
            Sql(@"UPDATE rvm SET rvm.BeneficiaryID = b.BeneficiaryID
            FROM RoutineVisitMember rvm JOIN 
            (select c.ChildID,BeneficiaryID from Child c 
            join Adult a on c.ChildID= a.ChildID 
            join Beneficiary b on b.AdultID=a.AdultId)b  ON rvm.ChildID = b.ChildID");
        }
        
        public override void Down()
        {
        }
    }
}
