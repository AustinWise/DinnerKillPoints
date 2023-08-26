using Austin.DkpLib;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DkpWeb.Models.MyDebtViewModels
{
    public class MyDebtModel
    {
        [Required]
        public Person Person { get; set; }

        [Required]
        public HtmlString ImageSvg { get; set; }

        [Required]
        public List<Tuple<Person, Money>> Creditors { get; set; }

        [Required]
        public Money OverallDebt { get; set; }
    }

}