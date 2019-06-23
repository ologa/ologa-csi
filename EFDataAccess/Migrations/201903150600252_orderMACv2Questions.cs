namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class orderMACv2Questions : DbMigration
    {
        public override void Up()
        {
            Sql(@" --
                 UPDATE [Question] SET [QuestionOrder] = 1 	where description='1 - Cumpre com o calendário de vacinação? (0-5 anos)'
	             UPDATE [Question] SET [QuestionOrder] = 2 	where description='2 - Bebe água tratada?'
	             UPDATE [Question] SET [QuestionOrder] = 3 	where description='3 - As últimas 3 vezes que a criança sentiu-se mal, foi atendida na unidade sanitária?'
	             UPDATE [Question] SET [QuestionOrder] = 4 	where description='4 - A criança esteve doente nas ultimas duas semanas?'
	             UPDATE [Question] SET [QuestionOrder] = 5 	where description='5 - Dorme dentro de uma rede mosquiteira tratada?'
	             UPDATE [Question] SET [QuestionOrder] = 6 	where description='6 - Tem acesso a uma latrina limpa, ou casa de banho e acesso a água para lavar as mãos?'
	             UPDATE [Question] SET [QuestionOrder] = 7 	where description='7 - Teve educação acerca do HIV nos últimos 2 meses? (10 - 17 anos)'
	             UPDATE [Question] SET [QuestionOrder] = 8 	where description='8 - Conhece o seu estado de HIV?'
	             UPDATE [Question] SET [QuestionOrder] = 9 	where description='9 - A criança está em TARV (Se for HIV+)?'
	             UPDATE [Question] SET [QuestionOrder] = 10	where description='10 - Teve educação ou acesso aos serviços de Saúde Sexual e Reproductiva? (10-17 anos)'
	             UPDATE [Question] SET [QuestionOrder] = 11	where description='11 - A criança come pelo menos duas refeições por dia?'
	             UPDATE [Question] SET [QuestionOrder] = 12	where description='12 - A criança variou os alimentos nos últimos 2 dias?'
	             UPDATE [Question] SET [QuestionOrder] = 13	where description='13 - A criança está inscrita no ensino pre-escolar, primário, secundário ou curso professional? (3-5 ou 6-17 anos)'
	             UPDATE [Question] SET [QuestionOrder] = 14	where description='14 - Foi à escola/escolinha todos os dias durante a semana anterior? (3-5 ou 6-17 anos)'
	             UPDATE [Question] SET [QuestionOrder] = 15	where description='15 - A criança tem uniforme escolar? (6-17 anos)'
	             UPDATE [Question] SET [QuestionOrder] = 16	where description='16 - A criança tem material escolar? (6-17 anos)'
	             UPDATE [Question] SET [QuestionOrder] = 17	where description='17 - A criança tem acompanhamento dos pais/cuidadores na vida escolar? (3-5 ou 6-17 anos)'
	             UPDATE [Question] SET [QuestionOrder] = 18	where description='18 - A criança tem um bom aproveitamento escolar? (3-5 ou 6-17 anos)'
	             UPDATE [Question] SET [QuestionOrder] = 19	where description='19 - A criança faz TPC e revisão das matérias? (6-17 anos)'
	             UPDATE [Question] SET [QuestionOrder] = 20	where description='20 - A criança passou de classe no ano passado? (6-17 anos)'
	             UPDATE [Question] SET [QuestionOrder] = 21	where description='21 - A criança é tratada de forma igual em relação as outras crianças da família?'
	             UPDATE [Question] SET [QuestionOrder] = 22	where description='22 - A criança teve educação sobre direitos e deveres da criança? (3-17 anos)'
	             UPDATE [Question] SET [QuestionOrder] = 23	where description='23 - A criança tem registo de nascimento?'
	             UPDATE [Question] SET [QuestionOrder] = 24	where description='24 - A criança foi ou é vítima de violência (psicóloga/física/sexual/negligência)?'
	             UPDATE [Question] SET [QuestionOrder] = 25	where description='25 - A criança respeita aos mais velhos (não desafia nem se revolta)? (5-17 anos)'
	             UPDATE [Question] SET [QuestionOrder] = 26	where description='26 - A criança participa de serviços religiosos ou grupos de suporte caso deseje? (5-17 anos)'
	             UPDATE [Question] SET [QuestionOrder] = 27	where description='27 - Tem um bom amigo ou fala com um adulto acerca dos problemas? (5-17 anos)'
	             UPDATE [Question] SET [QuestionOrder] = 28	where description='28 - A criança brinca com outras crianças?'
	             UPDATE [Question] SET [QuestionOrder] = 29	where description='29 - A criança está envolvida em alguma actividade onde consegue expressar a sua opinião (Família, clube escolar, CCPC, etc.)? (5-17 anos)'
	             UPDATE [Question] SET [QuestionOrder] = 30	where description='30 - O cuidador é capaz de dar exemplo de uma brincadeira que fez com a criança? (0-5 anos)'
	             UPDATE [Question] SET [QuestionOrder] = 31	where description='31 - Casa adequada, segura, seca e ventilada, com paredes e tecto fortes?'
	             UPDATE [Question] SET [QuestionOrder] = 32	where description='32 - A criança beneficia das actividades de geração de renda da família (Ex: Machamba, criação de animais)?'
	             UPDATE [Question] SET [QuestionOrder] = 33	where description='33 - O cuidador participa em algum grupo de poupança?'
              ");
        }

        public override void Down()
        {
        }
    }
}
