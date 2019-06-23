using System;
using System.ComponentModel.DataAnnotations;

namespace VPPS.CSI.Domain.CustomValidations
{
    public sealed class RestrictedDate : ValidationAttribute
    {
        public override bool IsValid(object date)
        {
            if (date == null)
            {
                return false;
            }

            DateTime beforeCurrentDate = (DateTime)date;
            return beforeCurrentDate <= DateTime.Now;
        }
    }
}