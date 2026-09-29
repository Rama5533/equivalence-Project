using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.EquivalencyApplication;

// DTO for updating the third step of an equivalency application خاص ببيانات الشهادة
    public class UpdateApplicationStep3Dto
    {
        public short? CountryId { get; set; }

        public int? InstitutionId { get; set; }

        public int? MajorId { get; set; }

        public int? GraduationYear { get; set; }

        public string? AdditionalNotes { get; set; }

    }
