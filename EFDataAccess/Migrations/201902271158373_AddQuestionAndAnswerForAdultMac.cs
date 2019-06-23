namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddQuestionAndAnswerForAdultMac : DbMigration
    {
        public override void Up()
        {

            //Adding  Question for MDC
            Sql(@"
                     
                     insert into Question(Description, Code, Goal, ExcludeFromReport, QuestionOrder, QuestionVersion, DomainID, FileID)
                     values
                     ('1  - Bebe água tratada?', '8A', 'Goal to be set', 0, 1, 3, 3,                                                                        (select [FileID] from [File] where [FileName]='Mdc_1')),
                     ('2  - Esteve doente nas ultimas duas semanas ?', '8B', 'Goal to be set', 0, 2, 3, 3,                                                  (select [FileID] from [File] where [FileName]='Mdc_2')),
                     ('3  - Dorme dentro de uma rede mosquiteira tratada?', '8C', 'Goal to be set', 0, 3, 3, 3,                                             (select [FileID] from [File] where [FileName]='Mdc_3')),
                     ('4  - tem acesso a uma latrina limpa, ou casa de banho e acesso à água para lavar as mãos ?', '8D', 'Goal to be set', 0, 4, 3, 3,     (select [FileID] from [File] where [FileName]='Mdc_4')),
                     ('5  - Teve educação acerca do HIV?', '8E', 'Goal to be set', 0, 5, 3, 3,                                                              (select [FileID] from [File] where [FileName]='Mdc_5')),
                     ('6  - Conhece o seu estado de HIV?', '8F', 'Goal to be set', 0, 6, 3, 3,                                                              (select [FileID] from [File] where [FileName]='Mdc_6')),
                     ('7  - Está em TARV? (Se for HIV+)', '8G', 'Goal to be set', 0, 7, 3, 3,                                                               (select [FileID] from [File] where [FileName]='Mdc_7')),
                     ('8  - Recebe apoio de uma Mâe Mentora? (Grávida ou nova Mâe HIV+)', '8H', 'Goal to be set', 0, 8, 3, 3,                               (select [FileID] from [File] where [FileName]='Mdc_8')),
                     ('9  - Teve informação ou acesso ãos serviços de saúde sexual e reprodutiva?', '8I', 'Goal to be set', 0, 9, 3, 3,                     (select [FileID] from [File] where [FileName]='Mdc_9')),
                     ('10 - Come pelo menos duas refeiçoes por dia?', '9A', 'Goal to be set', 0, 10, 3, 1,                                                  (select [FileID] from [File] where [FileName]='Mdc_10')),
                     ('11 - tem registo de nascimento?', '10A', 'Goal to be set', 0, 11, 3, 4,                                                              (select [FileID] from [File] where [FileName]='Mdc_11')),
                     ('12 - Foi ou é vítima de violência?', '10B', 'Goal to be set', 0, 12, 3, 4,                                                           (select [FileID] from [File] where [FileName]='Mdc_12')),
                     ('13 - Tem um bom amigo com quem fala sobre seus problemas?', '11A', 'Goal to be set', 0, 13, 3, 5,                                    (select [FileID] from [File] where [FileName]='Mdc_13')),
                     ('14 - Participa num grupo de apoio (Mãe para Mãe, GAAC)?', '11B', 'Goal to be set', 0, 14, 3, 5,                                      (select [FileID] from [File] where [FileName]='Mdc_14')),
                     ('15 - Casa adequada, segura, seca e ventilada, com paredes e tecto fortes?', '12A', 'Goal to be set', 0, 15, 3, 7,                    (select [FileID] from [File] where [FileName]='Mdc_15')),
                     ('16 - Possui ou realiza alguma actividade de geracao de renda (Machamba, criação de animais)?', '13A', 'Goal to be set', 0, 16, 3, 6, (select [FileID] from [File] where [FileName]='Mdc_16')),
                     ('17 - participa em algum grupo de poupança?', '13B', 'Goal to be set', 0, 17, 3, 6,                                                   (select [FileID] from [File] where [FileName]='Mdc_17'))
                    
              ");


            //Adding  Answers for MDC
            Sql(@"
                     insert into Answer(Description, QuestionID, ScoreID, ExcludeFromReport, FileID)
                     values
                     ('N/A',      (select QuestionID from question where QuestionOrder=1 and QuestionVersion=3), 1, 0, 153),
                     ('Nao',      (select QuestionID from question where QuestionOrder=1 and QuestionVersion=3), 2, 0, 154),
                     ('Talvez',   (select QuestionID from question where QuestionOrder=1 and QuestionVersion=3), 3, 0, 155),
                     ('Sim',      (select QuestionID from question where QuestionOrder=1 and QuestionVersion=3), 4, 0, 156), 
                                  
                     ('N/A',      (select QuestionID from question where QuestionOrder=2 and QuestionVersion=3), 1, 0, 153),
                     ('Nao',      (select QuestionID from question where QuestionOrder=2 and QuestionVersion=3), 2, 0, 154),
                     ('Talvez',   (select QuestionID from question where QuestionOrder=2 and QuestionVersion=3), 3, 0, 155),
                     ('Sim',      (select QuestionID from question where QuestionOrder=2 and QuestionVersion=3), 4, 0, 156), 
                                  
                     ('N/A',      (select QuestionID from question where QuestionOrder=3 and QuestionVersion=3), 1, 0, 153),
                     ('Nao',      (select QuestionID from question where QuestionOrder=3 and QuestionVersion=3), 2, 0, 154),
                     ('Talvez',   (select QuestionID from question where QuestionOrder=3 and QuestionVersion=3), 3, 0, 155),
                     ('Sim',      (select QuestionID from question where QuestionOrder=3 and QuestionVersion=3), 4, 0, 156), 
                                  
                     ('N/A',      (select QuestionID from question where QuestionOrder=4 and QuestionVersion=3), 1, 0, 153),
                     ('Nao',      (select QuestionID from question where QuestionOrder=4 and QuestionVersion=3), 2, 0, 154),
                     ('Talvez',   (select QuestionID from question where QuestionOrder=4 and QuestionVersion=3), 3, 0, 155),
                     ('Sim',      (select QuestionID from question where QuestionOrder=4 and QuestionVersion=3), 4, 0, 156),
                                  
                     ('N/A',      (select QuestionID from question where QuestionOrder=5 and QuestionVersion=3), 1, 0, 153),
                     ('Nao',      (select QuestionID from question where QuestionOrder=5 and QuestionVersion=3), 2, 0, 154),
                     ('Talvez',   (select QuestionID from question where QuestionOrder=5 and QuestionVersion=3), 3, 0, 155),
                     ('Sim',      (select QuestionID from question where QuestionOrder=5 and QuestionVersion=3), 4, 0, 156), 
                                  
                     ('N/A',      (select QuestionID from question where QuestionOrder=6 and QuestionVersion=3), 1, 0, 153),
                     ('Nao',      (select QuestionID from question where QuestionOrder=6 and QuestionVersion=3), 2, 0, 154),
                     ('Talvez',   (select QuestionID from question where QuestionOrder=6 and QuestionVersion=3), 3, 0, 155),
                     ('Sim',      (select QuestionID from question where QuestionOrder=6 and QuestionVersion=3), 4, 0, 156), 
                                  
                     ('N/A',      (select QuestionID from question where QuestionOrder=7 and QuestionVersion=3), 1, 0, 153),
                     ('Nao',      (select QuestionID from question where QuestionOrder=7 and QuestionVersion=3), 2, 0, 154),
                     ('Talvez',   (select QuestionID from question where QuestionOrder=7 and QuestionVersion=3), 3, 0, 155),
                     ('Sim',      (select QuestionID from question where QuestionOrder=7 and QuestionVersion=3), 4, 0, 156), 
                                  
                     ('N/A',      (select QuestionID from question where QuestionOrder=8 and QuestionVersion=3), 1, 0, 153),
                     ('Nao',      (select QuestionID from question where QuestionOrder=8 and QuestionVersion=3), 2, 0, 154),
                     ('Talvez',   (select QuestionID from question where QuestionOrder=8 and QuestionVersion=3), 3, 0, 155),
                     ('Sim',      (select QuestionID from question where QuestionOrder=8 and QuestionVersion=3), 4, 0, 156), 
                                  
                     ('N/A',      (select QuestionID from question where QuestionOrder=9 and QuestionVersion=3), 1, 0, 153),
                     ('Nao',      (select QuestionID from question where QuestionOrder=9 and QuestionVersion=3), 2, 0, 154),
                     ('Talvez',   (select QuestionID from question where QuestionOrder=9 and QuestionVersion=3), 3, 0, 155),
                     ('Sim',      (select QuestionID from question where QuestionOrder=9 and QuestionVersion=3), 4, 0, 156), 
                                  
                     ('N/A',      (select QuestionID from question where QuestionOrder=10 and QuestionVersion=3), 1, 0, 153),
                     ('Nao',      (select QuestionID from question where QuestionOrder=10 and QuestionVersion=3), 2, 0, 154),
                     ('Talvez',   (select QuestionID from question where QuestionOrder=10 and QuestionVersion=3), 3, 0, 155),
                     ('Sim',      (select QuestionID from question where QuestionOrder=10 and QuestionVersion=3), 4, 0, 156), 
                                  
                     ('N/A',      (select QuestionID from question where QuestionOrder=11 and QuestionVersion=3), 1, 0, 153),
                     ('Nao',      (select QuestionID from question where QuestionOrder=11 and QuestionVersion=3), 2, 0, 154),
                     ('Talvez',   (select QuestionID from question where QuestionOrder=11 and QuestionVersion=3), 3, 0, 155),
                     ('Sim',      (select QuestionID from question where QuestionOrder=11 and QuestionVersion=3), 4, 0, 156), 
                                  
                     ('N/A',      (select QuestionID from question where QuestionOrder=12 and QuestionVersion=3), 1, 0, 153),
                     ('Nao',      (select QuestionID from question where QuestionOrder=12 and QuestionVersion=3), 2, 0, 154),
                     ('Talvez',   (select QuestionID from question where QuestionOrder=12 and QuestionVersion=3), 3, 0, 155),
                     ('Sim',      (select QuestionID from question where QuestionOrder=12 and QuestionVersion=3), 4, 0, 156), 
                                  
                     ('N/A',      (select QuestionID from question where QuestionOrder=13 and QuestionVersion=3), 1, 0, 153),
                     ('Nao',      (select QuestionID from question where QuestionOrder=13 and QuestionVersion=3), 2, 0, 154),
                     ('Talvez',   (select QuestionID from question where QuestionOrder=13 and QuestionVersion=3), 3, 0, 155),
                     ('Sim',      (select QuestionID from question where QuestionOrder=13 and QuestionVersion=3), 4, 0, 156), 
                                  
                     ('N/A',      (select QuestionID from question where QuestionOrder=14 and QuestionVersion=3), 1, 0, 153),
                     ('Nao',      (select QuestionID from question where QuestionOrder=14 and QuestionVersion=3), 2, 0, 154),
                     ('Talvez',   (select QuestionID from question where QuestionOrder=14 and QuestionVersion=3), 3, 0, 155),
                     ('Sim',      (select QuestionID from question where QuestionOrder=14 and QuestionVersion=3), 4, 0, 156), 
                                  
                     ('N/A',      (select QuestionID from question where QuestionOrder=15 and QuestionVersion=3), 1, 0, 153),
                     ('Nao',      (select QuestionID from question where QuestionOrder=15 and QuestionVersion=3), 2, 0, 154),
                     ('Talvez',   (select QuestionID from question where QuestionOrder=15 and QuestionVersion=3), 3, 0, 155),
                     ('Sim',      (select QuestionID from question where QuestionOrder=15 and QuestionVersion=3), 4, 0, 156), 
                                  
                     ('N/A',      (select QuestionID from question where QuestionOrder=16 and QuestionVersion=3), 1, 0, 153),
                     ('Nao',      (select QuestionID from question where QuestionOrder=16 and QuestionVersion=3), 2, 0, 154),
                     ('Talvez',   (select QuestionID from question where QuestionOrder=16 and QuestionVersion=3), 3, 0, 155),
                     ('Sim',      (select QuestionID from question where QuestionOrder=16 and QuestionVersion=3), 4, 0, 156), 
                                  
                     ('N/A',      (select QuestionID from question where QuestionOrder=17 and QuestionVersion=3), 1, 0, 153),
                     ('Nao',      (select QuestionID from question where QuestionOrder=17 and QuestionVersion=3), 2, 0, 154),
                     ('Talvez',   (select QuestionID from question where QuestionOrder=17 and QuestionVersion=3), 3, 0, 155),
                     ('Sim',      (select QuestionID from question where QuestionOrder=17 and QuestionVersion=3), 4, 0, 156)

                     SET Identity_insert Question Off
                    
                    ");

        }

        public override void Down()
        {
            Sql(@"
                    DELETE Answer WHERE AnswerID >263
                    DELETE Question WHERE QuestionID >79
              ");
        }
    }
}
