##### ControlTypes.js

```jsx
import React from "react";
import { useConnect } from "dotnetify";
import { ControlTypesCss } from "../components/css";

const ControlTypes = () => {
  const { vm, state, setState } = useConnect("ControlTypesVM", {
    SearchResults: [],
    SimpleDropDownOptions: [],
    DropDownOptions: []
  });

  // Set up function to dispatch state to the back-end with optimistic update.
  const dispatchState = state => {
    setState(state);
    vm.$dispatch(state);
  };

  return (
    <ControlTypesCss>
      <table>
        <tbody>
          <tr>
            <td>
              Text box:
              <label>(updates on losing focus)</label>
            </td>
            <td>
              <input
                type="text"
                className="form-control"
                placeholder={state.TextBoxPlaceHolder}
                value={state.TextBoxValue}
                onChange={e => setState({ TextBoxValue: e.target.value })}
                onBlur={_ =>
                  dispatchState({
                    TextBoxValue: state.TextBoxValue
                  })
                }
              />
              <b>{state.TextBoxResult}</b>
            </td>
          </tr>

          <tr>
            <td>
              Search box:
              <label>(updates on keystroke)</label>
            </td>
            <td>
              <input
                type="text"
                className="form-control"
                placeholder={state.SearchBoxPlaceHolder}
                value={state.SearchBox}
                onChange={e => dispatchState({ SearchBox: e.target.value })}
              />
              {state.SearchResults.length > 0 && (
                <ul className="list-group">
                  {state.SearchResults.map((text, idx) => (
                    <li className="list-group-item" key={idx} onClick={_ => dispatchState({ SearchBox: text })}>
                      {text}
                    </li>
                  ))}
                </ul>
              )}
            </td>
          </tr>

          <tr>
            <td>Check box:</td>
            <td>
              <label>
                <input
                  type="checkbox"
                  style={{ marginRight: ".4rem" }}
                  checked={state.ShowMeCheckBox}
                  onChange={_ =>
                    dispatchState({
                      ShowMeCheckBox: !state.ShowMeCheckBox
                    })
                  }
                />
                <span>Show me</span>
              </label>
              <label>
                <input
                  type="checkbox"
                  style={{ marginRight: ".4rem" }}
                  checked={state.EnableMeCheckBox}
                  onChange={_ =>
                    dispatchState({
                      EnableMeCheckBox: !state.EnableMeCheckBox
                    })
                  }
                />
                <span>Enable me</span>
              </label>
              {state.ShowMeCheckBox && (
                <button className="btn btn-secondary" disabled={!state.EnableMeCheckBox}>
                  {state.CheckBoxResult}
                </button>
              )}
            </td>
          </tr>

          <tr>
            <td>
              Simple drop-down list:
              <label>(string data type)</label>
            </td>
            <td>
              <select
                className="form-control"
                value={state.SimpleDropDownValue}
                onChange={e => dispatchState({ SimpleDropDownValue: e.target.value })}
              >
                <option value="" disabled>
                  Choose...
                </option>
                {state.SimpleDropDownOptions.map((text, idx) => (
                  <option key={idx} value={text}>
                    {text}
                  </option>
                ))}
              </select>
              <b>{state.SimpleDropDownResult}</b>
            </td>
          </tr>

          <tr>
            <td>
              Drop-down list:
              <label>(object data type)</label>
            </td>
            <td>
              <select className="form-control" value={state.DropDownValue} onChange={e => dispatchState({ DropDownValue: e.target.value })}>
                <option value="0" disabled>
                  {state.DropDownCaption}
                </option>
                {state.DropDownOptions.map((opt, idx) => (
                  <option key={idx} value={opt.Id}>
                    {opt.Text}
                  </option>
                ))}
              </select>
              <b>{state.DropDownResult}</b>
            </td>
          </tr>

          <tr>
            <td>Radio button:</td>
            <td>
              <label>
                <input
                  type="radio"
                  value="green"
                  checked={state.RadioButtonValue == "green"}
                  onChange={e => dispatchState({ RadioButtonValue: e.target.value })}
                />
                <span>Green</span>
              </label>
              <label>
                <input
                  type="radio"
                  value="yellow"
                  checked={state.RadioButtonValue == "yellow"}
                  onChange={e => dispatchState({ RadioButtonValue: e.target.value })}
                />
                <span>Yellow</span>
              </label>
              <button className={"btn " + state.RadioButtonStyle}>Result</button>
            </td>
          </tr>

          <tr>
            <td>Button: </td>
            <td>
              <button type="button" className="btn btn-secondary" onClick={_ => dispatchState({ ButtonClicked: true })}>
                Click Me
              </button>
              {state.ClickCount > 0 && (
                <span style={{ marginLeft: "2rem" }}>
                  You clicked me <b>{state.ClickCount}</b>
                  &nbsp;times!
                </span>
              )}
            </td>
          </tr>
        </tbody>
      </table>
    </ControlTypesCss>
  );
};
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

    public Action<bool> ButtonClicked => _ => ClickCount++;

    public int ClickCount
    {
        get => Get<int>();
        set => Set(value);
    }
}
```
