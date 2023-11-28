using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StajyerTakipSistemi.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddEndDateToSIntern : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sender = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Receiver = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnixTime = table.Column<long>(type: "bigint", nullable: true, defaultValueSql: "(datediff(second,'1970-01-01 00:00:00',getutcdate()))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.MessageId);
                });

            migrationBuilder.CreateTable(
                name: "NewMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newsequentialid())"),
                    Sender = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Receiver = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnixTime = table.Column<long>(type: "bigint", nullable: true, defaultValueSql: "(datediff(second,'1970-01-01 00:00:00',getutcdate()))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PasswordResetTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExpirationTime = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PasswordResetToken__3214EC07F8C9F041", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "S_admin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_S_admin", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "S_applications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "date", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DesiredField = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Explanation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovalStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ApplicationDate = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__S_applic__3214EC07B0C25BF1", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "S_interns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "date", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DesiredField = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Explanation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccessStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: true, defaultValueSql: "(newsequentialid())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__S_intern__3214EC07A4DAB455", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "S_managers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: true, defaultValueSql: "(newsequentialid())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__S_manage__3214EC07506EE75C", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "S_taskDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Contents = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__S_taskDe__3214EC074A5D6F73", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "S_absenceInformation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InternId = table.Column<int>(type: "int", nullable: true),
                    AbsenceDate = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__S_absenc__3214EC07F8C9F041", x => x.Id);
                    table.ForeignKey(
                        name: "FK__S_absence__Inter__3F466844",
                        column: x => x.InternId,
                        principalTable: "S_interns",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "S_dailyReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InternId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Data = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    UnixTime = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "(datediff(second,'1970-01-01 00:00:00',getutcdate()))"),
                    internGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newsequentialid())"),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__S_dailyR__3214EC07B49E10A5", x => x.Id);
                    table.ForeignKey(
                        name: "FK__S_dailyRe__Conte__46E78A0C",
                        column: x => x.InternId,
                        principalTable: "S_interns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "S_internToManager",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InternId = table.Column<int>(type: "int", nullable: true),
                    ManagerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__S_intern__3214EC071E1FDB18", x => x.Id);
                    table.ForeignKey(
                        name: "FK__S_internT__Inter__4316F928",
                        column: x => x.InternId,
                        principalTable: "S_interns",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__S_internT__Manag__440B1D61",
                        column: x => x.ManagerId,
                        principalTable: "S_managers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "S_messagesforinterns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderId = table.Column<int>(type: "int", nullable: true),
                    ReceiverId = table.Column<int>(type: "int", nullable: true),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnixTimestamp = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__S_messag__3214EC07ABF4B60B", x => x.Id);
                    table.ForeignKey(
                        name: "FK__S_message__Recei__5070F446",
                        column: x => x.ReceiverId,
                        principalTable: "S_managers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__S_message__Sende__4F7CD00D",
                        column: x => x.SenderId,
                        principalTable: "S_interns",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "S_messagesformanagers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderId = table.Column<int>(type: "int", nullable: true),
                    ReceiverId = table.Column<int>(type: "int", nullable: true),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnixTimestamp = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__S_messag__3214EC073A5F8234", x => x.Id);
                    table.ForeignKey(
                        name: "FK__S_message__Recei__5441852A",
                        column: x => x.ReceiverId,
                        principalTable: "S_interns",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__S_message__Sende__534D60F1",
                        column: x => x.SenderId,
                        principalTable: "S_managers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "S_assignedTask",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InternId = table.Column<int>(type: "int", nullable: true),
                    TaskId = table.Column<int>(type: "int", nullable: true),
                    AssignmentDate = table.Column<DateTime>(type: "date", nullable: true),
                    DueDate = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__S_assign__3214EC07E47A0C5A", x => x.Id);
                    table.ForeignKey(
                        name: "FK__S_assigne__Inter__4BAC3F29",
                        column: x => x.InternId,
                        principalTable: "S_interns",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__S_assigne__TaskI__4CA06362",
                        column: x => x.TaskId,
                        principalTable: "S_taskDetails",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_Token",
                table: "PasswordResetTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_S_absenceInformation_InternId",
                table: "S_absenceInformation",
                column: "InternId");

            migrationBuilder.CreateIndex(
                name: "IX_S_assignedTask_InternId",
                table: "S_assignedTask",
                column: "InternId");

            migrationBuilder.CreateIndex(
                name: "IX_S_assignedTask_TaskId",
                table: "S_assignedTask",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_S_dailyReports_InternId",
                table: "S_dailyReports",
                column: "InternId");

            migrationBuilder.CreateIndex(
                name: "UQ__S_intern__536C85E4102FC61A",
                table: "S_interns",
                column: "Username",
                unique: true,
                filter: "[Username] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_S_internToManager_ManagerId",
                table: "S_internToManager",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "UQ__S_intern__6910EDE361C24DC6",
                table: "S_internToManager",
                column: "InternId",
                unique: true,
                filter: "[InternId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ__S_manage__536C85E4F41DE357",
                table: "S_managers",
                column: "Username",
                unique: true,
                filter: "[Username] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_S_messagesforinterns_ReceiverId",
                table: "S_messagesforinterns",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_S_messagesforinterns_SenderId",
                table: "S_messagesforinterns",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_S_messagesformanagers_ReceiverId",
                table: "S_messagesformanagers",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_S_messagesformanagers_SenderId",
                table: "S_messagesformanagers",
                column: "SenderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "NewMessages");

            migrationBuilder.DropTable(
                name: "PasswordResetTokens");

            migrationBuilder.DropTable(
                name: "S_absenceInformation");

            migrationBuilder.DropTable(
                name: "S_admin");

            migrationBuilder.DropTable(
                name: "S_applications");

            migrationBuilder.DropTable(
                name: "S_assignedTask");

            migrationBuilder.DropTable(
                name: "S_dailyReports");

            migrationBuilder.DropTable(
                name: "S_internToManager");

            migrationBuilder.DropTable(
                name: "S_messagesforinterns");

            migrationBuilder.DropTable(
                name: "S_messagesformanagers");

            migrationBuilder.DropTable(
                name: "S_taskDetails");

            migrationBuilder.DropTable(
                name: "S_interns");

            migrationBuilder.DropTable(
                name: "S_managers");
        }
    }
}
