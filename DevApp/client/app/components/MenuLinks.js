import React from 'react';
import { Label } from 'dotnetify-elements';

const textLinkCss = active => `
padding: .2rem .5rem;  
font-size: medium;
font-weight: 500;
color: ${active ? '#444' : '#aaa'}; 
${!active ? '&:hover {background: #f3f3f3; color: #666;}' : ''}
`;

export const CoreLink = ({ active }) => (
	<a href="/overview">
		<Label className={active ? 'active' : null} css={textLinkCss(active)}>
			Core
		</Label>
	</a>
);

export const ElementsLink = ({ active }) => (
	<a href="/elements">
		<Label css={textLinkCss(active)}>Elements</Label>
	</a>
);
