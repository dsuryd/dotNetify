import React from 'react';
import dotnetify from 'dotnetify';
import TextBox from '../components/TextBox';
import { HelloWorldCss } from '../components/css';

class HelloWorld extends React.Component {
  constructor(props) {
    super(props);
    this.state = { FirstName: '', LastName: '' };

    // Connect this component to the back-end view model.
    this.vm = dotnetify.react.connect('HelloWorldVM', this);

    // Set up function to dispatch state to the back-end.
    this.dispatchState = state => this.vm.$dispatch(state);
  }

  componentWillUnmount() {
    this.vm.$destroy();
  }

  render() {
    return (
      <HelloWorldCss>
        <section>
          <TextBox
            label="First Name:"
            placeholder="Type first name here"
            value={this.state.FirstName}
            onChange={value => this.setState({ FirstName: value })}
            onUpdate={value => this.dispatchState({ FirstName: value })}
          />
          <TextBox
            label="Last Name:"
            placeholder="Type last name here"
            value={this.state.LastName}
            onChange={value => this.setState({ LastName: value })}
            onUpdate={value => this.dispatchState({ LastName: value })}
          />
        </section>
        <div>
          Full name is <b>{this.state.FullName}</b>
        </div>
      </HelloWorldCss>
    );
  }
}

export default HelloWorld;
