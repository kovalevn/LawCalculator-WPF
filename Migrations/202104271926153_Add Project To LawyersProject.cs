namespace LawCalculator_WPF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProjectToLawyersProject : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LawyersProjects", "Project_Id", c => c.Int(nullable: true));
            Sql("UPDATE [dbo].[LawyersProjects] SET Project_Id = (SELECT TOP 1 [Id] FROM [dbo].[Projects])");
            AlterColumn("dbo.LawyersProjects", "Project_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.LawyersProjects", "Project_Id");
            AddForeignKey("dbo.LawyersProjects", "Project_Id", "dbo.Projects", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LawyersProjects", "Project_Id", "dbo.Projects");
            DropIndex("dbo.LawyersProjects", new[] { "Project_Id" });
            DropColumn("dbo.LawyersProjects", "Project_Id");
        }
    }
}
