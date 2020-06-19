import React from "react";
import styled from "styled-components";
import { Label, Panel } from "dotnetify-elements";
import logo from "../images/logo.png";

const Logo = styled.img`
  font-size: x-large;
  display: flex;
  align-items: center;
  padding-left: 1rem;
  width: 200px;
`;

export const LicenseNotice = styled.div`
  margin-top: auto;
  margin-left: auto;
  padding: 5px 8px;
  font-size: 0.8rem;
  color: #999;
  a {
    color: #337ab7;
    &:hover {
      color: #0056b3;
      text-decoration: none;
    }
    &:focus {
      color: #337ab7;
      > * {
        background: #e7e7e7;
      }
    }
  }
`;

const GitHubIcon = _ => (
  <span>
    <svg
      style={{ width: "24px", height: "24px", fill: "gray" }}
      focusable="false"
      viewBox="0 0 24 24"
      aria-hidden="true"
    >
      <path d="M12 .3a12 12 0 0 0-3.8 23.4c.6.1.8-.3.8-.6v-2c-3.3.7-4-1.6-4-1.6-.6-1.4-1.4-1.8-1.4-1.8-1-.7.1-.7.1-.7 1.2 0 1.9 1.2 1.9 1.2 1 1.8 2.8 1.3 3.5 1 0-.8.4-1.3.7-1.6-2.7-.3-5.5-1.3-5.5-6 0-1.2.5-2.3 1.3-3.1-.2-.4-.6-1.6 0-3.2 0 0 1-.3 3.4 1.2a11.5 11.5 0 0 1 6 0c2.3-1.5 3.3-1.2 3.3-1.2.6 1.6.2 2.8 0 3.2.9.8 1.3 1.9 1.3 3.2 0 4.6-2.8 5.6-5.5 5.9.5.4.9 1 .9 2.2v3.3c0 .3.1.7.8.6A12 12 0 0 0 12 .3" />
    </svg>
  </span>
);

const TwitterIcon = _ => (
  <span>
    <svg
      viewBox="0 0 2000 1625.36"
      width="22px"
      height="22px"
      style={{ fill: "#3aaae1" }}
    >
      <path d="m 1999.9999,192.4 c -73.58,32.64 -152.67,54.69 -235.66,64.61 84.7,-50.78 149.77,-131.19 180.41,-227.01 -79.29,47.03 -167.1,81.17 -260.57,99.57 C 1609.3399,49.82 1502.6999,0 1384.6799,0 c -226.6,0 -410.328,183.71 -410.328,410.31 0,32.16 3.628,63.48 10.625,93.51 -341.016,-17.11 -643.368,-180.47 -845.739,-428.72 -35.324,60.6 -55.5583,131.09 -55.5583,206.29 0,142.36 72.4373,267.95 182.5433,341.53 -67.262,-2.13 -130.535,-20.59 -185.8519,-51.32 -0.039,1.71 -0.039,3.42 -0.039,5.16 0,198.803 141.441,364.635 329.145,402.342 -34.426,9.375 -70.676,14.395 -108.098,14.395 -26.441,0 -52.145,-2.578 -77.203,-7.364 52.215,163.008 203.75,281.649 383.304,284.946 -140.429,110.062 -317.351,175.66 -509.5972,175.66 -33.1211,0 -65.7851,-1.949 -97.8828,-5.738 181.586,116.4176 397.27,184.359 628.988,184.359 754.732,0 1167.462,-625.238 1167.462,-1167.47 0,-17.79 -0.41,-35.48 -1.2,-53.08 80.1799,-57.86 149.7399,-130.12 204.7499,-212.41" />
    </svg>
  </span>
);

const LightThemeIcon = _ => (
  <span>
    <svg
      style={{ width: "24px", height: "24px", fill: "gray" }}
      focusable="false"
      viewBox="0 0 24 24"
      aria-hidden="true"
    >
      <path d="M9 21c0 .55.45 1 1 1h4c.55 0 1-.45 1-1v-1H9v1zm3-19C8.14 2 5 5.14 5 9c0 2.38 1.19 4.47 3 5.74V17c0 .55.45 1 1 1h6c.55 0 1-.45 1-1v-2.26c1.81-1.27 3-3.36 3-5.74 0-3.86-3.14-7-7-7zm2.85 11.1l-.85.6V16h-4v-2.3l-.85-.6C7.8 12.16 7 10.63 7 9c0-2.76 2.24-5 5-5s5 2.24 5 5c0 1.63-.8 3.16-2.15 4.1z" />
    </svg>
  </span>
);

const DarkThemeIcon = _ => (
  <span>
    <svg
      style={{ width: "24px", height: "24px", fill: "gray" }}
      focusable="false"
      viewBox="0 0 24 24"
      aria-hidden="true"
    >
      <path d="m9,21c0,0.55 0.45,1 1,1l4,0c0.55,0 1,-0.45 1,-1l0,-1l-6,0l0,1zm3,-19c-3.86,0 -7,3.14 -7,7c0,2.38 1.19,4.47 3,5.74l0,2.26c0,0.55 0.45,1 1,1l6,0c0.55,0 1,-0.45 1,-1l0,-2.26c1.81,-1.27 3,-3.36 3,-5.74c0,-3.86 -3.14,-7 -7,-7z" />
    </svg>
  </span>
);

const themeIconCss = `
   cursor: pointer;
   @media (max-width: 319px) {
      display: none;
    }   
`;

const twitterIconCss = `
   cursor: pointer;
   @media (max-width: 320px) {
      display: none;
    }   
`;

const githubSponsorCss = `
   border: 1px solid #ccc; 
   border-radius: 6px; 
   padding: 3px 12px; 
   background: #fafbfc;
   text-align: center;
   max-width: 8rem;
   box-shadow: 0 1px 0 rgba(27,31,35,.04), inset 0 1px 0 hsla(0,0%,100%,.25);
   a { display: flex; align-items: center; justify-content: center; }
   svg { color: #ea4aaa; fill: currentColor; margin-right: 5px; }
`;

export const GitHubLink = _ => (
  <Panel
    padding="1rem 1rem .5rem 1rem; &:hover { background: #efefef; }"
    horizontal
    middle
  >
    <a href="https://github.com/dsuryd/dotNetify">
      <Label icon={<GitHubIcon />} css="cursor: pointer">
        GitHub
      </Label>
    </a>
    <d-panel css={githubSponsorCss}>
      <a href="https://github.com/sponsors/dsuryd/">
        <svg height="16" viewBox="0 0 16 16" version="1.1" width="16">
          <path
            fillRule="evenodd"
            d="M4.25 2.5c-1.336 0-2.75 1.164-2.75 3 0 2.15 1.58 4.144 3.365 5.682A20.565 20.565 0 008 13.393a20.561 20.561 0 003.135-2.211C12.92 9.644 14.5 7.65 14.5 5.5c0-1.836-1.414-3-2.75-3-1.373 0-2.609.986-3.029 2.456a.75.75 0 01-1.442 0C6.859 3.486 5.623 2.5 4.25 2.5zM8 14.25l-.345.666-.002-.001-.006-.003-.018-.01a7.643 7.643 0 01-.31-.17 22.075 22.075 0 01-3.434-2.414C2.045 10.731 0 8.35 0 5.5 0 2.836 2.086 1 4.25 1 5.797 1 7.153 1.802 8 3.02 8.847 1.802 10.203 1 11.75 1 13.914 1 16 2.836 16 5.5c0 2.85-2.045 5.231-3.885 6.818a22.08 22.08 0 01-3.744 2.584l-.018.01-.006.003h-.002L8 14.25zm0 0l.345.666a.752.752 0 01-.69 0L8 14.25z"
          />
        </svg>{" "}
        Sponsor
      </a>
    </d-panel>
  </Panel>
);

export const TwitterLink = _ => (
  <a href="https://twitter.com/dotNetify">
    <Label icon={<TwitterIcon />} css={twitterIconCss} />
  </a>
);

export const ThemeToggle = props => (
  <a onClick={props.onClick}>
    <Label
      icon={props.name === "light" ? <LightThemeIcon /> : <DarkThemeIcon />}
      css={themeIconCss}
    />
  </a>
);

const NavHeader = styled.div`
  height: 55px;
  display: flex;
  align-items: center;
`;

const DotNetifyLogo = _ => (
  <NavHeader>
    <a href="/">
      <Logo src={logo} />
    </a>
  </NavHeader>
);

export default DotNetifyLogo;
