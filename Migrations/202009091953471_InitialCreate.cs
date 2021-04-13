namespace LawCalculator_WPF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Lawyers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Salary = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LawyersProjects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Percent = c.Single(nullable: false),
                        Lawyer_Id = c.Int(),
                        Partner_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Lawyers", t => t.Lawyer_Id)
                .ForeignKey("dbo.Partners", t => t.Partner_Id)
                .Index(t => t.Lawyer_Id)
                .Index(t => t.Partner_Id);
            
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Amount = c.Double(nullable: false),
                        Date = c.DateTime(nullable: false),
                        DateString = c.String(),
                        Currency = c.Int(nullable: false),
                        ProjectName = c.String(),
                        ToPay = c.Boolean(nullable: false),
                        Payed = c.Boolean(nullable: false),
                        LawyersProject_Id = c.Int(),
                        Project_Id = c.Int(),
                        Project_Id1 = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LawyersProjects", t => t.LawyersProject_Id)
                .ForeignKey("dbo.Projects", t => t.Project_Id)
                .ForeignKey("dbo.Projects", t => t.Project_Id1)
                .Index(t => t.LawyersProject_Id)
                .Index(t => t.Project_Id)
                .Index(t => t.Project_Id1);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ProjectCurrency = c.Int(nullable: false),
                        OriginatingPartnerPercent = c.Int(nullable: false),
                        ManagingPartnerPercent = c.Int(nullable: false),
                        OriginatorVisibilityTrigger = c.Byte(nullable: false),
                        ManagerVisibilityTrigger = c.Byte(nullable: false),
                        ManagingPartner_Id = c.Int(),
                        OriginatingPartner_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Partners", t => t.ManagingPartner_Id)
                .ForeignKey("dbo.Partners", t => t.OriginatingPartner_Id)
                .Index(t => t.ManagingPartner_Id)
                .Index(t => t.OriginatingPartner_Id);
            
            CreateTable(
                "dbo.Partners",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProjectLawyers",
                c => new
                    {
                        Project_Id = c.Int(nullable: false),
                        Lawyer_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Project_Id, t.Lawyer_Id })
                .ForeignKey("dbo.Projects", t => t.Project_Id, cascadeDelete: true)
                .ForeignKey("dbo.Lawyers", t => t.Lawyer_Id, cascadeDelete: true)
                .Index(t => t.Project_Id)
                .Index(t => t.Lawyer_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Payments", "Project_Id1", "dbo.Projects");
            DropForeignKey("dbo.Payments", "Project_Id", "dbo.Projects");
            DropForeignKey("dbo.Projects", "OriginatingPartner_Id", "dbo.Partners");
            DropForeignKey("dbo.Projects", "ManagingPartner_Id", "dbo.Partners");
            DropForeignKey("dbo.LawyersProjects", "Partner_Id", "dbo.Partners");
            DropForeignKey("dbo.ProjectLawyers", "Lawyer_Id", "dbo.Lawyers");
            DropForeignKey("dbo.ProjectLawyers", "Project_Id", "dbo.Projects");
            DropForeignKey("dbo.LawyersProjects", "Lawyer_Id", "dbo.Lawyers");
            DropForeignKey("dbo.Payments", "LawyersProject_Id", "dbo.LawyersProjects");
            DropIndex("dbo.ProjectLawyers", new[] { "Lawyer_Id" });
            DropIndex("dbo.ProjectLawyers", new[] { "Project_Id" });
            DropIndex("dbo.Projects", new[] { "OriginatingPartner_Id" });
            DropIndex("dbo.Projects", new[] { "ManagingPartner_Id" });
            DropIndex("dbo.Payments", new[] { "Project_Id1" });
            DropIndex("dbo.Payments", new[] { "Project_Id" });
            DropIndex("dbo.Payments", new[] { "LawyersProject_Id" });
            DropIndex("dbo.LawyersProjects", new[] { "Partner_Id" });
            DropIndex("dbo.LawyersProjects", new[] { "Lawyer_Id" });
            DropTable("dbo.ProjectLawyers");
            DropTable("dbo.Partners");
            DropTable("dbo.Projects");
            DropTable("dbo.Payments");
            DropTable("dbo.LawyersProjects");
            DropTable("dbo.Lawyers");
        }
    }
}
