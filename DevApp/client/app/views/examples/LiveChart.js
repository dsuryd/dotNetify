import React from 'react';
import LiveChartReact from './react/LiveChart';
import LiveChartKo from './knockout/adapters/LiveChart';
import LiveChartVue from './vue/adapters/LiveChart';
import Example from './components/Example';

export default _ => <Example vm="LiveChartExample" react={<LiveChartReact />} ko={<LiveChartKo />} vue={<LiveChartVue />} />;
