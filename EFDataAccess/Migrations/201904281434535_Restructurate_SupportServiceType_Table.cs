namespace EFDataAccess.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    
    public partial class Restructurate_SupportServiceType_Table : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SupportServiceType", "SupportServiceOrderInDomainTypeCode", c => c.String());

            // Set SupportServiceOrderInDomain Column to NULL To Reorder In Correct Way
            Sql(@"UPDATE sst SET sst.SupportServiceOrderInDomain = NULL FROM SupportServiceType sst");

            int rowNumber = 0;
            int totalRows = 0;

            // ORDER NUMBERS IN DOMAIN LIST
            List<int> SupportServiceOrderInDomainByNumbers = new List<int>(new int[]
            {
	            1, 2, 3, 4, //Fort. económico
	            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, //Saúde
	            1, 2, 3, 4, 5, 6, 7, //Seroestado
	            1, 2, 3, //Habitação
	            1, 2, 3, 4, 5, //MUAC (6-59 meses)
	            1, 2, 3, //Alim. e Nutrição
	            1, 2, 3, 4, 5, 6, 7, 8, //Educação
	            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, //Protec. e Apoio legal
	            1, 2, 3, //Apoio Psico-Social
	            1, 2, 3, 4, //DPI (0-5 anos)
	            1, 2, 3, //Outros
            });

            // Set the ORDER NUMBERS IN DOMAIN from the List in Correct Way
            rowNumber = 1;
            totalRows = SupportServiceOrderInDomainByNumbers.Count;
            foreach (int orderInDomainByNumber in SupportServiceOrderInDomainByNumbers)
            {
	            if (rowNumber <= totalRows)
	            {
		            Sql(@"UPDATE sst SET sst.SupportServiceOrderInDomain = '" + orderInDomainByNumber + "' " +
		            @"FROM SupportServiceType sst 
		            INNER JOIN
		            (
			            SELECT sst.SupportServiceTypeID,
			            row_number() OVER(ORDER BY SupportServiceTypeID, DomainOrder ASC) as rowNumber
			            FROM SupportServiceType sst
			            WHERE sst.Tool = 'routine-visit'
		            ) sst2
		            ON sst.SupportServiceTypeID = sst2.SupportServiceTypeID AND sst2.rowNumber = " + rowNumber + "");
	            }
	            rowNumber++;
            }

            // DESCRIPTIONS LIST
            List<string> SupportServiceTypesDescriptions = new List<string>(new string[]
            {
	            "Sensibilização/ Mobilização para  Participação no grupo de poupança (Todos)",
	            "Incentivar para a realização de pequenos negócios (todos)",
	            "Mobilização e referencia para serviços sociais (INAS) (Todos)",
	            "Participa ou continua a participar  num grupo de poupança? Se sim coloque X para todos os membros da família (Todos)",
	            "Aconselhamento sobre Prevenção de  HIV ( beneficiários de 10 anos ou mais)",
	            "Sensibilização/Mobilização para  referência a testagem (ATS)-(Criança ou Adulto)",
	            "Sensibilização/Mobilização para iniciar o TARV (Criança ou Adulto)",
	            "Acompanhamento para aderência ao TARV (Continuidade  no tratamento e toma correcta da medicação)(Criança ou Adulto)",
	            "Sensibilização/Mobilização para cuidados HIV - Vida positiva) (Criança e/ou Adulto)",
	            "Sensibilização para participação em grupos de PVHS ( beneficiários de 10 anos ou mais)",
	            "Sensibilização/Mobilização para eliminar barreiras e facilitar acesso à testagem do HIV (Criança ou Adulto)",
	            "Sensibilização para revelação do seroestado (Criança ou Adulto)",
	            "Sensibilização/Mobilização sobre a importância do cumprimento do calendários de vacinação (Criança 0-5 anos)",
	            "Mobilização/ Referência  para outros serviços de saúde (fora de HIV) (Criança ou Adulto)",
	            "Sensibilização sobre higiene, água e  saneamento (Todos)",
	            "Aconselhamento sobre saúde sexual reprodutiva ( beneficiários de 10 anos ou mais)",
	            "Mobilização/referencia para obtenção e uso correto de redes mosquiteiras (Crianças e Mulheres Gravidas)",
	            "Rastreados para o HIV (0-17 anos) -> Teste não recomendado",
	            "Rastreados para o HIV (0-17 anos) -> Referência para testagem (ATS)",
	            "HIV-",
	            "HIV+ -> Em TARV",
	            "HIV+ -> Não TARV",
	            "Conhece mas não revelou",
	            "Não conhece",
	            "Sensibilização para reabilitação ou construção da casa (todos)",
	            "Mobilização/Sensibilização da comunidade para reabilitação ou construção de casa de beneficiários (todos)",
	            "Sensibilização sobre  cuidados com a casa (casa fresca e ventilada) (Todos)",
	            "Resultado do rastreio do MUAC: Verde ",
	            "Resultado do rastreio do MUAC: Amarelo",
	            "Resultado do rastreio do MUAC: Vermelho",
	            "Mobilizacao e referencia de casos de desnutrição para  o Programa de Reabilitação Nutricional (PRN) ",
	            "Sensibilizacao/ Acompanhamento e acampanhamento na comunidade para aderência a reabilitação nutricional (consultas e toma de suplementos)",
	            "Educação / Aconselhamento Nutricional (amamentação, alimentação complementar/Equilibrada da família) (Todos)",
	            "Demonstração culinária para família (apoio na preparação de alimentos)(Todos)",
	            "Mobilização/Demonstração de hortas caseiras (Todos)",
	            "Mobilização para Matrícula Escolar (Crianças)",
	            "Apoio no pagamento ou isenção de taxas escolares(Crianças e adolescente e jovens no secundário de 18 a 20 anos)",
	            "Reintegração escolar (Crianças)",
	            "Apoio/Sensibilização para apoio no TPC (Crianças)",
	            "Mobilização para apoio em uniforme e/ou material escolar(Crianças)",
	            "Mobilização para permanência na escola (Crianças)",
	            "Monitoria da educação (A criança continua a frequentar a escola)",
	            "Monitoria do progresso escolar  (passagem de classe a ser preenchido no final do  ano escolar) (Crianças)",
	            "Mobilização/Referência aos serviços de registo de nascimento(Criança e Adulto)",
	            "Registo de nascimento efectuado (comprovativo do registo)  (Criança ou adulto)",
	            "Apoio para obtenção de Atestado de pobreza   (Criança ou adulto)",
	            "Aconselhamento pós-violência (todo tipo de violência)   (Criança ou adulto)",
	            "Mobilização para acesso aos serviços saúde pós-violência   (Criança ou adulto)",
	            "Mobilizar/Referir para o gabinete de atendimento de vitima de violência(polícia ou outra entidade)  (Criança ou adulto)",
	            "Habitação alternativa em casos de ambiente propicio a violência (Criança)",
	            "Integração no grupo de adolescentes (clube, grupo de estudo…..)  (Adolescentes)",
	            "Sensibilização e aconselhamento  dos adolescentes para Prevenção HIV e violência (Adolescentes)",
	            "Sensibilização cuidador para prevenção de violência e riscos sexuais nas crianças ",
	            "Mobilização para a integração nos grupos de apoio (Igreja, mãe para mãe, etc)",
	            "Mobilização /Referencia para apoio psicossocial especializado (Criança ou adulto)",
	            "Escutar e confortar a criança no caso de abandono/luto/abuso/descriminação (Criança)",
	            "Monitoria dos marcos de desenvolvimento ",
	            "Mobilização/Referência Suspeita de atraso de desenvolvimento",
	            "Estimulação da criança de acordo com a idade",
	            "Fabrico de Brinquedos",
	            "Monitoria do Plano de Acção da família",
	            "Monitoria da Aderência ao tratamento (monitorar se cada pessoa HIV+ está a aderir ao tratamento)",
	            "Atribuição do Apoio Social de emergência",
            });

            // Set the DESCRIPTIONS from the List in Correct Way
            rowNumber = 1;
            totalRows = SupportServiceTypesDescriptions.Count;
            foreach (string description in SupportServiceTypesDescriptions)
            {
	            if (rowNumber <= totalRows)
	            {
		            Sql(@"UPDATE sst SET sst.Description = '" + description + "' " +
		            @"FROM SupportServiceType sst 
		            INNER JOIN
		            (
			            SELECT sst.SupportServiceTypeID,
			            row_number() OVER(ORDER BY SupportServiceTypeID, DomainOrder ASC) as rowNumber
			            FROM SupportServiceType sst
			            WHERE sst.Tool = 'routine-visit'
		            ) sst2
		            ON sst.SupportServiceTypeID = sst2.SupportServiceTypeID AND sst2.rowNumber = " + rowNumber + "");
	            }
	            rowNumber++;

            }

            // ORDER TYPE CODES IN DOMAIN LIST
            List<string> SupportServiceOrderInDomainByTypeCodes = new List<string>(new string[]
            {
	            "FE01", "FE02", "FE03", "FE04",
	            "SD01", "SD02", "SD03", "SD04", "SD05", "SD06", "SD07", "SD08", "SD09", "SD10", "SD11", "SD12", "SD13",
	            "HIV01", "HIV02", "HIV03", "HIV04", "HIV05", "HIV06", "HIV07",
	            "HAB01", "HAB02", "HAB03",
	            "MUAC01", "MUAC02", "MUAC03", "MUAC04", "MUAC05",
	            "AN01", "AN02", "AN03",
	            "ED01", "ED02", "ED03", "ED04", "ED05", "ED06", "ED07", "ED08",
	            "PAL01", "PAL02", "PAL03", "PAL04", "PAL05", "PAL06", "PAL07", "PAL08", "PAL09", "PAL10",
	            "APS01", "APS02", "APS03",
	            "DPI01", "DPI02", "DPI03", "DPI04",
	            "OTR01", "OTR02", "OTR03",
            });

            // Set the ORDER TYPE CODES IN DOMAIN from the List in Correct Way
            rowNumber = 1;
            totalRows = SupportServiceOrderInDomainByTypeCodes.Count;
            foreach (string orderInDomainByTypeCode in SupportServiceOrderInDomainByTypeCodes)
            {
	            if (rowNumber <= totalRows)
	            {
		            Sql(@"UPDATE sst SET sst.SupportServiceOrderInDomainTypeCode = '" + orderInDomainByTypeCode + "' " +
		            @"FROM SupportServiceType sst 
		            INNER JOIN
		            (
			            SELECT sst.SupportServiceTypeID,
			            row_number() OVER(ORDER BY SupportServiceTypeID, DomainOrder ASC) as rowNumber
			            FROM SupportServiceType sst
			            WHERE sst.Tool = 'routine-visit'
		            ) sst2
		            ON sst.SupportServiceTypeID = sst2.SupportServiceTypeID AND sst2.rowNumber = " + rowNumber + "");
	            }
	            rowNumber++;
            }
        }
        
        public override void Down()
        {
            DropColumn("dbo.SupportServiceType", "SupportServiceOrderInDomainTypeCode");
        }
    }
}
