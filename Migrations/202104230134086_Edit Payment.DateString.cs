namespace LawCalculator_WPF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditPaymentDateString : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Payments", "DateString");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Payments", "DateString", c => c.String());
        }
    }
}
