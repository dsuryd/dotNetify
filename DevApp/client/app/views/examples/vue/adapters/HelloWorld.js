import RenderVue from './RenderVue';
import { HelloWorldCss } from '../../components/css';
import HelloWorldVue from '../HelloWorld.vue';

export default _ => (
	<HelloWorldCss>
		<RenderVue src={HelloWorldVue} />
	</HelloWorldCss>
);
