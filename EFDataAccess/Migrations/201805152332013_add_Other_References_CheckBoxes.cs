namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_Other_References_CheckBoxes : DbMigration
    {
        public override void Up()
        {
            Sql(@"INSERT INTO  [ReferenceType] ([ReferenceName],[ReferenceCategory]
                ,[ReferenceOrder],[FieldType],[OriginReferenceID])
                VALUES('Atestado de Pobreza', 'Social',12,'CheckBox',NULL),
                ('Registo de Nascimento/Cédula', 'Social',13,'CheckBox',NULL),
                ('Bilhete de Identidade (B.I)', 'Social',14,'CheckBox',NULL),
                ('Integração Escolar', 'Social',15,'CheckBox',NULL),
                ('Curso de Formação Vocacional', 'Social',16,'CheckBox',NULL),
                ('Material Escolar', 'Social',17,'CheckBox',NULL),
                ('Cesta Básica', 'Social',18,'CheckBox',NULL),
                ('Subsídios Sociais do INAS', 'Social',19,'CheckBox',NULL)", false);
        }
        
        public override void Down()
        {
            Sql(@"DELETE FROM  [ReferenceType] WHERE ReferenceName in 
                ('Atestado de Pobreza','Registo de Nascimento/Cédula',  'Bilhete de Identidade (B.I)', 'Integração Escolar',
                'Curso de Formação Vocacional', 'Material Escolar', 'Cesta Básica', 'Subsídios Sociais do INAS') 
                AND ReferenceCategory='Social' AND ReferenceOrder>11", false);
        }
    }
}
