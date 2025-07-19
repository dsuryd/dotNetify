import React, { useEffect, useState } from "react";
import ForceGraph2D, { GraphData } from "react-force-graph-2d";
export { GraphData, NodeObject, LinkObject } from "react-force-graph-2d";

type ForceGraphProps = {
  data: GraphData;
  onClickNode?: (nodeId: string) => void;
};

type Size = {
  width: number;
  height: number;
};

const graphColor = {
  nodeLabel: "black",
  link: "#ccc",
  highlightedNode: "#666",
  highlightedLink: "#666",
  selectedNode: "#337ab7"
};

export const ForceGraph = ({ data, onClickNode }: ForceGraphProps) => {
  const [size, setSize] = useState<Size>();

  useEffect(() => {
    const container = document.getElementById("ForceGraph").parentElement;
    setSize({ width: container.offsetWidth, height: container.offsetHeight });

    window.addEventListener("resize", () => {
      // Set height to zero to allow the parent container to shrink, then calculate the actual height on the next cycle.
      setSize({ width: container.offsetWidth, height: 0 });
      setTimeout(() => setSize({ width: container.offsetWidth, height: container.offsetHeight }));
    });
  }, []);

  const [highlightNodes, setHighlightNodes] = useState(new Set());
  const [highlightLinks, setHighlightLinks] = useState(new Set());
  const [hoverNode, setHoverNode] = useState(null);
  const [selectedNode, setSelectedNode] = useState(null);

  const updateHighlight = () => {
    setHighlightNodes(highlightNodes);
    setHighlightLinks(highlightLinks);
  };

  const drawNodeLabel = (node, ctx, globalScale) => {
    const label = node.name;
    const fontSize = 12 / globalScale;
    const fontWeight = hoverNode === node.id ? "bold " : "";
    ctx.font = `${fontWeight}${fontSize}px Sans-Serif`;
    ctx.textAlign = "center";
    ctx.textBaseline = "middle";
    ctx.fillStyle = graphColor.nodeLabel;
    ctx.fillText(label, node.x, node.y + 8);
  };

  const drawNodeRing = (node, ctx) => {
    if (node.groupHash !== 0) {
      ctx.beginPath();
      ctx.arc(node.x, node.y, 1.7, 0, 2 * Math.PI, false);
      ctx.fillStyle = "hsl(" + (node.groupHash % 360) + ",25%,50%)";
      ctx.fill();
    }

    if (highlightNodes.has(node.id)) {
      ctx.beginPath();
      ctx.arc(node.x, node.y, node.val + 3, 0, 2 * Math.PI, false);
      ctx.strokeStyle = graphColor.highlightedNode;
      ctx.stroke();
    }

    if (selectedNode === node.id) {
      ctx.beginPath();
      ctx.arc(node.x, node.y, node.val + 3, 0, 2 * Math.PI, false);
      ctx.strokeStyle = graphColor.selectedNode;
      ctx.stroke();
    }
  };

  const handleNodeHover = node => {
    highlightNodes.clear();
    highlightLinks.clear();
    if (node) {
      highlightNodes.add(node);
      const links = data.links.filter((link: any) => link.source.id == node.id || link.target.id == node.id);
      links.forEach((link: any) => {
        highlightLinks.add(link.id);
        highlightNodes.add(link.source.id);
        highlightNodes.add(link.target.id);
      });
    }

    setHoverNode(node?.id);
    updateHighlight();
  };

  const handleNodeClick = node => {
    const id = selectedNode != node.id ? node.id : null;
    setSelectedNode(id);
    onClickNode(id);
  };

  return (
    <div id="ForceGraph">
      <ForceGraph2D
        width={size?.width || 400}
        height={size?.height || 300}
        graphData={data}
        nodeLabel={() => ""}
        nodeCanvasObjectMode={() => "after"}
        nodeCanvasObject={(node: any, ctx, globalScale) => {
          drawNodeLabel(node, ctx, globalScale);
          drawNodeRing(node, ctx);
        }}
        linkColor={(link: any) =>
          link.isGroup ? "rgba(0,0,0,0)" : highlightLinks.has(link.id) ? graphColor.highlightedLink : graphColor.link
        }
        linkWidth={(link: any) => (link.isGroup ? 0 : highlightLinks.has(link.id) ? 2 : 1)}
        onNodeHover={handleNodeHover}
        onNodeClick={handleNodeClick}
        onNodeDragEnd={node => {
          node.fx = node.x;
          node.fy = node.y;
        }}
      />
    </div>
  );
};
