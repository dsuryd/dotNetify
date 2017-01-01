using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotNetify;

namespace ViewModels
{
   /// <summary>
   /// This example demonstrates how to build a web dashboard dynamically out of modular components or widgets.
   /// This panel view model dynamically identifies all the widget view model types from the current assembly and
   /// present them in a dropdown list.  When a selection is made, the dashboard view will load the widget's view
   /// and view model asynchronously into the DOM. The widgets can interact with each other; see how it's done in
   /// the master view model DashboardVM.
   /// </summary>
   public class DashboardPanelVM : BaseVM
   {
      private int _IdCounter;

      /// <summary>
      /// This class holds information on a type of widget that can be added to the dashboard.
      /// </summary>
      public class WidgetType
      {
         [Ignore]
         public Type Type { get; set; }

         public string DisplayName { get; set; }
         public string ViewName { get; set; }
      }

      /// <summary>
      /// This class holds information on a widget; its unique ID and HTML view name.
      /// </summary>
      public class WidgetInfo
      {
         public int Id { get; set; }
         public string ViewName { get; set; }
      }

      /// <summary>
      /// Types of widgets that can be added to the dashboard.
      /// </summary>
      public List<WidgetType> WidgetTypes
      {
         get
         {
            var widgetTypes = new List<WidgetType>();
            // Find all classes in this assembly that has a static property "WidgetName".
            // These will be the widget view model classes.
            var types = GetType().GetTypeInfo().Assembly.GetExportedTypes().Where(i => i.GetProperty("WidgetName") != null);
            foreach (var type in types)
               widgetTypes.Add(new WidgetType
               {
                  Type = type,
                  DisplayName = type.GetProperty("WidgetName").GetValue(null).ToString(),
                  ViewName = type.GetProperty("ViewName").GetValue(null).ToString()
               });
            return widgetTypes;
         }
      }

      /// <summary>
      /// This property is called when a widget type is selected from the dropdown on the view.
      /// </summary>
      public string AddWidget
      {
         get { return null; }
         set { NewWidget = new WidgetInfo { Id = ++_IdCounter, ViewName = value }; }
      }

      /// <summary>
      /// This property is used for adding a new widget.  On the browser, this is bound to a Javascript
      /// function that will use AJAX to load the widget's HTML markups, add it to the dashboard's DOM
      /// and then load its view model.
      /// </summary>
      public WidgetInfo NewWidget
      {
         get { return Get<WidgetInfo>(); }
         set { Set(value); }
      }

      /// <summary>
      /// Creates the view model for a widget.
      /// </summary>
      public BaseVM CreateWidgetVM(string iWidgetTypeName, string iWidgetId)
      {
         var widgetType = GetType().GetTypeInfo().Assembly.GetExportedTypes().FirstOrDefault(i => i.Name == iWidgetTypeName);
         return VMController.CreateInstance(widgetType, new object[] { iWidgetId }) as BaseVM;
      }
   }
}
