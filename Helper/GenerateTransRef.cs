using System;

namespace RevAssuranceApi.Helper
{
    public class GenerateTransRef
    {
           public string TransRef()   {
            //Get the bank 
            string bankcode = "07P";

            DateTime date = DateTime.Now;

            string year = date.Year.ToString().Substring(2, 2);

            string month = date.Month.ToString();

            int _month = int.Parse(month);
            if (_month < 10 && month.Length == 1)
            {
                month = "0" + _month.ToString();
            }

            string day = date.Day.ToString();

            int _day = int.Parse(day);
            if (_day < 10 && day.Length == 1)
            {
                day = "0" + _day.ToString();
            }

            string hour = date.Hour.ToString();

            int _hour = int.Parse(hour);
            if (_hour < 10 && hour.Length == 1)
            {
                hour = "0" + _hour.ToString();
            }

            string min = date.Minute.ToString();

            int _min = int.Parse(min);
            if (_min < 10 && min.Length == 1)
            {
                min = "0" + _min.ToString();
            }

            string sec = date.Second.ToString();

            int _sec = int.Parse(sec);
            if (_sec < 10 && sec.Length == 1)
            {
                sec = "0" + _sec.ToString();
            }

            string transDate = year + month + day + hour + min + sec;

            var rand = new Random();
            string part1 = rand.Next(1234, 3241).ToString();
            string part2 = rand.Next(3242, 4232).ToString();
            string part3 = rand.Next(4233, 9535).ToString();


            string suffix = part1 + part2 + part3;
            string dtgb = string.Format("{0:hhddyy}", DateTime.Now);
            string sessionID = bankcode + transDate + suffix + dtgb;
            if (sessionID.Length < 30)
            {
                int getLengthDiff = 30 - sessionID.Length;
                for(int i = 0; i < getLengthDiff; i++)
                {
                    sessionID = sessionID + "0";
                    if(sessionID.Length == 30)
                    {
                        break;
                    }
                }
            }
            else if(sessionID.Length > 30)
            {
                int getLengthDiff =   sessionID.Length - 30;
                sessionID = sessionID.Substring(getLengthDiff, 30);
            }

            return sessionID;
        }

    }
}