namespace JournalWork
{
    using System;
    using System.Globalization;

    public class TConvert
    {
        private static object Check(object Obj)
        {
            if (Obj == null)
            {
                Obj = "";
                return Obj;
            }
            if (Obj.ToString() == "undefined")
            {
                Obj = "";
            }
            if (Obj.ToString() == "null")
            {
                Obj = "";
            }
            return Obj;
        }

        public static bool ToBool(object Obj)
        {
            Obj = Check(Obj);
            if (ToInt(Obj) != 1)
            {
                return false;
            }
            return true;
        }

        public static DateTime ToDateTime(object Obj)
        {
            Obj = Check(Obj);
            try
            {
                return Convert.ToDateTime(Obj, new CultureInfo("ru-RU"));
            }
            catch (FormatException)
            {
                return Convert.ToDateTime((Obj.ToString() == "") ? null : Obj.ToString());
            }
        }

        public static object ToDbDateTime(DateTime Obj)
        {
            if (Obj.Year != 1)
            {
                return Obj;
            }
            return DBNull.Value;
        }

        public static double ToDouble(object Obj)
        {
            Obj = Check(Obj);
            double num = 0.0;
            try
            {
                num = Convert.ToDouble((Obj.ToString() == "") ? "0" : Obj.ToString());
            }
            catch
            {
                if (Obj.ToString().IndexOf(".") > 0)
                {
                    Obj = Obj.ToString().Replace(".", ",");
                }
                else
                {
                    Obj = Obj.ToString().Replace(",", ".");
                }
                try
                {
                    num = Convert.ToDouble((Obj.ToString() == "") ? "0" : Obj.ToString());
                }
                catch
                {
                    num = 0.0;
                }
            }
            return num;
        }

        public static decimal ToFloat(object Obj)
        {
            Obj = Check(Obj);
            decimal num = 0M;
            try
            {
                num = Convert.ToDecimal((Obj.ToString() == "") ? "0" : Obj.ToString());
            }
            catch
            {
                if (Obj.ToString().IndexOf(".") > 0)
                {
                    Obj = Obj.ToString().Replace(".", ",");
                }
                else
                {
                    Obj = Obj.ToString().Replace(",", ".");
                }
                try
                {
                    num = Convert.ToDecimal((Obj.ToString() == "") ? "0" : Obj.ToString());
                }
                catch
                {
                    num = 0M;
                }
            }
            return num;
        }

        public static int ToInt(object Obj)
        {
            Obj = Check(Obj);
            return Convert.ToInt32((Obj.ToString() == "") ? 0 : Obj);
        }

        public static long ToLong(object Obj)
        {
            Obj = Check(Obj);
            return Convert.ToInt64((Obj.ToString() == "") ? 0 : Obj);
        }

        public static string ToString(object Obj)
        {
            Obj = Check(Obj);
            return Obj.ToString();
        }
    }
}

