using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace WBBEntity.Extensions.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    sealed public class DateTimeFormatValidationAttribute : ValidationAttribute
    {
        public string FormatDate { get; set; }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            var dateTime = DateTime.Now;
            if (DateTime.TryParseExact(value.ToString(), FormatDate,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
            {
                return true;
            }

            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture,
              ErrorMessageString, name);
        }
    }
}
