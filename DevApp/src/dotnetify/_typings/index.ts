import { HubConnectionBuilder } from "@aspnet/signalr";
import * as React from "react";

declare const dotnetify: IDotnetify;
export default dotnetify;

export interface IDotnetify {
  // Supported JS framework.
  react?: IDotnetifyReact;
  vue?: IDotnetifyVue;
  ko?: any;

  // SignalR hub options.
  hub: IDotnetifyHub;
  hubOptions: HubOptionsType;
  hubPath: string;
  hubLib: any;

  // Debug mode.
  debug: boolean;
  debugFn: (vmId: string, direction: string, payload: any) => void;

  // Use this to get notified of connection state changed events.
  connectionStateHandler: (state: string, exception: ExceptionType, hub: IDotnetifyHub) => void;

  // Use this intercept a view model prior to establishing connection.
  connectHandler: (args: VMConnectArgsType) => VMConnectArgsType;

  // Connect to server. Use it for non-supported frameworks.
  connect: (vmId: string, options?: IConnectOptions) => IDotnetifyVM;

  // Creates a SignalR hub client.
  createHub: (iHubServerUrl: string, iHubPath: string, iHubLib: any) => IDotnetifyHub;

  // Creates a Web API hub client.
  createWebApiHub: (iBaseUrl: string, iRequestHandler: RequestHandlerType) => IDotnetifyHub;

  // Active view models.
  getViewModels(): IDotnetifyVM[];
}

export interface IDotnetifyReact {
  connect(iVMId: string, iReact: React.Component | any, iOptions?: IConnectOptions): IDotnetifyVM;
}

export interface IDotnetifyVue {
  connect(iVMId: string, iVue: any, iOptions?: IConnectOptions): IDotnetifyVM;
}

export interface IDotnetifyVM {
  $dispatch: (value: any) => void;
  $destroy: () => void;

  $routeTo: (route: RouteType) => void;
  onRouteEnter?: OnRouteEnterType;
}

export interface IConnectOptions {
  getState?: () => any;
  setState?: (state: any) => void;
  vmArg?: { [prop: string]: any };
  headers?: { [prop: string]: any };
  exceptionHandler?: ExceptionHandlerType;
  webApi?: boolean;
  onRouteEnter?: OnRouteEnterType;
}

export interface IDotnetifyHub {
  url: string;
  mode: string;
  reconnectDelay: number[];
  reconnectRetry: number;

  isConnected: boolean;
  isHubStarted: boolean;

  responseEvent: IEventEmitter;
  reconnectedEvent: IEventEmitter;
  connectedEvent: IEventEmitter;
  connectionFailedEvent: IEventEmitter;

  init: (hubPath: string, hubServerUrl: string, hubLib: any) => void;
  startHub: (hubOptions: HubOptionsType, doneHandler: () => void, failHandler: (ex: any) => void, iForceRestart: boolean) => void;

  stateChanged: (handler: (state: string) => void) => void;

  requestVM: (vmId: string, payload: RequestPayloadType) => void;
  updateVM: (vmId: string, payload: any) => void;
  disposeVM: (vmId: string) => void;
}

export interface IEventEmitter {
  emit: (...args: any) => void;
  subscribe: (subscriber: any) => () => void;
}

export type HubOptionsType = {
  transport: string[];
  connectionBuilder: (builder: HubConnectionBuilder) => HubConnectionBuilder;
};

export type RequestPayloadType = {
  $vmArg: { [prop: string]: any };
  $headers: { [prop: string]: any };
};

export type VMConnectArgsType = { vmId: string; options: IConnectOptions; hub: IDotnetifyHub };

export type ExceptionType = { name: string; message: string };

export type ExceptionHandlerType = (exception: ExceptionType) => void;

export type RequestHandlerType = (url: string, request: any, payload?: any) => void;

export interface IDotnetifyRouter {
  notFound404Url: string;
  urlPath: string;
  routedEvent: IEventEmitter;
}

export interface IDotnetifyVMRouter {
  initRouting: () => void;
}

export type RoutingStateType = {
  Root: string;
  Active: string;
  Templates: RoutingTemplateType[];
};

export type RoutingTemplateType = {
  Id: string;
  UrlPattern: string;
  Target: string;
  ViewUrl?: string;
  JSModuleUrl?: string;
};

export type RouteType = {
  TemplateId: string;
  Path: string;
  Url?: string;
  RedirectRoot?: string;
};

export type OnRouteEnterType = (path: string, template: RoutingTemplateType) => boolean | Promise<boolean>;

export interface IRouteLinkProps {
  vm: IDotnetifyVM;
  route: RouteType;
  style?: React.CSSProperties;
  className?: string;
  onClick?: (e: React.MouseEvent) => boolean;
}

export class RouteLink extends React.Component<IRouteLinkProps> {}

export interface IRouteTargetProps {
  id: string;
}

export class RouteTarget extends React.Component<IRouteTargetProps> {}

export interface IScopeProps {
  vm: IDotnetifyVM;
  options?: IConnectOptions;
}

export class Scope extends React.Component<IScopeProps> {}

export declare const useConnect: <T>(vmId: string, component?: any, options?: IConnectOptions) => { vm: IDotnetifyVM; state: T };
