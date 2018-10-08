import RenderVue from './RenderVue';
import { CompositeViewCss } from '../../components/css';
import CompositeView from '../CompositeView.vue';

export default _ => (
  <CompositeViewCss>
    <RenderVue src={CompositeView} />
  </CompositeViewCss>
);
