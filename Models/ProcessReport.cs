using System.ComponentModel;

namespace ExcelToMSSQL.Models
{
    public class ProcessReport
    {
        [DefaultValue(0)]
        public int PaidCount { set;get;}

        [DefaultValue(0)]
        public int PartialPCount { set;get;}

        [DefaultValue(0)]
        public int UnPaidCount { set;get;}

        [DefaultValue(0)]
        public int LegalProceedings { set; get; }
    }
}