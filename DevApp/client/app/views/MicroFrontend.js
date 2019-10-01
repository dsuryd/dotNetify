import React from 'react';
import { Markdown, withTheme } from 'dotnetify-elements';
import Article from '../components/Article';
import MfeImage from '../images/MicroFrontend.svg';

const Image = styled.img`
  display: flex;
  align-items: center;
  justify-content: center;
  max-width: 800px;
  width: 90%;
`;

const MicroFrontend = props => (
  <Article vm="MicroFrontend" id="Content">
    <Markdown id="Content">
      <Image src={MfeImage} />
    </Markdown>
  </Article>
);

export default withTheme(MicroFrontend);
