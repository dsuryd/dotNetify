// Type definitions for dotnetify-react.
// Project: https://github.com/dsuryd/dotnetify
// Definitions by: Dicky Suryadi, brimce <https://github.com/Brimce>

declare module 'dotnetify' {

   export interface ITemplate {
      Target: string;
   }

   export interface IRouter {
      ssrState: (vmName: string) => any;
   }

   export interface IConnectOptions {
      getState?: () => any;
      setState?: (state: any) => void;
      vmArg?: any;
      headers?: any;
      exceptionHandler?: (exception: any) => void;
   }

   export class dotnetifyVM {
      constructor(iVMId: String, iReact: React.Component<any, any>, iOptions?: IConnectOptions);
      $dispatch: (iValue: any) => void;
      $destroy: () => void;
      onRouteEnter: (path: string, template: ITemplate) => string;
   }

   export class react {
      static connect(iVMId: String, iReact: React.Component<any, any>, iOptions?: IConnectOptions): dotnetifyVM;
      static getViewModels(): dotnetifyVM[];
      static router: IRouter;
   }
}

