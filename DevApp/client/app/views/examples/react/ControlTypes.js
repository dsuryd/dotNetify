import React from 'react';
import dotnetify from 'dotnetify';
import { ControlTypesCss } from '../components/css';

export default class ControlTypes extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      TextBoxValue: '',
      SearchBox: '',
      SearchResults: [],
      ShowMeCheckBox: true,
      EnableMeCheckBox: true,
      SimpleDropDownValue: '',
      SimpleDropDownOptions: [],
      DropDownValue: '',
      DropDownOptions: []
    };

    // Connect this component to the back-end view model.
    this.vm = dotnetify.react.connect('ControlTypesVM', this);

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
                  placeholder={this.state.TextBoxPlaceHolder}
                  value={this.state.TextBoxValue}
                  onChange={e => this.setState({ TextBoxValue: e.target.value })}
                  onBlur={_ => this.dispatchState({ TextBoxValue: this.state.TextBoxValue })}
                />
                <b>{this.state.TextBoxResult}</b>
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
            </tr>

            <tr>
              <td>Check box:</td>
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
                {this.state.ShowMeCheckBox && (
                  <button className="btn btn-secondary" disabled={!this.state.EnableMeCheckBox}>
                    {this.state.CheckBoxResult}
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
                <b>{this.state.SimpleDropDownResult}</b>
              </td>
            </tr>

            <tr>
              <td>
                Drop-down list:
                <label>(object data type)</label>
              </td>
              <td>
                <select
                  className="form-control"
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
                <b>{this.state.DropDownResult}</b>
              </td>
            </tr>

            <tr>
              <td>Radio button:</td>
              <td>
                <label>
                  <input
                    type="radio"
                    value="green"
                    checked={this.state.RadioButtonValue == 'green'}
                    onChange={e => this.dispatchState({ RadioButtonValue: e.target.value })}
                  />
                  <span>Green</span>
                </label>
                <label>
                  <input
                    type="radio"
                    value="yellow"
                    checked={this.state.RadioButtonValue == 'yellow'}
                    onChange={e => this.dispatchState({ RadioButtonValue: e.target.value })}
                  />
                  <span>Yellow</span>
                </label>
                <button className={'btn ' + this.state.RadioButtonStyle}>Result</button>
              </td>
            </tr>

            <tr>
              <td>Button: </td>
              <td>
                <button type="button" className="btn btn-secondary" onClick={_ => this.dispatchState({ ButtonClicked: true })}>
                  Click Me
                </button>
                {this.state.ClickCount > 0 && (
                  <span style={{ marginLeft: '2rem' }}>
                    You clicked me <b>{this.state.ClickCount}</b>
                    &nbsp;times!
                  </span>
                )}
              </td>
            </tr>
          </tbody>
        </table>
      </ControlTypesCss>
    );
  }
}
