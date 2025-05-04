using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

using PhoneNumbers;

namespace WildlifeTracker.Helpers.DataAnotations
{
    /// <summary>
    /// Verify the phone number is international, valid and follows the E164 format. 
    /// Transform to E164 if possible
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = false)]
    public sealed class E164FormatValidatorAttribute : DataTypeAttribute
    {

        public E164FormatValidatorAttribute()
            : base(DataType.PhoneNumber)
        {
            this.ErrorMessage = "Phone number is not valid.";
        }

        public override bool IsValid(object? value)
        {
            if (value is not string valueAsString)
            {
                this.ErrorMessage = "Phone number must be a string.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(valueAsString))
            {
                this.ErrorMessage = "Phone number cannot be empty.";
                return false;
            }

            valueAsString = valueAsString.Trim();

            if (!valueAsString.StartsWith('+'))
            {
                this.ErrorMessage = "Phone number must start with a '+' sign.";
                return false;
            }

            try
            {
                PhoneNumberUtil phoneUtil = PhoneNumberUtil.GetInstance();
                var phone = phoneUtil.Parse(valueAsString, null);

                if (!phoneUtil.IsValidNumber(phone))
                {
                    return false;
                }

                var e164Mobile = phoneUtil.Format(phone, PhoneNumberFormat.E164);

                if (string.IsNullOrWhiteSpace(e164Mobile))
                {
                    this.ErrorMessage = "Phone number is not in international E.164 format.";
                    return false;
                }

                Regex e164ValidRegex = new(@"^\+[1-9]\d{1,14}$");

                return e164ValidRegex.IsMatch(e164Mobile);
            }
            catch (NumberParseException)
            {
                return false;
            }
        }
    }
}
