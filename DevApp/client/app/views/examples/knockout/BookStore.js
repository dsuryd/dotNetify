import RenderKnockout from '../components/RenderKnockout';
import { BookStoreCss } from '../components/css';
import BookStoreHtml from './BookStore.html';

export default (_) => (
	<BookStoreCss>
		<RenderKnockout html={BookStoreHtml} />
	</BookStoreCss>
);
