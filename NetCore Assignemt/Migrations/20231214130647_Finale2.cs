using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetCore_Assignemt.Migrations
{
    /// <inheritdoc />
    public partial class Finale2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "vnp_CardType",
                table: "Transaction",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "vnp_BankTranNo",
                table: "Transaction",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "vnp_BankCode",
                table: "Transaction",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "bebe160c-21a5-4b56-bea5-2a0b6f4b8df1", "AQAAAAIAAYagAAAAEPhnVf2IuH2gmnF8WaHVL0D75a79sDB2uVvt1QOsPGVzgjZCgrc2GymHnH8iWjLzJA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "4e3bd9e7-13f5-4797-97ba-966680c2677e", "AQAAAAIAAYagAAAAEFEix14gf27L9JvXlNcH2Nk8UzGRD+oRAE5oSJbo5SGGHK0x2sHSRB2Zf/G5y6MZtw==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "0d151d8a-7130-4c65-b850-c1730cb0a2a3", "AQAAAAIAAYagAAAAEJ6zhVWNN8nKw6uQ9qhqLom82qFvNExdORpaxvtzmyXsXQMC5NZBnrCM01QHUq5oww==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "vnp_CardType",
                table: "Transaction",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "vnp_BankTranNo",
                table: "Transaction",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "vnp_BankCode",
                table: "Transaction",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "29c85fbe-46a6-477d-8990-9f1cbcabfdff", "AQAAAAIAAYagAAAAEN8U3NFwLn4u5B6nnvEyyfP0DGvc+Y98umHOR4WNVXXb7Q/HthH3mzjbKnSZNN5XYg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "382205c4-7a9f-4e62-a018-89bd8bda1f54", "AQAAAAIAAYagAAAAEG3CeWJikNPiy2QIYFoTVtthjq7CXZnQWQRrucOBoK5Ep5q5PhMzdasN840JeCExNg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "b6866a6c-16b7-4991-9cbc-06349583236c", "AQAAAAIAAYagAAAAEKsmmChnG24z5pHZCY0wldUwBFjGsUwp7zr69U0JgDoBqcEcBqppE9TCUW98s22Fiw==" });
        }
    }
}
