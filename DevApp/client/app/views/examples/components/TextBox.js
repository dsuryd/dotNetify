import React, { createRef } from 'react';

class TextBox extends React.Component {
  constructor(props) {
    super(props);
    this.textInput = createRef();
    this.state = { changed: false };
  }

  focus() {
    this.textInput.current.focus();
  }

  handleChange = event => {
    this.setState({ changed: true });
    this.props.onChange(event.target.value);
  };

  handleBlur = event => {
    if (this.state.changed && this.props.onUpdate) {
      this.props.onUpdate(this.props.value);
    }
    this.setState({ changed: false });
    this.props.onBlur && this.props.onBlur(event);
  };

  handleKeydown = event => {
    if (event.keyCode == 13) this.handleBlur(event);
  };

  render() {
    return (
      <div>
        <label>{this.props.label}</label>
        <input
          type={this.props.type || 'text'}
          className="form-control"
          value={this.props.value}
          placeholder={this.props.placeholder}
          onChange={this.handleChange}
          onBlur={this.handleBlur}
          onKeyDown={this.handleKeydown}
          ref={this.textInput}
        />
        {this.props.errorText && <b>{this.props.errorText}</b>}
      </div>
    );
  }
}

export default TextBox;
