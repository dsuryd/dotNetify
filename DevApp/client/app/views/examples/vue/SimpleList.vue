<template>
  <div data-vm="SimpleListVM">
    <header>
      <input type="text" class="form-control" placeholder="First name" v-model="firstName">
      <input type="text" class="form-control" placeholder="Last name" v-model="lastName">
      <button type="button" class="btn btn-primary" v-on:click="add">Add</button>
    </header>
    <table>
      <thead>
        <tr>
          <th>First Name</th>
          <th>Last Name</th>
          <th></th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="employee in Employees" :key="employee.Id">
          <td>
            <InlineEdit :value="employee.FirstName"/>
          </td>
          <td>
            <div>
              <span class="editable" v-on:click="edit">{{employee.LastName}}</span>
              <input style="display:none" type="text" v-model.lazy="employee.LastName">
            </div>
          </td>
          <td>
            <div data-bind="vmCommand: remove">
              <i class="material-icons">clear</i>
            </div>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
import dotnetify from 'dotnetify/vue';
import InlineEdit from './InlineEdit.vue';

export default {
  name: 'SimpleList',
  components: {
    'InlineEdit': InlineEdit
  },
  created: function () {
    this.vm = dotnetify.vue.connect("SimpleListVM", this);
  },
  destroyed: function () {
    this.vm.$destroy();
  },
  data: function () {
    return {
      firstName: '',
      lastName: '',
      Employees: []
    }
  },
  methods: {
    add: function () {
      let fullName = `${this.firstName()} ${this.lastName()}`;
      if (fullName.trim() !== '') {
        this.vm.$dispatch({ Add: fullName });
        this.firstName = '';
        this.lastName = '';
      }
    },
    edit(employee, prop) {
      employee.edit = true;

    }
  }
}
</script>