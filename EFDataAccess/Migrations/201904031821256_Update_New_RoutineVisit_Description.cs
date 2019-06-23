namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_New_RoutineVisit_Description : DbMigration
    {
        public override void Up()
        {
            Sql(@"UPDATE sst
                SET sst.Description = 
                (CASE WHEN sst.SupportServiceOrderInDomain = 89 THEN 'Sensibilização/ Mobilização para  Participação no grupo de poupança (Todos)' 
                WHEN sst.SupportServiceOrderInDomain = 90 THEN 'Incentivar para a realização de pequenos negócios (todos)'
                WHEN sst.SupportServiceOrderInDomain = 91 THEN 'Mobilização e referencia para serviços sociais (INAS) (Todos)'
                WHEN sst.SupportServiceOrderInDomain = 92 THEN 'Participa ou continua a participar  num grupo de poupança? Se sim coloque X para todos os membros da família (Todos)'
                WHEN sst.SupportServiceOrderInDomain = 93 THEN 'Aconselhamento sobre Prevenção de  HIV ( beneficiários de 10 anos ou mais)'
                WHEN sst.SupportServiceOrderInDomain = 94 THEN 'Sensibilização/Mobilização para  referência a testagem (ATS)-(Criança ou Adulto)'
                WHEN sst.SupportServiceOrderInDomain = 95 THEN 'Sensibilização/Mobilização para iniciar o TARV (Criança ou Adulto)'
                WHEN sst.SupportServiceOrderInDomain = 96 THEN 'Acompanhamento para aderência ao TARV (Continuidade  no tratamento e toma correcta da medicação)(Criança ou Adulto)'
                WHEN sst.SupportServiceOrderInDomain = 97 THEN 'Sensibilização/Mobilização para cuidados HIV - Vida positiva) (Criança e/ou Adulto)'
                WHEN sst.SupportServiceOrderInDomain = 98 THEN 'Sensibilização para participação em grupos de PVHS ( beneficiários de 10 anos ou mais)'
                WHEN sst.SupportServiceOrderInDomain = 99 THEN 'Sensibilização/Mobilização para eliminar barreiras e facilitar acesso à testagem do HIV (Criança ou Adulto)'
                WHEN sst.SupportServiceOrderInDomain = 100 THEN 'Sensibilização para revelação do seroestado (Criança ou Adulto)'
                WHEN sst.SupportServiceOrderInDomain = 101 THEN 'Sensibilização/Mobilização sobre a importância do cumprimento do calendários de vacinação (Criança 0-5 anos)'
                WHEN sst.SupportServiceOrderInDomain = 102 THEN 'Mobilização/ Referência  para outros serviços de saúde (fora de HIV) (Criança ou Adulto)'
                WHEN sst.SupportServiceOrderInDomain = 103 THEN 'Sensibilização sobre higiene, água e  saneamento (Todos)'
                WHEN sst.SupportServiceOrderInDomain = 104 THEN 'Aconselhamento sobre saúde sexual reprodutiva ( beneficiários de 10 anos ou mais)'
                WHEN sst.SupportServiceOrderInDomain = 105 THEN 'Mobilização/referencia para obtenção e uso correto de redes mosquiteiras (Crianças e Mulheres Gravidas)'
                WHEN sst.SupportServiceOrderInDomain = 106 THEN 'Rastreados para o HIV (0-17 anos) -> Teste não recomendado'
                WHEN sst.SupportServiceOrderInDomain = 107 THEN 'Rastreados para o HIV (0-17 anos) -> Referência para testagem (ATS)'
                WHEN sst.SupportServiceOrderInDomain = 108 THEN 'HIV-'
                WHEN sst.SupportServiceOrderInDomain = 109 THEN 'HIV+ -> Em TARV'
                WHEN sst.SupportServiceOrderInDomain = 110 THEN 'HIV+ -> Não TARV'
                WHEN sst.SupportServiceOrderInDomain = 111 THEN 'Conhece mas não revelou'
                WHEN sst.SupportServiceOrderInDomain = 112 THEN 'Não conhece'
                WHEN sst.SupportServiceOrderInDomain = 113 THEN 'Sensibilização para reabilitação ou construção da casa (todos)'
                WHEN sst.SupportServiceOrderInDomain = 114 THEN 'Mobilização/Sensibilização da comunidade para reabilitação ou construção de casa de beneficiários (todos)'
                WHEN sst.SupportServiceOrderInDomain = 115 THEN 'Sensibilização sobre  cuidados com a casa (casa fresca e ventilada) (Todos)'
                WHEN sst.SupportServiceOrderInDomain = 116 THEN 'Resultado do rastreio do MUAC: Verde '
                WHEN sst.SupportServiceOrderInDomain = 117 THEN 'Resultado do rastreio do MUAC: Amarelo'
                WHEN sst.SupportServiceOrderInDomain = 118 THEN 'Resultado do rastreio do MUAC: Vermelho'
                WHEN sst.SupportServiceOrderInDomain = 119 THEN 'Mobilizacao e referencia de casos de desnutrição para  o Programa de Reabilitação Nutricional (PRN) '
                WHEN sst.SupportServiceOrderInDomain = 120 THEN 'Sensibilizacao/ Acompanhamento e acampanhamento na comunidade para aderência a reabilitação nutricional (consultas e toma de suplementos)'
                WHEN sst.SupportServiceOrderInDomain = 121 THEN 'Educação / Aconselhamento Nutricional (amamentação, alimentação complementar/Equilibrada da família) (Todos)'
                WHEN sst.SupportServiceOrderInDomain = 122 THEN 'Demonstração culinária para família (apoio na preparação de alimentos)(Todos)'
                WHEN sst.SupportServiceOrderInDomain = 123 THEN 'Mobilização/Demonstração de hortas caseiras (Todos)'
                WHEN sst.SupportServiceOrderInDomain = 124 THEN 'Mobilização para Matrícula Escolar (Crianças)'
                WHEN sst.SupportServiceOrderInDomain = 125 THEN 'Apoio no pagamento ou isenção de taxas escolares(Crianças e adolescente e jovens no secundário de 18 a 20 anos)'
                WHEN sst.SupportServiceOrderInDomain = 126 THEN 'Reintegração escolar (Crianças)'
                WHEN sst.SupportServiceOrderInDomain = 127 THEN 'Apoio/Sensibilização para apoio no TPC (Crianças)'
                WHEN sst.SupportServiceOrderInDomain = 128 THEN 'Mobilização para apoio em uniforme e/ou material escolar(Crianças)'
                WHEN sst.SupportServiceOrderInDomain = 129 THEN 'Mobilização para permanência na escola (Crianças)'
                WHEN sst.SupportServiceOrderInDomain = 130 THEN 'Monitoria da educação (A criança continua a frequentar a escola)'
                WHEN sst.SupportServiceOrderInDomain = 131 THEN 'Monitoria do progresso escolar  (passagem de classe a ser preenchido no final do  ano escolar) (Crianças)'
                WHEN sst.SupportServiceOrderInDomain = 132 THEN 'Mobilização/Referência aos serviços de registo de nascimento(Criança e Adulto)'
                WHEN sst.SupportServiceOrderInDomain = 133 THEN 'Registo de nascimento efectuado (comprovativo do registo)  (Criança ou adulto)'
                WHEN sst.SupportServiceOrderInDomain = 134 THEN 'Apoio para obtenção de Atestado de pobreza   (Criança ou adulto)'
                WHEN sst.SupportServiceOrderInDomain = 135 THEN 'Aconselhamento pós-violência (todo tipo de violência)   (Criança ou adulto)'
                WHEN sst.SupportServiceOrderInDomain = 136 THEN 'Mobilização para acesso aos serviços saúde pós-violência   (Criança ou adulto)'
                WHEN sst.SupportServiceOrderInDomain = 137 THEN 'Mobilizar/Referir para o gabinete de atendimento de vitima de violência(polícia ou outra entidade)  (Criança ou adulto)'
                WHEN sst.SupportServiceOrderInDomain = 138 THEN 'Habitação alternativa em casos de ambiente propicio a violência (Criança)'
                WHEN sst.SupportServiceOrderInDomain = 139 THEN 'Integração no grupo de adolescentes (clube, grupo de estudo…..)  (Adolescentes)'
                WHEN sst.SupportServiceOrderInDomain = 140 THEN 'Sensibilização e aconselhamento  dos adolescentes para Prevenção HIV e violência (Adolescentes)'
                WHEN sst.SupportServiceOrderInDomain = 141 THEN 'Sensibilização cuidador para prevenção de violência e riscos sexuais nas crianças '
                WHEN sst.SupportServiceOrderInDomain = 142 THEN 'Mobilização para a integração nos grupos de apoio (Igreja, mãe para mãe, etc)'
                WHEN sst.SupportServiceOrderInDomain = 143 THEN 'Mobilização /Referencia para apoio psicossocial especializado (Criança ou adulto)'
                WHEN sst.SupportServiceOrderInDomain = 144 THEN 'Escutar e confortar a criança no caso de abandono/luto/abuso/descriminação (Criança)'
                WHEN sst.SupportServiceOrderInDomain = 145 THEN 'Monitoria dos marcos de desenvolvimento '
                WHEN sst.SupportServiceOrderInDomain = 146 THEN 'Mobilização/Referência Suspeita de atraso de desenvolvimento'
                WHEN sst.SupportServiceOrderInDomain = 147 THEN 'Estimulação da criança de acordo com a idade'
                WHEN sst.SupportServiceOrderInDomain = 148 THEN 'Fabrico de Brinquedos'
                WHEN sst.SupportServiceOrderInDomain = 149 THEN 'Monitoria do Plano de Acção da família'
                WHEN sst.SupportServiceOrderInDomain = 150 THEN 'Monitoria da Aderência ao tratamento (monitorar se cada pessoa HIV+ está a aderir ao tratamento)'
                WHEN sst.SupportServiceOrderInDomain = 151 THEN 'Atribuição do Apoio Social de emergência'
                ELSE  sst.Description
                END)
                FROM [CSI_PROD].[dbo].[SupportServiceType] sst
                WHERE sst.Tool = 'routine-visit'");
        }
        
        public override void Down()
        {
        }
    }
}
