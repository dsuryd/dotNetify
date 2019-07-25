import React from 'react';
import { Cell, DropdownList, Form, NumberField, Panel, TextField, VMContext } from 'dotnetify-elements';

const AddressForm = () => (
  <Panel>
    <Cell header="Primary Address">
      <VMContext vm="AddressForm">
        <Form id="Address">
          <Panel flex childProps={{ horizontal: true }}>
            <TextField id="Address1" />
            <TextField id="Address2" />
            <TextField id="City" />
            <DropdownList id="State" />
            <NumberField id="ZipCode" />
          </Panel>
        </Form>
      </VMContext>s
    </Cell>
  </Panel>
);

export default AddressForm;
