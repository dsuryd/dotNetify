import React from 'react';
import { Frame, Markdown, Tab, TabItem, Theme, VMContext } from 'dotnetify-elements';

const frameCss = `
   margin-left: 3rem; 
   max-width: calc(100% - 6rem);
   @media (max-width: 1170px) {
      margin-left: 1rem;
      max-width: calc(100% - 2rem);
   }    
`;

const RenderExample = props => (
  <Theme>
    <VMContext vm={props.vm}>
      <Frame css={frameCss}>
        <Tab margin="1.5rem 0">
          <TabItem label="Output">{props.children}</TabItem>
          <TabItem label="Source">
            <Tab margin="1.5rem 0">
              <TabItem label="View">
                <Markdown id="ViewSource" css="max-width: 80rem" />
              </TabItem>
              <TabItem label="View Model">
                <Markdown id="ViewModelSource" css="max-width: 80rem" />
              </TabItem>
            </Tab>
          </TabItem>
        </Tab>
      </Frame>
    </VMContext>
  </Theme>
);

export default RenderExample;
