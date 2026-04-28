using System;
using System.Collections.Generic;
using AccessControl.Model.MapModels;
using AccessControl.Model.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AccessControl.Data
{
    public partial class ACSDBContext : IdentityDbContext<IdentityUser>
    {
        public ACSDBContext()
        {
        }

        public ACSDBContext(DbContextOptions<ACSDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AppGroup> AppGroups { get; set; }
        public virtual DbSet<AppRole> AppRoles { get; set; }
        public virtual DbSet<AppRoleGroup> AppRoleGroups { get; set; }
        public virtual DbSet<AppUser> AppUsers { get; set; }
        public virtual DbSet<AppUserGroup> AppUserGroups { get; set; }
        public virtual DbSet<AppUserRole> AppUserRoles { get; set; }
        public virtual DbSet<CardNo> CardNos { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<HeaderExcel> HeaderExcels { get; set; }
        public virtual DbSet<RealtimeMonitor> RealtimeMonitors { get; set; }
        public virtual DbSet<Regency> Regencies { get; set; }

        //
        public virtual DbSet<AppMenuMappingSQL> GetMenuList { get; set; }
        public virtual DbSet<EmployeeStatisticMapping> EmployeeStatisticMappings { get; set; }
        public virtual DbSet<CountResult> CountResults { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=192.168.10.222;Initial Catalog=DB_ACS;User ID=sa;Password=123456;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUserGroup>().ToTable("AppUserGroups").HasKey(x => new { x.UserId, x.GroupId });
            modelBuilder.Entity<AppRoleGroup>().ToTable("AppRoleGroups").HasKey(x => new { x.RoleId, x.GroupId });
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AppUserRoles").HasKey(i => new { i.UserId, i.RoleId });
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("AppUserLogins").HasKey(i => i.UserId);
            modelBuilder.Entity<IdentityRole>().ToTable("AppRoles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("AppUserClaims").HasKey(i => i.UserId);
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("AppUserTokens").HasKey(i => i.UserId);
            modelBuilder.Entity<IdentityUser<string>>().ToTable("AppUsers").HasKey(i => i.Id);

            modelBuilder.Entity<CardNo>(entity =>
            {
                entity.HasKey(e => e.CaId)
                    .HasName("PK_card_no21");

                entity.ToTable("CARD_NO");

                entity.Property(e => e.CaId).HasColumnName("CA_ID");

                entity.Property(e => e.CaNo)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("CA_NO");

                entity.Property(e => e.CaNumber)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("CA_NUMBER");

                entity.Property(e => e.CaStatus).HasColumnName("CA_STATUS");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(128)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EmId).HasColumnName("EM_ID");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(128)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");

                entity.Property(e => e.Using)
                    .HasColumnName("USING")
                    .HasComment("thẻ vẫn của người hiện tại thì using = true, thu hồi thẻ và cấp mới cho người khác thì người cũ = false, người mới = true");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.DepId)
                    .HasName("PK_T_DEPARTMENT");

                entity.ToTable("DEPARTMENT");

                entity.Property(e => e.DepId).HasColumnName("DEP_ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(128)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DepDescription)
                    .HasMaxLength(200)
                    .HasColumnName("DEP_DESCRIPTION");

                entity.Property(e => e.DepName)
                    .HasMaxLength(100)
                    .HasColumnName("DEP_NAME");

                entity.Property(e => e.DepStatus)
                    .HasColumnName("DEP_STATUS")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(128)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");

                entity.Property(e => e.DeleteBy)
                    .HasMaxLength(128)
                    .HasColumnName("DELETE_BY");

                entity.Property(e => e.DeleteDate)
                    .HasColumnType("datetime")
                    .HasColumnName("DELETE_DATE");
            });

            modelBuilder.Entity<Device>(entity =>
            {
                entity.HasKey(e => e.DevId)
                    .HasName("PK_A_DEVICE");

                entity.ToTable("DEVICE");

                entity.Property(e => e.DevId).HasColumnName("DEV_ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(128)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DevIp)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("DEV_IP");

                entity.Property(e => e.DevLastTime)
                    .HasPrecision(0)
                    .HasColumnName("DEV_LAST_TIME");

                entity.Property(e => e.DevMacaddress)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DEV_MACADDRESS");

                entity.Property(e => e.DevName)
                    .HasMaxLength(100)
                    .HasColumnName("DEV_NAME");

                entity.Property(e => e.DevPort).HasColumnName("DEV_PORT");

                entity.Property(e => e.DevSerialnumber)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DEV_SERIALNUMBER");

                entity.Property(e => e.DevStatus)
                    .HasColumnName("DEV_STATUS")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Online)
                    .HasColumnName("ONLINE")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.OnlineTime)
                    .HasColumnType("datetime")
                    .HasColumnName("ONLINE_TIME");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(128)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EmId)
                    .HasName("PK_T_EMPLOYEE");

                entity.ToTable("EMPLOYEE");

                entity.Property(e => e.EmId).HasColumnName("EM_ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(128)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DepId).HasColumnName("DEP_ID");

                entity.Property(e => e.DevIdSynchronized).HasColumnName("DEV_ID_SYNCHRONIZED");

                entity.Property(e => e.EditStatus)
                    .HasColumnName("EDIT_STATUS")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.EmAddress)
                    .HasMaxLength(400)
                    .HasColumnName("EM_ADDRESS");

                entity.Property(e => e.EmBirthdate)
                    .HasColumnType("date")
                    .HasColumnName("EM_BIRTHDATE");

                entity.Property(e => e.EmCode)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("EM_CODE");

                entity.Property(e => e.EmEmail)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("EM_EMAIL");

                entity.Property(e => e.EmGender)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("EM_GENDER")
                    .HasDefaultValueSql("((1))")
                    .IsFixedLength();

                entity.Property(e => e.EmIdentityNumber)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("EM_IDENTITY_NUMBER");

                entity.Property(e => e.EmImage).HasColumnName("EM_IMAGE");

                entity.Property(e => e.EmName)
                    .HasMaxLength(100)
                    .HasColumnName("EM_NAME");

                entity.Property(e => e.EmPhone)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("EM_PHONE");

                entity.Property(e => e.EmStatus)
                    .HasColumnName("EM_STATUS")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.RegId).HasColumnName("REG_ID");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(128)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");

                entity.Property(e => e.DeleteBy)
                    .HasMaxLength(128)
                    .HasColumnName("DELETE_BY");

                entity.Property(e => e.DeleteDate)
                    .HasColumnType("datetime")
                    .HasColumnName("DELETE_DATE");
            });

            modelBuilder.Entity<HeaderExcel>(entity =>
            {
                entity.ToTable("HEADER_EXCEL");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CompanyAddress)
                    .HasMaxLength(500)
                    .HasColumnName("COMPANY_ADDRESS");

                entity.Property(e => e.CompanyName)
                    .HasMaxLength(500)
                    .HasColumnName("COMPANY_NAME");

                entity.Property(e => e.HeightLogo).HasColumnName("HEIGHT_LOGO");

                entity.Property(e => e.Logo)
                    .HasMaxLength(50)
                    .HasColumnName("LOGO");

                entity.Property(e => e.MarginLogo).HasColumnName("MARGIN_LOGO");

                entity.Property(e => e.TitleAddress)
                    .HasMaxLength(100)
                    .HasColumnName("TITLE_ADDRESS");

                entity.Property(e => e.WidthLogo).HasColumnName("WIDTH_LOGO");
            });

            modelBuilder.Entity<RealtimeMonitor>(entity =>
            {
                entity.HasKey(e => e.RtId)
                    .HasName("PK_T_REALTIME_MONITOR");

                entity.ToTable("REALTIME_MONITOR");

                entity.Property(e => e.RtId).HasColumnName("RT_ID");

                entity.Property(e => e.CaId).HasColumnName("CA_ID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DevId).HasColumnName("DEV_ID");

                entity.Property(e => e.EmId).HasColumnName("EM_ID");

                entity.Property(e => e.IoStatus)
                    .HasColumnName("IO_STATUS")
                    .HasComment("trạng thái vào - ra 0: KHÔNG PHÂN BIỆT, 1: VÀO , 2: RA ");

                entity.Property(e => e.TacId).HasColumnName("TAC_ID");

                entity.Property(e => e.TatDate)
                    .HasColumnType("date")
                    .HasColumnName("TAT_DATE");

                entity.Property(e => e.TatTime)
                    .HasColumnType("time(0)")
                    .HasColumnName("TAT_TIME");
            });

            modelBuilder.Entity<Regency>(entity =>
            {
                entity.HasKey(e => e.RegId)
                    .HasName("PK_T_REGENCY");

                entity.ToTable("REGENCY");

                entity.Property(e => e.RegId).HasColumnName("REG_ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(128)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.RegDescription)
                    .HasMaxLength(200)
                    .HasColumnName("REG_DESCRIPTION");

                entity.Property(e => e.RegName)
                    .HasMaxLength(100)
                    .HasColumnName("REG_NAME");

                entity.Property(e => e.RegStatus)
                    .HasColumnName("REG_STATUS")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(128)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");

                entity.Property(e => e.DeleteBy)
                    .HasMaxLength(128)
                    .HasColumnName("DELETE_BY");

                entity.Property(e => e.DeleteDate)
                    .HasColumnType("datetime")
                    .HasColumnName("DELETE_DATE");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
