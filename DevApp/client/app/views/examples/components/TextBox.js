import React, { useRef, useLayoutEffect, useCallback } from "react";

const TextBox = ({ value, label, type, placeholder, errorText, onChange, onUpdate, onBlur }) => {
  const textInput = useRef();
  const changed = useRef(false);

  useLayoutEffect(() => {
    textInput.current.focus();
  }, []);

  const handleChange = useCallback(
    event => {
      changed.current = true;
      onChange && onChange(event.target.value);
    },
    [onChange]
  );

  const handleBlur = useCallback(
    event => {
      if (changed.current && onUpdate) {
        onUpdate(value);
      }
      changed.current = false;
      onBlur && onBlur(event);
    },
    [onUpdate]
  );

  const handleKeydown = event => {
    if (event.keyCode == 13) handleBlur(event);
  };

  return (
    <div>
      <label>{label}</label>
      <input
        type={type || "text"}
        className="form-control"
        value={value}
        placeholder={placeholder}
        onChange={handleChange}
        onBlur={handleBlur}
        onKeyDown={handleKeydown}
        ref={textInput}
      />
      {errorText && <b>{errorText}</b>}
    </div>
  );
};

export default TextBox;
