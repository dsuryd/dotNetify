import React from 'react';
import BookStoreReact, { BookDefault as BookDefaultReact, Book as BookReact } from './react/BookStore';
import BookStoreVue, { BookDefault as BookDefaultVue, Book as BookVue } from './vue/adapters/BookStore';
import BookStoreKo from './knockout/adapters/BookStore';
import Example from './components/Example';
import { getCurrentFramework } from 'app/components/SelectFramework';

export default props => {
  const framework = getCurrentFramework();
  if (framework === 'React') {
    window.BookDefault = BookDefaultReact;
    window.Book = BookReact;
  }
  else if (framework === 'Vue') {
    window.BookDefault = BookDefaultVue;
    window.Book = BookVue;
  }
  return <Example vm="BookStoreExample" react={<BookStoreReact {...props} />} ko={<BookStoreKo {...props} />} vue={<BookStoreVue />} />;
};
