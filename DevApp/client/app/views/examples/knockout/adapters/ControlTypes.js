import RenderKnockout from './RenderKnockout';
import { ControlTypesCss } from '../../components/css';
import ControlTypesHtml from '../ControlTypes.html';

export default _ => (
	<ControlTypesCss>
		<RenderKnockout html={ControlTypesHtml} />
	</ControlTypesCss>
);
