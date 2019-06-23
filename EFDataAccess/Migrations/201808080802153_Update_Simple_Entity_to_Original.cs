namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_Simple_Entity_to_Original : DbMigration
    {
        public override void Up()
        {
            //Allows explicit values to be inserted into the identity column of a Simple Entity
            Sql(@"SET IDENTITY_INSERT [SimpleEntity] ON");

            //Insert Simple Entities in Order
            Sql(@"begin
	                if not exists (SELECT * FROM [SimpleEntity] Where SimpleEntityID in (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21))
                    begin
                        insert into SimpleEntity(SimpleEntityID, Type, Code, description, LastUpdatedUserID, CreatedUserID, LastUpdatedDate, CreatedDate)
                        values
                        (1, 'fam-head-type', '00', 'Nenhum', 1, 1, GetDate(), GetDate()), 
                        (2, 'fam-head-type', '01', 'Avô/Idoso', 1, 1, GetDate(), GetDate()),
                        (3, 'fam-head-type', '02', 'Criança', 1, 1, GetDate(), GetDate()), 
                        (4, 'fam-head-type', '03', 'Mãe/Pai Solteiro', 1, 1, GetDate(), GetDate()),
                        (5, 'fam-head-type', '04', 'Doente Crónico Debilitado', 1, 1, GetDate(), GetDate()),
                        (6, 'fam-head-type', '05', 'Outro', 1, 1, GetDate(), GetDate()), 
                        (7, 'fam-origin-ref-type', '00', 'Nenhuma', 1, 1, GetDate(), GetDate()), 
                        (8, 'fam-origin-ref-type', '01', 'Comunidade', 1, 1, GetDate(), GetDate()),
                        (9, 'fam-origin-ref-type', '02', 'Unidade Sanitária', 1, 1, GetDate(), GetDate()), 
                        (10, 'fam-origin-ref-type', '03', 'Parceiro Clínico', 1, 1, GetDate(), GetDate()),
                        (11, 'fam-origin-ref-type', '04', 'Outra', 1, 1, GetDate(), GetDate()),
                        (12, 'degree-of-kinship', '00', 'Neto(a)', 1, 1, GetDate(), GetDate()), 
                        (13, 'degree-of-kinship', '01', 'Filho(a)', 1, 1, GetDate(), GetDate()), 
                        (14, 'degree-of-kinship', '02', 'Irmão(a)', 1, 1, GetDate(), GetDate()), 
                        (15, 'degree-of-kinship', '03', 'Sobrinho(a)', 1, 1, GetDate(), GetDate()), 
                        (16, 'degree-of-kinship', '04', 'Avô/Avó', 1, 1, GetDate(), GetDate()), 
                        (17, 'degree-of-kinship', '05', 'Outro', 1, 1, GetDate(), GetDate()), 
                        (18, 'degree-of-kinship', '06', 'Esposo(a)', 1, 1, GetDate(), GetDate()), 
                        (19, 'fam-head-type', '99', 'Indeterminado', 1, 1, GetDate(), GetDate()), 
                        (20, 'fam-origin-ref-type', '99', 'Indeterminado', 1, 1, GetDate(), GetDate()),
                        (21, 'degree-of-kinship', '99', 'Indeterminado', 1, 1, GetDate(), GetDate())
                    end
                  end"
                );

            //Update Family Head to Original
            Sql(@"UPDATE hh
                SET hh.FamilyHeadID = sefirst.SimpleEntityID
                FROM Household hh
                JOIN SimpleEntity se ON hh.FamilyHeadID = se.SimpleEntityID AND se.Type= 'fam-head-type'
                JOIN SimpleEntity sefirst ON sefirst.Code = se.Code
                WHERE sefirst.SimpleEntityID IN 
                ( SELECT obj.SimpleEntityID FROM
	                ( SELECT  row_number() OVER (PARTITION BY code ORDER BY SimpleEntityID ASC) AS numeroLinha
			          ,description ,SimpleEntityID ,type ,Code FROM SimpleEntity WHERE type = 'fam-head-type'
	                ) obj WHERE obj.numeroLinha=1
                )");

            //Update of KinshipToFamilyHeadID to Indeterminate, case FamilyHead is set to Adult
            Sql(@"UPDATE a
                SET a.KinshipToFamilyHeadID = 20
                --SELECT a.AdultId, a.FirstName, a.LastName, se.SimpleEntityID, se.Description, se.Type
                FROM[CSI_PROD].[dbo].[Adult] a
                JOIN[CSI_PROD].[dbo].SimpleEntity se ON se.SimpleEntityID = a.KinshipToFamilyHeadID
                AND se.type = 'fam-head-type'");

            //Update of KinshipToFamilyHeadID to Indeterminate, case FamilyHead is set to Child
            Sql(@"UPDATE c
                SET c.KinshipToFamilyHeadID = 20
                --SELECT c.ChildID, c.FirstName, c.LastName, se.SimpleEntityID, se.Description, se.Type
                FROM [CSI_PROD].[dbo].[Child] c
                JOIN [CSI_PROD].[dbo].[SimpleEntity] se ON se.SimpleEntityID = c.KinshipToFamilyHeadID
                AND se.type = 'fam-head-type'");


            //Delete Duplicated Family Head
            Sql(@"DELETE se
                FROM SimpleEntity se
                WHERE se.SimpleEntityID IN 
                ( SELECT obj.SimpleEntityID FROM
	                ( SELECT  row_number() OVER (PARTITION BY code ORDER BY SimpleEntityID ASC) AS numeroLinha
			          ,description ,SimpleEntityID ,type ,Code FROM SimpleEntity WHERE type = 'fam-head-type'
	                ) obj WHERE obj.numeroLinha>1
                )");

            
            //Update Family Origin to Original
            Sql(@"UPDATE hh
                SET hh.FamilyOriginRefID = sefirst.SimpleEntityID
                FROM Household hh
                JOIN SimpleEntity se ON hh.FamilyOriginRefID = se.SimpleEntityID AND se.Type= 'fam-origin-ref-type'
                JOIN SimpleEntity sefirst ON sefirst.Code = se.Code
                WHERE sefirst.SimpleEntityID IN 
                ( SELECT obj.SimpleEntityID FROM
	                ( SELECT  row_number() OVER (PARTITION BY code ORDER BY SimpleEntityID ASC) AS numeroLinha
			          ,description ,SimpleEntityID ,type ,Code FROM SimpleEntity WHERE type = 'fam-origin-ref-type'
	                ) obj WHERE obj.numeroLinha=1
                )");

            //Update of KinshipToFamilyHeadID to Indeterminate, case FamilyOrigin is set to Adult
            Sql(@"UPDATE a
                SET a.KinshipToFamilyHeadID = 20
                --SELECT a.AdultId, a.FirstName, a.LastName, se.SimpleEntityID, se.Description, se.Type
                FROM[CSI_PROD].[dbo].[Adult] a
                JOIN[CSI_PROD].[dbo].SimpleEntity se ON se.SimpleEntityID = a.KinshipToFamilyHeadID
                AND se.type = 'fam-origin-ref-type'");

            //Update of KinshipToFamilyHeadID to Indeterminate, case FamilyOrigin is set to Child
            Sql(@"UPDATE c
                SET c.KinshipToFamilyHeadID = 20
                --SELECT c.ChildID, c.FirstName, c.LastName, se.SimpleEntityID, se.Description, se.Type
                FROM [CSI_PROD].[dbo].[Child] c
                JOIN [CSI_PROD].[dbo].[SimpleEntity] se ON se.SimpleEntityID = c.KinshipToFamilyHeadID
                AND se.type = 'fam-origin-ref-type'");


            //Delete Duplicated Family Origin
            Sql(@"DELETE se
                FROM SimpleEntity se
                WHERE se.SimpleEntityID IN 
                ( SELECT obj.SimpleEntityID FROM
	                ( SELECT  row_number() OVER (PARTITION BY code ORDER BY SimpleEntityID ASC) AS numeroLinha
			          ,description ,SimpleEntityID ,type ,Code FROM SimpleEntity WHERE type = 'fam-origin-ref-type'
	                ) obj WHERE obj.numeroLinha>1
                )");

            //Update Kingship to Family Head of Child to Original
            Sql(@"UPDATE c
                SET c.KinshipToFamilyHeadID = sefirst.SimpleEntityID
                FROM Child c
                JOIN SimpleEntity se ON c.KinshipToFamilyHeadID = se.SimpleEntityID AND se.Type= 'degree-of-kinship'
                JOIN SimpleEntity sefirst ON sefirst.Code = se.Code
                WHERE sefirst.SimpleEntityID IN 
                ( SELECT obj.SimpleEntityID FROM
	                ( SELECT  row_number() OVER (PARTITION BY code ORDER BY SimpleEntityID ASC) AS numeroLinha
			          ,description ,SimpleEntityID ,type ,Code FROM SimpleEntity WHERE type = 'degree-of-kinship'
	                ) obj WHERE obj.numeroLinha=1
                )");

            //Update Kingship to Family Head of Adult to Original
            Sql(@"UPDATE a
                SET a.KinshipToFamilyHeadID = sefirst.SimpleEntityID
                FROM Adult a
                JOIN SimpleEntity se ON a.KinshipToFamilyHeadID = se.SimpleEntityID AND se.Type= 'degree-of-kinship'
                JOIN SimpleEntity sefirst ON sefirst.Code = se.Code
                WHERE sefirst.SimpleEntityID IN 
                ( SELECT obj.SimpleEntityID FROM
	                ( SELECT  row_number() OVER (PARTITION BY code ORDER BY SimpleEntityID ASC) AS numeroLinha
			          ,description ,SimpleEntityID ,type ,Code FROM SimpleEntity WHERE type = 'degree-of-kinship'
	                ) obj WHERE obj.numeroLinha=1
                )");

            //Delete Duplicated Kingship to Family Head
            Sql(@"DELETE se
                FROM SimpleEntity se
                WHERE se.SimpleEntityID IN 
                ( SELECT obj.SimpleEntityID FROM
	                ( SELECT  row_number() OVER (PARTITION BY code ORDER BY SimpleEntityID ASC) AS numeroLinha
			          ,description ,SimpleEntityID ,type ,Code FROM SimpleEntity WHERE type = 'degree-of-kinship'
	                ) obj WHERE obj.numeroLinha>1
                )");

            //Disable explicit values to be inserted into the identity column of a Simple Entity
            Sql(@"SET IDENTITY_INSERT [SimpleEntity] OFF");


        }
        
        public override void Down()
        {
        }
    }
}
