import RenderVue from '../components/RenderVue';
import { HelloWorldCss } from '../components/css';
import HelloWorldVue from './HelloWorld.vue';

export default _ => (
  <HelloWorldCss>
    <RenderVue src={HelloWorldVue} />
  </HelloWorldCss>
);
