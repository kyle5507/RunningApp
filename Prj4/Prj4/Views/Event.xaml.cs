using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;


namespace Prj4.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Event : ContentPage
    {
        public static SQLiteConnection conn { get; set; }
        private bool currentOrientationLandscape;
        private Boolean selected;
        public Event()
        {
            InitializeComponent();
            string libFolder = FileSystem.AppDataDirectory;
            string fname = System.IO.Path.Combine(libFolder, "Run.db");
            conn = new SQLiteConnection(fname);
            //conn.DropTable<Log>();
            conn.CreateTable<Log>();
            //GenerateRunData(2015, 6);
            OnAppearing();

        }
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            bool isNowLandscape = width > height;
            if (isNowLandscape != currentOrientationLandscape)
            {
                layout.Orientation = isNowLandscape ? StackOrientation.Horizontal : StackOrientation.Vertical;
                currentOrientationLandscape = isNowLandscape;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Preferences.Get("miles", true))
                labelDistance.Text = "Distance Miles";
            else
                labelDistance.Text = "Distance Kilometers";
            EventList.ItemsSource = conn.Table<Log>().ToList();
        }

        private void Add_Clicked(object sender, EventArgs e)
        {
            //merge decimal and make sure it's stored in miles.
            double distance = 0;
            if (Preferences.Get("miles", true))
                distance = double.Parse(Mile.Text + "." + MileDec.Text);
            else
                distance = (double.Parse(Mile.Text + "." + MileDec.Text)) / 1.609;

            Log newLog = new Log
            {
                Date = datePick.Date,
                Mile = distance,
                TimeHrs = TimeSpan.Parse(TimeHr.Text + ":" + TimeMin.Text)
            };
            conn.Insert(newLog);
            OnAppearing();
        }
        private void Update_Clicked(object sender, EventArgs e)
        {
            if (selected)
            {
                //merge decimal and make sure it's stored in miles.
                double distance = 0;
                if (Preferences.Get("miles", true))
                    distance = double.Parse(Mile.Text + "." + MileDec.Text);
                else
                    distance = (double.Parse(Mile.Text + "." + MileDec.Text)) / 1.609;

                Log old = EventList.SelectedItem as Log;
                Log newLog = new Log
                {
                    Date = datePick.Date,
                    Mile = distance,
                    TimeHrs = TimeSpan.Parse(TimeHr.Text + ":" + TimeMin.Text)
                };
                newLog.Id = old.Id;
                conn.Update(newLog);
                selected = false;
                OnAppearing();
            }
            else
                DisplayAlert("Error", "You need to click on a run to update it", "Ok");
        }
        private void Delete_Clicked(object sender, EventArgs e)
        {
            if (selected)
            {
                conn.Delete(EventList.SelectedItem);
                OnAppearing();
                selected = false;
            }
            else
                DisplayAlert("Error", "You need to click on a run to delete it", "Ok");

        }

        private void EventList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            selected = true;
        }

        public static void GenerateRunData(int startYear, int numYears)
        {
            const double baseMileage = 3.0;
            for (int dy = 0; dy < numYears; dy++)
            {
                int year = startYear + dy;
                double yearAdjustment = 1.0 + dy * 0.01;
                for (int month = 1; month <= 12; month++)
                {
                    double monthAdjustment = 1.0 + month * 0.02;
                    for (int day = 1; day <= DateTime.DaysInMonth(year, month); day++)
                    {
                        DateTime date = new DateTime(year, month, day);
                        double length = 0;
                        switch (date.DayOfWeek)
                        {
                            case DayOfWeek.Monday:
                                continue;
                                break;
                            case DayOfWeek.Tuesday:
                                length = baseMileage;
                                break;
                            case DayOfWeek.Wednesday:
                                length = 1.5 * baseMileage;
                                break;
                            case DayOfWeek.Thursday:
                                length = 2 * baseMileage;
                                break;
                            case DayOfWeek.Friday:
                                length = 2 * baseMileage;
                                break;
                            case DayOfWeek.Saturday:
                                length = baseMileage;
                                break;
                            case DayOfWeek.Sunday:
                                length = 4 * baseMileage;
                                break;
                        }
                        int runLengthInMiles = (int)Math.Round(yearAdjustment * monthAdjustment * length);
                        int secondsToCompleteRun = runLengthInMiles * 480;      // 8 minutes per mile
                        // Instead of printing, you should insert the run into your database.
                        Log newLog = new Log
                        {
                            Date = date,
                            Mile = runLengthInMiles,
                            TimeHrs = TimeSpan.FromSeconds(secondsToCompleteRun)
                        };
                        conn.Insert(newLog);
                    }
                }
            }
        }
    }
}