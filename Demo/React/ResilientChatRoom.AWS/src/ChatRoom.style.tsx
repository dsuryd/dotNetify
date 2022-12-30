import styled from "@emotion/styled";

export const ChatRoomView = styled.div`
  background-color: #eee;
  footer {
    max-width: 1268px;
    padding: 0.5rem;
    font-size: small;
    font-style: italic;
    margin-top: 1rem;
    display: flex;
    flex-direction: row;
  }
  .chatPanel {
    height: calc(100vh - 100px);
    max-width: 1268px;
    display: flex;
    flex: 1;
    flex-direction: row;
    border: 1px solid #ccc;
    border-radius: 0.2rem;
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
        line-height: 0.8rem;
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
      > div:first-of-type {
        padding: 1rem;
        flex: 1;
        overflow: auto;
        > div {
          margin-bottom: 0.5rem;
          &:nth-last-of-type(2),
          &:last-of-type {
            margin-bottom: 0;
          }
          span:first-of-type {
            font-weight: 500;
            margin-right: 1rem;
          }
          span:last-of-type {
            font-size: x-small;
            color: #999;
          }
        }
      }
      > div:last-of-type {
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
