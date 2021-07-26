import React, { createRef, HTMLAttributes } from "react";

export interface ITextBoxProps extends Omit<HTMLAttributes<HTMLInputElement>, "onChange" | "onBlur"> {
  value: string;
  label?: string;
  type?: string;
  placeholder?: string;
  errorText?: string;
  onChange: (value: string) => void;
  onUpdate: (value: string) => void;
  onBlur?: (value: string) => void;
}

export class TextBox extends React.Component<ITextBoxProps, any> {
  private textInput: React.RefObject<HTMLInputElement>;

  constructor(props: ITextBoxProps) {
    super(props);
    this.textInput = createRef();
    this.state = { changed: false };
  }

  focus() {
    this.textInput?.current?.focus();
  }

  blur(value: string) {
    if (this.state.changed && this.props.onUpdate) {
      this.props.onUpdate(this.props.value);
    }
    this.setState({ changed: false });
    this.props.onBlur && this.props.onBlur(value);
  }

  handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    this.setState({ changed: true });
    this.props.onChange(event.target.value);
  };

  handleBlur = (event: React.FocusEvent<HTMLInputElement>) => {
    this.blur(event.target.value);
  };

  handleKeydown = (event: React.KeyboardEvent<HTMLInputElement>) => {
    if (event.keyCode == 13 && this.textInput.current) this.blur(this.textInput.current.value);
  };

  render() {
    const { label, type, value, placeholder, errorText } = this.props;
    return (
      <div>
        <label>{label}</label>
        <input
          type={type || "text"}
          className="form-control"
          value={value}
          placeholder={placeholder}
          onChange={this.handleChange}
          onBlur={this.handleBlur}
          onKeyDown={this.handleKeydown}
          ref={this.textInput}
        />
        {errorText && <b>{errorText}</b>}
      </div>
    );
  }
}
