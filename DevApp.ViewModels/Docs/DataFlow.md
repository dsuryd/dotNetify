## Data Flow Pattern

#### Single View Model

This is the simplest pattern that we've used so far, in which the root component is connected to a single back-end view model, serving the initial state and occasionally receiving updates to be persisted to the database. This works great for app with shallow component tree, or if integrating with client-side stores.

[inset]

But you can make the view model do more. It can host any data-driven logic, such as selecting an item in the master list triggers new data being loaded into the details view. As demonstrated in the [Composite View Example](/core/examples/compositeview), with a single action-reaction cycle, the view model can process the selection changed event, load the data, and push the new state to your App view.

#### Siloed View Model

When the component tree is deeper, you may choose to connect some components to their own back-end view model. These view models are isolated from each other, and taken individually, they behave the same way as the above pattern.

[inset]

This pattern is well-suited for building an SPA, where each page is a stand-alone component. And it also works well with container components.

#### Scoped View Model
For a more complex app with many deeply-layered components that are emitting various events that trigger data updates on various other components, it's easy for your UI components to get bogged down with handling all the business logic. This pattern helps you to pull those logic out of your views and put them in the view models.

In this pattern, the view models are no longer isolated, but can be arranged hierarchically, in which a parent has access to its children and sets the context for them. Given this way, we can imagine the app as being composed of independent modules that are capable of interacting with one another through orchestration by a higher-order module, where each module is composed of front-end components and back-end view models that are working as a unit.

[inset]

#### Master-Details

To illustrate the scoped view model pattern, consider a master-details view consisting of _MasterList_ and _Details_ components. When the selection changes in the _MasterList_, the _Details_ updates its data. There are many ways to implement this, such as having a container component that listens to the selection changed event and trigger updates on _Details_, or the components themselves emit and subscribe to events using a global event aggregator, or they may emit actions and subscribe to the Redux store.

When the interaction grows more complex and it's getting harder to keep your components from getting bloated, adhere to single responsibility and maintain good decoupling, consider using this pattern and offload the logic into the view models. Here's a simple example on how it can be done:

```jsx
const MasterDetails = _ => (
  <div style={{ display: 'flex' }}>
    <MasterList />
    <Details />
  </div>
);

class MasterList extends React.Component {
  constructor() {
    super();
    this.state = { ListItems: [], selected: 0 };
    this.vm = dotnetify.react.connect('MasterDetails.MasterList', this);
  }
  componentWillUnmount() {
    this.vm.$destroy();
  }

  handleSelect = id => {
    this.setState({ selected: id });
    this.vm.$dispatch({ Select: id });
  };

  render() {
    const itemStyle = selected => ({ cursor: 'pointer', background: selected ? '#eee' : 'none' });
    const items = this.state.ListItems.map(item => (
      <li 
        key={item.Id} 
        style={itemStyle(item.Id === this.state.selected)} 
        onClick={() => this.handleSelect(item.Id)}
      >
        {item.Title}
      </li>
    ));
    return <ul>{items}</ul>;
  }
}

class Details extends React.Component {
  constructor() {
    super();
    this.state = { ItemImageUrl: null };
    this.vm = dotnetify.react.connect('MasterDetails.Details', this);
  }
  componentWillUnmount() {
    this.vm.$destroy();
  }  
  render() {
    if (!this.state.ItemImageUrl) return null;
    return <img src={this.state.ItemImageUrl} style={{ margin: '0 1rem' }} />;
  }
}
```

```csharp
public class MasterDetails : BaseVM
{
  private readonly IWebStoreService _webStoreService;

  private event EventHandler<int> SelectedItem;

  public MasterDetails(IWebStoreService webStoreService)
  {
      _webStoreService = webStoreService;
  }

  public override void OnSubVMCreated(BaseVM vm)
  {
      if (vm is MasterList)
      {
        var masterList = vm as MasterList;
        masterList.ListItems = _webStoreService.GetAllBooks();
        masterList.Selected += (sender, id) => SelectedItem?.Invoke(this, id);
      }
      else if (vm is Details)
      {
        var details = vm as Details;
        SelectedItem += (sender, id) => details.SetData(_webStoreService.GetBookById(id));
      }
  }
}

public class MasterList : BaseVM
{
  public IEnumerable<WebStoreRecord> ListItems
  {
      get => Get<IEnumerable<WebStoreRecord>>();
      set => Set(value);
  }

  public event EventHandler<int> Selected;

  public Action<int> Select => id => Selected?.Invoke(this, id);
}

public class Details : BaseVM
{
  public string ItemImageUrl
  {
      get => Get<string>();
      set => Set(value);
  }

  public void SetData(WebStoreRecord data) => ItemImageUrl = data.ImageUrl;
}
```

[inset]

Notice that in the UI view, the components are connecting to their own view models to get their state, and nothing is passed between components. It's their view models that are doing that on the back-end, and pushing the state change to the front-end.

The connect name of the components are qualified with the parent view model name. This allows the parent view model to intercept the lifecycle of its children. In the case above, the BookStore parent receives a call right after a child view model instance is created, which allows it to subscribe to the _Selected_ event of the _MasterList_, so that it can in turn update the Details.

In general, the parent view model serves as a light orchestrator of the view models under its scope. This is the same uni-directional data flow as in the front-end, and it keeps the child view models stay decoupled from one another. The parent too can be decoupled from the children, by using interfaces instead of concrete types.

The APIs that provide the lifecycle hooks for the parent view model are:

- __GetSubVM__(vmType): give the parent the opportunity to create its own instance of a child view model.
- __OnSubVMCreated__(vmObject): pass a newly created child view model instance to the parent.
- __OnSubVMDestroyed__(vmObject): pass the child view model instance that will soon be disposed to the parent.

#### Scope Component

By taking advantage of the React's context object, dotNetify can provide a special Scope component that allows us to avoid declaring the scope name inside the components. The React view above can be refactored to an even more decoupled fashion:

```jsx
import React from 'react';
import PropTypes from 'prop-types';
import { Scope } from 'dotnetify/react';

const App = _ => (
  <Scope vm="BookStore">
    <MasterList />
    <Details />
  </Scope>
);
 
class MasterList extends React.Component {
  static contextTypes =  { connect: PropTypes.func };

   constructor() {
      // ...
      this.context.connect("MasterList", this);
   }
   // ... 
}

class Details extends React.Component {
  static contextTypes =  { connect: PropTypes.func };

   constructor() {
      // ...
      this.context.connect("Details", this);
   }
   // ...	
}
```

With the Scope component, you can now set the parent view model name through its vm property. When the nested components use __this.context.connect__ to connect to the back-end, it will use the parent scope to resolve to the desired view model path, in this case _BookStore.MasterList_ and _BookStore.Details_.

The Scope component can be nested and helps make both your React view and their view models highly reusable. For a more fleshed-out example, see the [Composite View Example](/core/examples/compositeview).

This pattern gives a potential to package the components and their view models in modules and in different assemblies, which will make it easier for multiple teams towards working autonomously. And to take it even further, given the right orchestration middleware it's possible to go full-stack microservice on very complex apps, where the app is composed of autonomous modules on their own databases and server processes.