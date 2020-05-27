import * as React from 'react';
//@ts-ignore
import { useConnect } from 'dotnetify';

interface State {
  Greetings: string;
  ServerTime: string;
}

export const HelloWorld = () => {
  const { state } = useConnect<State>('HelloWorld', this);
  return (
    <div>
      <p>{state.Greetings}</p>
      <p>Server time is: {state.ServerTime}</p>
    </div>
  );
};
