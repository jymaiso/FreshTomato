using FreshTomato.Services;
using FreshTomato.ViewModel;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;

namespace FreshTomato
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<HistoriqueViewModel>();
            SimpleIoc.Default.Register<PersistanceService>();
            SimpleIoc.Default.Register<TimeLinesService>();

            SessionCollection data = SimpleIoc.Default.GetInstance<PersistanceService>().Load();

            if (data != null)
                SimpleIoc.Default.GetInstance<MainViewModel>().Sessions = data;
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            var data = SimpleIoc.Default.GetInstance<MainViewModel>().Sessions;

            SimpleIoc.Default.GetInstance<PersistanceService>().Save(data);
        }

    }
}
