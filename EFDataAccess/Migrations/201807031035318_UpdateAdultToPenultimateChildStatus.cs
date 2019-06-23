namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateAdultToPenultimateChildStatus : DbMigration
    {
        public override void Up()
        {
            Sql(@"Update ChildStatusHistory Set 
                    ChildStatusID = obj.CChildStatusID -- Estado do adulto, fica com Penultimo estado da crianca, que deve ser != de inicial e adulto
                    From
                    (
                    select 
                    c.FirstName As CFirstName, c.LastName As CLastName, c.DateOfBirth As CDateOfBirth, cs1.Description As CStatusDescription, 
                    csh.ChildStatusID As CChildStatusID, csh.ChildStatusHistoryID As CChildStatusHistoryID, csh.EffectiveDate As CEffectiveDate, 
                    a.FirstName As AFirstName, a.LastName As ALastName, a.DateOfBirth As ADateOfBirth, cs2.Description As AStatusDescription, 
                    ash.ChildStatusID As AChildStatusID, ash.ChildStatusHistoryID As AChildStatusHistoryID, ash.EffectiveDate As AEffectiveDate
                    From Adult a inner join Child c on (c.FirstName = a.FirstName and c.LastName = a.LastName and c.DateOfBirth = a.DateOfBirth)
                    inner join  [ChildStatusHistory] csh on (csh.ChildStatusHistoryID = -- Criancas com estado diferente de Inicial e Adulto
                    (SELECT (max(csh2.ChildStatusHistoryID)) FROM  [ChildStatusHistory] csh2 WHERE csh2.ChildID = c.ChildID and csh2.ChildStatusID != 6 and csh2.ChildStatusID != 1)) 
                    inner join  [ChildStatus] cs1 on (cs1.StatusID = csh.ChildStatusID)
                    inner join  [ChildStatusHistory] ash on (ash.ChildStatusHistoryID = -- Adultos com estado Inicial ou Adulto
                    (SELECT max(ash2.ChildStatusHistoryID) FROM  [ChildStatusHistory] ash2 WHERE ash2.AdultID = a.AdultId and (ash2.ChildStatusID = 6 or ash2.ChildStatusID = 1 ))) 
                    inner join  [ChildStatus] cs2 on (cs2.StatusID = ash.ChildStatusID)
                    ) obj
                    where ChildStatusHistoryID = obj.AChildStatusHistoryID -- Estado do Adulto");
        }
        
        public override void Down()
        {
        }
    }
}
