using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload.Common
{
    public class FormDto
    {
        public string FormTitle { get; set; }
        public string Url { get; set; }
        public string RequestBody { get; set; }

        public string HttpMethord { get; set; }

        public List<HeaderDto> Headers { get; set; }

        public string FilePath { get; set; }
        public bool IsSendFile { get; set; }
    }

    public class HeaderDto
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
