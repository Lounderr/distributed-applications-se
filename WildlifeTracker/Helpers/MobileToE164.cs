using PhoneNumbers;

using WildlifeTracker.Constants;
using WildlifeTracker.Exceptions;

namespace WildlifeTracker.Helpers
{
    public static class MobileToE164
    {
        public static string Convert(string mobile)
        {
            PhoneNumberUtil phoneNumberUtil = PhoneNumberUtil.GetInstance();
            PhoneNumber number;

            var invalidPhoneNumberException = new CustomValidationException(ErrorCodes.MobileInvalid, "The phone number is invalid");

            try
            {
                number = phoneNumberUtil.Parse(mobile, null);
            }
            catch
            {
                throw invalidPhoneNumberException;
            }


            if (!phoneNumberUtil.IsValidNumber(number))
            {
                throw invalidPhoneNumberException;
            }

            string e164Number = phoneNumberUtil.Format(number, PhoneNumberFormat.E164);

            if (e164Number == null)
            {
                throw invalidPhoneNumberException;
            }

            return e164Number;
        }
    }
}
