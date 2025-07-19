import React from "react";
import { Panel } from "components/common-ui";
import { ConnectionGraph } from "components/ConnectionGraph";
import { NodeInfo } from "components/NodeInfo";
import { HubInfo } from "components/HubInfo";

export const Connections = () => {
  return (
    <Panel id="Connections" noGap flex>
      <Panel horizontal noGap flex>
        <Panel flex="75%" childProps={{ flex: 1 }} noGap css="border-right:1px solid #ddd;">
          <ConnectionGraph />
        </Panel>
        <Panel flex="25%" noGap>
          <NodeInfo />
        </Panel>
      </Panel>
      <Panel css="flex-grow:0;border-top:1px solid #ddd;">
        <HubInfo />
      </Panel>
    </Panel>
  );
};
