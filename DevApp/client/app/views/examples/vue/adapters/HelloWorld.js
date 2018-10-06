import RenderVue from './RenderVue';
import { HelloWorldCss } from '../../components/css';
import HelloWorld from '../HelloWorld.vue';

export default _ => (
	<HelloWorldCss>
		<RenderVue src={HelloWorld} />
	</HelloWorldCss>
);
