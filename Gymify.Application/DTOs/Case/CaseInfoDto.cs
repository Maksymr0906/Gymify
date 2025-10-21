using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymify.Application.DTOs.Case;

public class CaseInfoDto
{
    public Guid CaseId { get; set; } = Guid.Empty;
    public string CaseName { get; set; } = string.Empty;
    public string CaseDescription { get; set; } = string.Empty;
    public string CaseImageUrl { get; set; } = string.Empty;
}
