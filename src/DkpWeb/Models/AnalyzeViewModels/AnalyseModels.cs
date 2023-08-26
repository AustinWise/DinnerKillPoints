using Austin.DkpLib;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DkpWeb.Models.AnalyzeViewModels
{
    public class AnalyseModel
    {
        [Required]
        public string LogOutput { get; set; }

        [Required]
        public HtmlString ImageSvg { get; set; }

        [Required]
        public List<Tuple<Person, Money>> Debtors { get; set; }
    }
}