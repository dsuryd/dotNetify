import RenderVue from './RenderVue';
import { ChatRoomCss } from '../../components/css';
import ChatRoom from '../ChatRoom.vue';

export default _ => (
	<ChatRoomCss>
		<RenderVue src={ChatRoom} />
	</ChatRoomCss>
);
