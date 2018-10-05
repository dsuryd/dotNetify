import RenderKnockout from './RenderKnockout';
import { LiveChartCss } from '../../components/css';
import LiveChartHtml from '../LiveChart.html';

window.LiveChartVM = require('../LiveChartVM.ts').default;

export default _ => (
	<LiveChartCss>
		<RenderKnockout html={LiveChartHtml} />
	</LiveChartCss>
);
