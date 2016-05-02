using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using DotNetify;

namespace ViewModels
{
   /// <summary>
   /// This view model demonstrates different types of bindings that can applied to HTML elements.
   /// This is by no means comprehensive; for more binding types, see KnockoutJS documentation.
   /// </summary>
   public class ControlTypes2VM : BaseVM
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
         Planets.Where(i => !String.IsNullOrEmpty(SearchBox) && i.StartsWith(SearchBox, StringComparison.InvariantCultureIgnoreCase));

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

      public List<string> SimpleDropDown_options => new List<string> { "One", "Two", "Three", "Four" };

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

      #region Button

      public ICommand ButtonClickCommand => new Command(() => ClickCount++);

      public int ClickCount
      {
         get { return Get<int>(); }
         set { Set(value); }
      }

      #endregion

      /// <summary>
      /// Constructor.
      /// </summary>
      public ControlTypes2VM()
      {
         ShowMeCheckBox = true;
         EnableMeCheckBox = true;
         RadioButtonValue = "green";
      }
   }
}
