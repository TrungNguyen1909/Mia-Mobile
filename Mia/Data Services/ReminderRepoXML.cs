using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using PCLStorage;
using System.Threading.Tasks;

namespace Mia.DataServices
{
    public class ReminderRepoXML
    {
        static string storeLocation;
        static List<Reminder> Reminders;
        public static IFolder rootFolder = FileSystem.Current.LocalStorage;
        static ReminderRepoXML()
        {
            // set the db location
            storeLocation = DatabaseFilePath;
            Reminders = new List<Reminder>();

            // deserialize XML from file at dbLocation
            ReadXml();
        }

        static async Task ReadXml()
        {
            
            if (await rootFolder.CheckExistsAsync(storeLocation)==ExistenceCheckResult.FileExists)
            {
                var serializer = new XmlSerializer(typeof(List<Reminder>));
                IFile DBFile = await rootFolder.GetFileAsync(storeLocation);
                Reminders = (List<Reminder>)serializer.Deserialize(await DBFile.OpenAsync(FileAccess.Read));
            }
        }
        static async Task WriteXml()
        {
            UpdateID();
            var serializer = new XmlSerializer(typeof(List<Reminder>));
            IFile DBFile = await rootFolder.CreateFileAsync(storeLocation,CreationCollisionOption.OpenIfExists);
            using (var writer = new StreamWriter(await DBFile.OpenAsync(FileAccess.ReadAndWrite)))
            {
                serializer.Serialize(writer, Reminders);
            }
        }
        static void UpdateID()
        {
            var last = 0;
            foreach(Reminder t in Reminders)
            {
                t.ID = ++last;
            }
        }
        public static string DatabaseFilePath
        {
            get
            {
                return "ReminderDB.xml";

            }
        }

        public static Reminder GetReminder(int id)
        {
            for (var t = 0; t < Reminders.Count; t++)
            {
                if (Reminders[t].ID == id)
                    return Reminders[t];
            }
            return new Reminder() { ID = id };
        }

        public static IEnumerable<Reminder> GetReminders()
        {
            return Reminders;
        }

        /// <summary>
        /// Insert or update a Reminder
        /// </summary>
        public static async Task<int> SaveReminderAsync(Reminder item)
        {
            var max = 0;
            if (Reminders.Count > 0)
                max = Reminders.Max(x => x.ID);

            if (item.ID == 0)
            {
                item.ID = ++max;
                Reminders.Add(item);
            }
            else
            {
                var i = Reminders.Find(x => x.ID == item.ID);
                i = item; // replaces item in collection with updated value
            }

            await WriteXml();
            return max;
        }

        public static async Task<int> DeleteReminderAsync(int id)
        {
            for (var t = 0; t < Reminders.Count; t++)
            {
                if (Reminders[t].ID == id)
                {
                    Reminders.RemoveAt(t);
                    await WriteXml();
                    return 1;
                }
            }

            return -1;
        }
    }
}
