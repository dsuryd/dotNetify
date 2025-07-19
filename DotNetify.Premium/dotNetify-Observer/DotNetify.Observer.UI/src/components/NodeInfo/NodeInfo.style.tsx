import styled from "styled-components";

export const View = styled.div`
  display: flex;
  flex-direction: column;
  flex: 1;
  padding: 1rem;
  background-color: white;
`;

export const InfoArea = styled.div`
  display: flex;
  flex-wrap: wrap;
  justify-content: space-between;
`;

export const InfoView = styled.div`
  margin-top: 0.5rem;
  margin-right: 1rem;
  min-width: 6rem;
`;

export const InfoLabel = styled.label`
  font-size: 0.75rem;
`;

export const InfoValue = styled.div`
  font-weight: 500;
  word-wrap: break-word;
`;
