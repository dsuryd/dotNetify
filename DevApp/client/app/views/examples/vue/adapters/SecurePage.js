import RenderVue from './RenderVue';
import { SecurePageCss } from '../../components/css';
import SecurePage from '../SecurePage.vue';

export default _ => (
  <SecurePageCss>
    <RenderVue src={SecurePage} />
  </SecurePageCss>
);
