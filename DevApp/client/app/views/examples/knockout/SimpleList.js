import RenderKnockout from '../components/RenderKnockout';
import { SimpleListCss } from '../components/css';
import SimpleListHtml from './SimpleList.html';

export default _ => (
  <SimpleListCss>
    <RenderKnockout html={SimpleListHtml} />
  </SimpleListCss>
);
