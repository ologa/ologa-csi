namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class insert_value_zero_to_otherReferences_CheckBoxes_on_existing_ReferenceServices : DbMigration
    {
        public override void Up()
        {

            //-----------------ACTIVISTA-----------------------

            //Activista/Atestado de Pobreza
            Sql(@"INSERT INTO [Reference]
            ([Value],[ReferenceTypeID],[ReferenceServiceID],[State],[SyncState])
            SELECT 
	            0, 
	            (
		            SELECT [ReferenceTypeID]
		            FROM  [ReferenceType] rType
		            WHERE rType.ReferenceName IN ('Atestado de Pobreza')
		            AND rType.ReferenceCategory IN ('Activist')
	            ),
	            rService.ReferenceServiceID, 0, 0 
            FROM  [ReferenceService] rService", false);


            //Activista/Registo de Nascimento/Cédula
            Sql(@"INSERT INTO [Reference]
            ([Value],[ReferenceTypeID],[ReferenceServiceID],[State],[SyncState])
            SELECT 
	            0, 
	            (
		            SELECT [ReferenceTypeID]
		            FROM  [ReferenceType] rType
		            WHERE rType.ReferenceName IN ('Registo de Nascimento/Cédula')
		            AND rType.ReferenceCategory IN ('Activist')
	            ),
	            rService.ReferenceServiceID, 0, 0 
            FROM  [ReferenceService] rService", false);


            //Activista/Bilhete de Identidade (B.I)
            Sql(@"INSERT INTO [Reference]
            ([Value],[ReferenceTypeID],[ReferenceServiceID],[State],[SyncState])
            SELECT 
	            0, 
	            (
		            SELECT [ReferenceTypeID]
		            FROM  [ReferenceType] rType
		            WHERE rType.ReferenceName IN ('Bilhete de Identidade (B.I)')
		            AND rType.ReferenceCategory IN ('Activist')
	            ),
	            rService.ReferenceServiceID, 0, 0 
            FROM  [ReferenceService] rService", false);


            //Activista/Integração Escolar
            Sql(@"INSERT INTO [Reference]
            ([Value],[ReferenceTypeID],[ReferenceServiceID],[State],[SyncState])
            SELECT 
	            0, 
	            (
		            SELECT [ReferenceTypeID]
		            FROM  [ReferenceType] rType
		            WHERE rType.ReferenceName IN ('Integração Escolar')
		            AND rType.ReferenceCategory IN ('Activist')
	            ),
	            rService.ReferenceServiceID, 0, 0 
            FROM  [ReferenceService] rService", false);


            //Activista/Curso de Formação Vocacional
            Sql(@"INSERT INTO [Reference]
            ([Value],[ReferenceTypeID],[ReferenceServiceID],[State],[SyncState])
            SELECT 
	            0, 
	            (
		            SELECT [ReferenceTypeID]
		            FROM  [ReferenceType] rType
		            WHERE rType.ReferenceName IN ('Curso de Formação Vocacional')
		            AND rType.ReferenceCategory IN ('Activist')
	            ),
	            rService.ReferenceServiceID, 0, 0 
            FROM  [ReferenceService] rService", false);


            //Activista/Material Escolar
            Sql(@"INSERT INTO [Reference]
            ([Value],[ReferenceTypeID],[ReferenceServiceID],[State],[SyncState])
            SELECT 
	            0, 
	            (
		            SELECT [ReferenceTypeID]
		            FROM  [ReferenceType] rType
		            WHERE rType.ReferenceName IN ('Material Escolar')
		            AND rType.ReferenceCategory IN ('Activist')
	            ),
	            rService.ReferenceServiceID, 0, 0 
            FROM  [ReferenceService] rService", false);


            //Activista/Cesta Básica
            Sql(@"INSERT INTO [Reference]
            ([Value],[ReferenceTypeID],[ReferenceServiceID],[State],[SyncState])
            SELECT 
	            0, 
	            (
		            SELECT [ReferenceTypeID]
		            FROM  [ReferenceType] rType
		            WHERE rType.ReferenceName IN ('Cesta Básica')
		            AND rType.ReferenceCategory IN ('Activist')
	            ),
	            rService.ReferenceServiceID, 0, 0 
            FROM  [ReferenceService] rService", false);


            //Activista/Subsídios Sociais do INAS
            Sql(@"INSERT INTO [Reference]
            ([Value],[ReferenceTypeID],[ReferenceServiceID],[State],[SyncState])
            SELECT 
	            0, 
	            (
		            SELECT [ReferenceTypeID]
		            FROM  [ReferenceType] rType
		            WHERE rType.ReferenceName IN ('Subsídios Sociais do INAS')
		            AND rType.ReferenceCategory IN ('Activist')
	            ),
	            rService.ReferenceServiceID, 0, 0 
            FROM  [ReferenceService] rService", false);




            //-----------------SERVIÇO SOCIAL-----------------------


            //Social/Atestado de Pobreza
            Sql(@"INSERT INTO [Reference]
            ([Value],[ReferenceTypeID],[ReferenceServiceID],[State],[SyncState])
            SELECT 
	            0, 
	            (
		            SELECT [ReferenceTypeID]
		            FROM  [ReferenceType] rType
		            WHERE rType.ReferenceName IN ('Atestado de Pobreza')
		            AND rtype.ReferenceOrder = 12
	            ),
	            rService.ReferenceServiceID, 0, 0 
            FROM  [ReferenceService] rService", false);
 

            //Social/Registo de Nascimento/Cédula
            Sql(@"INSERT INTO [Reference]
            ([Value],[ReferenceTypeID],[ReferenceServiceID],[State],[SyncState])
            SELECT 
	            0, 
	            (
		            SELECT [ReferenceTypeID]
		            FROM  [ReferenceType] rType
		            WHERE rType.ReferenceName IN ('Registo de Nascimento/Cédula')
		            AND rType.ReferenceCategory IN ('Social')
	            ),
	            rService.ReferenceServiceID, 0, 0 
            FROM  [ReferenceService] rService", false);


            //Social/Bilhete de Identidade (B.I)
            Sql(@"INSERT INTO [Reference]
            ([Value],[ReferenceTypeID],[ReferenceServiceID],[State],[SyncState])
            SELECT 
	            0, 
	            (
		            SELECT [ReferenceTypeID]
		            FROM  [ReferenceType] rType
		            WHERE rType.ReferenceName IN ('Bilhete de Identidade (B.I)')
		            AND rType.ReferenceCategory IN ('Social')
	            ),
	            rService.ReferenceServiceID, 0, 0 
            FROM  [ReferenceService] rService", false);


            //Social/Integração Escolar
            Sql(@"INSERT INTO [Reference]
            ([Value],[ReferenceTypeID],[ReferenceServiceID],[State],[SyncState])
            SELECT 
	            0, 
	            (
		            SELECT [ReferenceTypeID]
		            FROM  [ReferenceType] rType
		            WHERE rType.ReferenceName IN ('Integração Escolar')
		            AND rType.ReferenceCategory IN ('Social')
	            ),
	            rService.ReferenceServiceID, 0, 0 
            FROM  [ReferenceService] rService", false);


            //Social/Curso de Formação Vocacional
            Sql(@"INSERT INTO [Reference]
            ([Value],[ReferenceTypeID],[ReferenceServiceID],[State],[SyncState])
            SELECT 
	            0, 
	            (
		            SELECT [ReferenceTypeID]
		            FROM  [ReferenceType] rType
		            WHERE rType.ReferenceName IN ('Curso de Formação Vocacional')
		            AND rType.ReferenceCategory IN ('Social')
	            ),
	            rService.ReferenceServiceID, 0, 0 
            FROM  [ReferenceService] rService", false);


            //Social/Material Escolar
            Sql(@"INSERT INTO [Reference]
            ([Value],[ReferenceTypeID],[ReferenceServiceID],[State],[SyncState])
            SELECT 
	            0, 
	            (
		            SELECT [ReferenceTypeID]
		            FROM  [ReferenceType] rType
		            WHERE rType.ReferenceName IN ('Material Escolar')
		            AND rType.ReferenceCategory IN ('Social')
	            ),
	            rService.ReferenceServiceID, 0, 0 
            FROM  [ReferenceService] rService", false);


            //Social/Cesta Básica
            Sql(@"INSERT INTO [Reference]
            ([Value],[ReferenceTypeID],[ReferenceServiceID],[State],[SyncState])
            SELECT 
	            0, 
	            (
		            SELECT [ReferenceTypeID]
		            FROM  [ReferenceType] rType
		            WHERE rType.ReferenceName IN ('Cesta Básica')
		            AND rType.ReferenceCategory IN ('Social')
	            ),
	            rService.ReferenceServiceID, 0, 0 
            FROM  [ReferenceService] rService", false);


            //Social/Subsídios Sociais do INAS
            Sql(@"INSERT INTO [Reference]
            ([Value],[ReferenceTypeID],[ReferenceServiceID],[State],[SyncState])
            SELECT 
	            0, 
	            (
		            SELECT [ReferenceTypeID]
		            FROM  [ReferenceType] rType
		            WHERE rType.ReferenceName IN ('Subsídios Sociais do INAS')
		            AND rType.ReferenceCategory IN ('Social')
	            ),
	            rService.ReferenceServiceID, 0, 0 
            FROM  [ReferenceService] rService", false);
        }
        
        public override void Down()
        {
            //DELETE VALUES NAS OUTRAS REFERÊNCIAS DO SERVIÇO SOCIAL
            Sql(@"DELETE reference FROM  [ReferenceType] rType
                JOIN  [Reference] reference ON reference.ReferenceTypeID = rType.ReferenceTypeID 
                AND rType.ReferenceName IN ('Atestado de Pobreza','Registo de Nascimento/Cédula', 'Bilhete de Identidade (B.I)'
                , 'Integração Escolar', 'Curso de Formação Vocacional', 'Material Escolar', 'Cesta Básica', 'Subsídios Sociais do INAS') 
                AND rType.ReferenceCategory='Social' AND rType.ReferenceOrder>11", false);

            //DELETE VALUES NAS OUTRAS REFERÊNCIAS DO ACTIVISTA
            Sql(@"DELETE reference FROM  [ReferenceType] rType
                JOIN  [Reference] reference ON reference.ReferenceTypeID = rType.ReferenceTypeID 
                AND rType.ReferenceName IN ('Atestado de Pobreza','Registo de Nascimento/Cédula',  'Bilhete de Identidade (B.I)', 'Integração Escolar',
                'Curso de Formação Vocacional', 'Material Escolar', 'Cesta Básica', 'Subsídios Sociais do INAS') 
                AND rType.ReferenceCategory='Activist' AND rType.ReferenceOrder>29", false);

        }
    }
}
