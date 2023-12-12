using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetCore_Assignemt.Migrations
{
    /// <inheritdoc />
    public partial class finale1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "e5803474-7743-4bdf-a179-9ef6e5cd23f7", "AQAAAAIAAYagAAAAEBzWtqI1Z76OWwM54ZntJ6crcUKRDTN1Jy/ooenE61b9YMD/c3YyAYWc13NpGOz38w==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "94be1521-7152-453c-abf3-3f97212720e2", "AQAAAAIAAYagAAAAEM6LndPjpeoMQ+v/Fae0TnOeVZF3X/Qu5rnEOBqcyjaHTardq8Qocos29A/zrHbE3g==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "82a5339a-bc5e-4448-aaf4-d7855beda93a", "AQAAAAIAAYagAAAAELNRKteQnGJDpzr1PFCHhtJIeMNtpw+NizBOPlctVxowpCNYPSBswmr6Lf9TA1iqzw==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "14341078-1e15-49d9-a4ff-bc0386b2f93b", "AQAAAAIAAYagAAAAEJF1YnV6SeYmVqiX+j61steRqs/rNQ7JYh09pXMjEg1sAJjKrSyp7z1YS46KjVxWvw==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "8f3e2f6f-6194-4cd8-9e26-ed0d17a35ca2", "AQAAAAIAAYagAAAAEGtX1ZicQT5VNV4rwFnCB4yoo7hh9vOB/XmbRri0Jh9OW1sKeyvN2x45QXHI4hKnRA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "fd8b2d60-a3e2-4adf-9168-536c4f3ed69b", "AQAAAAIAAYagAAAAEL6esCDB0cxFXstU5BvTXAk4gBLdRrGKAGyIX/joA7nNH6EWRCEPHT04mafG3IxMHg==" });
        }
    }
}
