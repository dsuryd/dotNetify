import RenderVue from './RenderVue';
import { BookStoreCss } from '../../components/css';
import BookStore from '../BookStore.vue';

export default _ => (
  <BookStoreCss>
    <RenderVue src={BookStore} />
  </BookStoreCss>
);
