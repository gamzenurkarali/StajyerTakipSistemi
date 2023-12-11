using System;
using System.Collections.Generic;
using StajyerTakipSistemi.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace StajyerTakipSistemi.Web.Models;

public partial class StajyerTakipSistemiDbContext : DbContext
{
    public StajyerTakipSistemiDbContext()
    {
    }

    public StajyerTakipSistemiDbContext(DbContextOptions<StajyerTakipSistemiDbContext> options)
        : base(options)
    {
    }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
    public virtual DbSet<SAbsenceInformation> SAbsenceInformations { get; set; }

    public virtual DbSet<SApplication> SApplications { get; set; }

    public virtual DbSet<SAssignedTask> SAssignedTasks { get; set; }

    public virtual DbSet<SDailyReport> SDailyReports { get; set; }

    public virtual DbSet<SIntern> SInterns { get; set; }

    public virtual DbSet<SInternToManager> SInternToManagers { get; set; }

    public virtual DbSet<SManager> SManagers { get; set; }

    public virtual DbSet<SMessagesforintern> SMessagesforinterns { get; set; }

    public virtual DbSet<SMessagesformanager> SMessagesformanagers { get; set; }

    public virtual DbSet<STaskDetail> STaskDetails { get; set; }

    public virtual DbSet<Message> Messages { get; set; } = null!;
    public virtual DbSet<NewMessages> NewMessages { get; set; } = null!;
    public virtual DbSet<Sadmin> admin { get; set; } 
    public virtual DbSet<SFinal> SFinal { get; set; } 
    

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-SMOAMK0\\SQLEXPRESS;Initial Catalog=StajyerTakipSistemi;Integrated Security=True;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)

    {
        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PasswordResetToken__3214EC07F8C9F041");

            entity.ToTable("PasswordResetTokens");

            entity.Property(e => e.Guid).IsRequired();

            entity.Property(e => e.Token).IsRequired();

            entity.Property(e => e.ExpirationTime).HasColumnType("datetime");

            entity.HasIndex(e => e.Token).IsUnique();
             
        });

        modelBuilder.Entity<SAbsenceInformation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__S_absenc__3214EC07F8C9F041");

            entity.ToTable("S_absenceInformation");

            entity.Property(e => e.AbsenceDate).HasColumnType("date");

            entity.HasOne(d => d.Intern).WithMany(p => p.SAbsenceInformations)
                .HasForeignKey(d => d.InternId)
                .HasConstraintName("FK__S_absence__Inter__3F466844");
        });

        modelBuilder.Entity<SApplication>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__S_applic__3214EC07B0C25BF1");

            entity.ToTable("S_applications");

            entity.Property(e => e.ApplicationDate).HasColumnType("date");
            entity.Property(e => e.ApprovalStatus).HasMaxLength(50);
            entity.Property(e => e.BirthDate).HasColumnType("date");
            entity.Property(e => e.DesiredField).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(255);
            entity.Property(e => e.LastName).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
        });

        modelBuilder.Entity<SAssignedTask>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__S_assign__3214EC07E47A0C5A");

            entity.ToTable("S_assignedTask");

            entity.Property(e => e.AssignmentDate).HasColumnType("date");
            entity.Property(e => e.DueDate).HasColumnType("date");

            entity.HasOne(d => d.Intern).WithMany(p => p.SAssignedTasks)
               .HasForeignKey(d => d.InternId)
               .HasConstraintName("FK__S_assigne__Inter__4BAC3F29");

            entity.HasOne(d => d.Task).WithMany(p => p.SAssignedTasks)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK__S_assigne__TaskI__4CA06362");
        });

        modelBuilder.Entity<SDailyReport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__S_dailyR__3214EC07B49E10A5");

            entity.ToTable("S_dailyReports");
            entity.Property(e => e.internGuid).HasDefaultValueSql("(newsequentialid())");

            entity.Property(e => e.UnixTime).HasDefaultValueSql("(datediff(second,'1970-01-01 00:00:00',getutcdate()))");

            entity.HasOne(d => d.Intern).WithMany(p => p.SDailyReports)
                .HasForeignKey(d => d.InternId)
                .HasConstraintName("FK__S_dailyRe__Conte__46E78A0C");
        });

        modelBuilder.Entity<SIntern>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__S_intern__3214EC07A4DAB455");

            entity.ToTable("S_interns");

            entity.HasIndex(e => e.Username, "UQ__S_intern__536C85E4102FC61A").IsUnique();

            entity.Property(e => e.AccessStatus).HasMaxLength(50);
            entity.Property(e => e.BirthDate).HasColumnType("date");
            entity.Property(e => e.DesiredField).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            //entity.Property(e => e.EndDate).HasColumnType("date");
            entity.Property(e => e.FirstName).HasMaxLength(255);
            entity.Property(e => e.LastName).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.StartDate).HasColumnType("date");
            entity.Property(e => e.Username).HasMaxLength(50);
            entity.Property(e => e.Guid).HasDefaultValueSql("(newsequentialid())");
        });

        modelBuilder.Entity<Sadmin>(entity =>
        { 

            entity.ToTable("S_admin");


            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Username).HasMaxLength(50);
            entity.Property(e => e.Guid).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Email).HasMaxLength(255);
        });
        modelBuilder.Entity<SFinal>(entity =>
        {
            entity.ToTable("S_Final");

            entity.HasKey(e => e.Id).HasName("PK__final__3214EC07D2EAEF69");
             
            entity.HasOne<SIntern>()
                  .WithMany()  
                  .HasForeignKey(e => e.InternId)  
                  .OnDelete(DeleteBehavior.Cascade);  
 
            entity.Property(e => e.RelatedDocuments)
                  .HasColumnType("nvarchar(max)");

            entity.Property(e => e.GitHubLink)
                  .HasColumnType("nvarchar(max)");

            entity.Property(e => e.YouTubeLink)
                  .HasColumnType("nvarchar(max)");

            entity.Property(e => e.EvaluationStatus)
                  .HasColumnType("bit");


        });

        modelBuilder.Entity<SInternToManager>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__S_intern__3214EC071E1FDB18");

            entity.ToTable("S_internToManager");

            entity.HasIndex(e => e.InternId, "UQ__S_intern__6910EDE361C24DC6").IsUnique();

            entity.HasOne(d => d.Intern).WithOne(p => p.SInternToManager)
                .HasForeignKey<SInternToManager>(d => d.InternId)
                .HasConstraintName("FK__S_internT__Inter__4316F928");

            entity.HasOne(d => d.Manager).WithMany(p => p.SInternToManagers)
                .HasForeignKey(d => d.ManagerId)
                .HasConstraintName("FK__S_internT__Manag__440B1D61");
        });

        modelBuilder.Entity<SManager>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__S_manage__3214EC07506EE75C");

            entity.ToTable("S_managers");

            entity.HasIndex(e => e.Username, "UQ__S_manage__536C85E4F41DE357").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(255);
            entity.Property(e => e.LastName).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Username).HasMaxLength(50);
            entity.Property(e => e.Guid).HasDefaultValueSql("(newsequentialid())");
        });

        modelBuilder.Entity<SMessagesforintern>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__S_messag__3214EC07ABF4B60B");

            entity.ToTable("S_messagesforinterns");

            entity.HasOne(d => d.Receiver).WithMany(p => p.SMessagesforinterns)
                .HasForeignKey(d => d.ReceiverId)
                .HasConstraintName("FK__S_message__Recei__5070F446");

            entity.HasOne(d => d.Sender).WithMany(p => p.SMessagesforinterns)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("FK__S_message__Sende__4F7CD00D");
        });

        modelBuilder.Entity<SMessagesformanager>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__S_messag__3214EC073A5F8234");

            entity.ToTable("S_messagesformanagers");

            entity.HasOne(d => d.Receiver).WithMany(p => p.SMessagesformanagers)
                .HasForeignKey(d => d.ReceiverId)
                .HasConstraintName("FK__S_message__Recei__5441852A");

            entity.HasOne(d => d.Sender).WithMany(p => p.SMessagesformanagers)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("FK__S_message__Sende__534D60F1");
        });

        modelBuilder.Entity<STaskDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__S_taskDe__3214EC074A5D6F73");

            entity.ToTable("S_taskDetails");

            entity.Property(e => e.Subject).HasMaxLength(255);
        });
        modelBuilder.Entity<NewMessages>(entity =>
        {
            entity.ToTable("NewMessages");

            entity.Property(e => e.Guid).HasDefaultValueSql("(newsequentialid())");

            entity.Property(e => e.UnixTime).HasDefaultValueSql("(datediff(second,'1970-01-01 00:00:00',getutcdate()))");
        });
        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("Message");

            entity.Property(e => e.UnixTime).HasDefaultValueSql("(datediff(second,'1970-01-01 00:00:00',getutcdate()))");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
