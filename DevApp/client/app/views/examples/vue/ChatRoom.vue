<template>
  <div>
    <div class="chatPanel">
      <nav>
        <p v-for="user in Users" :key="user.Id">
          <b :class="user.CorrelationId == correlationId ? 'myself' : ''">{{user.Name}}</b>
          <span>{{user.IpAddress}}</span>
          <span>{{user.Browser}}</span>
        </p>
      </nav>
      <section>
        <div>
          <div v-for="(msg, idx) in Messages" :key="idx">
            <div>
              <span>{{getUserName(msg.UserId) || msg.UserName}}</span>
              <span>{{new Date(msg.Date).toLocaleString()}}</span>
            </div>
            <div :class="msg.private ? 'private' : ''">{{msg.Text}}</div>
          </div>
        </div>
        <div style="float: left; clear: both" ref="bottomElem"/>
        <input
          type="text"
          class="form-control"
          placeholder="Type your message here"
          v-model="message"
          @change="sendMessage"
        >
      </section>
    </div>
    <footer>
      <div>* Hint:</div>
      <ul>
        <li>type 'my name is ___' to introduce yourself</li>
        <li>type '&lt;username&gt;: ___' to send private message</li>
      </ul>
    </footer>
  </div>
</template>

<script>
export default {
  name: 'ChatRoom',
  created() {
    this.vm = dotnetify.vue.connect("ChatRoomVM", this);

    this.$watch("PrivateMessage", message => {
      this.scrollToBottom();
      message.Text = '(private) ' + message.Text;
      message.private = true;
      this.Messages.push(message);
      this.PrivateMessage = null;
    })
  },
  mounted() {
    this.vm.$dispatch({ AddUser: this.correlationId });
    this.scrollToBottom();
  },
  destroyed() {
    this.vm.$dispatch({ RemoveUser: null });
    this.vm.$destroy();
  },
  data() {
    return {
      Users: [],
      Messages: [],
      PrivateMessage: null,
      message: '',
      correlationId: `${Math.random()}`
    }
  },
  methods: {
    getUserName(userId) {
      const user = this.Users.find(x => x.Id === userId);
      return user ? user.Name : null;
    },
    scrollToBottom() {
      this.$refs.bottomElem.scrollIntoView({ behavior: 'smooth' });
    },
    sendMessage() {
      const match = /name is ([A-z]+)/i.exec(this.message);
      this.vm.$dispatch({
        SendMessage: {
          Text: this.message,
          Date: new Date(),
          UserName: match ? match[1] : ''
        }
      });
      this.message = '';
    }
  }
}
</script>