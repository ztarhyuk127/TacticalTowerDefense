import React from "react";
import { Outlet } from "react-router-dom";
import Greetings from "../layout/Greetings";
import styled from "styled-components";

const FixedGreetings = styled.div`
  position: fixed;
  top: 0;
  width: 100%;
  z-index: 100; // 다른 요소들 위에 표시되도록 z-index 설정
`;

const ContentContainer = styled.div`
  padding-top: 55px; // 오타 수정: 100px가 올바른 단위입니다.
`;

const Layout = () => {
  return (
    <div style={{overflow: "hidden"}}>
      <FixedGreetings>
        <Greetings />
      </FixedGreetings>
      <ContentContainer>
        <Outlet />
      </ContentContainer>
    </div>
  );
};

export default Layout;
