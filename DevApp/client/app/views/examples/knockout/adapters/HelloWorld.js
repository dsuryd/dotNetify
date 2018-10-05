import RenderKnockout from './RenderKnockout';
import { HelloWorldCss } from '../../components/css';
import HelloWorldHtml from '../HelloWorld.html';

export default _ => (
	<HelloWorldCss>
		<RenderKnockout html={HelloWorldHtml} />
	</HelloWorldCss>
);
