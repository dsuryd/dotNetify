import RenderVue from './RenderVue';
import { BookStoreCss } from '../../components/css';
import BookStore from '../BookStore.vue';
import Book from '../Book.vue';

const BookDefault = {
  name: 'BookDefault',
  template: '<div></div>'
};

export { BookDefault, Book };

export default props => (
  <BookStoreCss>
    <RenderVue src={BookStore} htmlAttrs={props} />
  </BookStoreCss>
);
