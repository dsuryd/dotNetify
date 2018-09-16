import React from 'react';
import { Label, Panel } from 'dotnetify-elements';

const menuCss = `
  margin-left: 11%;
  @media (max-width: 1170px) {
    margin-left: 5.5rem;
  }   
  @media (max-width: 767px) {
    margin-left: 2rem;
  } 
`;

const textLinkCss = active => `
padding: .2rem .5rem;  
font-size: medium;
font-weight: 500;
color: ${active ? '#444' : '#aaa'}; 
${!active ? '&:hover {background: #f3f3f3; color: #666;}' : ''}
`;

const CoreLink = ({ active }) => (
  <a href="/core">
    <Label className={active ? 'active' : null} css={textLinkCss(active)}>
      Core
    </Label>
  </a>
);

const ElementsLink = ({ active }) => (
  <a href="/elements">
    <Label css={textLinkCss(active)}>Elements</Label>
  </a>
);

export const MenuLinks = ({ active }) => (
  <Panel horizontal middle css={menuCss} gap="2rem">
    <CoreLink active={active === 'core'} />
    <ElementsLink active={active === 'elements'} />
  </Panel>
);

export default MenuLinks;
