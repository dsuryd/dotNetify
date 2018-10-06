import React from 'react';
import dotnetify from 'dotnetify';
import TextBox from '../components/TextBox';
import InlineEdit from '../components/InlineEdit';
import { SimpleListCss } from '../components/css';

export default class SimpleList extends React.Component {
  constructor(props) {
    super(props);
    this.state = { Employees: [], newName: '' };

    // Connect this component to the back-end view model.
    this.vm = dotnetify.react.connect('SimpleListVM', this);

    // Set up function to dispatch state to the back-end.
    this.dispatch = state => this.vm.$dispatch(state);

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
      <SimpleListCss>
        <header>
          <span>Add:</span>
          <TextBox
            placeholder="Type full name here"
            value={this.state.newName}
            onChange={value => this.setState({ newName: value })}
            onUpdate={value => {
              this.dispatch({ Add: value });
              this.setState({ newName: '' });
            }}
          />
        </header>
        <table>
          <tbody>
            <tr>
              <th>First Name</th>
              <th>Last Name</th>
              <th />
            </tr>
            {this.state.Employees.map(employee => (
              <tr key={employee.Id}>
                <td>
                  <InlineEdit
                    text={employee.FirstName}
                    onChange={value => this.dispatch({ Update: { Id: employee.Id, FirstName: value } })}
                  />
                </td>
                <td>
                  <InlineEdit
                    text={employee.LastName}
                    onChange={value => this.dispatch({ Update: { Id: employee.Id, LastName: value } })}
                  />
                </td>
                <td>
                  <div onClick={_ => this.dispatch({ Remove: employee.Id })}>
                    <i className="material-icons">clear</i>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </SimpleListCss>
    );
  }
}
