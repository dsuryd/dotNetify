using System;
using System.Collections.Generic;
using System.Linq;
using DotNetify;
using ViewModels.Components.MaterialUI;

namespace ViewModels
{
   /// <summary>
   /// This view model demonstrates different types of bindings that can applied to HTML elements.
   /// This is by no means comprehensive; for more binding types, see KnockoutJS documentation.
   /// </summary>
   public class ControlTypesVM : BaseVM
   {
      #region Text Field

      public string TextFieldValue
      {
         get { return Get<string>() ?? ""; }
         set
         {
            Set(value);
            Changed(nameof(TextFieldResult));

            // Server-side validation.
            ValidateTextField(value);
         }
      }

      public string TextFieldErrorText
      {
         get { return Get<string>(); }
         set { Set(value); }
      }

      public string TextFieldResult => !string.IsNullOrEmpty(TextFieldValue) ? $"You typed: {TextFieldValue}" : null;

      public TextField TextFieldProps => new TextField
      {
         floatingLabelText = "Text Field",
         hintText = "Type something here",
      };

      private void ValidateTextField(string value)
      {
         if (TextFieldProps != null)
         {
            var errorText = string.IsNullOrEmpty(value) ? "Oops! You must type something." : null;
            if (TextFieldErrorText != errorText)
               TextFieldErrorText = errorText;
         }
      }

      #endregion

      #region Auto Complete

      public string AutoCompleteValue
      {
         get { return Get<string>() ?? ""; }
         set
         {
            Set(value);
            Changed(nameof(AutoCompleteResults));
         }
      }

      private readonly List<string> _planets = new List<string> {
         "Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Neptune", "Uranus" };

      public IEnumerable<string> AutoCompleteResults =>
         _planets.Where(i => !string.IsNullOrEmpty(AutoCompleteValue)
                             && i.StartsWith(AutoCompleteValue, StringComparison.OrdinalIgnoreCase));


      public AutoComplete AutoCompleteProps = new AutoComplete
      {
         hintText = "Type a planet",
         floatingLabelText = "Auto Complete"
      };

      #endregion

      #region Checkbox

      public bool Checked
      {
         get { return Get<bool>(); }
         set
         {
            Set(value);
            Changed(nameof(CheckboxResult));
         }
      }

      public string CheckboxLabel => "Enable me";

      public string CheckboxResult => "I am " + (Checked ? "enabled" : "disabled");

      #endregion

      #region Radio Button

      public IEnumerable<RadioButton> RadioButtons => new List<RadioButton>
      {
         new RadioButton { label = "Default", value="default" },
         new RadioButton { label = "Primary", value="primary" },
         new RadioButton { label = "Secondary", value="secondary" }
      };

      public string RadioValue
      {
         get { return Get<string>() ?? "default"; }
         set
         {
            Set(value);
            Changed(nameof(RadioResult));
         }
      }

      public string RadioResult => RadioButtons.First(i => i.value == RadioValue).label;

      #endregion

      #region Toggle

      public bool Toggled
      {
         get { return Get<bool>(); }
         set
         {
            Set(value);
            Changed(nameof(ToggleLabel));
         }
      }

      public string ToggleLabel => Toggled ? "Wax on" : "Wax off";

      #endregion

      #region Select Field

      public IEnumerable<MenuItem> SelectFieldMenu => new List<MenuItem>
      {
         new MenuItem { value = "1", primaryText = "Object One" },
         new MenuItem { value = "2", primaryText = "Object Two" },
         new MenuItem { value = "3", primaryText = "Object Three" },
         new MenuItem { value = "4", primaryText = "Object Four" },
      };
      public string SelectFieldLabel => "Select an item ...";

      public string SelectFieldValue
      {
         get { return Get<string>(); }
         set
         {
            Set(value);
            Changed(nameof(SelectFieldResult));
         }
      }

      public string SelectFieldResult => !string.IsNullOrEmpty(SelectFieldValue) ?
         $"You selected: {SelectFieldMenu.First(i => i.value == SelectFieldValue).primaryText}" : null;

      #endregion

      #region Chip

      private readonly IEnumerable<Chip> _chips = new List<Chip>
      {
         new Chip { key="A", label="Filter A" },
         new Chip { key="B", label="Filter B" },
         new Chip { key="C", label="Filter C" },
         new Chip { key="D", label="Filter D" }
      };

      public IEnumerable<Chip> Chips
      {
         get { return Get<IEnumerable<Chip>>() ?? new List<Chip>(_chips); }
         set { Set(value); }
      }

      public Action<string> DeleteChip => key => Chips = Chips.Where(i => i.key != key);

      public Action ResetChips => () => Chips = new List<Chip>(_chips);

      #endregion
   }
}
