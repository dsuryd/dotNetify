import React from 'react';
import styled from 'styled-components';
import { Card, Cell, Element, Panel } from 'dotnetify-elements';

const panelCss = `
   overflow-x: hidden;
   .cell { border: none; }
   .cell-body { padding: .5rem 0 }
`;

const statusColors = [ '', 'silver', 'limegreen', 'red', 'gray', 'orange' ];
const userIconColors = [ '#00ce6f', '#a95df0', '#2ea7eb' ];

const UserIcon = styled.span`
  width: 25px;
  height: 25px;
  border-radius: 50%;
  color: white;
  background: ${props => props.color};
  font-weight: bold;
  margin-right: 1rem;
  text-align: center;
`;

const StatusIcon = styled.span`
  height: 14px;
  width: 14px;
  margin-left: 1rem;
  background-color: ${props => statusColors[props.status]};
  border-radius: 50%;
  display: inline-block;
`;

const Activity = ({ person }) => {
  const initial = person.PersonName[0].toUpperCase();
  const iconColor = userIconColors[initial.charCodeAt(0) % 3];
  return (
    <Panel horizontal css={panelCss}>
      <Cell flex>
        <UserIcon color={iconColor}>{initial}</UserIcon>
        {person.PersonName}
      </Cell>
      <Cell flex right middle>
        {person.Status}
        <StatusIcon status={person.StatusId} />
      </Cell>
    </Panel>
  );
};

export default class ActivitiesCard extends Element {
  render() {
    const activities = this.value || [];
    return (
      <Card horizontal>
        <h4>Activities</h4>
        {activities.map((person, idx) => <Activity key={idx} person={person} />)}
      </Card>
    );
  }
}
