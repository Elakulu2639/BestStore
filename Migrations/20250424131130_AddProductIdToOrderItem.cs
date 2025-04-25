using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BestStore.Migrations
{
    /// <inheritdoc />
    public partial class AddProductIdToOrderItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        IF NOT EXISTS (SELECT * FROM sys.columns 
                       WHERE object_id = OBJECT_ID('dbo.OrderItems') 
                       AND name = 'ProductId')
        BEGIN
            ALTER TABLE [OrderItems] ADD [ProductId] int NOT NULL DEFAULT 0;
        END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "OrderItems");
        }
    }
}
