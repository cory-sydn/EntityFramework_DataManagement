using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExcelToMSSQL.Models;
using ExcelToMSSQL.Services;
using ExcelToMSSQL.Services.Email;
using System.Globalization;
using System.Data;
using System.Text;
using EFCore.BulkExtensions;
using ExcelToMSSQL.Services.ProcessTarfin;

namespace ExcelToMSSQL.Controllers
{
    //[Authorize]
    //[ApiController]
    [Controller]
    [Route("[controller]")]
    public class DataManagerController : Controller
    {

        private readonly CompanyDBContext _dbContext;       
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;
        IExcelDataReader reader;

        public DataManagerController(CompanyDBContext dbContext, IWebHostEnvironment webHostEnvironment, IEmailSender emailSender)
        {
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
        }

        /// <summary>
        /// Gets TarfinKontak items.
        /// </summary>
        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Index(IFormFile postedFile)
        {            
            StringBuilder sb = new();
            // Very important to cement this format here.
            CultureInfo customCulture = new("en-US");
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            List<TblContact> InsertList = new();  // to display excel content                                                           
            List<TblContact> UpdateList = new();  // to display excel content
            ProcessReport report = new();                  // report container

            if (postedFile == null || postedFile.Length == 0)
            {
                sb.AppendLine("FileNo file selected for upload...");
                return View();
            }
            else
            {
                string fileName  = Path.GetFileName(postedFile.FileName);
                string contentType = postedFile.ContentType;
                sb.Append("\r\n File Name : ");
                sb.Append(fileName);
                sb.Append("\r\n Content Type : ");
                sb.Append(contentType);
                
                string dirPath = Path.Combine(_webHostEnvironment.WebRootPath,"uploadFile");
                string extension = Path.GetExtension(fileName);

                string[] allowedExtsnions = new string[] { ".xls", ".xlsx" };

                if (!allowedExtsnions.Contains(extension))
                    throw new Exception("Sorry! This file is not allowed, make sure that file having extension as either.xls or.xlsx is uploaded.");

                ViewData["Title"] = "CompanyDB Contact Data";

                // Store dosya no's from db
                List<RecordOnServer> recordOnServers = new();
                try
                {   //Get Dosya FileNo                    
                    recordOnServers = await _dbContext.TblContacts
                       // .Where(c => c.PaymentStatus == "Bekliyor" || c.PaymentStatus == "Kısmi Ödendi")                    
                        .Select(dbo => new RecordOnServer
                        {
                            No = dbo.FileNo,
                            ID = dbo.Id
                        }).ToListAsync();
                }
                catch(Exception ex) { sb.AppendLine("\r\n Error : " + ex.Message);}
            
                if(Directory.Exists(dirPath)) {Directory.CreateDirectory(dirPath);}

                // Save File Temporarily
                string FilePath = Path.Combine(dirPath, fileName);
                using (FileStream fileStream = new(FilePath, FileMode.Create)) { await postedFile.CopyToAsync(fileStream); }
                // Read Excel File
                using (FileStream fileStream = new(FilePath, FileMode.Open, FileAccess.Read))
                {
                    if (extension == ".xls")
                        reader = ExcelReaderFactory.CreateBinaryReader(fileStream);
                    else
                        reader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);

                    DataSet ds = reader.AsDataSet();
                    reader.Close();

                    if(ds != null && ds.Tables.Count > 0 )
                    {
                        DataTable tarfinTable = ds.Tables[0];
                        for (int i = 1; i < tarfinTable.Rows.Count && recordOnServers != null; i++ )
                        {
                            RecordOnServer record = recordOnServers.FirstOrDefault(r => r.No == tarfinTable.Rows[i][0].ToString().Trim());
                            if (record != null)
                            {
                                TblContact item = new()
                                {
                                    FileNo = tarfinTable.Rows[i][0].ToString().Trim(),
                                    Id = record.ID,                                  
                                    PaidAmount = tarfinTable.Rows[i][10].ToString().CleanMoney(),
                                    UnpaidAmount = tarfinTable.Rows[i][9].ToString().CleanMoney(),                                   
                                    PaymentStatus = tarfinTable.Rows[i][14].ToString().Trim().PaidStatement(),
                                };
                                sb.Append(item.DueDate);

                                PrepareReport.ProcessCounter(report, item);

                                UpdateList.Add(item);
                            }
                            else
                            {
                                TblContact item = new()
                                {
                                    InsertedDate = DateTime.Now,
                                    ListId = "LegalProceedings",
                                    FileNo = tarfinTable.Rows[i][0].ToString().Trim(),
                                    Dealer = tarfinTable.Rows[i][1].ToString().Trim().CleanName(),
                                    SalesChannel = "NULL",
                                    Debitor = tarfinTable.Rows[i][3].ToString().Trim().CleanName(),
                                    NationalID = tarfinTable.Rows[i][4].ToString().Trim(),
                                    TelephoneNum = tarfinTable.Rows[i][5].ToString().Trim().Clean(),
                                    Address = tarfinTable.Rows[i][13].ToString().Trim().CleanName(),
                                    Guarantor = "NULL",
                                    GuarantorNID = "NULL",
                                    GuarantorPhoneNum = "NULL",
                                    GuarantorAddress = "NULL",
                                    SourceOfFinance = "NULL",
                                    PayorBank = String.IsNullOrEmpty(tarfinTable.Rows[i][6].ToString()) ? "NULL" : tarfinTable.Rows[i][6].ToString().CleanName(),
                                    CollectingAgent = String.IsNullOrEmpty(tarfinTable.Rows[i][7].ToString()) ? "NULL" : tarfinTable.Rows[i][7].ToString().CleanName(),                                   
                                    GrossDebt = tarfinTable.Rows[i][9].ToString().CleanMoney() + tarfinTable.Rows[i][10].ToString().CleanMoney(),
                                    PaidAmount = tarfinTable.Rows[i][10].ToString().CleanMoney(),
                                    UnpaidAmount = tarfinTable.Rows[i][9].ToString().CleanMoney(),
                                    DueDate = tarfinTable.Rows[i][12].ToString().Trim().SortableDate(),
                                    PaymentStatus = tarfinTable.Rows[i][14].ToString().Trim().PaidStatement(),
                                    CallStatus = "Not Called"
                                };

                                PrepareReport.ProcessCounter(report, item);

                                InsertList.Add(item);
                            };

                        };

                        try
                        {
                            // INSERT
                            await _dbContext.BulkInsertAsync(InsertList);

                            // UPDATE                           
                            var bulkConfig = new BulkConfig { PropertiesToIncludeOnUpdate = new List<string> { "PaidAmount", "UnpaidAmount", "PaymentStatus" } };
                            _dbContext.BulkUpdate(UpdateList, bulkConfig);
                        }
                        catch(DbUpdateException dbE)
                        {
                            sb.Append(dbE.ToString());
                        }
                    };
                }
                StringBuilder messageString = new();
                messageString.Append("Merhaba," + "</br></br>" + fileName + " adlı excel dosyası Tarfin MAP veritabanına yüklenmiştir. </br> Yüklenen yeni data sayısı " + InsertList.Count + " adet, güncellenen data sayısı " + UpdateList.Count + " adet olmuştur." + "</br></br>" + "İlgili datalara ait ödeme durumu özeti aşağıdaki gibidir: </br></br>");
                messageString.Append("<table style='border-style:ridge; border-collapse: collapse; border: 1px solid black; padding: 10px' >\r\n <tbody>\r\n <tr>\r\n <td style='border-style:ridge; border-collapse: collapse; border: 1px solid black;padding-right: 30px;'>Ödendi</td>\r\n <td style='border-style:ridge; padding-left: 95px; border-collapse: collapse; border: 1px solid black;  align-text:end;'>" + report.PaidCount.ToString());
                messageString.Append("</td>\r\n </tr>\r\n <tr>\r\n <td style='padding-right: 30px; border-collapse: collapse; border: 1px solid black;'>Kısmi Ödendi</td>\r\n <td style='border-style:ridge; padding-left: 95px; border-collapse: collapse; border: 1px solid black; align-text:end;'>" + report.PartialPCount.ToString() + "</td>\r\n </tr>\r\n <tr>\r\n <td style='padding-right: 30px; border-collapse: collapse; border: 1px solid black;'>Bekliyor</td>\r\n <td style='border-style:ridge; padding-left: 95px; border-collapse: collapse; border: 1px solid black; align-text:end;'>" + report.UnPaidCount.ToString() + "</td>\r\n </tr>");
                messageString.Append("\r\n <tr>\r\n <td style='border-style:ridge; border-collapse: collapse; border: 1px solid black;padding-right: 30px;'>Yasal Takip</td>\r\n <td style='border-style:ridge; padding-left: 95px; border-collapse: collapse; border: 1px solid black;  align-text:end;'>" + report.LegalProceedings + "</td> </tr>\r\n </tbody>\r\n</table>");
                messageString.Append("</br>" + "İyi çalışmalar dilerim." + "</br></br></br>");

                var message = new Message( new string[] {"koray.soydan@procat.com.tr"}, "TEST Tarfin Idari Takip Data Yüklemesi", messageString.ToString());
                _emailSender.SendEmail(message);
            }

            ViewData["message"] = sb.ToString();
            return View(InsertList);
        }
    }
}