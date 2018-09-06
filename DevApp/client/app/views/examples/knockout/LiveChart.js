import RenderKnockout from '../components/RenderKnockout';
import { LiveChartCss } from '../components/css';
import LiveChartHtml from './LiveChart.html';

export default _ => (
  <LiveChartCss>
    <RenderKnockout html={LiveChartHtml} />
  </LiveChartCss>
);
