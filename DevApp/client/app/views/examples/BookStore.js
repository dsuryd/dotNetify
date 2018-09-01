import React from 'react';
import dotnetify from 'dotnetify';
import styled from 'styled-components';
import RenderExample from '../../components/RenderExample';

const Container = styled.div`
  padding: 0 1rem;
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

class BookStore extends React.Component {
  constructor(props) {
    super(props);

    // Connect this component to the back-end view model.
    this.vm = dotnetify.react.connect('BookStoreVM', this);

    // Set up function to dispatch state to the back-end.
    this.dispatchState = state => this.vm.$dispatch(state);
  }

  componentWillUnmount() {
    this.vm.$destroy();
  }

  render() {
    return <Container />;
  }
}

export default _ => (
  <RenderExample vm="BookStoreExample">
    <BookStore />
  </RenderExample>
);
