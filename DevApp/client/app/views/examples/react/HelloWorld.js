import React from "react";
import { useConnect } from "dotnetify";
import TextBox from "../components/TextBox";
import { HelloWorldCss } from "../components/css";

const initialState = {
  FirstName: "",
  LastName: ""
};

const HelloWorld = () => {
  const { vm, state, setState } = useConnect("HelloWorldVM", initialState);

  return (
    <HelloWorldCss>
      <section>
        <TextBox
          label="First Name:"
          placeholder="Type first name here"
          value={state.FirstName}
          onChange={value => setState({ FirstName: value })}
          onUpdate={value => vm.$dispatch({ FirstName: value })}
        />
        <TextBox
          label="Last Name:"
          placeholder="Type last name here"
          value={state.LastName}
          onChange={value => setState({ LastName: value })}
          onUpdate={value => vm.$dispatch({ LastName: value })}
        />
      </section>
      <div>
        Full name is <b>{state.FullName}</b>
      </div>
    </HelloWorldCss>
  );
};

export default HelloWorld;
