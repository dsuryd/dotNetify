import React, { useEffect, useState } from "react";
import { useConnect } from "dotnetify";
import { ForceGraph, GraphData } from "./ForceGraph";
import { Node, Link, GraphUpdate, GraphNode, GraphLink, hubNodeConfig, clientNodeConfig } from "./ConnectionGraph.model";
import { View, GraphView, Title } from "./ConnectionGraph.style";

type State = {
  GraphUpdate: GraphUpdate;
  SelectedNodeId: string;
};

export const ConnectionGraph = () => {
  const { vm, state } = useConnect<State>("ConnectionGraphVM", {});
  const [data, setData] = useState<GraphData>({ nodes: [], links: [] });
  const [selectedNodeId, setSelectedNodeId] = useState<string>();

  const toGraphNode = (node: Node): GraphNode => {
    let graphNode: any = { id: node.Id, name: node.Name, groupHash: node.GroupHash };
    Object.assign(graphNode, node.IsHub ? hubNodeConfig : clientNodeConfig);
    return graphNode;
  };

  const toGraphLink = (link: Link): GraphLink => ({
    id: link.Id,
    source: link.Source,
    target: link.Target,
    label: link.Label,
    isGroup: link.IsGroup
  });

  useEffect(() => {
    const update = state.GraphUpdate;
    if (update) {
      if (update.Removed) {
        data.nodes = data.nodes.filter((node: GraphNode) => !update.Nodes.some(x => x.Id == node.id));
        data.links = data.links.filter((link: GraphLink) => !update.Links.some(x => x.Id == link.id));
        if (update.Nodes.some(x => x.Id == selectedNodeId)) {
          handleClickNode(selectedNodeId);
        }
      } else {
        const nodes = data.nodes.filter(x => !update.Nodes.some(y => y.Id === x.id));
        const links = data.links.filter((x: GraphLink) => !update.Links.some(y => y.Id === x.id));
        data.nodes = [...nodes, ...update.Nodes.map(x => toGraphNode(x))];
        data.links = [...links, ...update.Links.filter(x => !x.IsGroup).map(x => toGraphLink(x))];
      }
      setData({ ...data });
    }
  }, [state]);

  const handleClickNode = (nodeId: string) => {
    if (selectedNodeId !== nodeId) {
      setSelectedNodeId(nodeId);
      vm.$dispatch({ SelectedNodeId: nodeId });
    } else {
      setSelectedNodeId(null);
      vm.$dispatch({ SelectedNodeId: null });
    }
  };

  const hasData = data.nodes.length > 0;

  return (
    <View id="ConnectionGraph">
      <Title>Connection Graph</Title>
      <GraphView>{hasData && <ForceGraph data={data} onClickNode={handleClickNode} />}</GraphView>
    </View>
  );
};
