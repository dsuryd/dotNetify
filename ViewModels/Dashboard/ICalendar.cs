using System;

namespace ViewModels
{
   /// <summary>
   /// ICalendar event arguments.
   /// </summary>
   public class DateEventArgs : EventArgs { public DateTime Value { get; set; } }
   public class ValueEventArgs : EventArgs { public int Value { get; set; } }

   /// <summary>
   /// This interface is implemented by the Calendar Widget to provide notifications to all
   /// calendar listeners when the current date or month is changed.
   /// </summary>
   public interface ICalendar
   {
      // Date that is selected on the calendar.
      DateTime SelectedDate { get; set; }

      // The month the calendar is currently on.  Not necessarily the same as SelectedDate's month.
      int CurrentMonth { get; set; }

      event EventHandler<DateEventArgs> DateChanged;

      event EventHandler<ValueEventArgs> MonthChanged;
   }

   /// <summary>
   /// This interface is implemented by Widgets that want to update its content when the current
   /// date or month on the Calendar Widget is changed.
   /// </summary>
   public interface ICalendarListener
   {
      void OnDateChanged(object sender, DateEventArgs e);

      void OnMonthChanged(object sender, ValueEventArgs e);

      void Init(DateTime iDate, int iMonth);
   }
}
