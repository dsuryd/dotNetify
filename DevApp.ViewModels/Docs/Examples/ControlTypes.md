##### ControlTypes.js

```jsx
import React from "react";
import dotnetify from "dotnetify";
import styled from "styled-components";

const Container = styled.table`
  /*...styles...*/
`;

class ControlTypes extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      TextBoxValue: "",
      SearchBox: "",
      SearchResults: [],
      ShowMeCheckBox: true,
      EnableMeCheckBox: true,
      SimpleDropDownValue: "",
      SimpleDropDownOptions: [],
      DropDownValue: "",
      DropDownOptions: []
    };

    // Connect this component to the back-end view model.
    this.vm = dotnetify.react.connect("ControlTypesVM", this);

    // Set up function to dispatch state to the back-end with optimistic update.
    this.dispatchState = state => {
      this.setState(state);
      this.vm.$dispatch(state);
    };
  }

  componentWillUnmount() {
    this.vm.$destroy();
  }

  render() {
    return (
      <Container>
        <tbody>
          <tr>
            <td>
              <b>Text box:</b>
              <label>(updates on losing focus)</label>
            </td>
            <td>
              <input
                type="text"
                className="form-control"
                placeholder={this.state.TextBoxPlaceHolder}
                value={this.state.TextBoxValue}
                onChange={e => this.setState({ TextBoxValue: e.target.value })}
                onBlur={_ => this.dispatchState({ TextBoxValue: this.state.TextBoxValue })}
              />
            </td>
            <td>{this.state.TextBoxResult}</td>
          </tr>

          <tr>
            <td>
              <b>Search box:</b>
              <label>(updates on keystroke)</label>
            </td>
            <td>
              <input
                type="text"
                className="form-control"
                placeholder={this.state.SearchBoxPlaceHolder}
                value={this.state.SearchBox}
                onChange={e => this.dispatchState({ SearchBox: e.target.value })}
              />
              {this.state.SearchResults.length > 0 && (
                <ul className="list-group">
                  {this.state.SearchResults.map((text, idx) => (
                    <li className="list-group-item" key={idx} onClick={_ => this.dispatchState({ SearchBox: text })}>
                      {text}
                    </li>
                  ))}
                </ul>
              )}
            </td>
            <td />
          </tr>

          <tr>
            <td>
              <b>Check box:</b>
            </td>
            <td>
              <label>
                <input
                  type="checkbox"
                  checked={this.state.ShowMeCheckBox}
                  onChange={_ => this.dispatchState({ ShowMeCheckBox: !this.state.ShowMeCheckBox })}
                />
                <span>Show me</span>
              </label>
              <label>
                <input
                  type="checkbox"
                  checked={this.state.EnableMeCheckBox}
                  onChange={_ => this.dispatchState({ EnableMeCheckBox: !this.state.EnableMeCheckBox })}
                />
                <span>Enable me</span>
              </label>
            </td>
            <td>
              {this.state.ShowMeCheckBox && (
                <button className="btn btn-secondary" disabled={!this.state.EnableMeCheckBox}>
                  {this.state.CheckBoxResult}
                </button>
              )}
            </td>
          </tr>

          <tr>
            <td>
              <b>Simple drop-down list:</b>
              <label>(string data type)</label>
            </td>
            <td>
              <select
                value={this.state.SimpleDropDownValue}
                onChange={e => this.dispatchState({ SimpleDropDownValue: e.target.value })}
              >
                <option value="" disabled>
                  Choose...
                </option>
                {this.state.SimpleDropDownOptions.map((text, idx) => (
                  <option key={idx} value={text}>
                    {text}
                  </option>
                ))}
              </select>
            </td>
            <td>{this.state.SimpleDropDownResult}</td>
          </tr>

          <tr>
            <td>
              <b>Drop-down list:</b>
              <label>(object data type)</label>
            </td>
            <td>
              <select
                value={this.state.DropDownValue}
                onChange={e => this.dispatchState({ DropDownValue: e.target.value })}
              >
                <option value="0" disabled>
                  {this.state.DropDownCaption}
                </option>
                {this.state.DropDownOptions.map((opt, idx) => (
                  <option key={idx} value={opt.Id}>
                    {opt.Text}
                  </option>
                ))}
              </select>
            </td>
            <td>{this.state.DropDownResult}</td>
          </tr>

          <tr>
            <td>
              <b>Radio button:</b>
            </td>
            <td>
              <label className={this.state.RadioButtonStyle}>
                <input
                  type="radio"
                  value="green"
                  checked={this.state.RadioButtonValue == 'green'}
                  onChange={e => this.dispatchState({ RadioButtonValue: e.target.value })}
                />
                &nbsp;Green
              </label>
              <label className={this.state.RadioButtonStyle}>
                <input
                  type="radio"
                  value="yellow"
                  checked={this.state.RadioButtonValue == 'yellow'}
                  onChange={e => this.dispatchState({ RadioButtonValue: e.target.value })}
                />
                &nbsp;Yellow
              </label>
            </td>
            <td />
          </tr>

          <tr>
            <td>
              <b>Button:</b>
            </td>
            <td>
              <button
                type="button"
                className="btn btn-primary"
                onClick={_ => this.dispatchState({ ButtonClicked: true })}
              >
                Click me
              </button>
            </td>
            <td>
              {this.state.ClickCount > 0 && (
                <span>
                  You clicked me <b>{this.state.ClickCount}</b>
                  &nbsp;times!
                </span>
              )}
            </td>
          </tr>
        </tbody>
      </Container>
    );
  }
}
```

##### ControlTypesVM.cs

```csharp
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
    public string CheckBoxResult => $"I am " + (EnableMeCheckBox ? "ENABLED" : "DISABLED"); 

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
```
