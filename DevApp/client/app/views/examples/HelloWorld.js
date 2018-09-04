import RenderExample from 'app/components/RenderExample';
import HelloWorld from './react/HelloWorld';
import HelloWorldHtml from './knockout/HelloWorld.html';

export default _ => (
  <RenderExample vm="HelloWorldExample">
    <HelloWorld />
    <div dangerouslySetInnerHTML={{ __html: HelloWorldHtml }} />
  </RenderExample>
);
