import React from 'react';
import styled from 'styled-components';
import { Element, Frame, MarkdownTOC, Panel, Tab, VMContext } from 'dotnetify-elements';

const Sidebar = styled.div`
  position: fixed;
  width: 16%;
  border-left: 1px solid orange;
  margin-left: 2rem;
  padding-left: 1rem;
  p {
    overflow: hidden;
    text-overflow: ellipsis;
  }
`;

const Title = styled.div`
  ${props => (!props.show ? 'display: none' : '')};
  ${props => props.theme.MarkdownTOC.Container};
  margin-bottom: 1rem;
  font-size: 1.1rem;
`;

const frameCss = `
   margin-left: 10%; 
   margin-right: 0;
   max-width: 1268px;
   @media (max-width: 1170px) {
      margin-left: 2rem;
      max-width: calc(100% - 2rem);
      > *:last-child {
         display: none;
      }
    }   
    @media (max-width: 414px) {
      margin-left: 1rem;
      max-width: calc(100% - 1rem);
    }      
`;

const panelCss = `
   max-width: calc(100% - 30rem); 
   min-width: 65%;
   @media (max-width: 1170px) {
      max-width: calc(100% - 2rem);
    }    
    @media (max-width: 414px) {
      max-width: calc(100% - 1rem);
    }  
`;

const scrollIntoView = id => document.getElementById(id).scrollIntoView({ behavior: 'smooth' });

const Article = props => (
  <VMContext vm={props.vm}>
    <Frame horizontal css={frameCss} gap="10%">
      <Panel css={panelCss} children={props.children} />
      <Sidebar>
        <Title show={props.tocTitle}>
          <a href="javascript:void(0)" onClick={_ => scrollIntoView(props.title)}>
            {props.tocTitle}
          </a>
        </Title>
        {props.id && <MarkdownTOC id={props.id} />}
      </Sidebar>
    </Frame>
  </VMContext>
);

export class TabsArticle extends React.Component {
  state = { id: this.props.id, tocTitle: null, title: null };
  render() {
    const { vm, children } = this.props;
    const { id, title, tocTitle } = this.state;
    const handleActivate = (key, label) => this.setState({ id: key.length > 1 ? key : null, tocTitle: key.length > 1 ? label : null });
    const handleTitle = title => this.setState({ title: title, tocTitle: this.props.id });
    return (
      <Article vm={vm} id={id} title={title} tocTitle={tocTitle}>
        <h2 id={this.state.title}>
          <Element id="Title" onChange={handleTitle} />
        </h2>
        <Tab css="margin: 0 .5rem; margin-top: 2rem" onActivate={handleActivate}>
          {children}
        </Tab>
      </Article>
    );
  }
}

export default Article;
