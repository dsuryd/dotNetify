using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetify;

namespace ViewModels
{
   /// <summary>
   /// This view model demonstrates using the Binder plugin to automate the binding declarations
   /// on the HTML elements.  To bind a view model property to an HTML element, set the ID tag to the
   /// property name.  The plugin will select the binding notation based on the type of the element.
   /// For property name that contains underscore, it will apply attribute binding to that property,
   /// with the attribute name set to the string following the underscore.
   /// </summary>
   public class AutoBindingVM : BaseVM
   {
      #region Text Box

      public string TextBox
      {
         get { return Get<string>(); }
         set
         {
            Set(value);
            Changed(() => TextBoxResult);
         }
      }

      public string TextBox_placeholder => "Type something here";

      public string TextBoxResult => !String.IsNullOrEmpty(TextBox) ? "You typed: <b>" + TextBox + "</b>" : null;

      #endregion

      #region Search Box

      private List<string> Planets = new List<string> { "Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Neptune", "Uranus" };

      public string SearchBox
      {
         get { return Get<string>(); }
         set
         {
            Set(value);
            Changed(() => SearchResults);
         }
      }

      public string SearchBox_placeholder => "Type a planet";

      public IEnumerable<string> SearchResults =>
         Planets.Where(i => !String.IsNullOrEmpty(SearchBox) && i.StartsWith(SearchBox, StringComparison.OrdinalIgnoreCase));

      #endregion

      #region Check Box

      public bool ShowMeCheckBox
      {
         get { return Get<bool>(); }
         set
         {
            Set(value);
            Changed(() => CheckBoxResult);
            Changed(() => CheckBoxButton_visible);
         }
      }

      public bool EnableMeCheckBox
      {
         get { return Get<bool>(); }
         set
         {
            Set(value);
            Changed(() => CheckBoxResult);
            Changed(() => CheckBoxButton_enable);
         }
      }

      public string CheckBoxResult => "I am <b>" + ( EnableMeCheckBox ? "enabled" : "disabled" ) + "</b>";

      public bool CheckBoxButton_enable => EnableMeCheckBox;

      public bool CheckBoxButton_visible => ShowMeCheckBox;

      #endregion

      #region Simple Drop-down

      public string SimpleDropDown
      {
         get { return Get<string>(); }
         set
         {
            Set(value);
            Changed(() => SimpleDropDownResult);
         }
      }

      public List<string> SimpleDropDown_options => new List<string> { "Option One", "Option Two", "Option Three", "Option Four" };

      public string SimpleDropDown_optionsCaption => "Choose...";

      public string SimpleDropDownResult => !String.IsNullOrEmpty(SimpleDropDown) ? "You selected: <b>" + SimpleDropDown + "</b>" : null;

      #endregion

      #region Drop Down Objects

      public class DropDownItem
      {
         public int Id { get; set; }
         public string Text { get; set; }
      }

      public int DropDown
      {
         get { return Get<int>(); }
         set
         {
            Set(value);
            Changed(() => DropDownResult);
         }
      }

      public string DropDown_optionsCaption => "Select an item ...";

      public string DropDown_optionsText => nameof(DropDownItem.Text);

      public string DropDown_optionsValue => nameof(DropDownItem.Id);

      public List<DropDownItem> DropDown_options => new List<DropDownItem>
         {
            new DropDownItem { Id = 1, Text = "Object One" },
            new DropDownItem { Id = 2, Text = "Object Two" },
            new DropDownItem { Id = 3, Text = "Object Three" },
            new DropDownItem { Id = 4, Text = "Object Four" }
         };

      public string DropDownResult => DropDown > 0 ? "You selected: <b>" + DropDown_options.First(i => i.Id == DropDown).Text + "</b>" : null;

      #endregion

      #region Radio Buttons

      public string RadioButtonValue
      {
         get { return Get<string>(); }
         set
         {
            Set(value);
            Changed(() => RadioButtonStyle_css);
         }
      }

      public string RadioButtonStyle_css => RadioButtonValue == "green" ? "label-success" : "label-warning";

      #endregion

      #region Toggle Button

      public bool ToggleButton
      {
         get { return Get<bool>(); }
         set
         {
            Set(value);
            Changed(() => ToggleButtonStyle_css);
         }
      }

      public string ToggleButtonStyle_css => ToggleButton ? "label-primary" : "label-danger";

      #endregion

      #region Button

      public ICommand ButtonClickCommand => new Command(() => ClickCount++);

      public int ClickCount
      {
         get { return Get<int>(); }
         set { Set(value); }
      }

      #endregion

      #region Icon Button and Badge

      public ICommand IconUpCommand => new Command(() => Badge++);

      public ICommand IconDownCommand => new Command(() => Badge--);

      public int Badge
      {
         get { return Get<int>(); }
         set { Set(value); }
      }

      #endregion

      #region Progress

      public ICommand StartProgressCommand => new Command(() =>
      {
         Progress = 0;
         Task.Run(async () =>
         {
            while ( Progress < 100 )
            {
               Progress++;
               if ( Progress_secondary_progress < 100 )
                  Progress_secondary_progress += 2;
               PushUpdates();
               await Task.Delay(100);
            }
         });
      });

      public int Progress
      {
         get { return Get<int>(); }
         set { Set(value); }
      }

      public int Progress_secondary_progress
      {
         get { return Get<int>(); }
         set { Set(value); }
      }

      #endregion

      #region Slider

      public int Slider
      {
         get { return Get<int>(); }
         set
         {
            Set(value);
            Changed(() => SliderResult);
         }
      }

      public int Slider_min => 1;

      public int Slider_max => 100;

      public string SliderResult => $"Slider value = {Slider}";

      #endregion

      #region Tabs

      public int Tab
      {
         get { return Get<int>(); }
         set
         {
            Set(value);
            TabPage = value;
         }
      }

      public int TabPage
      {
         get { return Get<int>(); }
         set { Set(value); }
      }

      #endregion

      #region Toast

      public ICommand ShowToastCommand => new Command(() => Toast = true );

      public bool Toast
      {
         get { return Get<bool>(); }
         set { Set(value); }
      }

      public string Toast_text => "This is the toast message.";

      #endregion

      /// <summary>
      /// Constructor.
      /// </summary>
      public AutoBindingVM()
      {
         ShowMeCheckBox = true;
         EnableMeCheckBox = true;
         RadioButtonValue = "green";
         Slider = 20;
      }
   }
}
