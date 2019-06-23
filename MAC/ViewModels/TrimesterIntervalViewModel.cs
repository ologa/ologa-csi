using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MAC.ViewModels
{
    public class TrimesterIntervalViewModel
    {
        [DisplayName("Trimestre")]
        public int SequenceID { get; set; }
    }
}
