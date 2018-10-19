import styled from 'styled-components';

export const HelloWorldCss = styled.div`
  padding: 0 1rem;
  b {
    color: #ce4844;
    font-weight: 500;
  }
  section {
    display: flex;
    max-width: 1268px;
    margin-bottom: 1rem;
    > * {
      flex: 1;
      margin-right: 1rem;
    }
    label {
      font-weight: 500;
    }
  }
`;

export const ControlTypesCss = styled.div`
  max-width: 1268px;
  padding: 0 1rem;
  b {
    color: #ce4844;
    font-weight: 500;
  }
  table {
    font-size: unset;
    width: 100%;
  }
  tr {
    td {
      padding-bottom: 1.5rem;
    }
    td:first-child {
      font-weight: 500;
      display: flex;
      flex-direction: column;
      > label {
        font-weight: normal;
        font-size: small;
      }
    }
    td:nth-child(2) {
      width: 70%;
      padding-right: 1.5rem;
      li {
        cursor: pointer;
      }
      li:hover {
        background: #efefef;
      }
      input ~ * {
        margin-left: 0rem;
        margin-right: 0rem;
        > li {
          padding-left: .75rem;
        }
      }
      label {
        cursor: pointer;
        user-select: none;
        margin-right: 1.5rem;
      }
    }
    button.label-success {
      margin-left: 3rem;
      background: #5cb85c;
    }
    button.label-warning {
      margin-left: 3rem;
      background: #f0ad4e;
    }
    button {
      min-width: 6rem;
    }
  }
`;

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
  .example-root > * {
    display: flex;
    align-items: center;
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
  }
  @media (max-width: 1170px) {
    > div:first-child,
    > div:last-child {
      display: block;
      width: 100%;
      margin-bottom: 2rem;
    }
    .example-root > * {
      flex-direction: column;
      > div:first-child,
      > div:last-child {
        display: block;
        width: 100%;
        margin-bottom: 2rem;
      }
    }
  }
`;

export const SecurePageCss = styled.div`
  padding: 0 1rem;
  max-width: 1268px;
  display: flex;
  > * {
    flex: 1;
    margin-right: 1rem;
  }
  .example-root > div {
    display: flex;
    > * {
      flex: 1;
      margin-right: 1rem;
    }
  }
  b {
    color: #ce4844;
    font-weight: 500;
  }
  label {
    font-weight: 500;
    margin-top: 1rem;
  }
  .card-body > div:last-child {
    margin-bottom: 2rem;
  }
  .card-body > strong {
    display: block;
    margin-bottom: 1rem;
  }
  .card-footer {
    display: flex;
    justify-content: flex-end;
  }
  .logout {
    display: flex;
    align-items: center;
    justify-content: center;
  }
`;

export const BookStoreCss = styled.div`
  padding: 0 1rem;
  max-width: 1268px;
  > header,
  .example-root > section > header {
    padding: 1rem;
    margin-bottom: 1rem;
    background: #efefef;
    border-radius: 5px;
    border: 1px solid #ddd;
  }
  > section,
  .example-root > section > div {
    display: flex;
    flex-wrap: wrap;
    > div {
      flex: 1 1 25%;
      margin-bottom: 2.5rem;
      img {
        padding: 4px;
        border-radius: 4px;
        background: white;
        border: 1px solid #ddd;
        margin-bottom: 1rem;
      }
    }
  }
`;

export const BookCss = styled.div`
  display: flex;
  > *:last-child {
    margin: 1rem;
    button {
      margin-top: 1rem;
    }
  }
`;

export const ChatRoomCss = styled.div`
  footer {
    max-width: 1268px;
    padding: .5rem;
    font-size: small;
    font-style: italic;
    margin-top: 1rem;
    display: flex;
    flex-direction: row;
  }
  .chatPanel {
    height: calc(100vh - 23rem);
    max-width: 1268px;
    display: flex;
    flex: 1;
    flex-direction: row;
    border: 1px solid #ccc;
    border-radius: .2rem;
    nav {
      padding: 1rem;
      min-width: 11rem;
      border-right: 1px solid #ccc;
      overflow: auto;
      b {
        font-weight: 500;
        &.myself {
          text-decoration: underline;
        }
      }
      p > *:not(b) {
        font-size: x-small;
        line-height: .8rem;
        color: #999;
      }
      p > span {
        display: block;
      }
    }
    section {
      display: flex;
      flex-direction: column;
      flex: 1;
      background: #fff;
      .private {
        font-style: italic;
      }
      > div:first-child {
        padding: 1rem;
        flex: 1;
        overflow: auto;
        > div {
          margin-bottom: .5rem;
          &:nth-last-child(2),
          &:last-child {
            margin-bottom: 0;
          }
          span:first-child {
            font-weight: 500;
            margin-right: 1rem;
          }
          span:last-child {
            font-size: x-small;
            color: #999;
          }
        }
      }
      > div:last-child {
        input {
          border: none;
          border-top: 1px solid #ccc;
          border-top-left-radius: unset;
          border-top-right-radius: unset;
        }
      }
    }
  }
`;
