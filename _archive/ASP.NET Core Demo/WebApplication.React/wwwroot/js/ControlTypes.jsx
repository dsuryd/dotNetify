class ControlTypes extends React.Component {

   constructor(props) {
      super(props);

      // Connect this component to the back-end view model.
      this.vm = dotnetify.react.connect("ControlTypesVM", this);

      // Set up function to dispatch state to the back-end.
      this.dispatchState = state => {
         this.setState(state);
         this.vm.$dispatch(state); 
      }

      // This component's JSX was loaded along with the VM's initial state for faster rendering.
      this.state = window.vmStates.ControlTypesVM;
   }
   componentWillUnmount() {
      this.vm.$destroy();
   }
   render() {
      const radioButtons = this.state.RadioButtons.map(radio =>
         <RadioButton key={radio.value} {...radio} />
      );

      const selectFieldMenu = this.state.SelectFieldMenu.map(menu =>
         <MenuItem key={menu.value} {...menu} />
         );

      const chipStyles = {
         chip: { margin: 4 },
         wrapper: { display: 'flex', flexWrap: 'wrap' },
      };

      const chips = this.state.Chips.map(chip =>
         <Chip key={chip.key} style={chipStyles.chip} onRequestDelete={() => this.dispatchState({ DeleteChip: chip.key }) }>{chip.label}</Chip>
         );

      return (
         <div className="container-fluid">
            <div className="header clearfix">
               <h3>Example: Control Types</h3>
            </div>
            <MuiThemeProvider>
               <div className="jumbotron">
                  {/* Text Field */}
                  <div className="row">
                     <div className="col-md-6">
                        <TextField id="TextField" fullWidth={true}
                                   {...this.state.TextFieldProps}
                                   value={this.state.TextFieldValue}
                                   errorText={this.state.TextFieldErrorText}
                                   onChange={event => this.setState({ TextFieldValue: event.target.value })}
                                   onBlur={event => this.dispatchState({ TextFieldValue: this.state.TextFieldValue })} />

                     </div>
                     <div className="col-md-6">
                        <div style={{"marginTop": "3em"}}>{this.state.TextFieldResult}</div>
                     </div>
                  </div>
                  {/* Auto Complete */}
                  <div className="row">
                     <div className="col-md-6">
                        <AutoComplete id="AutoComplete" fullWidth={true}
                                      {...this.state.AutoCompleteProps}
                                      filter={AutoComplete.caseInsensitiveFilter}
                                      dataSource={this.state.AutoCompleteResults}
                                      onUpdateInput={value => this.dispatchState({ AutoCompleteValue: value })} />
                     </div>
                  </div>
                  <br />
                  {/* Checkbox */}
                  <div className="row">
                     <div className="col-md-6">
                        <Checkbox label={this.state.CheckboxLabel}
                                  checked={this.state.Checked}
                                  onCheck={(event, value) => this.dispatchState({ Checked: value })} />
                     </div>
                     <div className="col-md-6">
                        <RaisedButton label={this.state.CheckboxResult} disabled={!this.state.Checked} />
                     </div>
                  </div>
                  <br />
                  {/* Radio button */}
                  <div className="row">
                     <div className="col-md-6">
                        <RadioButtonGroup name="Radio"
                                          valueSelected={this.state.RadioValue}
                                          onChange={(event, value) =>this.dispatchState({ RadioValue: value })}>
                           {radioButtons}
                        </RadioButtonGroup>
                     </div>
                     <div className="col-md-6">
                        <RaisedButton label={this.state.RadioResult}
                                      primary={this.state.RadioValue == "primary"}
                                      secondary={this.state.RadioValue == "secondary"} />
                     </div>
                  </div>
                  <br />
                  {/* Toggle button */}
                  <div className="row">
                     <div className="col-md-6">
                        <Toggle labelPosition="right"
                                label={this.state.ToggleLabel}
                                toggled={this.state.Toggled}
                                onToggle={(event, value) => this.dispatchState({ Toggled: value })} />
                     </div>
                  </div>
                  {/* Select Field */}
                  <div className="row">
                     <div className="col-md-6">
                        <SelectField id="SelectField"
                                     floatingLabelText={this.state.SelectFieldLabel}
                                     value={this.state.SelectFieldValue}
                                     onChange={(event, idx, value) => this.dispatchState({ SelectFieldValue: value })}>
                           {selectFieldMenu}
                        </SelectField>
                     </div>
                     <div className="col-md-6">
                        <div style={{ marginTop: "3em" } }>{this.state.SelectFieldResult}</div>
                     </div>
                  </div>
                  <br />
                  {/* Chip */}
                  <div className="row">
                     <div className="col-md-6">
                        <div style={chipStyles.wrapper}>
                           {chips}
                           <FlatButton label="Reset" onClick={() => this.dispatchState({ ResetChips: true })}></FlatButton>
                        </div>
                     </div>
                  </div>
               </div>
            </MuiThemeProvider>
         </div>
      );
   }
}