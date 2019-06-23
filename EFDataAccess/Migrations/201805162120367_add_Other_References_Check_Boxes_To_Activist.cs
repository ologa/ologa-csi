namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_Other_References_Check_Boxes_To_Activist : DbMigration
    {
        public override void Up()
        {
            Sql(@"INSERT INTO  [ReferenceType] ([ReferenceName],[ReferenceCategory]
                ,[ReferenceOrder],[FieldType],[OriginReferenceID])
                VALUES('Atestado de Pobreza', 'Activist',30,'CheckBox',NULL),
                ('Registo de Nascimento/Cédula', 'Activist',31,'CheckBox',NULL),
                ('Bilhete de Identidade (B.I)', 'Activist',32,'CheckBox',NULL),
                ('Integração Escolar', 'Activist',33,'CheckBox',NULL),
                ('Curso de Formação Vocacional', 'Activist',34,'CheckBox',NULL),
                ('Material Escolar', 'Activist',35,'CheckBox',NULL),
                ('Cesta Básica', 'Activist',36,'CheckBox',NULL),
                ('Subsídios Sociais do INAS', 'Activist',37,'CheckBox',NULL)", false);
        }

        public override void Down()
        {

            Sql(@"DELETE FROM  [ReferenceType] WHERE ReferenceName in 
                ('Atestado de Pobreza','Registo de Nascimento/Cédula',  'Bilhete de Identidade (B.I)', 'Integração Escolar',
                'Curso de Formação Vocacional', 'Material Escolar', 'Cesta Básica', 'Subsídios Sociais do INAS') 
                AND ReferenceCategory='Activist' AND ReferenceOrder>29", false);
        }
    }
}
