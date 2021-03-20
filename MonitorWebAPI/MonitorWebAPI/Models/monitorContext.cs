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
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserGroup> UserGroups { get; set; }
        public virtual DbSet<UserTask> UserTasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=167.99.244.168;Database=monitor;Username=si-baza;Password=sipassword2021");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "C.UTF-8");

            modelBuilder.Entity<Device>(entity =>
            {
                entity.ToTable("DEVICE");

                entity.HasIndex(e => new { e.Name, e.Location }, "DEVICE_Name_Location_key")
                    .IsUnique();

                entity.HasIndex(e => e.DeviceId, "device_deviceid_uindex")
                    .IsUnique();

                entity.Property(e => e.DeviceId).HasDefaultValueSql("nextval('\"device_DeviceId_seq\"'::regclass)");

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

                entity.Property(e => e.TimeStamp).HasColumnType("timestamp with time zone");

                entity.HasOne(d => d.Device)
                    .WithMany()
                    .HasForeignKey(d => d.DeviceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("device_status_log_device_deviceid_fk");
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

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("ROLE");

                entity.HasIndex(e => e.RoleId, "role_roleid_uindex")
                    .IsUnique();

                entity.Property(e => e.RoleId).HasDefaultValueSql("nextval('\"role_RoleId_seq\"'::regclass)");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("USER");

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

            modelBuilder.Entity<UserTask>(entity =>
            {
                entity.HasKey(e => e.TaskId)
                    .HasName("user_task_pk");

                entity.ToTable("USER_TASK");

                entity.HasIndex(e => e.TaskId, "user_task_taskid_uindex")
                    .IsUnique();

                entity.Property(e => e.TaskId).HasDefaultValueSql("nextval('\"user_task_TaskId_seq\"'::regclass)");

                entity.Property(e => e.Time).HasColumnType("timestamp with time zone");

                entity.HasOne(d => d.Device)
                    .WithMany(p => p.UserTasks)
                    .HasForeignKey(d => d.DeviceId)
                    .HasConstraintName("user_task_device_deviceid_fk");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserTasks)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_task_user_userid_fk");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
