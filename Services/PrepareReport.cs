using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Index.HPRtree;
using ExcelToMSSQL.Models;

namespace ExcelToMSSQL.Services.ProcessTarfin
{
    public class PrepareReport
    {        
        public static ProcessReport ProcessCounter(ProcessReport report, TblContact item)
        {
            _ = item.OdemeDurumu == "Tahsil Edildi"
                ? report.PaidCount++
                : item.OdemeDurumu == "Kısmi Ödendi"
                ? report.PartialPCount++
                : item.OdemeDurumu == "Yasal Takip"
                ? report.LegalProceedings++
                : report.UnPaidCount++;

            return report;            
        }

      
    }
}
