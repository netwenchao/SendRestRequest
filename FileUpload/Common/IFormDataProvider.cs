using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload.Common
{
    public interface IFormDataProvider
    {
        FormDto GetFormData();
        void ApplyFormData(FormDto dto);
    }
}
