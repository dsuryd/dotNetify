import RenderKnockout from '../components/RenderKnockout';
import { SimpleListCss } from '../components/css';
import SimpleListHtml from './SimpleList.html';
import SimpleListVM from './SimpleListVM.ts';

export default _ => (
  <SimpleListCss>
    <RenderKnockout html={SimpleListHtml} js={SimpleListVM} />
  </SimpleListCss>
);
