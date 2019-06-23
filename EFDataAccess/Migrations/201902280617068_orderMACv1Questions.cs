namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class orderMACv1Questions : DbMigration
    {
        public override void Up()
        {
            Sql(@"UPDATE [Question] SET [QuestionOrder] = 1 WHERE Code = '3A'
                UPDATE [Question] SET [QuestionOrder] = 2 WHERE Code = '3B'
                UPDATE [Question] SET [QuestionOrder] = 3 WHERE Code = '3C'
                UPDATE [Question] SET [QuestionOrder] = 4 WHERE Code = '3D'
                UPDATE [Question] SET [QuestionOrder] = 5 WHERE Code = '3E'
                UPDATE [Question] SET [QuestionOrder] = 6 WHERE Code = '3F'
                UPDATE [Question] SET [QuestionOrder] = 7 WHERE Code = '3G'
                UPDATE [Question] SET [QuestionOrder] = 8 WHERE Code = '1A'
                UPDATE [Question] SET [QuestionOrder] = 9 WHERE Code = '1B'
                UPDATE [Question] SET [QuestionOrder] = 10 WHERE Code = '2A'
                UPDATE [Question] SET [QuestionOrder] = 11 WHERE Code = '2B'
                UPDATE [Question] SET [QuestionOrder] = 12 WHERE Code = '2C'
                UPDATE [Question] SET [QuestionOrder] = 13 WHERE Code = '2D'
                UPDATE [Question] SET [QuestionOrder] = 14 WHERE Code = '2E'
                UPDATE [Question] SET [QuestionOrder] = 15 WHERE Code = '2F'
                UPDATE [Question] SET [QuestionOrder] = 16 WHERE Code = '2G'
                UPDATE [Question] SET [QuestionOrder] = 17 WHERE Code = '2H'
                UPDATE [Question] SET [QuestionOrder] = 18 WHERE Code = '4A'
                UPDATE [Question] SET [QuestionOrder] = 19 WHERE Code = '4B'
                UPDATE [Question] SET [QuestionOrder] = 20 WHERE Code = '4C'
                UPDATE [Question] SET [QuestionOrder] = 21 WHERE Code = '4D'
                UPDATE [Question] SET [QuestionOrder] = 22 WHERE Code = '7A'
                UPDATE [Question] SET [QuestionOrder] = 23 WHERE Code = '5A'
                UPDATE [Question] SET [QuestionOrder] = 24 WHERE Code = '5B'
                UPDATE [Question] SET [QuestionOrder] = 25 WHERE Code = '5C'
                UPDATE [Question] SET [QuestionOrder] = 26 WHERE Code = '5D'
                UPDATE [Question] SET [QuestionOrder] = 27 WHERE Code = '5E'
                UPDATE [Question] SET [QuestionOrder] = 28 WHERE Code = '5F'
                UPDATE [Question] SET [QuestionOrder] = 29 WHERE Code = '5G'
                UPDATE [Question] SET [QuestionOrder] = 30 WHERE Code = '5H'
                UPDATE [Question] SET [QuestionOrder] = 31 WHERE Code = '5I'
                UPDATE [Question] SET [QuestionOrder] = 32 WHERE Code = '6A'
                UPDATE [Question] SET [QuestionOrder] = 33 WHERE Code = '6B'");
        }
        
        public override void Down()
        {
        }
    }
}
