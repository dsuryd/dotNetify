namespace ViewModels
{
   /// <summary>
   /// Live chart widget.
   /// </summary>
   public class WidgetLineChartVM : LiveChartVM
   {
      public static string WidgetName { get { return "Server Activities"; } }
      public static string ViewName { get { return "WidgetLineChart"; } }

      public string Id { get; set; }
      public string Title { get { return WidgetName; } }
      public string Description { get { return "Average CPU Load"; } }
      public double CurrentValue { get { return Data.Length > 0 ? Data[Data.Length / 2 - 1, 1] : 0; } }

      /// <summary>
      /// Constructor.
      /// </summary>
      public WidgetLineChartVM(string iWidgetId)
      {
         Id = iWidgetId;
         PropertyChanged += (sender, e) => Changed(() => CurrentValue);
      }
   }
}
