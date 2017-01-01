using System;
using DotNetify;

namespace ViewModels
{
   /// <summary>
   /// Calendar widget that provides notification when the current date or month is changed.
   /// </summary>
   public class WidgetCalendarVM : BaseVM, ICalendar
   {
      public static string WidgetName { get { return "Calendar"; } }
      public static string ViewName { get { return "WidgetCalendar"; } }

      public string Id { get; set; }

      public DateTime SelectedDate
      {
         get { return Get<DateTime>(); }
         set
         {
            Set(value);
            if (DateChanged != null)
               DateChanged(this, new DateEventArgs { Value = value });
         }
      }

      public int CurrentMonth
      {
         get { return Get<int>(); }
         set
         {
            Set(value);
            if (MonthChanged != null)
               MonthChanged(this, new ValueEventArgs { Value = value });
         }
      }

      public event EventHandler<DateEventArgs> DateChanged;
      public event EventHandler<ValueEventArgs> MonthChanged;

      /// <summary>
      /// Constructor.
      /// </summary>
      public WidgetCalendarVM(string iWidgetId)
      {
         Id = iWidgetId;
         SelectedDate = DateTime.Now;
         CurrentMonth = DateTime.Now.Month;
      }

      public void Changed()
      {
         if (DateChanged != null)
            DateChanged(this, new DateEventArgs { Value = SelectedDate });

         if (MonthChanged != null)
            MonthChanged(this, new ValueEventArgs { Value = CurrentMonth });
      }
   }
}
