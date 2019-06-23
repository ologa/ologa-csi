namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTypeDescriptionAndPopulateSupportServiceType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SupportServiceType", "TypeDescription", c => c.String());

            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Sensibilização/ Mobilização para Participação no grupo de poupança (Todos)', 'FE', 'Fort. económico', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Incentivar para a realização de pequenos negócios (todos)', 'FE', 'Fort. económico', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Mobilização e referencia para serviços sociais (INAS)', 'FE', 'Fort. económico', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Participa ou continua a participar num grupo de poupança? Se sim coloque X para todos os membros da família', 'FE', 'Fort. económico', 'routine-visit')");

            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Aconselhamento sobre Prevenção de HIV ( beneficiários de 10 anos ou mais)', 'SD', 'Saúde', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Sensibilização/Mobilização para referência a testagem (ATS)-(Criança ou Adulto)', 'SD', 'Saúde', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Sensibilização/Mobilização para iniciar o TARV (Criança ou Adulto)', 'SD', 'Saúde', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Acompanhamento para aderência ao TARV (Continuidade no tratamento e toma correcta da medicação)(Criança ou Adulto)', 'SD', 'Saúde', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Sensibilização/Mobilização para cuidados HIV - Vida positiva) (Criança e/ou Adulto)', 'SD', 'Saúde', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Sensibilização para participação em grupos de PVHS ( beneficiários de 10 anos ou mais)', 'SD', 'Saúde', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Sensibilização/Mobilização para eliminar barreiras e facilitar acesso à testagem do HIV (Criança ou Adulto)', 'SD', 'Saúde', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Sensibilização para revelação do seroestado (Criança ou Adulto)', 'SD', 'Saúde', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Sensibilização/Mobilização sobre a importância do cumprimento do calendários de vacinação (Criança 0-5 anos)', 'SD', 'Saúde', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Mobilização/ Referência  para outros serviços de saúde (fora de HIV) (Criança ou Adulto)', 'SD', 'Saúde', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Sensibilização sobre higiene, água e  saneamento (Todos)', 'SD', 'Saúde', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Aconselhamento sobre saúde sexual reprodutiva ( beneficiários de 10 anos ou mais)', 'SD', 'Saúde', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Mobilização/referencia para obtenção e uso correto de redes mosquiteiras (Crianças e Mulheres Gravidas)', 'SD', 'Saúde', 'routine-visit')");

            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Rastreados para o HIV (0-17 anos) -> Teste não recomendado', 'HIV', 'Seroestado', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Rastreados para o HIV (0-17 anos) -> Referência para testagem (ATS)', 'HIV', 'Seroestado', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('HIV-', 'HIV', 'Seroestado', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('HIV+ -> Em TARV', 'HIV', 'Seroestado', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('HIV+ -> Não TARV', 'HIV', 'Seroestado', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Conhece mas não revelou', 'HIV', 'Seroestado', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Não conhece', 'HIV', 'Seroestado', 'routine-visit')");

            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Sensibilização para reabilitação ou construção da casa (todos)', 'HAB', 'Habitação', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Mobilização/Sensibilização da comunidade para reabilitação ou construção de casa de beneficiários (todos)', 'HAB', 'Habitação', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Sensibilização sobre  cuidados com a casa (casa fresca e ventilada) (Todos)', 'HAB', 'Habitação', 'routine-visit')");

            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Resultado do rastreio do MUAC: Verde', 'MUAC', 'MUAC (6-69 meses)', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Resultado do rastreio do MUAC: Amarelo', 'MUAC', 'MUAC (6-69 meses)', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Resultado do rastreio do MUAC: Vermelho ', 'MUAC', 'MUAC (6-69 meses)', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Mobilizacao e referencia de casos de desnutrição para  o Programa de Reabilitação Nutricional (PRN)', 'MUAC', 'MUAC (6-69 meses)', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Sensibilizacao/ Acompanhamento e acampanhamento na comunidade para aderência a reabilitação nutricional (consultas e toma de suplementos)', 'MUAC', 'MUAC (6-69 meses)', 'routine-visit')");

            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Educação / Aconselhamento Nutricional (amamentação, alimentação complementar/Equilibrada da família) (Todos)', 'AN', 'Alim. e Nutrição', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Demonstração culinária para família (apoio na preparação de alimentos)(Todos)', 'AN', 'Alim. e Nutrição', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Mobilização/Demonstração de hortas caseiras (Todos)', 'AN', 'Alim. e Nutrição', 'routine-visit')");

            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Mobilização para Matrícula Escolar (Crianças)', 'ED', 'Educação', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Apoio no pagamento ou isenção de taxas escolares(Crianças e adolescente e jovens no secundário de 18 a 21 anos)', 'ED', 'Educação', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Reintegração escolar (Crianças)', 'ED', 'Educação', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Apoio/Sensibilização para apoio no TPC (Crianças)', 'ED', 'Educação', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Mobilização para apoio em uniforme e/ou material escolar(Crianças)', 'ED', 'Educação', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Mobilização para permanência na escola (Crianças)', 'ED', 'Educação', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Monitoria da educação (A criança continua a frequentar a escola)', 'ED', 'Educação', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Monitoria do progresso escolar  (passagem de classe a ser preenchido no final do  ano escolar) (Crianças)', 'ED', 'Educação', 'routine-visit')");

            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Mobilização/Referência aos serviços de registo de nascimento(Criança e Adulto)', 'PAL', 'Protec. e Apoio legal', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Registo de nascimento efectuado (comprovativo do registo)  (Criança ou adulto)', 'PAL', 'Protec. e Apoio legal', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Apoio para obtenção de Atestado de pobreza   (Criança ou adulto)', 'PAL', 'Protec. e Apoio legal', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Aconselhamento pós-violência (todo tipo de violência)   (Criança ou adulto)', 'PAL', 'Protec. e Apoio legal', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Mobilização para acesso aos serviços saúde pós-violência   (Criança ou adulto)', 'PAL', 'Protec. e Apoio legal', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Mobilizar/Referir para o gabinete de atendimento de vitima de violência(polícia ou outra entidade)  (Criança ou adulto)', 'PAL', 'Protec. e Apoio legal', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Habitação alternativa em casos de ambiente propicio a violência  ou de violência Criança)', 'PAL', 'Protec. e Apoio legal', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Integração no grupo de adolescentes (clube, grupo de estudo…..)  (Adolescentes)', 'PAL', 'Protec. e Apoio legal', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Sensibilização e aconselhamento  dos adolescentes para Prevenção HIV e violência (Adolescentes)', 'PAL', 'Protec. e Apoio legal', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Sensibilização cuidador para prevenção de violência e riscos sexuais nas crianças (Todos)', 'PAL', 'Protec. e Apoio legal', 'routine-visit')");

            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Mobilização para a integração nos grupos de apoio (Igreja, mãe para mãe, etc) (Todos)', 'APS', 'Apoio Psico-Social', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Mobilização /Referencia para apoio psicossocial especializado (Criança ou adulto)', 'APS', 'Apoio Psico-Social', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Escutar e confortar a criança no caso de abandono/luto/abuso/descriminação (Criança)', 'APS', 'Apoio Psico-Social', 'routine-visit')");

            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Monitoria dos marcos de desenvolvimento', 'DPI', 'DPI (0-5 anos)', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Mobilização/Referência Suspeita de atraso de desenvolvimento', 'DPI', 'DPI (0-5 anos)', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Estimulação da criança de acordo com a idade', 'DPI', 'DPI (0-5 anos)', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Fabrico de Brinquedos', 'DPI', 'DPI (0-5 anos)', 'routine-visit')");

            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Monitoria do Plano de Acção da família', 'OTR', 'Outros', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Monitoria da Aderência ao tratamento (monitorar se cada pessoa HIV+ está a aderir ao tratamento)', 'OTR', 'Outros', 'routine-visit')");
            Sql(@"INSERT INTO SupportServiceType ([Description], TypeCode, TypeDescription, Tool)
                VALUES ('Atribuição do Apoio Social de emergência', 'OTR', 'Outros', 'routine-visit')");
        }
        
        public override void Down()
        {
            DropColumn("dbo.SupportServiceType", "TypeDescription");
        }
    }
}
