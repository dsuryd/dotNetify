import React, { useState, useEffect } from "react";
import { useConnect } from "dotnetify";
import { Title } from "../common-ui";
import { View, InfoView, InfoRow } from "./HubInfo.style";
import { HubInfoItem } from "./HubInfo.model";

type State = {
  InfoItems: HubInfoItem[];
};

export const HubInfo = () => {
  const { vm, state } = useConnect<State>("HubInfoVM", { InfoItems: [] });
  const [count, setCount] = useState(0);

  useEffect(() => {
    if (count < state.InfoItems?.length) {
      setCount(state.InfoItems.length);
      window.dispatchEvent(new Event("resize"));
    }
  }, [state]);
  return (
    <View>
      {state.InfoItems?.length > 0 && (
        <React.Fragment>
          <Title>Hub Info</Title>
          <InfoView>
            <InfoRow header={true}>
              <div className="name"></div>
              <div className="clients">Total Clients</div>
              <div className="cpu">Total CPU</div>
              <div className="mem">Total Memory</div>
            </InfoRow>

            {state.InfoItems?.map(info => (
              <InfoRow key={info.Id}>
                <div className="name">{info.Name}</div>
                <div className="clients">{info.Clients}</div>
                <div className="cpu">{info.Cpu} %</div>
                <div className="mem">{info.Memory} %</div>
              </InfoRow>
            ))}
          </InfoView>
        </React.Fragment>
      )}
    </View>
  );
};
