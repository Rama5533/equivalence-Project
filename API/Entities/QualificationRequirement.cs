using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Enums;

namespace API.Entities;

    public class QualificationRequirement
    {
        public int Id{get; set;}

        public QualificationType QualificationType { get; set; }

        public QualificationType RequiredQualificationType { get; set; }
    }
