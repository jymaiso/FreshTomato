using FreshTomato.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreshTomato.Services
{
    public class TimeLinesService
    {
        public  ObservableCollection<DisplayedTime> SessionsToTimeLine(IList<Session> sessions)
        {
            ObservableCollection<DisplayedTime> list = new ObservableCollection<DisplayedTime>();

            for (int i = 0; i < sessions.Count; i++)
            {
                var currentSession = sessions[i];

                //Pause Avant 09:00
                if (i == 0 && currentSession.Start.Hour >= 9)
                {
                    list.Add(new DisplayedTime
                    {
                        Start = currentSession.Start.Date.AddHours(9),
                        End = currentSession.Start,
                        TimeCategory = TimeCategory.Pause
                    });

                }

                //Work
                list.Add(new DisplayedTime
                {
                    Start = currentSession.Start,
                    End = currentSession.End,
                    TimeCategory = TimeCategory.Work
                });

                //Pause Avant Prochaine Session
                if (i < sessions.Count - 1 && currentSession.End != null)
                {
                    var nextSession = sessions[i + 1];
                    list.Add(new DisplayedTime
                    {
                        Start = currentSession.End.Value,
                        End = nextSession.Start,
                        TimeCategory = TimeCategory.Pause
                    });
                }

                //Pause Jusqu'a maintenant
                if (i == sessions.Count - 1 && currentSession.End != null && DateTime.Now < currentSession.End.Value.Date.AddHours(18))
                {
                    list.Add(new DisplayedTime
                    {
                        Start = currentSession.End.Value,
                        End =DateTime.Now ,
                        TimeCategory = TimeCategory.Pause
                    });
                }
                else if (i == sessions.Count - 1 && currentSession.End != null && currentSession.End < currentSession.End.Value.Date.AddHours(18))
                {
                    list.Add(new DisplayedTime
                    {
                        Start = currentSession.End.Value,
                        End = currentSession.End.Value.Date.AddHours(18),
                        TimeCategory = TimeCategory.Pause
                    });
                }
              



                //Future
                if (i == sessions.Count - 1 && currentSession.End != null && DateTime.Now < currentSession.End.Value.Date.AddHours(18))
                {
                    list.Add(new DisplayedTime
                    {
                        Start = DateTime.Now,
                        End = currentSession.End.Value.Date.AddHours(18),
                        TimeCategory = TimeCategory.Future
                    });
                }
            }
            return list;
        }
    }
}
