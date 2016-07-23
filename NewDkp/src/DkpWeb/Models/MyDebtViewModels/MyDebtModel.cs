using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Austin.DkpLib;
using Microsoft.AspNetCore.Html;

namespace DkpWeb.Models.MyDebtViewModels
{
    public class MyDebtModel
    {
        [Required]
        public Person Person { get; set; }

        [Required]
        public HtmlString ImageSvg { get; set; }

        [Required]
        public List<Tuple<Person, int>> Creditors { get; set; }

        [Required]
        public int OverallDebt { get; set; }
    }

}