using System;
using System.Globalization;

namespace RevAssuranceApi.OperationImplemention
{
    public class Formatter
    {
        public string FormattedAmount(decimal amount)
        {
            return amount.ToString("N2", CultureInfo.InvariantCulture);
        }
      
        public string FormatToDateYearMonthDay(string Param)
        {
            DateTime decChck = DateTime.Now;
            try
            {
                string con = string.Empty;
                DateTime val = Param == null ? decChck : DateTime.TryParse(Param, out decChck) ? decChck : decChck;
                if(val != null){
                    
                    con = string.Format("{0: yyyy-MM-dd}", val);
                }
                 
                return con;
            }
            catch (Exception ex)
            {
                var exM = ex == null ? ex.InnerException.Message : ex.Message;
                return string.Empty;
            }
        }
    public string FormatTransDate(DateTime? dt)
    {
        if (dt != null)
        {
            return string.Format("{0:dd-MMM-yyyy}", dt);
        }
        return null;
    }
         public string RemoveLast(string txt)
        {
            if (!string.IsNullOrWhiteSpace(txt))
            {
                var getLast = txt.Substring(txt.Length - 1);
                if (getLast != null)
                {
                    if (getLast == ",")
                    {
                        var rem = txt.Substring(0, txt.Length - 1);
                        return rem;
                    }
                }
            }
         

            return txt;
        }
    
          public int ValIntergers(string Param)
        {
            try
            {
                int intChck = 0;
                int val = Param == null ? 0 : int.TryParse(Param, out intChck) ? intChck : intChck;

                return val;
            }
            catch (Exception ex)
            {
                var exM = ex == null ? ex.InnerException.Message : ex.Message;
                return 0;
            }
        }

        public decimal ValDecimal(string Param)
        {
            try
            {
                decimal decChck = 0;
                decimal val = Param == null ? 0 : decimal.TryParse(Param, out decChck) ? decChck : decChck;

                return val;
            }
            catch (Exception ex)
            {
                var exM = ex == null ? ex.InnerException.Message : ex.Message;
                return 0;
            }
        }

        public long ValLong (string Param)
        {
            try
            {
                long decChck = 0;
                long val = Param == null ? 0 : long.TryParse(Param, out decChck) ? decChck : decChck;

                return val;
            }
            catch (Exception ex)
            {
                var exM = ex == null ? ex.InnerException.Message : ex.Message;
                return 0;
            }
        }

        public DateTime ValidateDate(string Param)
        {
            DateTime decChck = DateTime.Now;
            try
            {
                
                DateTime val = Param == null ? decChck : DateTime.TryParse(Param, out decChck) ? decChck : decChck;

                return val;
            }
            catch (Exception ex)
            {
                var exM = ex == null ? ex.InnerException.Message : ex.Message;
                return decChck;
            }
        }

        public string ValidateDateReturnString(string Param)
        {
            DateTime decChck = DateTime.Now;
            try
            {
                
                DateTime val = Param == null ? decChck : DateTime.TryParse(Param, out decChck) ? decChck : decChck;
                    return string.Format("{0:yyyyMMdd}", val);
               
            }
            catch (Exception ex)
            {
                var exM = ex == null ? ex.InnerException.Message : ex.Message;
                return null;
            }
        }

    

        public DateTime? ValidateDateReturnNull(string Param)
        {
           
            try
            {
                DateTime val = Convert.ToDateTime(Param);
                return val;
            }
            catch (Exception ex)
            {
                var exM = ex == null ? ex.InnerException.Message : ex.Message;
                return null;
            }
        }

        public string ReturnNull(string valValue)
        {
            if (string.IsNullOrWhiteSpace(valValue))
            {
                return null; 
            }

            return valValue;
        }

        public string ReturnValidActtno(string acctno, string BranchCode)
        {
            if (!string.IsNullOrWhiteSpace(acctno))
            {
                if (acctno.Contains("***"))
                {
                    string act = acctno.Replace("***", BranchCode);
                    acctno = act;
                    return acctno;
                }
                
            }

            return acctno;
        }

        public string GetfirstValue(string value)
        {
            if (value != null)
            {
                return value.Substring(0, 1);
            }
            else
            {
                return value;
            }
        }

        public string FormatDateCurrProcessing(DateTime? dt)
        {
            if (dt != null)
            {
                return string.Format("{0:yyyy-MM-dd }", dt);
            }
            return null;
        }


        public string AddComma(string value){
            string val = string.Empty;
            val += value  + ",";
            value = val;

            return value;
        }
    }
}