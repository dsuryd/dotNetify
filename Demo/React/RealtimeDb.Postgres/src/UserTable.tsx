import React, { useState } from "react";
import { useConnect } from "dotnetify";
import { TextBox, InlineEdit } from "./components";

class User {
  UserId: number;
  UserName: string;
}

interface State {
  Users: User[];
}

export const UserTable = () => {
  const { vm, state } = useConnect<State>("UserTableVM", this);
  const [newName, setNewName] = useState<string>("");

  const addUser = (name: string) => {
    vm.$dispatch({ AddUser: name });
    setNewName("");
  };

  const updateUser = (id: number, name: string) => {
    vm.$dispatch({ UpdateUser: { UserId: id, UserName: name } as User });
  };

  const removeUser = (UserId: number) => {
    vm.$dispatch({ RemoveUser: UserId });
  };

  return (
    <div className="container">
      <h2>Realtime Database Demo</h2>
      <div style={{ padding: "1rem 0" }}>
        <TextBox tabIndex={0} placeholder="Add new user name" value={newName} onChange={setNewName} onUpdate={addUser} />
      </div>
      <table className="table table-striped">
        <tbody>
          <tr>
            <th>User Id</th>
            <th>User Name</th>
            <th />
          </tr>
          {state.Users?.map(user => (
            <tr key={user.UserId}>
              <td>{user.UserId}</td>
              <td>
                <InlineEdit text={user.UserName} onChange={(value: string) => updateUser(user.UserId, value)} />
              </td>
              <td>
                <button type="button" className="btn btn-link" onClick={_ => removeUser(user.UserId)}>
                  Delete
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};
