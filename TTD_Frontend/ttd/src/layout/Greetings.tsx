import React from "react";
import styled from "styled-components";
import logo from "../asset/image/TTDlogo.png";
import newsletterIcon from "../asset/image/newsletter.png";
import headerback from "../asset/image/Headerback.png";
import { useNavigate } from "react-router-dom";

const HeaderContainer = styled.header`
  height: 6vh;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1vh 2vw; // 적당한 패딩
  background: url(${headerback}); // 헤더 배경 이미지
  color: white;
`;

const Logo = styled.img`
  height: 7vh; // 로고의 높이
  cursor: pointer;
`;

const Menu = styled.nav`
  display: flex;
  justify-content: center;
  flex-grow: 1;

  span {
    margin: 0 1.5vh; // 메뉴 항목 사이의 간격
    font-size: 2rem;
    color: White;
    cursor: pointer;
    &:hover {
      text-decoration: underline;
    }
  }
`;

const NewsletterButton = styled.button`
  background: transparent;
  border: none;
  color: white;
  cursor: pointer;
  display: flex; // Flex 컨테이너 설정
  align-items: center; // 가운데 정렬
  div {
    display: flex; // Flex 컨테이너 설정
    flex-direction: row; // 컬럼 방향으로 내부 아이템 정렬
    align-items: center; // 가운데 정렬
    text-align: center; // 텍스트 중앙 정렬

    img {
      height: 7vh; // 아이콘 크기
    }

    span {
      margin-top: 0.8vh; // 아이콘과 텍스트 사이의 여백
      font-size: 1rem; // 텍스트 사이즈
    }
  }
`;

const Greetings = () => {
  const navigate = useNavigate();
  const GoMain = () => {
    navigate("/");
  };
  return (
    <HeaderContainer>
      <Logo src={logo} alt="게임 로고" onClick={GoMain} />
      <Menu>
        <span onClick={() => navigate("/")}>게임</span>
        <span onClick={() => navigate("/patch")}>패치내역</span>
        <span onClick={() => navigate("/ranking")}>랭킹</span>
      </Menu>
      <NewsletterButton>
        <div>
          <img src={newsletterIcon} alt="뉴스레터" />
        </div>
      </NewsletterButton>
    </HeaderContainer>
  );
};

export default Greetings;
