import React from 'react';
import {
   Alert,
   Button,
   Checkbox,
   CheckboxGroup,
   DateField,
   Panel,
   DropdownList,
   Form,
   MultiselectList,
   TextField,
   NumberField,
   TextAreaField,
   PasswordField,
   RadioGroup,
   RadioToggle,
   VMContext
} from 'dotnetify-elements';
import RenderExample from '../../components/RenderExample';

const ControlTypes = _ => (
   <VMContext vm="ControlTypesVM">
      <Panel>
         <Form>
            <Panel childProps={{ horizontal: true }}>
               <TextField id="MyText" />
               <TextField id="MyMoney" />
               <DateField id="MyDate" />
               <DropdownList id="MyDropdown" />
               <MultiselectList id="MyMultiselect" />
               <TextAreaField id="MyTextArea" />
               <RadioGroup id="MyRadio" />
               <RadioToggle id="MyRadioToggle" />
               <CheckboxGroup id="MyCheckboxGroup" />
               <Panel right>
                  <Button label="Cancel" cancel secondary />
                  <Button label="Submit" id="Submit" submit primary />
               </Panel>
            </Panel>
         </Form>
         <Alert id="SubmitSuccess" success />
      </Panel>
   </VMContext>
);

export default _ => (
   <RenderExample vm="ControlTypesExample">
      <ControlTypes />
   </RenderExample>
);
