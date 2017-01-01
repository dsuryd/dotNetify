using System.Collections.Generic;
using System.Linq;
using DotNetify;

namespace ViewModels
{
   /// <summary>
   /// This example demonstrates how to implement communication among dynamically loaded components.
   /// This class is a master view model for the DashboardPanelVM.  It serves as a factory for the widget
   /// view models that are selected from the dropdown on the view, which allows this class the opportunity
   /// to facilitate communication between Calendar widget and other widgets using the Observer pattern.
   /// </summary>
   public class DashboardVM : BaseVM
   {
      private DashboardPanelVM _PanelVM = new DashboardPanelVM();
      private List<BaseVM> _WidgetVMs = new List<BaseVM>();

      /// <summary>
      /// Overrides this method to handle creation of view models within this master view model's scope.
      /// </summary>
      public override BaseVM GetSubVM(string iVMTypeName, string iVMInstanceId)
      {
         if (iVMTypeName == typeof(DashboardPanelVM).Name)
            return _PanelVM;

         var widgetVM = _PanelVM.CreateWidgetVM( iVMTypeName, iVMInstanceId );
         if (widgetVM != null)
         {
            OnNewWidget( widgetVM );
            return widgetVM;
         }

         return base.GetSubVM(iVMTypeName, iVMInstanceId);
      }

      /// <summary>
      /// When a new widget is added, check whether it's a Calendar widget or calendar listener widgets and
      /// establish communication between them through the Observer pattern.
      /// </summary>
      private void OnNewWidget(BaseVM iWidgetVM)
      {
         if (iWidgetVM is ICalendarListener)
         {
            var calendar = _WidgetVMs.FirstOrDefault(i => i is ICalendar) as ICalendar;
            if (calendar != null)
            {
               var listener = iWidgetVM as ICalendarListener;
               calendar.DateChanged += listener.OnDateChanged;
               calendar.MonthChanged += listener.OnMonthChanged;
               listener.Init(calendar.SelectedDate, calendar.CurrentMonth);
            }
         }
         else if (iWidgetVM is ICalendar)
         {
            var calendar = iWidgetVM as ICalendar;
            foreach (ICalendarListener listener in _WidgetVMs.Where(i => i is ICalendarListener))
            {
               calendar.DateChanged += listener.OnDateChanged;
               calendar.MonthChanged += listener.OnMonthChanged;
               listener.Init(calendar.SelectedDate, calendar.CurrentMonth);
            }
         }

         iWidgetVM.Disposed += (sender, e) => _WidgetVMs.Remove(iWidgetVM);
         _WidgetVMs.Add(iWidgetVM);
      }
   }
}
