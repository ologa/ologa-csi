namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateReferenceTypesColumnsAndRowNumbers : DbMigration
    {
        public override void Up()
        {
            Sql(@"--ACTIVISTA
                UPDATE [ReferenceType] SET [ReferenceColumn] = 1 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Maternidade p/ Parto'
                UPDATE [ReferenceType] SET [ReferenceRow] = 1 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Maternidade p/ Parto'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 1 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'CPN'
                UPDATE [ReferenceType] SET [ReferenceRow] = 2 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'CPN'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 1 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'CPN Familiar'
                UPDATE [ReferenceType] SET [ReferenceRow] = 3 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'CPN Familiar'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 1 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Consulta Pós-Parto'
                UPDATE [ReferenceType] SET [ReferenceRow] = 4 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Consulta Pós-Parto'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 1 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'CCR'
                UPDATE [ReferenceType] SET [ReferenceRow] = 5 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'CCR'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 1 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'PTV'
                UPDATE [ReferenceType] SET [ReferenceRow] = 6 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'PTV'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 1 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Suspeito de Malnutrição'
                UPDATE [ReferenceType] SET [ReferenceRow] = 8 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Suspeito de Malnutrição'
                UPDATE [ReferenceType] SET[ReferenceColumn] = 1 WHERE[ReferenceCategory] = 'Activist' AND[ReferenceName] = 'Outros Motivos/Especificar cuidados/Serviço prestado ou a ser prestado'
                UPDATE [ReferenceType] SET [ReferenceRow] = 9 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Outros Motivos/Especificar cuidados/Serviço prestado ou a ser prestado'

                UPDATE [ReferenceType] SET [ReferenceColumn] = 2 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'ATS'
                UPDATE [ReferenceType] SET [ReferenceRow] = 1 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'ATS'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 2 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'ITS'
                UPDATE [ReferenceType] SET [ReferenceRow] = 2 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'ITS'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 2 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Pré-TARV/IO'
                UPDATE [ReferenceType] SET [ReferenceRow] = 3 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Pré-TARV/IO'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 2 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Testado HIV+'
                UPDATE [ReferenceType] SET [ReferenceRow] = 4 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Testado HIV+'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 2 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Abandono TARV'
                UPDATE [ReferenceType] SET [ReferenceRow] = 5 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Abandono TARV'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 2 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'PPE'
                UPDATE [ReferenceType] SET [ReferenceRow] = 6 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'PPE'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 2 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Circuncisao Masculina'
                UPDATE [ReferenceType] SET [ReferenceRow] = 7 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Circuncisao Masculina'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 2 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Banco de Socorro/Controle de triagem'
                UPDATE [ReferenceType] SET [ReferenceRow] = 8 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Banco de Socorro/Controle de triagem'

                UPDATE [ReferenceType] SET [ReferenceColumn] = 3 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Suspeito de TB'
                UPDATE [ReferenceType] SET [ReferenceRow] = 1 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Suspeito de TB'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 3 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Contacto de TB'
                UPDATE [ReferenceType] SET [ReferenceRow] = 2 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Contacto de TB'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 3 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Controlo de BK'
                UPDATE [ReferenceType] SET [ReferenceRow] = 3 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Controlo de BK'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 3 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Abandono de TTB'
                UPDATE [ReferenceType] SET [ReferenceRow] = 4 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Abandono de TTB'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 3 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Reacções do TTB'
                UPDATE [ReferenceType] SET [ReferenceRow] = 5 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Reacções do TTB'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 3 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Suspeito de Malária'
                UPDATE [ReferenceType] SET [ReferenceRow] = 6 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Suspeito de Malária'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 3 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Controlo da Dor'
                UPDATE [ReferenceType] SET [ReferenceRow] = 8 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Controlo da Dor'

                UPDATE [ReferenceType] SET [ReferenceColumn] = 4 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'OCB/Apoio Comunitário'
                UPDATE [ReferenceType] SET [ReferenceRow] = 1 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'OCB/Apoio Comunitário'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 4 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Educação'
                UPDATE [ReferenceType] SET [ReferenceRow] = 2 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Educação'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 4 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Acção Social'
                UPDATE [ReferenceType] SET [ReferenceRow] = 3 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Acção Social'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 4 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'GAVV'
                UPDATE [ReferenceType] SET [ReferenceRow] = 4 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'GAVV'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 4 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Apoio Psico-Social'
                UPDATE [ReferenceType] SET [ReferenceRow] = 5 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Apoio Psico-Social'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 4 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Posto Policial'
                UPDATE [ReferenceType] SET [ReferenceRow] = 6 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Posto Policial'

                --SAÚDE
                UPDATE [ReferenceType] SET [ReferenceColumn] = 1 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Maternidade p/ parto'
                UPDATE [ReferenceType] SET [ReferenceRow] = 1 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Maternidade p/ parto'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 1 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'CPN'
                UPDATE [ReferenceType] SET [ReferenceRow] = 2 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'CPN'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 1 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'CPN Familiar'
                UPDATE [ReferenceType] SET [ReferenceRow] = 3 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'CPN Familiar'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 1 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Consulta Pós-parto'
                UPDATE [ReferenceType] SET [ReferenceRow] = 4 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Consulta Pós-parto'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 1 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'PTV'
                UPDATE [ReferenceType] SET [ReferenceRow] = 5 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'PTV'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 1 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Outros atendimentos/Especifique'
                UPDATE [ReferenceType] SET [ReferenceRow] = 7 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Outros atendimentos/Especifique'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 1 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Referido para'
                UPDATE [ReferenceType] SET [ReferenceRow] = 8 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Referido para'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 1 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Observações'
                UPDATE [ReferenceType] SET [ReferenceRow] = 9 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Observações'

                UPDATE [ReferenceType] SET [ReferenceColumn] = 2 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'CCR'
                UPDATE [ReferenceType] SET [ReferenceRow] = 1 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'CCR'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 2 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'ATS'
                UPDATE [ReferenceType] SET [ReferenceRow] = 2 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'ATS'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 2 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'ITS'
                UPDATE [ReferenceType] SET [ReferenceRow] = 3 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'ITS'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 2 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Testado HIV+'
                UPDATE [ReferenceType] SET [ReferenceRow] = 4 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Testado HIV+'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 2 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Pré TARV/IO'
                UPDATE [ReferenceType] SET [ReferenceRow] = 5 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Pré TARV/IO'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 2 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'CD'
                UPDATE [ReferenceType] SET [ReferenceRow] = 6 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'CD'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 2 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Motivo de referência'
                UPDATE [ReferenceType] SET [ReferenceRow] = 8 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Motivo de referência'

                UPDATE [ReferenceType] SET [ReferenceColumn] = 3 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'TARV'
                UPDATE [ReferenceType] SET [ReferenceRow] = 1 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'TARV'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 3 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Sem TB'
                UPDATE [ReferenceType] SET [ReferenceRow] = 2 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Sem TB'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 3 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Profilaxia por contacto TB'
                UPDATE [ReferenceType] SET [ReferenceRow] = 3 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Profilaxia por contacto TB'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 3 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Tratamento de TB'
                UPDATE [ReferenceType] SET [ReferenceRow] = 4 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Tratamento de TB'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 3 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Doenças crónicas'
                UPDATE [ReferenceType] SET [ReferenceRow] = 5 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Doenças crónicas'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 3 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Controlo da Dor'
                UPDATE [ReferenceType] SET [ReferenceRow] = 6 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Controlo da Dor'

                UPDATE [ReferenceType] SET [ReferenceColumn] = 4 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Controlo de BK'
                UPDATE [ReferenceType] SET [ReferenceRow] = 1 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Controlo de BK'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 4 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Reacções do TTB'
                UPDATE [ReferenceType] SET [ReferenceRow] = 2 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Reacções do TTB'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 4 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'PPE'
                UPDATE [ReferenceType] SET [ReferenceRow] = 3 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'PPE'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 4 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Terapia Nutricional'
                UPDATE [ReferenceType] SET [ReferenceRow] = 4 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Terapia Nutricional'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 4 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'CCS'
                UPDATE [ReferenceType] SET [ReferenceRow] = 5 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'CCS'

                UPDATE [ReferenceType] SET [ReferenceColumn] = 5 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Mês'
                UPDATE [ReferenceType] SET [ReferenceRow] = 1 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Mês'


                --ACCAO SOCIAL
                UPDATE [ReferenceType] SET [ReferenceColumn] = 1 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'Subsídios de alimentos'
                UPDATE [ReferenceType] SET [ReferenceRow] = 1 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'Subsídios de alimentos'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 1 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'Habitação'
                UPDATE [ReferenceType] SET [ReferenceRow] = 2 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'Habitação'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 1 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'Outras redes/grupos/instituição de Apoio'
                UPDATE [ReferenceType] SET [ReferenceRow] = 4 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'Outras redes/grupos/instituição de Apoio'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 1 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'Especificar cuidado/serviço prestado ou a ser prestado'
                UPDATE [ReferenceType] SET [ReferenceRow] = 5 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'Especificar cuidado/serviço prestado ou a ser prestado'

                UPDATE [ReferenceType] SET [ReferenceColumn] = 2 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'Educação'
                UPDATE [ReferenceType] SET [ReferenceRow] = 1 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'Educação'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 2 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'Atestado de pobreza'
                UPDATE [ReferenceType] SET [ReferenceRow] = 2 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'Atestado de pobreza'

                UPDATE [ReferenceType] SET [ReferenceColumn] = 3 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'OCB'
                UPDATE [ReferenceType] SET [ReferenceRow] = 1 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'OCB'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 3 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'Acção Social produtiva'
                UPDATE [ReferenceType] SET [ReferenceRow] = 2 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'Acção Social produtiva'

                UPDATE [ReferenceType] SET [ReferenceColumn] = 4 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'GAVV'
                UPDATE [ReferenceType] SET [ReferenceRow] = 1 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'GAVV'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 4 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'Apoio monetário'
                UPDATE [ReferenceType] SET [ReferenceRow] = 2 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'Apoio monetário'
                UPDATE [ReferenceType] SET [ReferenceColumn] = 4 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'INAS'
                UPDATE [ReferenceType] SET [ReferenceRow] = 3 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'INAS'


                --OUTRAS OUTRAS PARA ACTIVISTA
                UPDATE [ReferenceType] SET [ReferenceColumn] = 1 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Atestado de Pobreza'
                UPDATE [ReferenceType] SET [ReferenceRow] = 10 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Atestado de Pobreza'


                UPDATE [ReferenceType] SET [ReferenceColumn] = 2 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Registo de Nascimento/Cédula'
                UPDATE [ReferenceType] SET [ReferenceRow] = 10 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Registo de Nascimento/Cédula'

                UPDATE [ReferenceType] SET [ReferenceColumn] = 3 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Bilhete de Identidade (B.I)'
                UPDATE [ReferenceType] SET [ReferenceRow] = 10 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Bilhete de Identidade (B.I)'

                UPDATE [ReferenceType] SET [ReferenceColumn] = 4 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Integração Escolar'
                UPDATE [ReferenceType] SET [ReferenceRow] = 10 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Integração Escolar'

                UPDATE [ReferenceType] SET [ReferenceColumn] = 1 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Curso de Formação Vocacional'
                UPDATE [ReferenceType] SET [ReferenceRow] = 11 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Curso de Formação Vocacional'

                UPDATE [ReferenceType] SET [ReferenceColumn] = 2 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Material Escolar'
                UPDATE [ReferenceType] SET [ReferenceRow] = 11 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Material Escolar'

                UPDATE [ReferenceType] SET [ReferenceColumn] = 3 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Cesta Básica'
                UPDATE [ReferenceType] SET [ReferenceRow] = 11 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Cesta Básica'

                UPDATE [ReferenceType] SET [ReferenceColumn] = 4 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Subsídios Sociais do INAS'
                UPDATE [ReferenceType] SET [ReferenceRow] = 11 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Subsídios Sociais do INAS'

                UPDATE [ReferenceType] SET [ReferenceColumn] = 1 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'SAAJ'
                UPDATE [ReferenceType] SET [ReferenceRow] = 12 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'SAAJ'

                UPDATE [ReferenceType] SET [ReferenceColumn] = 2 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Desnutrição'
                UPDATE [ReferenceType] SET [ReferenceRow] = 12 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Desnutrição'

                UPDATE [ReferenceType] SET [ReferenceColumn] = 3 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Atraso no Desenvolvimento'
                UPDATE [ReferenceType] SET [ReferenceRow] = 12 WHERE [ReferenceCategory] = 'Activist' AND [ReferenceName] = 'Atraso no Desenvolvimento'


                --OUTRAS OUTRAS SAÚDE
                UPDATE [ReferenceType] SET [ReferenceColumn] = 1 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'SAAJ'
                UPDATE [ReferenceType] SET [ReferenceRow] = 10 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'SAAJ'

                UPDATE [ReferenceType] SET [ReferenceColumn] = 2 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Desnutrição'
                UPDATE [ReferenceType] SET [ReferenceRow] = 10 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Desnutrição'

                UPDATE [ReferenceType] SET [ReferenceColumn] = 3 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Atraso no Desenvolvimento'
                UPDATE [ReferenceType] SET [ReferenceRow] = 10 WHERE [ReferenceCategory] = 'Health' AND [ReferenceName] = 'Atraso no Desenvolvimento'



                --OUTRAS OUTRAS PARA SOCIAL
                UPDATE [ReferenceType] SET [ReferenceColumn] = 1 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'Registo de Nascimento/Cédula'
                UPDATE [ReferenceType] SET [ReferenceRow] = 6 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'Registo de Nascimento/Cédula'

                UPDATE [ReferenceType] SET [ReferenceColumn] = 2 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'Bilhete de Identidade (B.I)'
                UPDATE [ReferenceType] SET [ReferenceRow] = 6 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'Bilhete de Identidade (B.I)'

                UPDATE [ReferenceType] SET [ReferenceColumn] = 3 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'Integração Escolar'
                UPDATE [ReferenceType] SET [ReferenceRow] = 6 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'Integração Escolar'

                UPDATE [ReferenceType] SET [ReferenceColumn] = 4 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'Curso de Formação Vocacional'
                UPDATE [ReferenceType] SET [ReferenceRow] = 6 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'Curso de Formação Vocacional'

                UPDATE [ReferenceType] SET [ReferenceColumn] = 1 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'Material Escolar'
                UPDATE [ReferenceType] SET [ReferenceRow] = 7 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'Material Escolar'

                UPDATE [ReferenceType] SET [ReferenceColumn] = 2 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'Cesta Básica'
                UPDATE [ReferenceType] SET [ReferenceRow] = 7 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'Cesta Básica'

                UPDATE [ReferenceType] SET [ReferenceColumn] = 3 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'Subsídios Sociais do INAS'
                UPDATE [ReferenceType] SET [ReferenceRow] = 7 WHERE [ReferenceCategory] = 'Social' AND [ReferenceName] = 'Subsídios Sociais do INAS'");
        }
        
        public override void Down()
        {
        }
    }
}
