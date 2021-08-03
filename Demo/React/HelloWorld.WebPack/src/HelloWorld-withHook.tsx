import React from 'react';
import { useConnect } from 'dotnetify';

interface State {
  Greetings: string;
  ServerTime: string;
}

export const HelloWorld = () => {
  const { state } = useConnect<State>('HelloWorld');
  return (
    <div>
      <p>{state.Greetings}</p>
      <p>Server time is: {state.ServerTime}</p>
    </div>
  );
};
