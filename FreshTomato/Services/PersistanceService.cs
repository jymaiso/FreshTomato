using FreshTomato.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FreshTomato.Services
{

    public class PersistanceService
    {

        public void Save(SessionCollection data)
        {
            var path = Path.Combine(Environment.CurrentDirectory, "_data_TimeLine.xml");

            XmlSerializer x = new XmlSerializer(typeof(SessionCollection));
            using (StreamWriter writer = new StreamWriter(path))
            {
                x.Serialize(writer, data);
            }

            if (data.Ranges.First() != null)
            {
                DateTime day = data.Ranges.First().Start;
                var pathDay = Path.Combine(Environment.CurrentDirectory, day.ToString("yyyyMMdd") + "_TimeLine.xml");
                //if (day.Date == DateTime.Now.Date || !File.Exists(pathDay))
                //{
                using (StreamWriter writer = new StreamWriter(pathDay))
                {
                    x.Serialize(writer, data);
                }
                //}
            }


        }

        public SessionCollection Load()
        {
            return Load(Path.Combine(Environment.CurrentDirectory, "_data_TimeLine.xml"));
        }

        public SessionCollection Load(string path)
        {
            SessionCollection data = null;

            if (File.Exists(path))
            {
                XmlSerializer x = new XmlSerializer(typeof(SessionCollection));
                using (StreamReader writer = new StreamReader(path))
                {
                    data = x.Deserialize(writer) as SessionCollection;
                }
            }
            return data;
        }
    }
}
