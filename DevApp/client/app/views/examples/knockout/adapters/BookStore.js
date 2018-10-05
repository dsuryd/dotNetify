import RenderKnockout from './RenderKnockout';
import { BookStoreCss } from '../../components/css';
import BookStoreHtml from '../BookStore.html';

window.BookStoreVM = require('../BookStoreVM.ts').default;

export default props => (
	<BookStoreCss>
		<RenderKnockout html={BookStoreHtml} htmlAttrs={props} />
	</BookStoreCss>
);
