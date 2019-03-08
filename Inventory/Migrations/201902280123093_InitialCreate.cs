namespace Inventory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Edificios",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Codigo = c.Int(nullable: false),
                        Direccion = c.String(),
                        Tecnico = c.String(),
                        Estado = c.Int(nullable: false),
                        Fecha = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Materials",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Codigo = c.Int(nullable: false),
                        Nombre = c.String(),
                        Edificio_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Edificios", t => t.Edificio_Id)
                .Index(t => t.Edificio_Id);
            
            CreateTable(
                "dbo.EdificiosMateriales",
                c => new
                    {
                        EdificioId = c.Int(nullable: false),
                        MaterialId = c.Int(nullable: false),
                        Cantidad = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.EdificioId, t.MaterialId })
                .ForeignKey("dbo.Edificios", t => t.EdificioId, cascadeDelete: true)
                .ForeignKey("dbo.Materials", t => t.MaterialId, cascadeDelete: true)
                .Index(t => t.EdificioId)
                .Index(t => t.MaterialId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EdificiosMateriales", "MaterialId", "dbo.Materials");
            DropForeignKey("dbo.EdificiosMateriales", "EdificioId", "dbo.Edificios");
            DropForeignKey("dbo.Materials", "Edificio_Id", "dbo.Edificios");
            DropIndex("dbo.EdificiosMateriales", new[] { "MaterialId" });
            DropIndex("dbo.EdificiosMateriales", new[] { "EdificioId" });
            DropIndex("dbo.Materials", new[] { "Edificio_Id" });
            DropTable("dbo.EdificiosMateriales");
            DropTable("dbo.Materials");
            DropTable("dbo.Edificios");
        }
    }
}
