using System;
using System.Collections.Generic;
using System.Linq;
using DotNetify;

namespace ViewModels
{
   /// <summary>
   /// Simple to-do list widget that can observe the Calendar widget to display the current day and weekday.
   /// </summary>
   public class WidgetToDoListVM : BaseVM, ICalendarListener
   {
      public static string WidgetName { get { return "To Do List"; } }
      public static string ViewName { get { return "WidgetToDoList"; } }

      private int _ToDoId;

      public class ToDoInfo
      {
         public int Id { get; set; }
         public bool Done { get; set; }
         public string Description { get; set; }
      }

      public string Id { get; set; }

      /// <summary>
      /// When the Calendar widget is shown, use this property to show the current day.
      /// </summary>
      public string Day { get; set; }

      /// <summary>
      /// When the Calendar widget is shown, use this property to show the current weekday.
      /// </summary>
      public string Weekday { get; set; }

      /// <summary>
      /// A list of to-dos.  These are just kept in the view model's memory for this example's convenience.
      /// </summary>
      public List<ToDoInfo> ToDoList { get; set; }

      /// <summary>
      /// DotNetify calls this method to access to a to-do list item when an edit is made on the view.
      /// </summary>
      public ToDoInfo ToDoList_get(string iKey)
      {
         int id;
         return int.TryParse(iKey, out id) ? ToDoList.FirstOrDefault(i => i.Id == id) : null;
      }

      /// <summary>
      /// When a new to-do item is entered, this property will receive the new text.
      /// </summary>
      public string NewToDo
      {
         get { return null; }
         set
         {
            if (!String.IsNullOrEmpty(value))
            {
               ToDoList.Add(new ToDoInfo { Id = ++_ToDoId, Description = value });
               Changed(() => ToDoList);
               Set("");  // Reset the input text.
            }
         }
      }

      /// <summary>
      /// When a to-do item is removed, this property will receive its Id.
      /// </summary>
      public int RemoveToDo
      {
         get { return 0; }
         set
         {
            var todo = ToDoList.FirstOrDefault(i => i.Id == value);
            if (todo != null)
            {
               ToDoList.Remove(todo);
               Changed(() => ToDoList);
            }
         }
      }

      /// <summary>
      /// Constructor.
      /// </summary>
      public WidgetToDoListVM(string iWidgetId)
      {
         Id = iWidgetId;

         // Initial values for to do list.
         ToDoList = new List<ToDoInfo>
            {
               new ToDoInfo { Id = ++_ToDoId, Description = "Buy groceries" },
               new ToDoInfo { Id = ++_ToDoId, Description = "Pick up kids from school", Done = true },
               new ToDoInfo { Id = ++_ToDoId, Description = "Mow the lawn" }
            };
      }

      #region ICalendarListener Methods

      public void Init(DateTime iDate, int iMonth)
      {
         OnDateChanged(this, new DateEventArgs { Value = iDate });
      }

      public void OnDateChanged(object sender, DateEventArgs e)
      {
         Day = e.Value.Day.ToString();
         Weekday = e.Value.DayOfWeek.ToString();
         Changed(() => Day);
         Changed(() => Weekday);
      }

      public void OnMonthChanged(object sender, ValueEventArgs e)
      {
         // Nothing to do.
      }

      #endregion
   }
}
