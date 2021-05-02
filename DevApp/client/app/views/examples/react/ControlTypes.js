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
                  style={{ marginRight: ".4rem" }}
                  checked={state.RadioButtonValue == "green"}
                  onChange={e => dispatchState({ RadioButtonValue: e.target.value })}
                />
                <span>Green</span>
              </label>
              <label>
                <input
                  type="radio"
                  value="yellow"
                  style={{ marginRight: ".4rem" }}
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

export default ControlTypes;
