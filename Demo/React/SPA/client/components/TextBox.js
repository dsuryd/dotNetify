import React, { useState, useEffect, useRef } from 'react';

export default ({ label, type, errorText, placeholder, value, onBlur, onChange, onUpdate }) => {
  const textInput = useRef();
  const [ changed, setChanged ] = useState(false);

  useEffect(() => {
    textInput.current.focus();
  }, []);

  const handleChange = event => {
    setChanged(true);
    onChange(event.target.value);
  };

  const handleBlur = event => {
    if (changed && onUpdate) onUpdate(value);

    setChanged(false);
    onBlur && onBlur(event);
  };

  const handleKeydown = event => event.keyCode == 13 && handleBlur(event);

  return (
    <div>
      {label && <label>{label}</label>}
      <input
        type={type || 'text'}
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
