##### SimpleList.js

```jsx
import React from "react";
import dotnetify from "dotnetify";
import { SimpleListCss } from './components/css';
import InlineEdit from "./components/InlineEdit";

class SimpleList extends React.Component {
  constructor(props) {
    super(props);
    this.state = { Employees: [] };

    // Connect this component to the back-end view model.
    this.vm = dotnetify.react.connect("SimpleListVM", this);

    // Set up function to dispatch state to the back-end.
    this.dispatch = state => this.vm.$dispatch(state);

    this.dispatchState = state => {
      this.setState(state);
      this.vm.$dispatch(state);
    };
  }

  componentWillUnmount() {
    this.vm.$destroy();
  }

  render() {
    return (
      <SimpleListCss>
        <header>
          <span>Add:</span>
          <TextBox
            placeholder="Type full name here"
            value={this.state.newName}
            onChange={value => this.setState({ newName: value })}
            onUpdate={value => {
              this.dispatch({ Add: value });
              this.setState({ newName: "" });
            }}
          />
        </header>
        <table>
          <tbody>
            <tr>
              <th>First Name</th>
              <th>Last Name</th>
              <th />
            </tr>
            {this.state.Employees.map(employee => (
              <tr key={employee.Id}>
                <td>
                  <InlineEdit
                    text={employee.FirstName}
                    onChange={value => this.dispatch({ Update: { Id: employee.Id, FirstName: value } })}
                  />
                </td>
                <td>
                  {" "}
                  <InlineEdit
                    text={employee.LastName}
                    onChange={value => this.dispatch({ Update: { Id: employee.Id, LastName: value } })}
                  />
                </td>
                <td>
                  <div onClick={_ => this.dispatch({ Remove: employee.Id })}>
                    <i className="material-icons">clear</i>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      <SimpleListCss>
    );
  }
}
```

##### InlineEdit.js

```jsx
import React from 'react';
import styled from 'styled-components';
import TextBox from './TextBox';

const EditableText = styled.div`
  /* styles */
`;

class InlineEdit extends React.Component {
  state = {
    edit: false,
    value: this.props.text
  };

  componentDidUpdate(prevProps) {
    if (prevProps.text !== this.props.text) this.setState({ value: this.props.text });
  }

  handleBlue = _ => {
    this.setState({ edit: false });
  };

  handleClick = event => {
    event.stopPropagation();
    if (!this.state.edit) this.setState({ edit: true });
  };

  handleUpdate = _ => {
    this.setState({ edit: false });
    if (this.state.value.length > 0 && this.state.value != this.props.text) {
      this.props.onChange(this.state.value);
    } else this.setState({ value: this.props.text });
  };

  render() {
    if (this.state.edit)
      return (
        <div>
          <TextBox
            id='EditField'
            ref={input => input && input.focus()}
            value={this.state.value}
            onClick={this.handleClick}
            onBlur={this.handleBlur}
            onUpdate={this.handleUpdate}
            onChange={value => this.setState({ value: value })}
          />
        </div>
      );

    return <EditableText onClick={this.handleClick}>{this.state.value}</EditableText>;
  }
}

export default InlineEdit;
```

##### TextBox.js

```jsx
import React, { createRef } from 'react';

class TextBox extends React.Component {
  constructor(props) {
    super(props);
    this.textInput = createRef();
    this.state = { changed: false };
  }

  focus() {
    this.textInput.current.focus();
  }

  handleChange = event => {
    this.setState({ changed: true });
    this.props.onChange(event.target.value);
  };

  handleBlur = event => {
    if (this.state.changed) this.props.onUpdate(this.props.value);
    this.setState({ changed: false });
    this.props.onBlur && this.props.onBlur(event);
  };

  handleKeydown = event => {
    if (event.keyCode == 13) this.handleBlur(event);
  };

  render() {
    return (
      <div>
        <label>{this.props.label}</label>
        <input
          type='text'
          className='form-control'
          value={this.props.value}
          placeholder={this.props.placeholder}
          onChange={this.handleChange}
          onBlur={this.handleBlur}
          onKeyDown={this.handleKeydown}
          ref={this.textInput}
        />
      </div>
    );
  }
}

export default TextBox;
```

##### SimpleListVM.cs

```csharp
public class SimpleListVM : MulticastVM
{
    private readonly IEmployeeRepository _repository;
    private readonly IConnectionContext _connectionContext;

    public class EmployeeInfo
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    [ItemKey(nameof(Employee.Id))]
    public IEnumerable<EmployeeInfo> Employees => _repository
        .GetAll()
        .Select(i => new EmployeeInfo { Id = i.Id, FirstName = i.FirstName, LastName = i.LastName });

    public SimpleListVM(IEmployeeRepository repository, IConnectionContext connectionContext)
    {
        _repository = repository;
        _connectionContext = connectionContext;
    }

    public void Add(string fullName)
    {
        var names = fullName.Split(new char[] { ' ' }, 2);
        var employee = new Employee
        {
            FirstName = names.First(),
            LastName = names.Length > 1 ? names.Last() : ""
        };

        // Use CRUD base method to add the list item on the client.
        this.AddList("Employees", new EmployeeInfo
        {
            Id = _repository.Add(employee),
            FirstName = employee.FirstName,
            LastName = employee.LastName
        });
    }

    public void Update(EmployeeInfo employeeInfo)
    {
        var employee = _repository.Get(employeeInfo.Id);
        if (employee != null)
        {
            employee.FirstName = employeeInfo.FirstName ?? employee.FirstName;
            employee.LastName = employeeInfo.LastName ?? employee.LastName;
            _repository.Update(employee);

            this.UpdateList(nameof(Employees), new EmployeeInfo
            {
              Id = employee.Id,
              FirstName = employee.FirstName,
              LastName = employee.LastName
            });
        }
    }

    public void Remove(int id)
    {
        _repository.Remove(id);

        // Use CRUD base method to remove the list item on the client.
        this.RemoveList(nameof(Employees), id);
    }

    // Clients from the same IP address will share the same VM instance.
    public override string GroupName => _connectionContext.HttpConnection.RemoteIpAddress.ToString();
}
```
