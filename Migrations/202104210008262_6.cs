namespace LawCalculator_WPF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _6 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Projects", "OriginatorVisibilityTrigger");
            DropColumn("dbo.Projects", "ManagerVisibilityTrigger");
        }
    }
}
