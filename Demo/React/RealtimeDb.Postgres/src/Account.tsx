import React, { useState } from "react";
import { useConnect } from "dotnetify";
import { TextBox, InlineEdit } from "./components";
import { UserModel } from "./models/UserModel";

interface State {
  Users: UserModel[];
}

export const Account = () => {
  const { vm, state } = useConnect<State>("AccountVM", this);
  const [newName, setNewName] = useState<string>("");

  const addUser = (name: string) => {
    vm.$dispatch({ AddUser: new UserModel(0, name) });
    setNewName("");
  };

  const updateUser = (id: number, name: string) => {
    vm.$dispatch({ UpdateUser: new UserModel(id, name) });
  };

  const removeUser = (id: number) => {
    vm.$dispatch({ RemoveUser: new UserModel(id, "") });
  };

  return (
    <div className="container">
      <h2>User Account</h2>
      <div style={{ padding: "1rem 0" }}>
        <TextBox tabIndex={0} placeholder="Add user name here" value={newName} onChange={setNewName} onUpdate={addUser} />
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
                <button type="button" className="btn btn-link" onClick={_ => removeUser(user.UserId)} children="Delete" />
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};
