import React from 'react';
import { useConnect } from 'dotnetify';

export default props => {
  const { state } = useConnect('Home', { props, state: { Greetings: '', ServerTime: '' } });
  return (
    <React.Fragment>
      <h1>Home</h1>
      <div>
        <p>{state.Greetings}</p>
        <p>Server time is: {state.ServerTime}</p>
      </div>
    </React.Fragment>
  );
};
