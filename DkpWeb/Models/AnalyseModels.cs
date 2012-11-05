using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Austin.DkpLib;

namespace DkpWeb.Models
{
    public class AnalyseModel
    {
        [Required]
        public string LogOutput { get; set; }

        [Required]
        public string ImageBase64 { get; set; }

        [Required]
        public List<Tuple<Person, int>> Debtors { get; set; }
    }
}