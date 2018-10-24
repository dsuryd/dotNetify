<template>
  <section>
    <header>
      <input type="text" class="form-control" placeholder="First name" v-model="firstName">
      <input type="text" class="form-control" placeholder="Last name" v-model="lastName">
      <button type="button" class="btn btn-primary" @click="add">Add</button>
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
            <InlineEdit
              :value="employee.FirstName"
              @update="onUpdate(employee, 'FirstName', $event)"
            />
          </td>
          <td>
            <InlineEdit
              :value="employee.LastName"
              @update="onUpdate(employee, 'LastName', $event)"
            />
          </td>
          <td>
            <div @click="onRemove(employee.Id)">
              <i class="material-icons">clear</i>
            </div>
          </td>
        </tr>
      </tbody>
    </table>
  </section>
</template>

<script>
import dotnetify from 'dotnetify/vue';
import InlineEdit from './components/InlineEdit.vue';

export default {
  name: 'SimpleList',
  components: {
    'InlineEdit': InlineEdit
  },
  created() {
    this.vm = dotnetify.vue.connect("SimpleListVM", this);
  },
  destroyed() {
    this.vm.$destroy();
  },
  data() {
    return {
      firstName: '',
      lastName: '',
      Employees: []
    }
  },
  methods: {
    add: function () {
      let fullName = `${this.firstName} ${this.lastName}`;
      if (fullName.trim() !== '') {
        this.vm.$dispatch({ Add: fullName });
        this.firstName = '';
        this.lastName = '';
      }
    },
    onUpdate(employee, prop, value) {
      employee[prop] = value;
      this.vm.$dispatch({ Update: { Id: employee.Id, [prop]: value } });
    },
    onRemove(id) {
      this.vm.$dispatch({ Remove: id });
    }
  }
}
</script>

<style lang="scss" scoped>
section {
	padding: 1rem;
	header {
		display: flex;
		align-items: center;
		margin-bottom: 1rem;
		> * {
			margin-right: 1rem;
		}
		input[type="text"] {
			max-width: 15rem;
		}
	}
	table {
		font-size: unset;
		width: 100%;
		max-width: 1268px;
		td,
		th {
			padding: 0.5rem 0;
			padding-right: 2rem;
			border-bottom: 1px solid #ddd;
			width: 50%;
		}
		th {
			font-weight: 500;
		}
		td:last-child,
		th:last-child {
			width: 5rem;
			> div {
				display: flex;
				align-items: center;
				cursor: pointer;
			}
		}
		tr:hover {
			background: #efefef;
		}
		i.material-icons {
			font-size: 1.2rem;
		}
		span.editable:hover {
			&:after {
				font-family: "Material Icons";
				content: "edit";
			}
		}
	}
}
</style>