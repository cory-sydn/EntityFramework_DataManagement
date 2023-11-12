using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExcelToMSSQL.Models;

public partial class TblUserList
{
    public int? UserId { get; set; }

    public string? UserName { get; set; }
    public string? ProjectName { get; set; }

    [DataType(DataType.Password)]
    public string? Password { get; set; }

    public bool? IsPersonelActive { get; set; }

    public bool? IsExecutive { get; set; }
}
