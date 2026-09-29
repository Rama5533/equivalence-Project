using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using API.Enums;

namespace API.DTOs.EquivalencyApplication;

public class DocumentReviewDto
{
    public int DocumentId { get; set; }

    public DocumentType DocumentType { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public DateTime UploadedAt { get; set; }
}