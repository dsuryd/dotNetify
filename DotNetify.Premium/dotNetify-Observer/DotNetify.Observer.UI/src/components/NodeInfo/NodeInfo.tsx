import React from "react";
import { useConnect } from "dotnetify";
import { Title } from "../common-ui";
import { View, InfoView, InfoArea, InfoLabel, InfoValue } from "./NodeInfo.style";
import { NodeInfoItem } from "./NodeInfo.model";

type State = {
  InfoItems: NodeInfoItem[];
};

const InfoField = (info: NodeInfoItem) => (
  <InfoView key={info.Label}>
    <InfoLabel>{info.Label}</InfoLabel>
    <InfoValue>{info.Value}</InfoValue>
  </InfoView>
);

export const NodeInfo = () => {
  const { state } = useConnect<State>("NodeInfoVM", {});
  return (
    <View>
      {state.InfoItems && (
        <React.Fragment>
          <Title>Node Info</Title>
          <InfoArea>{state.InfoItems?.map(info => InfoField(info))}</InfoArea>
        </React.Fragment>
      )}
    </View>
  );
};
