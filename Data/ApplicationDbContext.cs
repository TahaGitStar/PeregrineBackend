using Microsoft.EntityFrameworkCore;
using PeregrineBackend.API.Models;

namespace PeregrineBackend.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ContractType> ContractTypes { get; set; }
        public DbSet<Guard> Guards { get; set; }
        public DbSet<WorkSchedule> WorkSchedules { get; set; }
        public DbSet<LeaveDay> LeaveDays { get; set; }
        public DbSet<Accident> Accidents { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // تكوين العلاقات بين الكيانات
            
            // علاقة المستخدم بالأدوار
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // علاقة الفرع بالعقود
            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Branch)
                .WithMany(b => b.Contracts)
                .HasForeignKey(c => c.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // علاقة العقد بالحراس
            modelBuilder.Entity<Guard>()
                .HasOne(g => g.Contract)
                .WithMany(c => c.Guards)
                .HasForeignKey(g => g.ContractId)
                .OnDelete(DeleteBehavior.Restrict);

            // علاقة الحارس بجداول العمل
            modelBuilder.Entity<WorkSchedule>()
                .HasOne(ws => ws.Guard)
                .WithMany(g => g.Schedule)
                .HasForeignKey(ws => ws.GuardId)
                .OnDelete(DeleteBehavior.Cascade);

            // علاقة الحارس بالإجازات
            modelBuilder.Entity<LeaveDay>()
                .HasOne(ld => ld.Guard)
                .WithMany(g => g.LeaveDays)
                .HasForeignKey(ld => ld.GuardId)
                .OnDelete(DeleteBehavior.Cascade);

            // علاقة الحارس البديل بالإجازة
            modelBuilder.Entity<LeaveDay>()
                .HasOne(ld => ld.ReplacementGuard)
                .WithMany()
                .HasForeignKey(ld => ld.ReplacementId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // علاقة الحادث بالحارس
            modelBuilder.Entity<Accident>()
                .HasOne(a => a.Guard)
                .WithMany(g => g.Accidents)
                .HasForeignKey(a => a.GuardId)
                .OnDelete(DeleteBehavior.Restrict);

            // علاقة الحادث بالعقد
            modelBuilder.Entity<Accident>()
                .HasOne(a => a.Contract)
                .WithMany(c => c.Accidents)
                .HasForeignKey(a => a.ContractId)
                .OnDelete(DeleteBehavior.Restrict);

            // علاقة التعليق بالحادث
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Accident)
                .WithMany(a => a.Comments)
                .HasForeignKey(c => c.AccidentId)
                .OnDelete(DeleteBehavior.Cascade);

            // إعداد البيانات الأولية
            SeedInitialData(modelBuilder);
        }

        private void SeedInitialData(ModelBuilder modelBuilder)
        {
            // أنواع العقود الأساسية
            modelBuilder.Entity<ContractType>().HasData(
                new ContractType { Id = 1, Name = "security", ArabicName = "خدمة حراسة", IconName = "shield" },
                new ContractType { Id = 2, Name = "personal", ArabicName = "حماية شخصية", IconName = "user_shield" },
                new ContractType { Id = 3, Name = "event", ArabicName = "تأمين فعاليات", IconName = "calendar_check" },
                new ContractType { Id = 4, Name = "consultation", ArabicName = "استشارات أمنية", IconName = "file_text" },
                new ContractType { Id = 5, Name = "other", ArabicName = "خدمات أخرى", IconName = "more_horizontal" }
            );

            // مستخدم افتراضي (مدير النظام)
            var adminId = new Guid("11111111-1111-1111-1111-111111111111");
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = adminId,
                    Username = "admin",
                    DisplayName = "مدير النظام",
                    Email = "admin@peregrine.com",
                    PasswordHash = "AQAAAAIAAYagAAAAELTUuYFE5qkVQxPpvxdQJHN+Pn7ZZzH3zGwYnOkKcbbHsLLLlzwEehYYnuHQrYkCdA==", // "Admin@123"
                    Role = "admin",
                    LastLogin = new DateTime(2025, 1, 1),
                    IsActive = true
                }
            );

            // دور المدير
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole
                {
                    Id = 1,
                    UserId = adminId,
                    RoleName = "admin",
                    Permissions = "{\"canManageUsers\":true,\"canManageBranches\":true,\"canManageContracts\":true,\"canManageGuards\":true,\"canApproveLeaves\":true,\"canViewReports\":true}"
                }
            );
        }
    }
}
