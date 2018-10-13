import React from 'react';
import BookStoreReact, { BookDefault as BookDefaultReact, Book as BookReact } from './react/BookStore';
import BookStoreVue, { BookDefault as BookDefaultVue, Book as BookVue } from './vue/adapters/BookStore';
import BookStoreKo from './knockout/adapters/BookStore';
import Example from './components/Example';
import { currentFramework, frameworkSelectEvent } from 'app/components/SelectFramework';

const registerComponents = framework => {
  if (framework === 'React') {
    window.BookDefault = BookDefaultReact;
    window.Book = BookReact;
  }
  else if (framework === 'Vue') {
    window.BookDefault = BookDefaultVue;
    window.Book = BookVue;
  }
};

export default class extends React.Component {
  constructor() {
    super();
    registerComponents(currentFramework);
    this.unsubs = frameworkSelectEvent.subscribe(framework => registerComponents(framework));
  }

  componentWillUnmount() {
    this.unsubs();
  }

  render() {
    const props = this.props;
    return (
      <Example
        vm="BookStoreExample"
        react={<BookStoreReact {...props} />}
        ko={<BookStoreKo {...props} />}
        vue={<BookStoreVue {...props} />}
      />
    );
  }
}
