using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Serialization;

namespace FreshTomato.ViewModel
{
    public class Data : ViewModelBase
    {

        private string _TotalEllapsed;
        public string TotalEllapsed
        {
            get
            {
                return _TotalEllapsed;
            }
            set
            {
                if (_TotalEllapsed != value)
                {
                    _TotalEllapsed = value;
                    RaisePropertyChanged(() => TotalEllapsed);
                }
            }
        }


        private string _totalRemaining;
        public string TotalRemaining
        {
            get
            {
                return _totalRemaining;
            }
            set
            {
                if (_totalRemaining != value)
                {
                    _totalRemaining = value;
                    RaisePropertyChanged(() => TotalRemaining);
                }
            }
        }


        private string _CurrentEllapsed;
        public string CurrentEllapsed
        {
            get
            {
                return _CurrentEllapsed;
            }
            set
            {
                if (_CurrentEllapsed != value)
                {
                    _CurrentEllapsed = value;
                    RaisePropertyChanged(() => CurrentEllapsed);
                }
            }
        }


        private string _currentRemaining;
        public string CurrentRemaining
        {
            get
            {
                return _currentRemaining;
            }
            set
            {
                if (_currentRemaining != value)
                {
                    _currentRemaining = value;
                    RaisePropertyChanged(() => CurrentRemaining);
                }
            }
        }


        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    RaisePropertyChanged(() => Title);
                }
            }
        }


        private string _totalMorning;
        public string TotalMorning
        {
            get
            {
                return _totalMorning;
            }
            set
            {
                if (_totalMorning != value)
                {
                    _totalMorning = value;
                    RaisePropertyChanged(() => TotalMorning);
                }
            }
        }


        private string _totalAfternoon;
        public string TotalAfternoon
        {
            get
            {
                return _totalAfternoon;
            }
            set
            {
                if (_totalAfternoon != value)
                {
                    _totalAfternoon = value;
                    RaisePropertyChanged(() => TotalAfternoon);
                }
            }
        }


        private string _Pourcentage;
        public string Pourcentage
        {
            get
            {
                return _Pourcentage;
            }
            set
            {
                if (_Pourcentage != value)
                {
                    _Pourcentage = value;
                    RaisePropertyChanged(() => Pourcentage);
                }
            }
        }

        public decimal PourcentageAsDecimal { get; set; }

        public void Refresh(SessionCollection sessions, Configuration configuration, string title)
        {
            //decimal workDurationSecondes = sessions.Ranges.Count > 0 && sessions.Ranges.First().Start.DayOfWeek == DayOfWeek.Friday ? 420 * 60 : 465 * 60;
            decimal workDurationSecondes = (((465 - (2 * 20)) * 4) + ((420 - (2 * 20)))) / 5 * 60;

            var totalSeconds = sessions.TotalSeconds;
            var currentSessionSeconds = sessions.CurrentSessionSeconds;

            TotalEllapsed = new TimeSpan(0, 0, totalSeconds).ToString();
            CurrentEllapsed = new TimeSpan(0, 0, currentSessionSeconds).ToString();

            TotalRemaining = new TimeSpan(0, 0, ((int)(workDurationSecondes)) - totalSeconds).ToString();
            CurrentRemaining = new TimeSpan(0, 0, (configuration.PomodoroMinutes * 60) - currentSessionSeconds).ToString();

            Title = title;

            if (sessions.Ranges.Count > 0)
            {
                var limit = sessions.Ranges.First().Start.Date.AddHours(13).AddMinutes(30);
                var totalSecondsMorning = sessions.Ranges.Where(a => a.End != null && a.End < limit).Sum(a => a.TotalSeconds);
                TotalMorning = new TimeSpan(0, 0, totalSecondsMorning).ToString();
                TotalAfternoon = new TimeSpan(0, 0, totalSeconds - totalSecondsMorning).ToString();

                Pourcentage = Math.Truncate(totalSeconds / workDurationSecondes * 100).ToString() + "%";

                PourcentageAsDecimal = Math.Truncate(totalSeconds / workDurationSecondes * 100);
            }
        }
    }

    public class SessionCollection : ViewModelBase
    {
        private ObservableCollection<Session> _sessions;

        public ObservableCollection<Session> Ranges
        {
            get
            {
                return _sessions;
            }
            set
            {
                if (_sessions != value)
                {
                    _sessions = value;
                    RaisePropertyChanged(() => Ranges);
                }
            }
        }

        public State CurrentState { get; set; }

        public int TotalSeconds
        {
            get { return Ranges.Sum(a => a.TotalSeconds); }
        }

        public int CurrentSessionSeconds
        {
            get { return Ranges.LastOrDefault() != null ? Ranges.Last().TotalSeconds : 0; }
        }

        public SessionCollection()
        {
            Ranges = new ObservableCollection<Session>();
            CurrentState = State.Stopped;
        }

        public void Start()
        {
            var range = new Session(TimeCategory.Work) { Start = DateTime.Now };
            Ranges.Add(range);
            CurrentState = State.Started;
        }

        public void StartLearn()
        {
            var range = new Session(TimeCategory.Learn) { Start = DateTime.Now };
            Ranges.Add(range);
            CurrentState = State.Started;
        }

        public void Stop()
        {
            Ranges.Last().End = DateTime.Now;
            CurrentState = State.Stopped;
        }

        public enum State
        {
            Started,
            Stopped
        }

    }

    public class Session : ViewModelBase
    {
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }


        public TimeCategory TimeCategory { get; set; }
        public int TotalSeconds
        {
            get { return End != null ? (int)((End.Value - Start).TotalSeconds) : (int)(DateTime.Now - Start).TotalSeconds; }
        }

        public Session()
        {
            TimeCategory = ViewModel.TimeCategory.Work;
        }

        public Session(TimeCategory timeCategory)
        {
            TimeCategory = timeCategory;
        }
    }

    public class Configuration : ViewModelBase
    {
        public int PomodoroMinutes { get; set; }


        public Configuration()
        {
            PomodoroMinutes = 25;

        }
    }

    public class DisplayedTime : ViewModelBase
    {
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }

        public TimeCategory TimeCategory { get; set; }

        public SolidColorBrush Color
        {
            get
            {
                switch (TimeCategory)
                {
                    case TimeCategory.Work:
                        return new SolidColorBrush(Colors.LightCoral);
                    case TimeCategory.Pause:
                        return new SolidColorBrush(Colors.LightGreen);
                    case TimeCategory.Future:
                        return new SolidColorBrush(Colors.LightGray);
                    case TimeCategory.Learn:
                        return new SolidColorBrush(Colors.Orange);
                    default:
                        break;
                }
                return new SolidColorBrush(Colors.Red);
            }
        }

        public int Size { get { return Seconds / 60; } }

        public int Seconds { get { return (int)((End ?? DateTime.Now) - Start).TotalSeconds; } }
        public int Minutes { get { return (int)((End ?? DateTime.Now) - Start).TotalMinutes; } }
    }

    public enum TimeCategory
    {
        Work,
        Pause,
        Future,
        Learn
    }
}
