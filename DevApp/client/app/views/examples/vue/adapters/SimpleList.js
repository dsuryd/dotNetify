import RenderVue from './RenderVue';
import { SimpleListCss } from '../../components/css';
import SimpleList from '../SimpleList.vue';

export default _ => (
  <SimpleListCss>
    <RenderVue src={SimpleList} />
  </SimpleListCss>
);
