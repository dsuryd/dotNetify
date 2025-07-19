using DotNetify.Testing;
using DevApp.ViewModels;
using Xunit;

namespace DotNetify.Testing.UnitTests
{
   public class ControlTypesTest
   {
      private HubEmulator _hubEmulator;

      public ControlTypesTest()
      {
         _hubEmulator = new HubEmulatorBuilder()
            .Register<ControlTypesVM>(nameof(ControlTypesVM))
            .Build();
      }

      [Fact]
      public void SetTextBox_ReturnsTextBoxResult()
      {
         var client = _hubEmulator.CreateClient();

         var initialState = client.Connect(nameof(ControlTypesVM)).As<dynamic>();
         var response = client.Dispatch(new { TextBoxValue = "testing" }).As<dynamic>();

         Assert.Equal("Type something here", (string) initialState.TextBoxPlaceHolder);
         Assert.Equal("You typed: testing", (string) response.TextBoxResult);
      }

      [Fact]
      public void SetSearchBox_ReturnsSearchBoxResult()
      {
         var client = _hubEmulator.CreateClient();

         client.Connect(nameof(ControlTypesVM));
         var response = client.Dispatch(new { SearchBox = "m" }).As<dynamic>();

         Assert.Equal(2, response.SearchResults.Count);
         Assert.Equal("Mercury", (string) response.SearchResults[0]);
         Assert.Equal("Mars", (string) response.SearchResults[1]);

         response = client.Dispatch(new { SearchBox = "ma" }).As<dynamic>();

         Assert.Equal(1, response.SearchResults.Count);
         Assert.Equal("Mars", (string) response.SearchResults[0]);
      }

      [Fact]
      public void SetCheckBox_ReturnsCheckBoxResult()
      {
         var client = _hubEmulator.CreateClient();

         client.Connect(nameof(ControlTypesVM));
         var response = client.Dispatch(new { EnableMeCheckBox = false }).As<dynamic>();

         Assert.Equal("Disabled", (string) response.CheckBoxResult);

         response = client.Dispatch(new { EnableMeCheckBox = true }).As<dynamic>();

         Assert.Equal("Enabled", (string) response.CheckBoxResult);
      }

      [Fact]
      public void SelectSimpleDropDown_ReturnsDropDownResult()
      {
         var client = _hubEmulator.CreateClient();

         var initialState = client.Connect(nameof(ControlTypesVM)).As<dynamic>();
         var response = client.Dispatch(new { SimpleDropDownValue = "Two" }).As<dynamic>();

         Assert.Equal(4, initialState.SimpleDropDownOptions.Count);
         Assert.Equal("One", (string) initialState.SimpleDropDownOptions[0]);
         Assert.Equal("Two", (string) initialState.SimpleDropDownOptions[1]);
         Assert.Equal("Three", (string) initialState.SimpleDropDownOptions[2]);
         Assert.Equal("Four", (string) initialState.SimpleDropDownOptions[3]);
         Assert.Equal("You selected: Two", (string) response.SimpleDropDownResult);
      }

      [Fact]
      public void SelectDropDown_ReturnsDropDownResult()
      {
         var client = _hubEmulator.CreateClient();

         var initialState = client.Connect(nameof(ControlTypesVM)).As<dynamic>();
         var response = client.Dispatch(new { DropDownValue = "3" }).As<dynamic>();

         Assert.Equal(4, initialState.DropDownOptions.Count);
         Assert.Equal(1, (int) initialState.DropDownOptions[0].Id);
         Assert.Equal(2, (int) initialState.DropDownOptions[1].Id);
         Assert.Equal(3, (int) initialState.DropDownOptions[2].Id);
         Assert.Equal(4, (int) initialState.DropDownOptions[3].Id);
         Assert.Equal("Object One", (string) initialState.DropDownOptions[0].Text);
         Assert.Equal("Object Two", (string) initialState.DropDownOptions[1].Text);
         Assert.Equal("Object Three", (string) initialState.DropDownOptions[2].Text);
         Assert.Equal("Object Four", (string) initialState.DropDownOptions[3].Text);
         Assert.Equal("You selected: Object Three", (string) response.DropDownResult);
      }

      [Fact]
      public void SetRadioButton_ReturnsRadioButtonResult()
      {
         var client = _hubEmulator.CreateClient();

         client.Connect(nameof(ControlTypesVM));
         var response = client.Dispatch(new { RadioButtonValue = "green" }).As<dynamic>();

         Assert.Equal("label-success", (string) response.RadioButtonStyle);

         response = client.Dispatch(new { RadioButtonValue = "yellow" }).As<dynamic>();

         Assert.Equal("label-warning", (string) response.RadioButtonStyle);
      }

      [Fact]
      public void ClickButton_ReturnsClickCount()
      {
         var client = _hubEmulator.CreateClient();

         client.Connect(nameof(ControlTypesVM));
         var response = client.Dispatch(new { ButtonClicked = true }).As<dynamic>();

         Assert.Equal(1, (int) response.ClickCount);

         response = client.Dispatch(new { ButtonClicked = true }).As<dynamic>();

         Assert.Equal(2, (int) response.ClickCount);
      }
   }
}