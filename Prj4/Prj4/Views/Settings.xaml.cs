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
    public partial class Settings : ContentPage
    {
        public Settings()
        {
            InitializeComponent();

            if (!Preferences.ContainsKey("miles"))
                Preferences.Set("miles", true);
            miles.IsToggled = Preferences.Get("miles", true);

            if (!Preferences.ContainsKey("dob"))
                Preferences.Set("dob", DateTime.Today);
            dob.Date = Preferences.Get("dob", DateTime.Today);

            if (!Preferences.ContainsKey("gender"))
                Preferences.Set("gender", "Female");
            if (Preferences.Get("gender", "Female").Equals("Female"))
            {
                Gender.SelectedIndex = 0;
            }
            else
            {
                Gender.SelectedIndex = 1;
            }

        }

        protected void Credits_Clicked(object sender, EventArgs e)
        {
            Browser.OpenAsync("https://www.miamioh.edu");
        }

        private void dob_DateSelected(object sender, DateChangedEventArgs e)
        {
            Preferences.Set("dob", dob.Date);
        }

        private void miles_Toggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("miles", miles.IsToggled);
            
        }

        private void Gender_SelectedIndexChanged(object sender, EventArgs e)
        {
            Preferences.Set("gender", Gender.SelectedItem.ToString());
        }
    }
}