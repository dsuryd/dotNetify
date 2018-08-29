import React from "react";
import TextBox from "./TextBox";

class InlineEdit extends React.Component {
  state = {
    edit: false,
    value: this.props.text
  };

  handleClick = event => {
    event.stopPropagation();
    if (!this.state.edit) this.setState({ edit: true });
  };

  handleUpdate = _ => {
    this.setState({ edit: false });
    if (this.state.value.length > 0 && this.state.value != this.props.text) {
      this.props.onChange(this.state.value);
    } else this.setState({ value: this.props.text });
  };

  render() {
    const textStyle = { display: "inline-block" };

    if (this.state.edit)
      return (
        <div>
          <TextBox
            id="EditField"
            ref={input => input && input.focus()}
            value={this.state.value}
            onClick={this.handleClick}
            onBlur={this.handleUpdate}
            onUpdate={this.handleUpdate}
            onChange={value => this.setState({ value: value })}
          />
        </div>
      );

    return (
      <div style={textStyle} onClick={this.handleClick}>
        {this.state.value}
      </div>
    );
  }
}

export default InlineEdit;
