import React from 'react';
import BookStoreReact, { BookDefault, Book } from './react/BookStore';
import BookStoreKo from './knockout/BookStore';
import Example from './components/Example';

export { BookDefault, Book };
export default props => <Example vm="BookStoreExample" react={<BookStoreReact {...props} />} ko={<BookStoreKo {...props} />} />;
