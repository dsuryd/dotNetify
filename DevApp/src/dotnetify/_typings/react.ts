import * as React from "react";
import { RouteType, IDotnetifyVM, IConnectOptions } from "./index";
export { IConnectOptions } from "./index";

export interface IDotnetifyReact {
  connect(iVMId: string, iReact: React.Component | any, iOptions?: IConnectOptions): IDotnetifyVM;
}

export interface IRouteLinkProps {
  vm: IDotnetifyVM;
  route: RouteType;
  style?: React.CSSProperties;
  className?: string;
  children?: React.ReactNode;
  onClick?: (e: React.MouseEvent) => boolean;
}

export class RouteLink extends React.Component<IRouteLinkProps> {}

export interface IRouteTargetProps extends React.HTMLAttributes<HTMLElement> {
  id: string;
}

export class RouteTarget extends React.Component<IRouteTargetProps> {}

export interface IScopeProps {
  vm: IDotnetifyVM;
  options?: IConnectOptions;
  children?: React.ReactNode;
}

export class Scope extends React.Component<IScopeProps> {}

export declare const useConnect: <T>(
  vmId: string,
  component?: any,
  options?: IConnectOptions
) => { vm: IDotnetifyVM; state: T; setState: React.Dispatch<any> };
