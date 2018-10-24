#### Master-Details

To illustrate the scoped view model pattern, consider a master-details view consisting of _MasterList_ and _Details_ components. When the selection changes in the _MasterList_, the _Details_ updates its data. There are many ways to implement this, such as having a container component that listens to the selection changed event and trigger updates on _Details_, or the components themselves emit and subscribe to events using a global event aggregator, or they may emit actions and subscribe to the Vuex store.

When the interaction grows more complex and it's getting harder to keep your components from getting bloated, adhere to single responsibility and maintain good decoupling, consider using this pattern and offload the logic into the view models. Here's a simple example on how it can be done:

```jsx
const MasterDetails = new Vue({
  el: '#MasterDetails',
  components: {
    MasterList,
    Details
  },
  template: `
  <div style='display: flex'>
      <MasterList />
      <Details />
    </div>
  `
});

const MasterList = Vue.component('MasterList', {
  template: `
    <ul>
      <li 
        v-for="item in ListItems" 
        :key="item.Id" 
        :style="getStyle(item.Id)" 
        @click="onClick(item.Id)"
      >
        {{item.Title}}
      </li>  
    </ul>
  `,
  created() {
    this.vm = dotnetify.vue.connect('MasterDetails.MasterList', this);
  },
  destroyed() {
    this.vm.$destroy();
  },
  data() {
    return { ListItems: [], selected: null };
  },
  methods: {
    getStyle(id) {
      return { cursor: 'pointer', background: id === this.selected ? '#eee' : 'none' };
    },
    onClick(id) {
      this.selected = id;
      this.vm.$dispatch({ Select: id });
    }
  }
});

const Details = Vue.component('Details, {
  template: '<img v-if="ItemImageUrl" :src="ItemImageUrl" style="margin: 0 1rem" />',
  created() {
    this.vm = dotnetify.vue.connect('MasterDetails.Details', this);
  },
  destroyed() {
    this.vm.$destroy();
  },
  data() {
    return { ItemImageUrl: null };
  },  
});
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

By taking advantage of the Vue's _Provide/Inject_ API, dotNetify can provide a special Scope component that allows us to avoid declaring the scope name inside the components. The components above can be refactored to an even more decoupled fashion:

```jsx
import Vue from 'vue';
import dotnetify from 'dotnetify/vue';

const App = new Vue({
  el: "#App",
  components: {
    'Scope': dotnetify.vue.Scope,
    MasterList,
    Details
  },
  template: `
    <Scope vm="BookStore">
      <MasterList />
      <Details />
    </Scope>  
  `;  
});
 
const MasterList = Vue.component('MasterList', {
  inject: ['connect'],
  created() {
    this.vm = this.connect('MasterList', this);
  },
  // ...
});

const Details = Vue.component('Details', {
  inject: ['connect'],
  created() {
    this.vm = this.connect('Details', this);
  },
  // ...
});
```

With the Scope component, you can now set the parent view model name through its vm property. When the nested components use the injected __connect__ function to connect to the back-end, it will use the parent scope to resolve to the desired view model path, in this case _BookStore.MasterList_ and _BookStore.Details_.

The Scope component can be nested and helps make both your view and their view models highly reusable. For a more fleshed-out example, see the [Composite View Example](/core/examples/compositeview).

This pattern gives a potential to package the components and their view models in modules and in different assemblies, which will make it easier for multiple teams towards working autonomously. And to take it even further, given the right orchestration middleware it's possible to go full-stack microservice on very complex apps, where the app is composed of autonomous modules on their own databases and server processes.