<template>
  <table>
    <tbody>
      <tr>
        <td>
          Text box:
          <label>(updates on losing focus)</label>
        </td>
        <td>
          <input
            type="text"
            class="form-control"
            v-model.lazy="TextBoxValue"
            :placeholder="TextBoxPlaceHolder"
          >
          <b>{{TextBoxResult}}</b>
        </td>
      </tr>
      <tr>
        <td>
          Search box:
          <label>(updates on keystroke)</label>
        </td>
        <td>
          <input
            type="text"
            class="form-control"
            v-model="SearchBox"
            :placeholder="SearchBoxPlaceHolder"
          >
          <ul class="list-group">
            <li
              class="list-group-item"
              v-for="(result, i) in SearchResults"
              :key="i"
              @click="SearchBox = result"
            >{{result}}</li>
          </ul>
        </td>
      </tr>
      <tr>
        <td>Check box:</td>
        <td>
          <label>
            <input type="checkbox" v-model="ShowMeCheckBox">
            <span>Show me</span>
          </label>
          <label>
            <input type="checkbox" v-model="EnableMeCheckBox">
            <span>Enable me</span>
          </label>
          <button
            class="btn btn-secondary"
            :disabled="!EnableMeCheckBox"
            v-if="ShowMeCheckBox"
          >{{CheckBoxResult}}</button>
        </td>
      </tr>
      <tr>
        <td>Simple drop-down list:</td>
        <td>
          <select class="form-control" v-model="SimpleDropDownValue">
            <option value="" disabled>Choose...</option>
            <option v-for="(option, i) in SimpleDropDownOptions" :key="i">{{option}}</option>
          </select>
          <b>{{SimpleDropDownResult}}</b>
        </td>
      </tr>
      <tr>
        <td>Drop-down list:</td>
        <td>
          <select class="form-control" v-model="DropDownValue">
            <option value="0" disabled>{{DropDownCaption}}</option>
            <option
              v-for="option in DropDownOptions"
              :key="option.Id"
              :value="option.Id"
            >{{option.Text}}</option>
          </select>
          <b>{{DropDownResult}}</b>
        </td>
      </tr>
      <tr>
        <td>Radio button:</td>
        <td>
          <label>
            <input type="radio" value="green" v-model="RadioButtonValue">
            <span>Green</span>
          </label>
          <label>
            <input type="radio" value="yellow" v-model="RadioButtonValue">
            <span>Yellow</span>
          </label>
          <button class="btn" :class="RadioButtonStyle">Result</button>
        </td>
      </tr>
      <tr>
        <td>Button:</td>
        <td>
          <button class="btn btn-secondary" type="button" @click="onButtonClick">Click me</button>
          <span style="margin-left: 2rem" v-if="ClickCount > 0">
            You clicked me
            <b>{{ClickCount}}</b>&nbsp;times!
          </span>
        </td>
      </tr>
    </tbody>
  </table>
</template>

<script>
import dotnetify from 'dotnetify/vue';

export default {
  name: 'ControlTypes',
  created() {
    this.vm = dotnetify.vue.connect("ControlTypesVM", this, {
      watch: [
        'TextBoxValue', 'SearchBox', 'ShowMeCheckBox', 'EnableMeCheckBox',
        'SimpleDropDownValue', 'DropDownValue', 'RadioButtonValue'
      ]
    });
  },
  destroyed() {
    this.vm.$destroy();
  },
  data() {
    return {
      TextBoxValue: '',
      TextBoxPlaceHolder: '',
      TextBoxResult: '',
      SearchBox: '',
      SearchBoxPlaceHolder: '',
      SearchResults: [],
      ShowMeCheckBox: true,
      EnableMeCheckBox: true,
      CheckBoxResult: '',
      SimpleDropDownValue: '',
      SimpleDropDownResult: '',
      SimpleDropDownOptions: [],
      DropDownCaption: '',
      DropDownValue: '',
      DropDownResult: '',
      DropDownOptions: [],
      RadioButtonValue: '',
      RadioButtonStyle: '',
      ClickCount: 0
    }
  },
  methods: {
    onButtonClick: function () {
      this.vm.$dispatch({ ButtonClicked: true });
    }
  }
}
</script>