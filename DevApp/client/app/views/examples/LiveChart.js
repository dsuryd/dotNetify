import React from 'react';
import LiveChartReact from './react/LiveChart';
import LiveChartKo from './knockout/LiveChart';
import Example from './components/Example';

export default _ => <Example vm="LiveChartExample" react={<LiveChartReact />} ko={<LiveChartKo />} />;
