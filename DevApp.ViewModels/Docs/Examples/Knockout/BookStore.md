##### BookStore.html

```html
<section data-vm="BookStoreVM"> 
    <header>
    Each product here is represented by a unique URL that can be entered into the address bar to go directly to that
        specific product page.
    </header>
    <div data-bind="foreach: Books">
      <div>
        <center>
          <a data-bind="vmRoute: Route">
              <img class="thumbnail" data-bind="attr: {src: Info.ImageUrl}" />
          </a>
          <div>
              <b data-bind="text: Info.Title"></b>
              <div>by <span data-bind="text: Info.Author"></span></div>
          </div>
        </center>
      </div>
    </div>
</section>
<div id="BookPanel" />      
```

##### BookStoreVM.ts

```jsx
export default class BookStoreVM {
  constructor(vm: any) {
    vm.onRouteEnter = (path: string, template: any) => {
      template.Target('BookPanel');
      if (path !== '') template.ViewUrl('/examples/Book.html');
    };
  }
}
```

##### Book.html

```html
<div id="BookDetails" data-vm="BookDetailsVM" class="modal fade">
  <div class="modal-dialog modal-dialog-centered">
    <div class="modal-content">
      <div class="modal-body" style="display: flex">
        <img class="thumbnail" data-bind="attr: {src: Book.ImageUrl}" />
        <div style="margin-left: 1rem">
          <h3 data-bind="text: Book.Title"></h3>
          <h5 data-bind="text: Book.Author"></h5>
          <button class="btn btn-primary">Buy</button>
        </div>
      </div>
      <div class="modal-footer">
          <button type="button" class="btn btn-success" data-dismiss="modal">Back</button>
      </div>
    </div>
  </div>
</div>
<script>
  $('#BookDetails').modal();
  $('#BookDetails').on('hidden.bs.modal', function (e) { window.history.back(); });
</script>
```