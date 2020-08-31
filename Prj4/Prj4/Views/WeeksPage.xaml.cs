using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using Xamarin.Essentials;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Diagnostics;

namespace Prj4.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WeeksPage : ContentPage
    {
        public static SQLiteConnection conn { get; set; }
        public static Dictionary<int, TotalEntry> totalDict = new Dictionary<int, TotalEntry>();
        public int pageYear;
        public string pageMonth;
        public WeeksPage(int year, string month)
        {
            InitializeComponent();
            pageYear = year;
            pageMonth = month;
            header.Text = String.Format("{0}, {1}", pageMonth, pageYear);
            string libFolder = FileSystem.AppDataDirectory;
            string fname = System.IO.Path.Combine(libFolder, "Run.db");
            conn = new SQLiteConnection(fname);
            conn.CreateTable<Log>();
            NavigationPage.SetBackButtonTitle(this, "Year");
            OnAppearing();
        }
        DaysPage DaysPage;
        protected override void OnAppearing()
        {
            base.OnAppearing();
            totalDict = new Dictionary<int, TotalEntry>();
            foreach (Log entry in conn.Table<Log>().ToList())
            {
                if (entry.Date.Year == pageYear && entry.Date.ToString("MMMM") == pageMonth)
                {
                    int key = -1;
                    if (entry.Date.Day >= 1 && entry.Date.Day <= 7)
                        key = 1;
                    else if (entry.Date.Day >= 8 && entry.Date.Day <= 14)
                        key = 2;
                    else if (entry.Date.Day >= 15 && entry.Date.Day <= 21)
                        key = 3;
                    else if (entry.Date.Day >= 22 && entry.Date.Day <= 28)
                        key = 4;
                    else
                        key = 5;

                    if (totalDict.ContainsKey(key))
                    {
                        totalDict[key].mileSum += entry.Mile;
                    }
                    else
                    {
                        totalDict.Add(key, new TotalEntry
                        {
                            key = key,
                            mileSum = entry.Mile,
                        });
                    }
                }
            }
            lv.ItemsSource = totalDict.Values.ToList();
        }
        private void PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs args)
        {

            float max = 0;
            foreach (TotalEntry ent in totalDict.Values)
            {
                if (ent.mileSum > max)
                    max = (float)ent.mileSum;
            }

            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;
            canvas.Clear();
            SKPaint paintA = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = Color.Red.ToSKColor(),
                StrokeWidth = 3
            };
            SKPaint paintBG = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = Color.LightGray.ToSKColor(),
                StrokeWidth = 3
            };
            SKPaint font = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = Color.Black.ToSKColor(),
                StrokeWidth = 10,
                TextSize = 35 * info.Width / info.Height
            };

            canvas.DrawRect(0, 0, info.Width, info.Height, paintBG);

            float width = (info.Width / ((totalDict.Values.Count * 2) + 1));
            int i = 1;
            foreach (TotalEntry ent in totalDict.Values)
            {
                float value = (float)ent.mileSum;
                float perc = value / (max * 1.2f);
                float height = info.Height * perc;
                canvas.DrawRect(width * i, info.Height - height, width, height, paintA);
                canvas.DrawText("Week"+ent.key.ToString(), width * i, info.Height - height, font);
                i += 2;
            }
        }


        public class TotalEntry
        {
            public int key;
            public double mileSum;


            public override string ToString()
            {
                if (Preferences.Get("miles", true))
                    return String.Format("Week {0} - {1} miles", key, mileSum);
                else
                    return String.Format("Week {0} - {1} km", key, mileSum * 1.60934);
            }
        }

        private async void lv_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            TotalEntry clicked = ((TotalEntry)lv.SelectedItem);
            DaysPage = new DaysPage(pageYear, pageMonth, clicked.key);
            await Navigation.PushModalAsync(DaysPage, true);
        }
    }
}