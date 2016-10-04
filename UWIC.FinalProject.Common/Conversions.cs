using System;

namespace UWIC.FinalProject.Common
{
    public static class Conversions
    {
        /// <summary>
        /// This method will convert an enum value to an integer and return in
        /// </summary>
        /// <param name="value">Enum value</param>
        /// <returns>integer value of Enum</returns>
        public static int ConvertEnumToInt(Enum value)
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// This method is used to convert an integer to an Enum of type T
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="value">integer value</param>
        /// <returns>Enum value of type T</returns>
        public static T ConvertIntegerToEnum<T>(int value) where T : struct
        {
            try
            {
                if (!typeof(T).IsEnum)
                    throw new ArgumentException("T must be an enumerated type");
                return (T)Enum.ToObject(typeof(T), value);
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// This method is used to convert an string to an Enum of type T
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="value">string value</param>
        /// <returns>Enum value of type T</returns>
        public static T ConvertStringToEnum<T>(string value) where T : struct
        {
            try
            {
                if (!typeof(T).IsEnum)
                    throw new ArgumentException("T must be an enumerated type");
                T enumValue;
                if (Enum.TryParse(value, out enumValue))
                    return enumValue;
                throw new Exception("Could not convert the value :" + value + " to the enum type of : " +
                                    typeof (T));
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }
    }
}
