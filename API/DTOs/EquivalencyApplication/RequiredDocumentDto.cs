using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Enums;

namespace API.DTOs.EquivalencyApplication;

public class RequiredDocumentDto
{
    public DocumentType DocumentType { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool Required { get; set; }
}