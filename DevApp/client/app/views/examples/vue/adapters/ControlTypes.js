import RenderVue from './RenderVue';
import { ControlTypesCss } from '../../components/css';
import ControlTypes from '../ControlTypes.vue';

export default _ => (
	<ControlTypesCss>
		<RenderVue src={ControlTypes} />
	</ControlTypesCss>
);
