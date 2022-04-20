using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Mantin.Controls.Wpf.Notification;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Media;
using System.Diagnostics;
using FreshTomato.Services;
using GalaSoft.MvvmLight.Ioc;

namespace FreshTomato.ViewModel
{
    public class WorkingDay
    {
        public ObservableCollection<DisplayedTime> TimeLines { get; set; }
        public Data Data { get; set; }

    }

    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class HistoriqueViewModel : ViewModelBase
    {

        public String AvgPourcentage
        {
            get
            {
                return TimeLines.Average(a => a.Data.PourcentageAsDecimal).ToString("n2") ;
            }
        }

        public String AvgPourcentageLast10
        {
            get
            {
                return TimeLines.Skip(TimeLines.Count - 11).Take(10).Average(a => a.Data.PourcentageAsDecimal).ToString("n2");
            }
        }

        public ObservableCollection<WorkingDay> TimeLines
        {
            get
            {
                if (_timeLines == null)
                {
                    ObservableCollection<WorkingDay> list = new ObservableCollection<WorkingDay>();

                    foreach (var item in Directory.GetFiles(Environment.CurrentDirectory, "*_TimeLine.xml").OrderBy(a => a))
                    {
                        SessionCollection sessionCollection = SimpleIoc.Default.GetInstance<PersistanceService>().Load(item);

                        var data = new Data();
                        data.Refresh(sessionCollection, new Configuration(), new FileInfo(item).Name);
                        list.Add(new WorkingDay
                        {
                            TimeLines = SimpleIoc.Default.GetInstance<TimeLinesService>().SessionsToTimeLine(sessionCollection.Ranges),
                            Data = data
                        });
                    }

                    _timeLines = list;
                }
                return _timeLines;
            }
        }
        private ObservableCollection<WorkingDay> _timeLines;

        public HistoriqueViewModel()
        {

        }
    }
}