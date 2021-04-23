namespace LawCalculator_WPF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditPaymentToPay : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Payments", "ToPay");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Payments", "ToPay", c => c.Boolean(nullable: false));
        }
    }
}
