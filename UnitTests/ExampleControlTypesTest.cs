using DotNetify.DevApp;
using DotNetify.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
   public class ExampleControlTypesTest
   {
      private HubEmulator _hubEmulator;

      public ExampleControlTypesTest()
      {
         _hubEmulator = new HubEmulatorBuilder()
            .Register<ControlTypesVM>()
            .Build();
      }

      [TestMethod]
      public void ExampleControlTypes_SetTextBox_ReturnsTextBoxResult()
      {
         var client = _hubEmulator.CreateClient();

         var initialState = client.Connect(nameof(ControlTypesVM)).As<dynamic>();
         var response = client.Dispatch(new { TextBoxValue = "testing" }).As<dynamic>();

         Assert.AreEqual("Type something here", (string) initialState.TextBoxPlaceHolder);
         Assert.AreEqual("You typed: testing", (string) response.TextBoxResult);
      }

      [TestMethod]
      public void ExampleControlTypes_SetSearchBox_ReturnsSearchBoxResult()
      {
         var client = _hubEmulator.CreateClient();

         client.Connect(nameof(ControlTypesVM));
         var response = client.Dispatch(new { SearchBox = "m" }).As<dynamic>();

         Assert.AreEqual(2, response.SearchResults.Count);
         Assert.AreEqual("Mercury", (string) response.SearchResults[0]);
         Assert.AreEqual("Mars", (string) response.SearchResults[1]);

         response = client.Dispatch(new { SearchBox = "ma" }).As<dynamic>();

         Assert.AreEqual(1, response.SearchResults.Count);
         Assert.AreEqual("Mars", (string) response.SearchResults[0]);
      }

      [TestMethod]
      public void ExampleControlTypes_SetCheckBox_ReturnsCheckBoxResult()
      {
         var client = _hubEmulator.CreateClient();

         client.Connect(nameof(ControlTypesVM));
         var response = client.Dispatch(new { EnableMeCheckBox = false }).As<dynamic>();

         Assert.AreEqual("Disabled", (string) response.CheckBoxResult);

         response = client.Dispatch(new { EnableMeCheckBox = true }).As<dynamic>();

         Assert.AreEqual("Enabled", (string) response.CheckBoxResult);
      }

      [TestMethod]
      public void ExampleControlTypes_SelectSimpleDropDown_ReturnsDropDownResult()
      {
         var client = _hubEmulator.CreateClient();

         var initialState = client.Connect(nameof(ControlTypesVM)).As<dynamic>();
         var response = client.Dispatch(new { SimpleDropDownValue = "Two" }).As<dynamic>();

         Assert.AreEqual(4, initialState.SimpleDropDownOptions.Count);
         Assert.AreEqual("One", (string) initialState.SimpleDropDownOptions[0]);
         Assert.AreEqual("Two", (string) initialState.SimpleDropDownOptions[1]);
         Assert.AreEqual("Three", (string) initialState.SimpleDropDownOptions[2]);
         Assert.AreEqual("Four", (string) initialState.SimpleDropDownOptions[3]);
         Assert.AreEqual("You selected: Two", (string) response.SimpleDropDownResult);
      }

      [TestMethod]
      public void ExampleControlTypes_SelectDropDown_ReturnsDropDownResult()
      {
         var client = _hubEmulator.CreateClient();

         var initialState = client.Connect(nameof(ControlTypesVM)).As<dynamic>();
         var response = client.Dispatch(new { DropDownValue = "3" }).As<dynamic>();

         Assert.AreEqual(4, initialState.DropDownOptions.Count);
         Assert.AreEqual(1, (int) initialState.DropDownOptions[0].Id);
         Assert.AreEqual(2, (int) initialState.DropDownOptions[1].Id);
         Assert.AreEqual(3, (int) initialState.DropDownOptions[2].Id);
         Assert.AreEqual(4, (int) initialState.DropDownOptions[3].Id);
         Assert.AreEqual("Object One", (string) initialState.DropDownOptions[0].Text);
         Assert.AreEqual("Object Two", (string) initialState.DropDownOptions[1].Text);
         Assert.AreEqual("Object Three", (string) initialState.DropDownOptions[2].Text);
         Assert.AreEqual("Object Four", (string) initialState.DropDownOptions[3].Text);
         Assert.AreEqual("You selected: Object Three", (string) response.DropDownResult);
      }

      [TestMethod]
      public void ExampleControlTypes_SetRadioButton_ReturnsRadioButtonResult()
      {
         var client = _hubEmulator.CreateClient();

         client.Connect(nameof(ControlTypesVM));
         var response = client.Dispatch(new { RadioButtonValue = "green" }).As<dynamic>();

         Assert.AreEqual("label-success", (string) response.RadioButtonStyle);

         response = client.Dispatch(new { RadioButtonValue = "yellow" }).As<dynamic>();

         Assert.AreEqual("label-warning", (string) response.RadioButtonStyle);
      }

      [TestMethod]
      public void ExampleControlTypes_ClickButton_ReturnsClickCount()
      {
         var client = _hubEmulator.CreateClient();

         client.Connect(nameof(ControlTypesVM));
         var response = client.Dispatch(new { ButtonClicked = true }).As<dynamic>();

         Assert.AreEqual(1, (int) response.ClickCount);

         response = client.Dispatch(new { ButtonClicked = true }).As<dynamic>();

         Assert.AreEqual(2, (int) response.ClickCount);
      }
   }
}