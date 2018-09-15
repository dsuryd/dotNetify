import React from 'react';
import styled from 'styled-components';
import { Frame, Markdown, MarkdownTOC, Panel, Tab, TabItem, Theme, VMContext } from 'dotnetify-elements';

const frameCss = `
  margin-left: 10%; 
  max-width: 85%;
  @media (max-width: 1170px) {
    margin-left: 2rem;
    max-width: calc(100% - 2rem);
  }    
`;

const panelCss = `
  > *:first-child {
  max-width: 1080px;
  overflow-y: auto;
  @media (max-width: 1170px) {
    max-width: 100%;
    flex-basis: 100%;
  }  
}
`;

const Sidebar = styled.div`
	position: fixed;
	border-left: 1px solid orange;
	margin-left: 2rem;
	padding-left: 1rem;
	@media (max-width: 1170px) {
		display: none;
	}
`;

const RenderExample = props => (
	<Theme>
		<VMContext vm={props.vm} options={props.vmOptions}>
			<Frame css={frameCss}>
				<Tab margin="1.5rem 0">
					<TabItem label="Output">{props.children}</TabItem>
					<TabItem label="Source">
						<Tab margin="1.5rem 0">
							<TabItem label="View">
								<Panel horizontal css={panelCss}>
									<Markdown id="ViewSource" flex="72%" css="width: 100%" />
									<Sidebar flex>
										<MarkdownTOC id="ViewSource" />
									</Sidebar>
								</Panel>
							</TabItem>
							<TabItem label="View Model">
								<Panel horizontal css={panelCss}>
									<Markdown id="ViewModelSource" flex="72%" css="width: 100%" />
									<Sidebar flex>
										<MarkdownTOC id="ViewModelSource" />
									</Sidebar>
								</Panel>
							</TabItem>
						</Tab>
					</TabItem>
				</Tab>
			</Frame>
		</VMContext>
	</Theme>
);

export default RenderExample;
