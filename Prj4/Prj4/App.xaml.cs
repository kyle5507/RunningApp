﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Prj4.Services;
using Prj4.Views;

namespace Prj4
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
