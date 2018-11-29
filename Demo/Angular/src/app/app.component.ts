import { Component } from '@angular/core';
import dotnetify from 'dotnetify/dist/dotnetify';

dotnetify.hubServerUrl = 'http://localhost:5000';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: [ './app.component.scss' ]
})
export class AppComponent {
  state: any = {};
  vm: any;

  ngOnInit() {
    this.vm = dotnetify.react.connect(
      'HelloWorld',
      {},
      {
        getState: () => this.state,
        setState: (state: any) => Object.assign(this.state, state)
      }
    );
  }

  ngOnDestroy() {
    this.vm.$destroy();
  }
}
