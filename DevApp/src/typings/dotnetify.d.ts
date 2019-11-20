import { HubConnectionBuilder } from '@aspnet/signalr';

// Type definitions for dotnetify-react.
// Project: https://github.com/dsuryd/dotnetify
// Copyright 2017-2019 Dicky Suryadi

declare module 'dotnetify' {
  import * as React from 'react';

  export interface ITemplate {
    Target: string;
  }

  export interface IRouter {
    ssrState: (vmName: string) => any;
    redirect: (url: string, viewModels: any[]) => void;
  }

  export interface IConnectOptions {
    getState?: () => any;
    setState?: (state: any) => void;
    vmArg?: any;
    headers?: any;
    exceptionHandler?: (exception: any) => void;
  }

  export class dotnetifyVM {
    constructor(vmId: String, react: React.Component<any, any>, options?: IConnectOptions);
    /**Send state to the back-end */
    $dispatch: (value: any) => void;
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
       * @param vmId Identifier (name) of ViewModel.
       * @param react React component to bind state
       * @param options Advanced connection options
       * @returns ViewModel Connection
       */
    connect(vmId: String, react: React.Component<any, any>, options?: IConnectOptions): dotnetifyVM;
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
    static connect: (vmId: String, options?: IConnectOptions) => dotnetifyVM;
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
    /**transport layers (webSockets, longPolling, etc) */
    transport: Array<string>;
    connectionBuilder: (builder: HubConnectionBuilder) => HubConnectionBuilder;
  }

  export type ScopeProp = {
    vm: string;
  };

  export class Scope extends React.Component<ScopeProp> {}

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

  export class RouteTarget extends React.PureComponent<IRouteTargetProps, any> {}

  export function useConnect<T>(vmId: String, initialState: T, options?: IConnectOptions): { vm: dotnetifyVM; state: T };
}
