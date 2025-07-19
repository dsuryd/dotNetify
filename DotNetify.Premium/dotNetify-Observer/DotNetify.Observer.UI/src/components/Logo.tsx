import React from "react";
import styled from "styled-components";

const Logo = styled.img`
  font-size: x-large;
  display: flex;
  align-items: center;
  padding-left: 1rem;
  width: 200px;
`;

const NavHeader = styled.div`
  height: 55px;
  display: flex;
  align-items: center;
`;

export const DotNetifyLogo = _ => (
  <NavHeader>
    <a href="/">
      <Logo src="../observer-ui/logo.png" />
    </a>
  </NavHeader>
);
