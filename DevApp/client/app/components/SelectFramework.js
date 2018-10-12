import styled from 'styled-components';
import { Element } from 'dotnetify-elements';
import * as utils from '../utils';

const Select = styled.select`
  width: calc(100% - 1.5rem);

  margin-left: .75rem;
  margin-top: 1rem;
  font-weight: 500;
  border-color: #92d050;
  background-color: transparent;
  &:focus {
    background-color: transparent;
  }
`;

export const frameworkSelectEvent = utils.createEventEmitter();
export const getCurrentFramework = () => window.localStorage['framework'] || React;
export let currentFramework = getCurrentFramework();

frameworkSelectEvent.subscribe(framework => {
  if (framework) {
    currentFramework = framework;
    window.localStorage['framework'] = currentFramework;
  }
});

export default class SelectFramework extends Element {
  handleChange = value => {
    frameworkSelectEvent.emit(value);
    this.dispatch(value);
    this.props.onChange(value);
  };

  componentDidMount() {
    if (this.value !== currentFramework) this.dispatch(currentFramework);
  }

  render() {
    return (
      <Select className="form-control" value={currentFramework} onChange={e => this.handleChange(e.target.value)}>
        <option>React</option>
        <option>Vue</option>
        <option>Knockout</option>
      </Select>
    );
  }
}
