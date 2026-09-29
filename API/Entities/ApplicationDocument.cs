using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Enums;

namespace API.Entities;

public class ApplicationDocument
{
    public int Id { get; set; }

    // The application this document belongs to
    public int EquivalencyApplicationId { get; set; }

    public EquivalencyApplication EquivalencyApplication { get; set; } = null!;

    // Type of document
    public DocumentType DocumentType { get; set; }

    // File information
    public required string Url { get; set; }

    public string? PublicId { get; set; }

    public string? FileName { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}