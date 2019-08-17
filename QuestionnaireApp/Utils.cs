using System;
using System.Collections.Generic;
using System.Text;

namespace QuestionnaireApp
{
    public static class Utils
    {
        public static int GetAge(DateTime birthDate)
        {
            int diff = DateTime.Now.Year - birthDate.Year;
            if ((birthDate.Month > DateTime.Now.Month) || (birthDate.Month == DateTime.Now.Month && birthDate.Day > DateTime.Now.Day))
                diff--;
            return diff;
        }
    }
}
