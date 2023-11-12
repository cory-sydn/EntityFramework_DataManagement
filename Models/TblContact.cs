using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExcelToMSSQL.Models;

public partial class TblContact
{
    public int Id { get; set; }

    public DateTime? InsertedDate { get; set; }

    public string? ListId { get; set; }

    public string? FileNo { get; set; }

    public string? Dealer { get; set; }

    public string? SalesChannel { get; set; }

    public string? Debitor { get; set; }

    public string? NationalID { get; set; }

    public string? TelephoneNum { get; set; }

    public string? Address { get; set; }

    public string? Guarantor { get; set; }

    public string? GuarantorNID{ get; set; }

    public string? GuarantorPhoneNum { get; set; }

    public string? GuarantorAddress { get; set; }

    public decimal? GrossDebt { get; set; }

    public decimal? PaidAmount { get; set; }

    public decimal? UnpaidAmount { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DueDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime? PaymentDate { get; set; }

    public string? DelayDaysCount { get; set; }

    public string? PaymentStatus { get; set; }

    public string? SourceOfFinance { get; set; }

    public string? PayorBank { get; set; }

    public string? CollectingAgent { get; set; }    

    public int? CallId { get; set; }

    public string? CallStatus { get; set; }

    public string? RcMain { get; set; }

    public string? RcSub { get; set; }

    public DateTime? LastCallDate { get; set; }

    public string? LastCallAgentID { get; set; }

    public DateTime? AppointDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime? PromisedDate { get; set; }
}
