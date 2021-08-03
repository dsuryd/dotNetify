import React, { useState } from "react";
import { useConnect } from "dotnetify";
import { TextBox, InlineEdit } from "./components";
import StarRatings from "react-star-ratings";
import { Business } from "./models/Business";

interface State {
  Businesses: Business[];
}

export const Businesses = () => {
  const { vm, state } = useConnect<State>("BusinessesVM");
  const [newName, setNewName] = useState<string>("");

  const addBusiness = (name: string) => {
    vm.$dispatch({ Add: new Business(0, name) });
    setNewName("");
  };

  const updateBusiness = (id: number, name: string, rating: number) => {
    vm.$dispatch({ Update: new Business(id, name, rating) });
  };

  const removeBusiness = (id: number) => {
    vm.$dispatch({ Remove: new Business(id) });
  };

  return (
    <div className="container" style={{ padding: "1rem" }}>
      <table className="table table-striped">
        <tbody>
          <tr>
            <th>Business Id</th>
            <th>Business Name</th>
            <th>Rating</th>
            <th style={{ width: "2rem" }} />
          </tr>
          {state.Businesses?.map(business => (
            <tr key={business.Id}>
              <td style={{ verticalAlign: "middle" }}>{business.Id}</td>
              <td style={{ verticalAlign: "middle" }}>
                <InlineEdit text={business.Name} onChange={(value: string) => updateBusiness(business.Id, value, business.Rating)} />
              </td>
              <td>
                <StarRatings
                  rating={business.Rating}
                  starDimension="2rem"
                  numberOfStars={5}
                  starRatedColor="orange"
                  changeRating={(value: number) => updateBusiness(business.Id, business.Name, value)}
                />
              </td>
              <td>
                <button type="button" className="btn btn-link" onClick={_ => removeBusiness(business.Id)} children="Delete" />
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      <div>
        <TextBox tabIndex={0} placeholder="Add Business name here" value={newName} onChange={setNewName} onUpdate={addBusiness} />
      </div>
    </div>
  );
};
