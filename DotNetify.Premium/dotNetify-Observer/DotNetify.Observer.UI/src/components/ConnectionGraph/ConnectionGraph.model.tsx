import { NodeObject, LinkObject } from "./ForceGraph";

export class Node {
  Id: string;
  Name: string;
  GroupHash: string;
  IsHub: boolean;
}

export class Link {
  Id: string;
  Source: string;
  Target: string;
  Label: string;
  IsGroup: boolean;
}

export class GraphUpdate {
  Nodes: Node[] = [];
  Links: Link[] = [];
  Removed: boolean;
}

export interface GraphNode extends NodeObject {}

export interface GraphLink extends LinkObject {
  id: string;
  label: string;
  isGroup: boolean;
}

export const hubNodeConfig = {
  color: "#92d050",
  val: 2
};

export const clientNodeConfig = {
  color: "#999",
  val: 1
};
