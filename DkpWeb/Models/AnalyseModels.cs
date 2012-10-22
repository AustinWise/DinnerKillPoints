using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DkpWeb.Models
{
    public class AnalyseModel
    {
        [Required]
        public string LogOutput { get; set; }

        [Required]
        public string ImageBase64 { get; set; }
    }
}