using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mia.DataServices
{
    public static class ReminderManager
    {
        static ReminderManager()
        { }
        public static Reminder GetReminder(int id)
        {
            return ReminderRepoXML.GetReminder(id);
        }
        public static List<Reminder> GetReminder()
        {
            return new List<Reminder>(ReminderRepoXML.GetReminders());
        }
        public static async Task<int> SaveReminderAsync(Reminder item)
        {
            return await ReminderRepoXML.SaveReminderAsync(item);
        }
        public static async Task<int> DeleteReminderAsync(int id)
        {
            return await ReminderRepoXML.DeleteReminderAsync(id);
        }
    }
}
