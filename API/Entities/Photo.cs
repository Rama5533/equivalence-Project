using System;
using System.Text.Json.Serialization;

namespace API.Entities
{
    public class Photo
    {
        public int Id { get; set; }

        public required string Url { get; set; }

        public string? PublicId { get; set; }

        //Navigation property
        [JsonIgnore]
        public Applicant Applicant { get; set; }=null!;
        
        public string ApplicantId { get; set; }=null!;

    }
}