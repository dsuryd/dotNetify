/* 
Copyright 2017-2018 Dicky Suryadi

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
 */

import React from 'react';
import dotnetify from './dotnetify-react';

// <RouteTarget> is a helper component to provide DOM target for routes, and is essential for server-side rendering.
export default class RouteTarget extends React.Component {
  componentWillMount() {
    var elem = document.getElementById(this.props.id);
    if (elem != null && window.ssr && !window.ssr[this.props.id]) {
      window.ssr[this.props.id] = true;
      this.initialHtml = { __html: elem.innerHTML };
    }
    else this.initialHtml = { __html: '' };
  }

  getDOMNode() {
    return this.elem;
  }

  render() {
    const { id, ...props } = this.props;
    return <div id={id} ref={el => (this.elem = el)} {...this.props} dangerouslySetInnerHTML={this.initialHtml} />;
  }
}

dotnetify.react.router.RouteTarget = RouteTarget;
