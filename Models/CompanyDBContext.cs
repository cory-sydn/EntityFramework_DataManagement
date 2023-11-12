using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ExcelToMSSQL.Models;

public partial class CompanyDBContext : DbContext
{
    public CompanyDBContext()
    {
    }

    public CompanyDBContext(DbContextOptions<CompanyDBContext> options) : base(options)
    {
    }

    public virtual DbSet<TblContact> TblContacts { get; set; }

    public virtual DbSet<TblUserList> TblUserLists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:constring");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region spnippedTblContact

        modelBuilder.Entity<TblContact>(entity =>
        {
            entity.ToTable("tbl_Contact");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Address).HasColumnName("Address");
            entity.Property(e => e.Dealer).HasColumnName("Dealer");
            entity.Property(e => e.CallId).HasColumnName("CallID");
            entity.Property(e => e.Debitor)
                .HasMaxLength(40)
                .HasColumnName("Debitor");
            entity.Property(e => e.SourceOfFinance)
                .HasMaxLength(150)
                .HasColumnName("SourceOfFinance");
            entity.Property(e => e.DelayDaysCount)
                .HasMaxLength(5)
                .HasColumnName("DelayDaysCount");
            entity.Property(e => e.UnpaidAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("UnpaidAmount");
            entity.Property(e => e.Guarantor)
                .HasMaxLength(40)
                .HasColumnName("Guarantor");
            entity.Property(e => e.GuarantorAddress).HasColumnName("GuarantorAddress");
            entity.Property(e => e.GuarantorNID)
                .HasMaxLength(11)
                .HasColumnName("GuarantorNID");
            entity.Property(e => e.GuarantorPhoneNum)
                .HasMaxLength(10)
                .HasColumnName("GuarantorPhoneNum");
            entity.Property(e => e.ListId)
                .HasMaxLength(20)
                .HasColumnName("ListID");
            entity.Property(e => e.FileNo)
                .HasMaxLength(10)
                .HasColumnName("FileNo");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(30)
                .HasColumnName("PaymentStatus");          
            entity.Property(e => e.PromisedDate)
                .HasColumnType("date")
                .HasColumnName("PromisedDate");           
            entity.Property(e => e.PaymentDate)
                .HasColumnType("date")
                .HasColumnName("PaymentDate");
            entity.Property(e => e.PaidAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("PaidAmount");
            entity.Property(e => e.AppointDate)
                .HasColumnType("datetime")
                .HasColumnName("AppointDate");
            entity.Property(e => e.SalesChannel)
                .HasMaxLength(20)
                .HasColumnName("SalesChannel");
            entity.Property(e => e.PayorBank)
                .HasMaxLength(150)
                .HasColumnName("PayorBank");            
            entity.Property(e => e.RcMain)
                .HasMaxLength(100)
                .HasColumnName("Rc_Main");
            entity.Property(e => e.RcSub)
                .HasMaxLength(100)
                .HasColumnName("Rc_Sub");
            entity.Property(e => e.LastCallDate)
                .HasColumnType("datetime")
                .HasColumnName("LastCallDate");
            entity.Property(e => e.LastCallAgentID)
                .HasMaxLength(50)
                .HasColumnName("LastCallAgentID");
            entity.Property(e => e.CollectingAgent).HasColumnName("CollectingAgent");
            entity.Property(e => e.NationalID)
                .HasMaxLength(11)
                .HasColumnName("NationalID");
            entity.Property(e => e.TelephoneNum)
                .HasMaxLength(10)
                .HasColumnName("TelephoneNum");
            entity.Property(e => e.CallStatus)
                .HasMaxLength(50)
                .HasColumnName("CallStatus");
            entity.Property(e => e.DueDate)
                .HasColumnType("date")
                .HasColumnName("DueDate");
            entity.Property(e => e.GrossDebt)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("GrossDebt");
            entity.Property(e => e.InsertedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("InsertedDate");
        });
        #endregion

        #region snippedTblUserList  

        modelBuilder.Entity<TblUserList>(entity =>
                {
                    entity.HasKey(e => e.UserId);

                    entity.ToTable("tbl_userList");

                    entity.Property(e => e.UserId).HasColumnName("userID");
                    entity.Property(e => e.Password)
                        .HasMaxLength(10)
                        .HasColumnName("password");
                    entity.Property(e => e.IsPersonelActive).HasColumnName("IsPersonelActive");
                    entity.Property(e => e.ProjectName)
                        .HasMaxLength(50)
                        .HasColumnName("projectName");
                    entity.Property(e => e.UserName)
                        .HasMaxLength(50)
                        .HasColumnName("userName");
                    entity.Property(e => e.IsExecutive).HasColumnName("IsExecutive");
                });
        
        #endregion
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
