import RenderKnockout from './RenderKnockout';
import { CompositeViewCss } from '../../components/css';
import CompositeViewHtml from '../CompositeView.html';

export default _ => (
	<CompositeViewCss>
		<RenderKnockout html={CompositeViewHtml} />
	</CompositeViewCss>
);
