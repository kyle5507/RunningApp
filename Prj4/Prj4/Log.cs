using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using Xamarin.Essentials;

namespace Prj4
{
    [Table("runs")]
    public class Log
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double Mile { get; set; }
        public TimeSpan TimeHrs { get; set; }

        public override string ToString()
        {
            if(Preferences.Get("miles", true))
                return String.Format("{0} miles {1} {2} {3}", Mile, Date.Date, TimeHrs, Id);
            else
                return String.Format("{0} km {1} {2} {3}", Mile*1.60934, Date.Date, TimeHrs, Id);
        }
    }
}
