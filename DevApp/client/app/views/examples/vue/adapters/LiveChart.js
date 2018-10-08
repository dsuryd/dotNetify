import RenderVue from './RenderVue';
import { LiveChartCss } from '../../components/css';
import LiveChart from '../LiveChart.vue';

export default _ => (
  <LiveChartCss>
    <RenderVue src={LiveChart} />
  </LiveChartCss>
);
