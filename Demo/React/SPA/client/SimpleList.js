import React, { useState } from 'react';
import { useConnect } from 'dotnetify';
import TextBox from './components/TextBox';
import InlineEdit from './components/InlineEdit';
import { SimpleListCss } from './components/css';

export default () => {
  const [ newName, setNewName ] = useState('');

  // Connect this component to the back-end view model.
  const { vm, state } = useConnect('SimpleListVM', { Employees: [] });

  // Set up function to dispatch state to the back-end.
  const dispatch = state => vm.$dispatch(state);

  return (
    <SimpleListCss>
      <header>
        <span>Add:</span>
        <TextBox
          placeholder="Type full name here"
          value={newName}
          onChange={value => setNewName(value)}
          onUpdate={value => {
            dispatch({ Add: value });
            setNewName('');
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
          {state.Employees.map(employee => (
            <tr key={employee.Id}>
              <td>
                <InlineEdit text={employee.FirstName} onChange={value => dispatch({ Update: { Id: employee.Id, FirstName: value } })} />
              </td>
              <td>
                <InlineEdit text={employee.LastName} onChange={value => dispatch({ Update: { Id: employee.Id, LastName: value } })} />
              </td>
              <td>
                <div onClick={_ => dispatch({ Remove: employee.Id })}>
                  <i className="material-icons">clear</i>
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </SimpleListCss>
  );
};
