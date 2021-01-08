import { Component } from '@angular/core';
import dotnetify from 'dotnetify/dist/dotnetify';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent {
  state: any = {};
  vm: any;

  ngOnInit() {
    this.vm = dotnetify.connect('HelloWorld', {
      getState: () => this.state,
      setState: (state: any) => Object.assign(this.state, state),
    });
  }

  ngOnDestroy() {
    this.vm.$destroy();
  }
}
