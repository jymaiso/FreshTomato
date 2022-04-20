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
using System.Collections.Generic;
using FreshTomato.Services;
using GalaSoft.MvvmLight.Ioc;

namespace FreshTomato.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public ObservableCollection<DisplayedTime> TimeLines
        {
            get
            {
                IList<Session> sessions = Sessions.Ranges;

                ObservableCollection<DisplayedTime> list = SimpleIoc.Default.GetInstance<TimeLinesService>().SessionsToTimeLine(sessions);

                return list;
            }

        }

        public SessionCollection Sessions { get; set; }
        public Configuration Configuration { get; set; }
        public Data Data { get; set; }

        public RelayCommand StartSession { get; set; }
        public RelayCommand LearnSession { get; set; }
        public RelayCommand StopSession { get; set; }
        public RelayCommand Reset { get; set; }
        public RelayCommand OpenFolder { get; set; }
        public RelayCommand Archive { get; set; }
        public MainViewModel()
        {
            Sessions = new SessionCollection();
            Configuration = new Configuration();
            Data = new Data();

            StartSession = new RelayCommand(() =>
            {
                Sessions.Start();
                Refresh();
            },
            () =>
            {
                return Sessions.CurrentState == SessionCollection.State.Stopped;
            });

            LearnSession = new RelayCommand(() =>
            {
                Sessions.StartLearn();
                Refresh();
            },
          () =>
          {
              return Sessions.CurrentState == SessionCollection.State.Stopped;
          });

            StopSession = new RelayCommand(() =>
            {
                Sessions.Stop();
                Refresh();
                AlertDisplayed = false;
            },
            () =>
            {
                return Sessions.CurrentState == SessionCollection.State.Started;
            });

            Reset = new RelayCommand(() =>
            {
                Sessions = new SessionCollection();
                Refresh();
            },
           () =>
           {
               return Sessions.CurrentState == SessionCollection.State.Stopped;
           });

            OpenFolder = new RelayCommand(() =>
            {
                Process.Start(Environment.CurrentDirectory);
            });

            Archive = new RelayCommand(() =>
            {
                HistoriqueView h = new HistoriqueView();
                h.Show();
            });

            DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(TimeLines_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 1, 0);
            dispatcherTimer.Start();
        }

        private void TimeLines_Tick(object sender, EventArgs e)
        {
            RaisePropertyChanged(() => TimeLines);
        }

        private void Alert()
        {
            // If you don't need any events fired, you can do this.
            ToastPopUp toast = new ToastPopUp("Fresh Tomato", "It's over.", NotificationType.Information)
            {


            };

            toast.ClosedByUser += toast_ClosedByUser;

            toast.Show();
        }

        void toast_ClosedByUser(object sender, EventArgs e)
        {
            AlertDisplayed = true;
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            Data.Refresh(Sessions, Configuration, DateTime.Now.ToString("yyyyMMdd"));

            if (Sessions.CurrentSessionSeconds > Configuration.PomodoroMinutes * 60 && Sessions.CurrentState == SessionCollection.State.Started && !AlertDisplayed)
            {
                AlertDisplayed = true;

                Alert();

                Task.Factory.StartNew(() =>
                {

                    for (int i = 0; i < 6; i++)
                    {
                        if (Sessions.CurrentState == SessionCollection.State.Started)
                        {
                            SystemSounds.Hand.Play();
                            Thread.Sleep(1000);
                        }
                    }
                });
            }
        }

        private bool AlertDisplayed = false;

        private void Refresh()
        {
            StartSession.RaiseCanExecuteChanged();
            StopSession.RaiseCanExecuteChanged();
            Reset.RaiseCanExecuteChanged();

            RaisePropertyChanged(() => TimeLines);
        }
    }
}