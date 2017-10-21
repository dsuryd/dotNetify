// Type definitions for dotnetify-react.router.
// Project: https://github.com/dsuryd/dotnetify
// Definitions by: brimce <https://github.com/Brimce>

declare module 'dotnetify/dist/dotnetify-react.router' {

   import { dotnetifyVM } from "dotnetify";

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