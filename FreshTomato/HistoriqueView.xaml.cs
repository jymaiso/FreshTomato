using FreshTomato.ViewModel;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FreshTomato
{
    /// <summary>
    /// Logique d'interaction pour HistoriqueView.xaml
    /// </summary>
    public partial class HistoriqueView : Window
    {
        public HistoriqueView()
        {
            InitializeComponent();

            this.DataContext = SimpleIoc.Default.GetInstance<HistoriqueViewModel>();
        }
    }
}
