import React, { useState } from "react";
import { useConnect } from "dotnetify";
import TextBox from "../components/TextBox";
import InlineEdit from "../components/InlineEdit";
import { SimpleListCss } from "../components/css";

const SimpleList = () => {
  const { vm, state } = useConnect("SimpleListVM", { Employees: [] });
  const [newName, setNewName] = useState();
  return (
    <SimpleListCss>
      <d-alert info="true">
        <i class="material-icons">info_outlined</i>
        This is a multicast list. Your edits will appear on all other browser views in real-time.
      </d-alert>
      <header>
        <span>Add:</span>
        <TextBox
          tabIndex="0"
          placeholder="Type full name here"
          value={newName}
          onChange={value => setNewName(value)}
          onUpdate={value => {
            vm.$dispatch({ Add: value });
            setNewName("");
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
                <InlineEdit
                  text={employee.FirstName}
                  onChange={value =>
                    vm.$dispatch({
                      Update: { Id: employee.Id, FirstName: value }
                    })
                  }
                />
              </td>
              <td>
                <InlineEdit
                  text={employee.LastName}
                  onChange={value =>
                    vm.$dispatch({
                      Update: { Id: employee.Id, LastName: value }
                    })
                  }
                />
              </td>
              <td>
                <div onClick={_ => vm.$dispatch({ Remove: employee.Id })}>
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

export default SimpleList;
