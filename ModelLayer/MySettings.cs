using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer
{
    public class MySettingsModel
    {
        public string DbConnection { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string SMTPPort { get; set; }
        public string Host { get; set; }
    }
}
