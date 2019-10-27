import React, { useState } from 'react';
import styled from 'styled-components';
import TextBox from './TextBox';

const EditableText = styled.div`
  display: inline-block;
  &:hover {
    &:after {
      font-family: "Material Icons";
      content: "edit";
    }
  }
`;

export default ({ text, onChange }) => {
  const [ edit, setEdit ] = useState(false);
  const [ value, setValue ] = useState(text);

  const handleBlur = _ => setEdit(false);

  const handleClick = event => {
    event.stopPropagation();
    if (!edit) setEdit(true);
  };

  const handleUpdate = _ => {
    setEdit(false);
    if (value.length > 0 && value != text) onChange(value);
    else setValue(text);
  };

  if (edit)
    return (
      <div>
        <TextBox
          id="EditField"
          value={value}
          onBlur={handleBlur}
          onClick={handleClick}
          onUpdate={handleUpdate}
          onChange={value => setValue(value)}
        />
      </div>
    );

  return <EditableText onClick={handleClick}>{value}</EditableText>;
};
