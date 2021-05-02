/* 
Copyright 2019-2020 Dicky Suryadi

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

import { useState, useEffect, useRef } from "react";
import * as $ from "../libs/jquery-shim";
import dotnetify from "./dotnetify-react";
import { IDotnetifyVM, IConnectOptions } from "../_typings";

dotnetify.react.useConnect = function <T>(
  iVMId: string,
  iComponent?: { state: T; props: any } | any,
  iOptions?: IConnectOptions
): { vm: IDotnetifyVM; state: T; setState: React.Dispatch<any> } {
  if (useState == null || useEffect == null) throw "Error: using React hooks requires at least v16.8.";

  let { state, props } = iComponent || {};
  if (state == null) state = iComponent || {};

  const [_state, _setState] = useState(state);
  const vm = useRef<IDotnetifyVM>();
  const vmData = useRef(_state);

  const setState = (newState: any) => {
    vmData.current = $.extend({}, vmData.current, newState);
    _setState(vmData.current);
  };

  useEffect(() => {
    vm.current = dotnetify.react.connect(
      iVMId,
      {
        props: props,
        get state() {
          return vmData.current;
        },
        setState
      },
      iOptions
    );
    return () => vm.current.$destroy();
  }, []);

  return { vm: vm.current, state: _state, setState };
};

export default function <T>(
  iVMId: string,
  iComponent?: { state: T; props: any } | any,
  iOptions?: IConnectOptions
): { vm: IDotnetifyVM; state: T; setState: React.Dispatch<any> } {
  return dotnetify.react.useConnect(iVMId, iComponent, iOptions);
}
