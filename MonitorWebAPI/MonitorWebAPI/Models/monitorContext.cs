using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace MonitorWebAPI.Models
{
    public partial class monitorContext : DbContext
    {
        public monitorContext()
        {
        }

        public monitorContext(DbContextOptions<monitorContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<DeviceGroup> DeviceGroups { get; set; }
        public virtual DbSet<DeviceStatusLog> DeviceStatusLogs { get; set; }
        public virtual DbSet<ErrorDictionary> ErrorDictionaries { get; set; }
        public virtual DbSet<ErrorLog> ErrorLogs { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<ReportInstance> ReportInstances { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<SecurityQuestion> SecurityQuestions { get; set; }
        public virtual DbSet<TaskStatus> TaskStatuses { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserGroup> UserGroups { get; set; }
        public virtual DbSet<UserSecurityQuestion> UserSecurityQuestions { get; set; }
        public virtual DbSet<UserTask> UserTasks { get; set; }
        public virtual DbSet<UserTracker> UserTrackers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=167.99.244.168;Port=5432;Database=monitor;Username=si-baza;Password=sipassword2021");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp")
                .HasAnnotation("Relational:Collation", "C.UTF-8");

            modelBuilder.Entity<Device>(entity =>
            {
                entity.ToTable("DEVICE");

                entity.HasIndex(e => new { e.Name, e.Location }, "DEVICE_Name_Location_key")
                    .IsUnique();

                entity.HasIndex(e => e.DeviceId, "device_deviceid_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.DeviceUid, "device_deviceuid_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.InstallationCode, "device_installationcode_uindex")
                    .IsUnique();

                entity.Property(e => e.DeviceId).HasDefaultValueSql("nextval('\"device_DeviceId_seq\"'::regclass)");

                entity.Property(e => e.DeviceUid)
                    .HasColumnName("deviceUID")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.LastTimeOnline).HasColumnType("timestamp with time zone");

                entity.Property(e => e.Location).IsRequired();

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<DeviceGroup>(entity =>
            {
                entity.ToTable("DEVICE_GROUP");

                entity.HasIndex(e => e.Id, "device_group_id_uindex")
                    .IsUnique();

                entity.HasOne(d => d.Device)
                    .WithMany(p => p.DeviceGroups)
                    .HasForeignKey(d => d.DeviceId)
                    .HasConstraintName("device_group_device_deviceid_fk");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.DeviceGroups)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("device_group_group_groupid_fk");
            });

            modelBuilder.Entity<DeviceStatusLog>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("DEVICE_STATUS_LOG");

                entity.Property(e => e.Gpuusage).HasColumnName("GPUUsage");

                entity.Property(e => e.Hddusage).HasColumnName("HDDUsage");

                entity.Property(e => e.TimeStamp).HasColumnType("timestamp with time zone");

                entity.HasOne(d => d.Device)
                    .WithMany()
                    .HasForeignKey(d => d.DeviceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("device_status_log_device_deviceid_fk");
            });

            modelBuilder.Entity<ErrorDictionary>(entity =>
            {
                entity.ToTable("ERROR_DICTIONARY");

                entity.HasIndex(e => e.Code, "error_dictionary_code_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.Id, "error_dictionary_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Type).IsRequired();
            });

            modelBuilder.Entity<ErrorLog>(entity =>
            {
                entity.ToTable("ERROR_LOG");

                entity.HasIndex(e => e.Id, "error_log_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('error_log_id_seq'::regclass)");

                entity.Property(e => e.ErrorTime).HasColumnType("timestamp with time zone");

                entity.HasOne(d => d.Device)
                    .WithMany(p => p.ErrorLogs)
                    .HasForeignKey(d => d.DeviceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("error_log_device_device_id_fk");

                entity.HasOne(d => d.ErrorType)
                    .WithMany(p => p.ErrorLogs)
                    .HasForeignKey(d => d.ErrorTypeId)
                    .HasConstraintName("error_log_error_dictionary__fk");
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.ToTable("GROUP");

                entity.HasIndex(e => e.GroupId, "group_groupid_uindex")
                    .IsUnique();

                entity.Property(e => e.Name).IsRequired();

                entity.HasOne(d => d.ParentGroupNavigation)
                    .WithMany(p => p.InverseParentGroupNavigation)
                    .HasForeignKey(d => d.ParentGroup)
                    .HasConstraintName("group_group_groupid_fk");
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("REPORTS");

                entity.HasIndex(e => e.ReportId, "active_reports_reportid_uindex")
                    .IsUnique();

                entity.Property(e => e.ReportId).HasDefaultValueSql("nextval('\"active_reports_ReportId_seq\"'::regclass)");

                entity.Property(e => e.Frequency).IsRequired();

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.NextDate).HasColumnType("timestamp with time zone");

                entity.Property(e => e.Query).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("active_reports_user_userid_fk");
            });

            modelBuilder.Entity<ReportInstance>(entity =>
            {
                entity.ToTable("REPORT_INSTANCES");

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"report_instances_Id_seq\"'::regclass)");

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.UriLink)
                    .IsRequired()
                    .HasColumnName("URI_link");

                entity.HasOne(d => d.Report)
                    .WithMany(p => p.ReportInstances)
                    .HasForeignKey(d => d.ReportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("report_instances_reports_reportid_fk");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("ROLE");

                entity.HasIndex(e => e.RoleId, "role_roleid_uindex")
                    .IsUnique();

                entity.Property(e => e.RoleId).HasDefaultValueSql("nextval('\"role_RoleId_seq\"'::regclass)");
            });

            modelBuilder.Entity<SecurityQuestion>(entity =>
            {
                entity.HasKey(e => e.QuestionId)
                    .HasName("security_question_pk");

                entity.ToTable("SECURITY_QUESTION");

                entity.Property(e => e.QuestionId).HasDefaultValueSql("nextval('\"SECURITY_QUESTION_QuestionID_seq\"'::regclass)");

                entity.Property(e => e.Question)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<TaskStatus>(entity =>
            {
                entity.HasKey(e => e.StatusId)
                    .HasName("task_status_pk");

                entity.ToTable("TASK_STATUS");

                entity.HasIndex(e => e.StatusId, "task_status_statusid_uindex")
                    .IsUnique();

                entity.Property(e => e.StatusId).HasDefaultValueSql("nextval('\"task_status_StatusId_seq\"'::regclass)");

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("USER");

                entity.HasIndex(e => e.Email, "user_email_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.UserId, "user_userid_uindex")
                    .IsUnique();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Lastname)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.Password).IsRequired();

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.QrSecret).HasMaxLength(64);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_role__fk");
            });

            modelBuilder.Entity<UserGroup>(entity =>
            {
                entity.ToTable("USER_GROUP");

                entity.HasIndex(e => e.Id, "user_group_id_uindex")
                    .IsUnique();

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.UserGroups)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("user_group_group_groupid_fk");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserGroups)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("user_group_user_userid_fk");
            });

            modelBuilder.Entity<UserSecurityQuestion>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("USER_SECURITY_QUESTION");

                entity.Property(e => e.Answer)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<UserTask>(entity =>
            {
                entity.HasKey(e => e.TaskId)
                    .HasName("user_task_pk");

                entity.ToTable("USER_TASK");

                entity.HasIndex(e => e.TaskId, "user_task_taskid_uindex")
                    .IsUnique();

                entity.Property(e => e.TaskId).HasDefaultValueSql("nextval('\"user_task_TaskId_seq\"'::regclass)");

                entity.Property(e => e.EndTime).HasColumnType("timestamp with time zone");

                entity.Property(e => e.StartTime).HasColumnType("timestamp with time zone");

                entity.HasOne(d => d.Device)
                    .WithMany(p => p.UserTasks)
                    .HasForeignKey(d => d.DeviceId)
                    .HasConstraintName("user_task_device_deviceid_fk");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.UserTasks)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_task_task_status_statusid_fk");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserTasks)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_task_user_userid_fk");
            });

            modelBuilder.Entity<UserTracker>(entity =>
            {
                entity.ToTable("USER_TRACKER");

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"user_tracker_Id_seq\"'::regclass)");

                entity.Property(e => e.Time).HasColumnType("timestamp with time zone");

                entity.HasOne(d => d.UserTask)
                    .WithMany(p => p.UserTrackers)
                    .HasForeignKey(d => d.UserTaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_tracker_user_task_taskid_fk");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
