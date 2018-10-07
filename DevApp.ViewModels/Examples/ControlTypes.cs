using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DotNetify;
using DotNetify.Elements;

namespace DotNetify.DevApp
{
    public class ControlTypesExample : BaseVM
    {
        public ControlTypesExample()
        {
            var markdown = new Markdown("DotNetify.DevApp.Docs.Examples.ControlTypes.md");

            AddProperty("ViewSource", markdown.GetSection(null, "ControlTypesVM.cs"))
               .SubscribeTo(AddInternalProperty<string>("Framework").Select(GetViewSource));

            AddProperty("ViewModelSource", markdown.GetSection("ControlTypesVM.cs"));
        }

        private string GetViewSource(string framework)
        {
            return framework == "Knockout" ?
               new Markdown("DotNetify.DevApp.Docs.Knockout.Examples.ControlTypes.md") :
               framework == "Vue" ?
               new Markdown("DotNetify.DevApp.Docs.Vue.Examples.ControlTypes.md") :
               new Markdown("DotNetify.DevApp.Docs.Examples.ControlTypes.md").GetSection(null, "ControlTypesVM.cs");
        }
    }

    public class ControlTypesVM : BaseVM
    {
        // Text Box

        public string TextBoxValue
        {
            get => Get<string>() ?? "";
            set
            {
                Set(value);
                Changed(() => TextBoxResult);
            }
        }

        public string TextBoxPlaceHolder => "Type something here";
        public string TextBoxResult => !string.IsNullOrEmpty(TextBoxValue) ? $"You typed: {TextBoxValue}" : null;

        // Search Box

        private List<string> Planets = new List<string> { "Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Neptune", "Uranus" };

        public string SearchBox
        {
            get => Get<string>() ?? "";
            set
            {
                Set(value);
                Changed(() => SearchResults);
            }
        }

        public string SearchBoxPlaceHolder => "Type a planet";

        public IEnumerable<string> SearchResults => Planets.Where(i => !string.IsNullOrEmpty(SearchBox)
          && i.ToLower().StartsWith(SearchBox.ToLower())
          && i.ToLower() != SearchBox.ToLower());

        // Check Box

        public bool ShowMeCheckBox
        {
            get => Get<bool?>() ?? true;
            set
            {
                Set(value);
                Changed(() => CheckBoxResult);
            }
        }

        public bool EnableMeCheckBox
        {
            get => Get<bool?>() ?? true;
            set
            {
                Set(value);
                Changed(() => CheckBoxResult);
            }
        }

        public string CheckBoxResult => EnableMeCheckBox ? "Enabled" : "Disabled";

        // Simple Drop-down

        public List<string> SimpleDropDownOptions => new List<string> { "One", "Two", "Three", "Four" };

        public string SimpleDropDownValue
        {
            get => Get<string>() ?? "";
            set
            {
                Set(value);
                Changed(() => SimpleDropDownResult);
            }
        }

        public string SimpleDropDownResult => !string.IsNullOrEmpty(SimpleDropDownValue) ? $"You selected: {SimpleDropDownValue}" : null;

        // Drop Down Objects

        public class DropDownItem
        {
            public int Id { get; set; }
            public string Text { get; set; }
        }

        public string DropDownCaption => "Select an item ...";

        public List<DropDownItem> DropDownOptions
        {
            get => new List<DropDownItem>
         {
            new DropDownItem { Id = 1, Text = "Object One" },
            new DropDownItem { Id = 2, Text = "Object Two" },
            new DropDownItem { Id = 3, Text = "Object Three" },
            new DropDownItem { Id = 4, Text = "Object Four" }
         };
        }

        public int DropDownValue
        {
            get => Get<int>();
            set
            {
                Set(value);
                Changed(() => DropDownResult);
            }
        }

        public string DropDownResult => DropDownValue > 0 ? $"You selected: {DropDownOptions.First(i => i.Id == DropDownValue).Text}" : null;

        // Radio Buttons

        public string RadioButtonValue
        {
            get => Get<string>() ?? "green";
            set
            {
                Set(value);
                Changed(() => RadioButtonStyle);
            }
        }

        public string RadioButtonStyle => RadioButtonValue == "green" ? "label-success" : "label-warning";

        // Button

        public bool ButtonClicked
        {
            get => false;
            set => ClickCount++;
        }

        public int ClickCount
        {
            get => Get<int>();
            set => Set(value);
        }
    }
}