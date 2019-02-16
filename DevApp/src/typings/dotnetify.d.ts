// Type definitions for dotnetify-react.
// Project: https://github.com/dsuryd/dotnetify
// Copyright 2017-2018 Dicky Suryadi
// Contributors: brimce <https://github.com/Brimce>, MrFoxPro <https://github.com/MrFoxPro>

declare module "dotnetify" {
    import * as React from 'react';

    export interface ITemplate {
      Target: string;
   }

   export interface IRouter {
      ssrState: (vmName: string) => any;
      redirect: (iUrl: string, viewModels: any[]) => void
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
      /**Send state to the back-end */
      $dispatch: (iValue: any) => void;
      $destroy: () => void;       

      /**To set the target DOM element, implement the vm.onRouteEnter function in getInitialState. 
       * This API will pass the URL path and the route template object to the function whenever a route is 
       * activated, and you will just set the Target property to the ID of the target DOM element. 
       * It also allows you to cancel the operation by returning false. 
       */
      onRouteEnter: (path: string, template: ITemplate) => boolean; 
   }

   class react {
      /**
       * @param iVMId Identifier (name) of ViewModel.
       * @param iReact React component to bind state
       * @param iOptions Advanced connection options
       * @returns ViewModel Connection
       */
      connect(iVMId: String, iReact: React.Component<any, any>, iOptions?: IConnectOptions): dotnetifyVM;
      getViewModels(): dotnetifyVM[];
      router: IRouter;
      Scope: Scope;
   }

   export default class dotnetify {
      /**React binding */
      static react: react;
      /**Url of SignalR instance with port */
      static hubServerUrl: string;
      /**Hub options (Transport type, etc)*/
      static hubOptions: IHubOptions;
      /**Hub presentation */
      static Hub: Hub;
      /** Path to hub, -> Hub.hubPath */
      static hubPath: string;
      /** Debug mode */
      static debug: boolean;
      /** Debug function */
      static debugFn: () => void;
      /** Check connection status to hub */
      static isConnected: () => boolean;
      /** Check if dotnetify hub has started */
      static isHubStarted: () => boolean; 
      /** Generic connect function for non-React app. */
      static connect: (iVMId: String, iOptions?: IConnectOptions) => dotnetifyVM;
      /**Handler for receiving notifications */
      static connectionStateHandler: (state: any) => void;
   }

   export class Hub {
      /** Init Hub connection*/
      init(): void;
      /** Hub Path, Defaults to /dotnetify */
      hubPath: string;
      /**Indicates that the reconnect attempt is limited to {value} times after every disconnected event. */
      reconnectRetry: Number;
      /**Initial and everytime delay */
      reconnectDelay: Array<Number>;
      /** Check connectivity status */
      isConnected: () => boolean;
   }

   export interface IHubOptions {
      /**transport layers (webScokets, longPolling, etc) */
      transport: Array<string>;
   }

   export type ScopeProp = {
      vm: string
   }

   export class Scope extends React.Component<ScopeProp> { }

   export interface IRouteProps {
      vm: dotnetifyVM;
      route: string;
      className?: string;
      style?: string;
      onClick?: () => void;
   }

   export const RouteLink: React.SFC<IRouteProps>;

   export interface IRouteTargetProps {
      id: string;
      style?: React.CSSProperties;
   }

   export class RouteTarget extends React.PureComponent<IRouteTargetProps, any>{ }
 }