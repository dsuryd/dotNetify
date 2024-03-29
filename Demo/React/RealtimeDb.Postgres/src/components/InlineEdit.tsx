import React from "react";
import styled from "styled-components";
import { TextBox } from "./TextBox";

const EditableText = styled.div`
  display: inline-block;
  &:hover,
  &:focus {
    &:after {
      font-family: "Material Icons";
      content: "edit";
    }
  }
`;

export class InlineEdit extends React.Component<any> {
  state = {
    edit: false,
    value: this.props.text
  };

  componentDidUpdate(prevProps) {
    if (prevProps.text !== this.props.text) this.setState({ value: this.props.text });
  }

  handleBlur = () => this.setState({ edit: false });

  handleClick = event => {
    event && event.stopPropagation();
    if (!this.state.edit) this.setState({ edit: true });
  };

  handleUpdate = _ => {
    this.setState({ edit: false });
    if (this.state.value.length > 0 && this.state.value != this.props.text) {
      this.props.onChange(this.state.value);
    } else this.setState({ value: this.props.text });
  };

  render() {
    if (this.state.edit)
      return (
        <div>
          <TextBox
            id="EditField"
            ref={input => input && input.focus()}
            value={this.state.value}
            onClick={this.handleClick}
            onUpdate={this.handleUpdate}
            onBlur={this.handleBlur}
            onChange={value => this.setState({ value: value })}
          />
        </div>
      );

    return (
      <EditableText
        tabIndex={0}
        className="editable"
        onClick={this.handleClick}
        onKeyDown={e => e.keyCode === 13 && this.handleClick(null)}
      >
        {this.state.value}
      </EditableText>
    );
  }
}
