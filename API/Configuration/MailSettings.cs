using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Configuration
{
    public class MailSettings
    {
        public string DisplayName { get; set; }=string.Empty;
        public string From { get; set; }=string.Empty;
        public string UserName { get; set; }=string.Empty;
        public string Password { get; set; }=string.Empty;
        public string Host { get; set; }=string.Empty;
        public int Port { get; set; }
        public bool UseSSL { get; set; }
        public bool UseStartTls { get; set; }
    }
}