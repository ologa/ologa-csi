namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InsertAdultStatusForAllAdults : DbMigration
    {
        public override void Up()
        {
            Sql(@"insert into ChildStatusHistory(EffectiveDate,CreatedDate,ChildStatusID,CreatedUserID,AdultID)
                    select h.RegistrationDate,h.RegistrationDate,6,h.CreatedUserID,a.AdultId from Adult a
                    left join ChildStatusHistory  ch on ch.AdultID=a.AdultId
                    inner join HouseHold h on h.HouseHoldID=a.HouseholdID
                    where ch.AdultID is null");
        }
        
        public override void Down()
        {
        }
    }
}
