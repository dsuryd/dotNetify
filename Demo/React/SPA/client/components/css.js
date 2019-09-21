import styled from 'styled-components';

export const SimpleListCss = styled.div`
  padding: 0 1rem;
  header {
    display: flex;
    align-items: center;
    margin-bottom: 1rem;
    > * {
      margin-right: 1rem;
    }
    input[type="text"] {
      max-width: 15rem;
    }
  }
  table {
    font-size: unset;
    width: 100%;
    max-width: 1268px;
    td,
    th {
      padding: 0.5rem 0;
      padding-right: 2rem;
      border-bottom: 1px solid #ddd;
      width: 50%;
    }
    th {
      font-weight: 500;
    }
    td:last-child,
    th:last-child {
      width: 5rem;
      > div {
        display: flex;
        align-items: center;
        cursor: pointer;
      }
    }
    tr:hover {
      background: #efefef;
    }
    i.material-icons {
      font-size: 1.2rem;
    }
    span.editable:hover {
      &:after {
        font-family: "Material Icons";
        content: "edit";
      }
    }
  }
`;

export const LiveChartCss = styled.div`
  padding: 0 1rem;
  width: 100%;
  max-width: 1268px;
  > div:first-child {
    display: inline-block;
    width: 70%;
  }
  > div:last-child {
    display: inline-block;
    width: 30%;
    > *:last-child {
      margin-top: 2rem;
    }
  }
  @media (max-width: 1170px) {
    > div:first-child,
    > div:last-child {
      display: block;
      width: 100%;
      margin-bottom: 2rem;
    }
  }
`;

export const CompositeViewCss = styled.div`
  padding: 0 1rem;
  max-width: 1268px;
  display: flex;
  flex-wrap: wrap;
  .example-root {
    width: 100%;
    max-width: 1268px;
    > div {
      display: flex;
      flex-wrap: wrap;
    }
  }
  .example-root section,
  > div {
    width: 70%;
    margin-right: 1rem;
  }
  .example-root aside,
  > aside {
    width: calc(30% - 1rem);
    min-width: 10rem;
  }
  @media (max-width: 860px) {
    .example-root section,
    > div {
      width: 100%;
      margin-bottom: 2rem;
    }
    .example-root aside,
    > aside {
      width: 100%;
    }
  }
  .card {
    width: 100%;
    p {
      font-size: unset;
    }
    .card-header > *:first-child {
      font-size: large;
    }
  }
  table {
    font-size: unset;
    width: 100%;
    tr {
      &:hover {
        background: #eee;
        cursor: pointer;
      }
      &.selected {
        background: #ddd;
      }
    }
    td,
    th {
      width: 25%;
      padding: 0.5rem;
      padding-right: 2rem;
      border-bottom: 1px solid #ddd;
      &:nth-child(1),
      &:nth-child(3) {
        width: 10%;
      }
    }
  }
  .pagination {
    user-select: none;
    div {
      margin-top: 1rem;
      padding: 0.5rem 1rem;
      border: 1px solid #ccc;
      &.current {
        color: #aaa;
        background: #eee;
      }
    }
    div:hover:not(.current) {
      background: #eee;
      cursor: pointer;
    }
  }
  .filter {
    margin-top: 1rem;
    .card-header {
      font-weight: bold;
    }
    .card-body {
      > * {
        margin-bottom: 1rem;
      }
    }
    .card-footer {
      display: flex;
      justify-content: flex-end;
    }
    .chip {
      margin-right: 0.25rem;
      margin-bottom: 0.25rem;
      display: inline-flex;
      font-size: x-small;
      background: #eee;
      border-radius: 0.25rem;
      align-items: center;
      padding: 0.2rem 0.4rem;
      i.material-icons {
        font-size: small;
        font-weight: bold;
        cursor: pointer;
        margin-left: 0.5rem;
      }
    }
  }
`;
