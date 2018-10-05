import RenderKnockout from './RenderKnockout';
import { SimpleListCss } from '../../components/css';
import SimpleListHtml from '../SimpleList.html';

window.SimpleListVM = require('../SimpleListVM.ts').default;

export default _ => (
	<SimpleListCss>
		<RenderKnockout html={SimpleListHtml} />
	</SimpleListCss>
);
