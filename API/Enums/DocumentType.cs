using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Enums;

public enum DocumentType
{
    PersonalPhoto = 1,
    PassportOrApprovedId = 2,

    BachelorCertificate = 3,
    BachelorTranscript = 4,

    MasterCertificate = 5,
    MasterTranscript = 6,

    doctorateCertificate = 7,
    doctorateTranscript = 8
}