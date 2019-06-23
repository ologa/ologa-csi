namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Delete_Poverty_Proof_from_Others_in_Social_ReferenceService : DbMigration
    {
        public override void Up()
        {
            //DELETE DAS REFERENCIAS COM ATESTADO DE POBREZA NAS OUTRAS REFERÊNCIAS DO SERVIÇO SOCIAL
            Sql(@"DELETE r
                FROM [Reference] r
                JOIN [ReferenceType] rtype ON rtype.ReferenceTypeID = r.ReferenceTypeID
                AND rtype.ReferenceName = 'Atestado de Pobreza' AND rtype.ReferenceOrder = 12", false);


            //DELETE DO ATESTADO DE POBREZA NAS OUTRAS REFERÊNCIAS DO SERVIÇO SOCIAL
            Sql(@"DELETE rtype
            FROM  [ReferenceType] rtype
            WHERE rtype.ReferenceName = 'Atestado de Pobreza' AND rtype.ReferenceOrder = 12", false);


            //UPDATE DO ORDER ANTIGO MENOS 1 ("- 1")
            Sql(@"UPDATE r
            SET r.ReferenceOrder = r.ReferenceOrder - 1
            FROM [ReferenceType] r
            WHERE r.ReferenceName IN ('Registo de Nascimento/Cédula','Bilhete de Identidade (B.I)','Integração Escolar','Curso de Formação Vocacional','Material Escolar','Cesta Básica','Subsídios Sociais do INAS')
            AND r.ReferenceCategory = 'Social'", false);
        }

        public override void Down()
        {

            //UPDATE DO ORDER ANTIGO MAIS 1 ("+ 1")
            Sql(@"UPDATE r
            SET r.ReferenceOrder = r.ReferenceOrder + 1
            FROM [ReferenceType] r
            WHERE r.ReferenceName IN ('Registo de Nascimento/Cédula','Bilhete de Identidade (B.I)','Integração Escolar','Curso de Formação Vocacional','Material Escolar','Cesta Básica','Subsídios Sociais do INAS')
            AND r.ReferenceCategory = 'Social'", false);

            //INSERIR ATESTADO DE POBREZA NAS OUTRAS REFERÊNCIAS DO SERVIÇO SOCIAL
            Sql(@"INSERT INTO  [ReferenceType] ([ReferenceName],[ReferenceCategory]
                ,[ReferenceOrder],[FieldType],[OriginReferenceID])
                VALUES('Atestado de Pobreza', 'Social',12,'CheckBox',NULL)", false);


            //ATRIBUIR ATESTADO DE POBREZA COM VALOR ZERO (0), PARA AS GUIAS DE REFERÊNCIAS QUE AINDA NÃO TINHAM
            Sql(@"INSERT INTO  [Reference]
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
        }
    }
}
