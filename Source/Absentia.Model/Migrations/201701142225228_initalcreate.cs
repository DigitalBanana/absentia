namespace Absentia.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initalcreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        NotificationId = c.Int(nullable: false, identity: true),
                        ProcessingMessage = c.String(maxLength: 4000),
                        ProcessingResult = c.Boolean(),
                        NotificationRecievedTimeUtc = c.DateTime(nullable: false),
                        NotificationProcessedTimeUtc = c.DateTime(),
                        ChangeType = c.String(maxLength: 4000),
                        ClientState = c.String(maxLength: 4000),
                        Resource = c.String(maxLength: 4000),
                        SubscriptionExpirationDateTime = c.DateTime(nullable: false),
                        SubscriptionId = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.NotificationId);
            
            CreateTable(
                "dbo.ResourceDatas",
                c => new
                    {
                        ResourceDataId = c.Int(nullable: false),
                        NotificationId = c.Int(nullable: false),
                        Id = c.String(maxLength: 4000),
                        ODataEtag = c.String(maxLength: 4000),
                        ODataId = c.String(maxLength: 4000),
                        ODataType = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.ResourceDataId)
                .ForeignKey("dbo.Notifications", t => t.ResourceDataId, cascadeDelete: true)
                .Index(t => t.ResourceDataId);
            
            CreateTable(
                "dbo.SubscriptionAttempts",
                c => new
                    {
                        SubscriptionAttemptId = c.Int(nullable: false, identity: true),
                        UserName = c.String(maxLength: 4000),
                        Success = c.Boolean(nullable: false),
                        Message = c.String(maxLength: 4000),
                        AttemptTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SubscriptionAttemptId);
            
            CreateTable(
                "dbo.Subscriptions",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 200),
                        ChangeType = c.String(maxLength: 4000),
                        ClientState = c.String(maxLength: 4000),
                        NotificationUrl = c.String(maxLength: 4000),
                        Resource = c.String(maxLength: 4000),
                        ExpirationDateTime = c.DateTime(nullable: false),
                        User_DirectoryUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DirectoryUsers", t => t.User_DirectoryUserId, cascadeDelete: true)
                .Index(t => t.User_DirectoryUserId);
            
            CreateTable(
                "dbo.DirectoryUsers",
                c => new
                    {
                        DirectoryUserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.DirectoryUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Subscriptions", "User_DirectoryUserId", "dbo.DirectoryUsers");
            DropForeignKey("dbo.ResourceDatas", "ResourceDataId", "dbo.Notifications");
            DropIndex("dbo.Subscriptions", new[] { "User_DirectoryUserId" });
            DropIndex("dbo.ResourceDatas", new[] { "ResourceDataId" });
            DropTable("dbo.DirectoryUsers");
            DropTable("dbo.Subscriptions");
            DropTable("dbo.SubscriptionAttempts");
            DropTable("dbo.ResourceDatas");
            DropTable("dbo.Notifications");
        }
    }
}
