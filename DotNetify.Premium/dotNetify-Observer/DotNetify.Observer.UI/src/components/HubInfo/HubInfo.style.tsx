import styled from "styled-components";

export const View = styled.div`
  display: flex;
  flex-direction: column;
  flex: 1;
  padding: 1rem;
  background-color: white;
  border-bottom: 1px solid #ddd;
`;

export const InfoView = styled.div`
  margin-top: 0.5rem;
  .row:nth-child(even) {
    background-color: #efefef;
  }
  .row.header {
    font-size: 0.75rem;
  }
  .row:not(.header) {
    font-weight: 500;
  }
`;

export const InfoRow = styled.div.attrs((props: any) => ({
  className: "row" + (props.header ? " header" : ""),
  header: props.header
}))`
  display: flex;
  > * {
    margin-right: 1rem;
  }
  .name {
    min-width: 20rem;
  }
  .clients,
  .cpu,
  .mem {
    min-width: 5rem;
    text-align: right;
  }
`;
