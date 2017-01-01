using System;
using DotNetify;

namespace ViewModels
{
   /// <summary>
   /// Monthy sales bar chart widget that can observe the Calendar widget to highlight its month bar
   /// according to the current month displayed on the calendar.
   /// </summary>
   public class WidgetBarChartVM : BaseVM, ICalendarListener
   {
      public static string WidgetName { get { return "Sales by Month"; } }
      public static string ViewName { get { return "WidgetBarChart"; } }

      public string Id { get; set; }
      public string Title { get { return WidgetName; } }
      public string Description { get { return "Average sales YoY"; } }
      public string TotalValue { get; set; }
      public string SpecificValue { get; set; }

      public string[,] Data
      {
         get { return Get<string[,]>(); }
         set { Set(value); }
      }

      public int SelectedMonth { get; set; }

      /// <summary>
      /// Constructor.
      /// </summary>
      public WidgetBarChartVM(string iWidgetId)
      {
         Id = iWidgetId;

         // Create data for the chart.
         Random random = new Random();
         double totalValue = 0;
         Data = new string[12, 2];
         for (int label = 0; label < 12; label++)
         {
            var value = random.Next(5000, 20000);
            Data[label, 0] = new DateTime(2015, label + 1, 1).ToString("MMM");
            Data[label, 1] = value.ToString();
            totalValue += value;
         }
         TotalValue = totalValue.ToString("C0");
      }

      #region ICalendarListener Methods

      public void Init(DateTime iDate, int iMonth)
      {
         OnMonthChanged(this, new ValueEventArgs { Value = iMonth });
      }

      public void OnDateChanged(object sender, DateEventArgs e)
      {
         // Nothing to do.
      }

      public void OnMonthChanged(object sender, ValueEventArgs e)
      {
         SelectedMonth = e.Value;
         SpecificValue = new DateTime(2015, e.Value, 1).ToString("MMM") + ": " + int.Parse(Data[e.Value - 1, 1]).ToString("C0");

         Changed(() => SelectedMonth);
         Changed(() => SpecificValue);
      }

      #endregion
   }
}
