using System;
using System.Collections.Generic;
using System.Linq;
using DotNetify;

namespace ViewModels
{
   /// <summary>
   /// This view model demonstrates different types of bindings that can applied to HTML elements.
   /// This is by no means comprehensive; for more binding types, see KnockoutJS documentation.
   /// </summary>
   public class ControlTypesVM : BaseVM
   {
      #region Text Box

      public string TextBoxValue
      {
         get { return Get<string>(); }
         set
         {
            Set(value);
            Changed(() => TextBoxResult);
         }
      }

      public string TextBoxPlaceHolder
      {
         get { return "Type something here"; }
      }

      public string TextBoxResult
      {
         get { return !String.IsNullOrEmpty(TextBoxValue) ? "You typed: <b>" + TextBoxValue + "</b>" : null; }
      }

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

      public string SearchBoxPlaceHolder
      {
         get { return "Type a planet"; }
      }

      public IEnumerable<string> SearchResults
      {
         get { return Planets.Where(i => !String.IsNullOrEmpty(SearchBox) && i.ToLower().StartsWith(SearchBox.ToLower())); }
      }

      #endregion

      #region Check Box

      public bool ShowMeCheckBox
      {
         get { return Get<bool>(); }
         set
         {
            Set(value);
            Changed(() => CheckBoxResult);
         }
      }

      public bool EnableMeCheckBox
      {
         get { return Get<bool>(); }
         set
         {
            Set(value);
            Changed(() => CheckBoxResult);
         }
      }

      public string CheckBoxResult
      {
         get { return "I am <b>" + (EnableMeCheckBox ? "enabled" : "disabled") + "</b>"; }
      }

      #endregion

      #region Simple Drop-down

      public List<string> SimpleDropDownOptions
      {
         get { return new List<string> { "One", "Two", "Three", "Four" }; }
      }

      public string SimpleDropDownValue
      {
         get { return Get<string>(); }
         set
         {
            Set(value);
            Changed(() => SimpleDropDownResult);
         }
      }

      public string SimpleDropDownResult
      {
         get { return !String.IsNullOrEmpty(SimpleDropDownValue) ? "You selected: <b>" + SimpleDropDownValue + "</b>" : null; }
      }

      #endregion

      #region Drop Down Objects

      public class DropDownItem
      {
         public int Id { get; set; }
         public string Text { get; set; }
      }

      public string DropDownCaption
      {
         get { return "Select an item ..."; }
      }

      public List<DropDownItem> DropDownOptions
      {
         get
         {
            return new List<DropDownItem>
                {
                    new DropDownItem { Id = 1, Text = "Object One" },
                    new DropDownItem { Id = 2, Text = "Object Two" },
                    new DropDownItem { Id = 3, Text = "Object Three" },
                    new DropDownItem { Id = 4, Text = "Object Four" }
                };
         }
      }

      public int DropDownValue
      {
         get { return Get<int>(); }
         set
         {
            Set(value);
            Changed(() => DropDownResult);
         }
      }

      public string DropDownResult
      {
         get { return DropDownValue > 0 ? "You selected: <b>" + DropDownOptions.First(i => i.Id == DropDownValue).Text + "</b>" : null; }
      }

      #endregion

      #region Radio Buttons

      public string RadioButtonValue
      {
         get { return Get<string>(); }
         set
         {
            Set(value);
            Changed(() => RadioButtonStyle);
         }
      }

      public string RadioButtonStyle
      {
         get { return RadioButtonValue == "green" ? "label-success" : "label-warning"; }
      }

      #endregion

      #region Button

      public bool ButtonClicked
      {
         get { return false; }
         set { ClickCount++; }
      }

      public int ClickCount
      {
         get { return Get<int>(); }
         set { Set(value); }
      }

      #endregion

      /// <summary>
      /// Constructor.
      /// </summary>
      public ControlTypesVM()
      {
         ShowMeCheckBox = true;
         EnableMeCheckBox = true;
         RadioButtonValue = "green";
      }
   }
}
