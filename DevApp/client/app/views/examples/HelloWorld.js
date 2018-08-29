import React from "react";
import dotnetify from "dotnetify";
import styled from "styled-components";
import TextBox from './components/TextBox';
import RenderExample from "../../components/RenderExample";

const Container = styled.div`
  > section {
    display: flex;
    max-width: 1268px;
    margin-bottom: 1rem;
    > * {
      flex: 1;
      margin-right: 1rem;
    }
  }
`;

class HelloWorld extends React.Component {
  constructor(props) {
    super(props);
    this.state = { FirstName: "", LastName: "" };

    // Connect this component to the back-end view model.
    this.vm = dotnetify.react.connect("HelloWorldVM", this);

    // Set up function to dispatch state to the back-end.
    this.dispatchState = state => this.vm.$dispatch(state);
  }

  componentWillUnmount() {
    this.vm.$destroy();
  }

  render() {
    return (
      <Container>
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
        <footer>
          Full name is <b>{this.state.FullName}</b>
        </footer>
      </Container>
    );
  }
}

export default _ => (
  <RenderExample vm="HelloWorldExample">
    <HelloWorld />
  </RenderExample>
);
