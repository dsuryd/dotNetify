import styled from "styled-components";
import { Element } from "dotnetify-elements";
import * as utils from "../utils";

const Select = styled.select`
  width: calc(100% - 1.5rem);

  margin-left: 0.75rem;
  margin-top: 1rem;
  font-weight: 500;
  border-color: #92d050;
  background-color: transparent;
  &:focus {
    background-color: transparent;
  }
`;

export const frameworkSelectEvent = utils.createEventEmitter();
export const getCurrentFramework = () => {
  const params = Array.from(
    new URLSearchParams(window.location.search).keys()
  ).map(x => x.toLowerCase());
  if (params.includes("react")) return "React";
  else if (params.includes("vue")) return "Vue";
  else if (params.includes("knockout")) return "Knockout";
  else if (params.includes("blazor")) return "Blazor";
  return window.localStorage["framework"] || "React";
};
export let currentFramework = getCurrentFramework();

frameworkSelectEvent.subscribe(framework => {
  if (framework) {
    currentFramework = framework;
    window.localStorage["framework"] = currentFramework;
  }
});

export default class SelectFramework extends Element {
  handleChange = value => {
    if (value === "Blazor")
      window.location.href = "https://dotnetify-blazor.herokuapp.com";
    frameworkSelectEvent.emit(value);
    this.dispatch(value);
    this.props.onChange(value);
  };

  componentDidMount() {
    if (this.value !== currentFramework) this.handleChange(currentFramework);
  }

  render() {
    return (
      <Select
        className="form-control"
        value={currentFramework}
        onChange={e => this.handleChange(e.target.value)}
      >
        <option>React</option>
        <option>Vue</option>
        <option>Knockout</option>
        <option>Blazor</option>
      </Select>
    );
  }
}
