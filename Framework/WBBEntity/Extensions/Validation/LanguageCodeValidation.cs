using System;
using System.ComponentModel.DataAnnotations;

namespace WBBEntity.Extensions.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true)]
    public class LanguageCodeValidationAttribute : ValidationAttribute
    {
        public string AllowLanguageCode { get; set; }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (value.ToString() == AllowLanguageCode)
            {
                return true;
            }

            return false;
        }
    }
}
