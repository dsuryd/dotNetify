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

// <RouteLink> is a helper component to set anchor tags for routes.
export default class RouteLink extends React.Component {
  render() {
    const props = this.props;

    const handleClick = event => {
      event.preventDefault();
      if (props.vm == null) {
        console.error("RouteLink requires 'vm' property.");
        return;
      }
      if (props.route == null) {
        console.error("RouteLink requires 'route' property.");
        return;
      }
      if (typeof props.onClick === 'function' && props.onClick(event) === false) return;
      return props.vm.$handleRoute(event);
    };

    return (
      <a
        style={props.style}
        className={props.className}
        href={props.route && props.vm ? props.vm.$route(props.route) : '/'}
        children={props.children}
        onClick={handleClick}
      />
    );
  }
}

dotnetify.react.router.RouteLink = RouteLink;
