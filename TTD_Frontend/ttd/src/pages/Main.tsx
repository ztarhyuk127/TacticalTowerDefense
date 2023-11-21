import React, { useEffect, useState } from "react";
import styled, { keyframes } from "styled-components";
import secondback from "../asset/image/secondback.png";
import thirdback from "../asset/image/thirdback.png";
import firstpagetitle from "../asset/image/firsttitle2.png";
import characterGif from "../asset/image/unit1.gif";
import characterGif2 from "../asset/image/boss4.gif";
import characterGif3 from "../asset/image/boss1.gif";
import characterGif4 from "../asset/image/unit2attack.gif";
import characterGif5 from "../asset/image/unit5skill.gif";
import woodboard from "../asset/image/woodboard.png";
import starboard from "../asset/image/starboard.png";
import weaponback from "../asset/image/weaponback.png";
import starbackground from "../asset/image/starback.png";
import clickme from "../asset/image/greenclick.png";
import clickme2 from "../asset/image/purpleclick.png";
import Rdownload from "../asset/image/downloadright.png";
import Ldownload from "../asset/image/downloadleft.png";
import ReactPlayer from "react-player";
import { PC_File_URL, Mobile_File_URL } from "../apis/Url";

export interface BackgroundProps {
  backgroundimage: string;
  height: number;
}

export const Background = styled.div<BackgroundProps>`
  box-sizing: border-box;
  height: ${(props) => props.height}vh;
  width: 100vw;
  background: url(${(props) => props.backgroundimage}) no-repeat center center;
  background-size: cover;
  position: relative;
  overflow: hidden;
`;

interface TalkCharacterProps {
  characterGif: string;
  position: string;
}

const TalkCharacter = styled.div<TalkCharacterProps>`
  background-image: url(${(props) => props.characterGif});
  background-size: contain;
  background-repeat: no-repeat;
  height: 100vh;
  width: 100vh;
  position: absolute;
  bottom: -10vh;
  // pointer-events: none;
  cursor: pointer;
  ${(props) => (props.position === "left" ? `left: ovh;` : `right: -10vh;`)}
`;

const TitleImage = styled.div`
  background-image: url(${firstpagetitle});
  background-size: contain;
  background-repeat: no-repeat;
  position: relative;
  left: 50%;
  top: 50%;
  transform: translate(-50%, -50%);
  width: 40vh;
  height: 40vh;
`;

export const CharactersContainer = styled.div``;

interface MainProps {
  backgroundImage: string;
  characterGif: string;
}

export interface Position {
  x: number;
  y: number;
}
const VideoBackground = styled.div`
  box-sizing: border-box;
  height: 100vh;
  width: 100vw;
  position: relative;
  overflow: hidden;
  pointer-events: none;
  margin-top: -5vh;
`;

const Woodback = styled.div`
  background-image: url(${woodboard});
  background-size: cover;
  background-repeat: no-repeat;
  position: absolute;
  bottom: -75%;
  left: 70%;
  transform: translate(-50%, -50%);
  width: 130vh;
  height: 85vh;
  display: flex;
  align-items: center;
  justify-content: center;
  text-align: center;
  padding-top: 1em;
  cursor: pointer;
`;

const Downloadback = styled.div`
  background-image: url(${Rdownload});
  background-size: cover;
  background-repeat: no-repeat;
  position: absolute;
  bottom: -25%;
  left: 80%;
  transform: translate(-50%, -50%);
  width: 60vh;
  height: 60vh;
  display: flex;
  align-items: center;
  justify-content: center;
  text-align: center;
  padding-top: 1em;
  cursor: pointer;
`;

const Downloadback2 = styled.div`
  background-image: url(${Ldownload});
  background-size: cover;
  background-repeat: no-repeat;
  position: absolute;
  bottom: -25%;
  right: 46%;
  transform: translate(-50%, -50%);
  width: 60vh;
  height: 60vh;
  display: flex;
  align-items: center;
  justify-content: center;
  text-align: center;
  padding-top: 1em;
  cursor: pointer;
`;

const Starback = styled.div`
  background-image: url(${starboard});
  background-size: cover;
  background-repeat: no-repeat;
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 130vh;
  height: 120vh;
  display: flex;
  align-items: center;
  justify-content: center;
  text-align: center;
  padding-top: 1em;
  cursor: pointer;
`;

interface BlackTalkProps {
  direction: string;
  textdirection: string;
}
const BlackTalk = styled.div<BlackTalkProps>`
  position: absolute;
  bottom: -15%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 100vw;
  height: 30vh;
  display: flex;
  align-items: center;
  justify-content: ${(props) => props.direction || "flex-start"}; /* 텍스트 정렬 방향에 따라 스타일 적용 */
  text-align: ${(props) => props.textdirection || "left"}; /* 텍스트 정렬 방향에 따라 스타일 적용 */
  padding-top: 1em;
  padding-left: ${(props) => (props.textdirection === "left" ? "2em" : "0")}; /* 조건부 패딩 적용 */
  padding-right: ${(props) => (props.textdirection === "right" ? "10em" : "0")}; /* 조건부 패딩 적용 */
  cursor: pointer;
  background-color: rgba(0, 0, 0, 0.8); /* 검정색 배경에 50% 투명도 적용 */
`;

const BubbleText = styled.h1`
  display: block;
  max-width: 90%;
  word-wrap: break-word;
  color: White;
  font-size: 6vh;
  margin-top: -15vh;
`;

interface BrownTalkProps {
  height: string;
}

const BrownTalk = styled.div<BrownTalkProps>`
  position: absolute;
  bottom: ${(props) => props.height};
  right: -14.5%;
  transform: translate(-50%, -50%);
  width: 30vw;
  height: 10vh;
  display: flex;
  align-items: center;
  justify-content: center;
  text-align: center;
  padding-top: 1em;
  cursor: pointer;
  background-color: rgba(255, 255, 255, 0.8);
  border-radius: 15px;
`;

const BrownText = styled.h1`
  display: block;
  max-width: 90%;
  word-wrap: break-word;
  color: black;
  font-size: 4vh;
`;

const WoodText = styled.h1`
  display: block;
  font-weight: bold; /* 볼드체로 설정 */
  max-width: 90%;
  word-wrap: break-word;
  color: Black;
  font-size: 2.5vh;
  margin-left: 22vh;
`;

const StarText = styled.h1`
  display: block;
  font-weight: bold;
  max-width: 90%;
  word-wrap: break-word;
  color: black;
  font-size: 2.5vh;
  margin-top: 0; /* 수정: 0으로 설정하여 정중앙으로 이동 */
  text-align: center; /* 수정: 가운데 정렬 추가 */
  margin-left: 6vh;
`;

const StarText1 = styled.h1`
  display: block;
  font-weight: bold;
  max-width: 90%;
  word-wrap: break-word;
  color: black;
  font-size: 3vh;
  margin-top: 0; /* 수정: 0으로 설정하여 정중앙으로 이동 */
  text-align: center; /* 수정: 가운데 정렬 추가 */
  margin-left: 3vh;
`;

const StarText2 = styled.h1`
  display: block;
  font-weight: bold;
  max-width: 90%;
  word-wrap: break-word;
  color: black;
  font-size: 3vh;
  margin-top: 0; /* 수정: 0으로 설정하여 정중앙으로 이동 */
  text-align: center; /* 수정: 가운데 정렬 추가 */
  margin-left: 2vh;
`;
export interface CharacterProps {
  charactergif: string;
}

export const Character = styled.div<CharacterProps>`
  background-image: url(${(props) => props.charactergif});
  background-size: contain;
  background-repeat: no-repeat;
  height: 20vh;
  width: 20vw;
  position: absolute;
  pointer-events: none;
`;

const blinkAnimation = keyframes`
  0%, 100% { opacity: 0; }
  50% { opacity: 1; }
`;

// 2. 컴포넌트 스타일링
const BlinkingImage = styled.div`
  position: absolute;
  top: 10vh; // 위치 조정
  right: 20vh; // 위치 조정
  width: 20vw;
  height: 30vh;
  background-image: url(${clickme});
  background-size: cover;
  z-index: 10; // Z-인덱스 추가
  animation: ${blinkAnimation} 3s ease-in-out infinite;
`;

const BlinkingImage2 = styled.div`
  position: absolute;
  top: 0vh; // 위치 조정
  left: 30vh; // 위치 조정
  width: 20vw;
  height: 30vh;
  background-image: url(${clickme2});
  background-size: cover;
  z-index: 10; // Z-인덱스 추가
  animation: ${blinkAnimation} 3s ease-in-out infinite;
`;

const Main = () => {
  const [mousePosition, setMousePosition] = useState<Position>({ x: 0, y: 0 });
  const [playing, setPlaying] = useState(true);
  const [message, setMessage] = useState("멀봐!");
  const [message2, setMessage2] = useState("안녕하세요.");
  const [message3, setMessage3] = useState("▶ 들어본다");
  const [typingIndex, setTypingIndex] = useState(0);
  const [typingInterval, setTypingInterval] = useState<number | null>(null);
  const [showTalk, setshowTalk] = useState(false);
  const [showClick, setshowClick] = useState(true);
  const [showClick2, setshowClick2] = useState(true);
  const [showTalk2, setshowTalk2] = useState(false);
  const [showTalk3, setshowTalk3] = useState(false);
  const [isCharacterVisible, setIsCharacterVisible] = useState(false);
  const [isCharacterVisible2, setIsCharacterVisible2] = useState(false);
  const [isCharacterVisible3, setIsCharacterVisible3] = useState(false);
  const [isCharacterVisible4, setIsCharacterVisible4] = useState(false);
  const [isCharacterVisible5, setIsCharacterVisible5] = useState(false);

  const handleSpeechBubbleClick = (e: any) => {
    e.preventDefault();
    if (message === "멀봐!") {
      startTyping("그만누르고 내려가!", setMessage);
    } else if (message == "그만누르고 내려가!") {
      startTyping("왜 또 올라왔어! 게임하러가!", setMessage);
      const targetSection = document.getElementById("talkWizard");
      if (targetSection) {
        targetSection.scrollIntoView({ behavior: "smooth" });
      }
    } else if (message == "왜 또 올라왔어! 게임하러가!") {
      startTyping("왜 또 올라왔어! 게임하러가!", setMessage);
      const targetSection = document.getElementById("download");
      if (targetSection) {
        targetSection.scrollIntoView({ behavior: "smooth" });
      }
    }
  };

  const handleSpeechBubbleClick2 = (e: any) => {
    e.preventDefault();
    if (message2 === "안녕하세요.") {
      startTyping("I am 착한 마녀에요.", setMessage2);
    } else if (message2 == "I am 착한 마녀에요.") {
      startTyping("급하게 전할 말이 있어요.", setMessage2);
      setTimeout(() => {
        setshowTalk3(true);
      }, 2000);
    }
  };

  const handleSpeechBubbleClick3 = (e: any) => {
    e.preventDefault();
    if (message3 === "▶ 들어본다") {
      setshowTalk3(false);
      startTyping("지금 몬스터들이 마을을 침공해요.", setMessage2);
      setMessage3("▶ 지금 가야겠어");
      setTimeout(() => {
        setshowTalk3(true);
      }, 2000);
    } else if (message3 == "▶ 지금 가야겠어") {
      setshowTalk3(false);
      startTyping("가기전에 잠시..!", setMessage2);
      setMessage3("▶ 왜?? 지금 당장 가야해!");
      setTimeout(() => {
        setshowTalk3(true);
      }, 2000);
    } else if (message3 == "▶ 왜?? 지금 당장 가야해!") {
      setshowTalk3(false);
      startTyping("제발 튜토리얼을 읽어줘..ㅠ", setMessage2);
      setMessage3("▶ 왜?? 지금 당장 가야해!");
      setTimeout(() => {
        const targetSection = document.getElementById("explain");
        if (targetSection) {
          targetSection.scrollIntoView({ behavior: "smooth" });
        }
      }, 2000);
    }
  };
  const handleSpeechBubbleClick4 = (e: any) => {
    e.preventDefault();
    const targetSection = document.getElementById("download");
    if (targetSection) {
      targetSection.scrollIntoView({ behavior: "smooth" });
    }
  };
  const DownloadforPC = () => {
    const redirectToPC = () => {
      window.location.href = PC_File_URL;
    };
    redirectToPC();
  };
  const DownloadforMobile = () => {
    const redirectToMobile = () => {
      window.location.href = Mobile_File_URL;
    };
    redirectToMobile();
  };

  const changeShowTalk = (e: any) => {
    e.preventDefault();
    setshowClick(false);
    setshowTalk(true);
  };
  const changeShowTalk2 = (e: any) => {
    e.preventDefault();
    setshowClick2(false);
    setshowTalk2(true);
  };

  useEffect(() => {
    const handleMouseMove = (e: MouseEvent) => {
      e.preventDefault();
      setMousePosition({
        x: e.clientX - 100,
        y: e.clientY - 100,
      });
    };

    window.addEventListener("mousemove", handleMouseMove);

    return () => {
      window.removeEventListener("mousemove", handleMouseMove);
    };
  }, []);

  const startTyping = (text: string, setMessageFunction: (message: string) => void) => {
    let index = 0;
    setTypingIndex(0);

    const interval = setInterval(() => {
      if (index <= text.length) {
        setMessageFunction(text.slice(0, index));
        index++;
      } else {
        clearInterval(interval);
      }
    }, 100); // 한 글자씩 나타나는 시간 (0.1초)
    setTypingInterval(interval as any);
  };
  useEffect(() => {
    const handleMouseMove = (e: MouseEvent) => {
      // 각 섹션의 요소를 가져옵니다.
      const showplayElement = document.getElementById("showplay");
      const talkOakElement = document.getElementById("talkOak");
      const talkWizardElement = document.getElementById("talkWizard");
      const explainElement = document.getElementById("explain");
      const downloadElement = document.getElementById("download");

      // 마우스 위치에 따라 각 캐릭터의 표시 여부를 결정합니다.
      setIsCharacterVisible(checkMouseWithin(showplayElement, e));
      setIsCharacterVisible2(checkMouseWithin(talkOakElement, e));
      setIsCharacterVisible3(checkMouseWithin(talkWizardElement, e));
      setIsCharacterVisible4(checkMouseWithin(explainElement, e));
      setIsCharacterVisible5(checkMouseWithin(downloadElement, e));
      // 다른 섹션에 대한 로직도 추가 가능
    };

    // 주어진 요소 내에 마우스가 있는지 확인하는 함수
    const checkMouseWithin = (element: HTMLElement | null, e: any) => {
      if (element) {
        const rect = element.getBoundingClientRect();
        return e.clientX >= rect.left && e.clientX <= rect.right && e.clientY >= rect.top && e.clientY <= rect.bottom;
      }
      return false;
    };

    document.addEventListener("mousemove", handleMouseMove);

    return () => {
      document.removeEventListener("mousemove", handleMouseMove);
    };
  }, []);

  return (
    <>
      <section id="showplay">
        <VideoBackground>
          <ReactPlayer
            url="https://www.youtube.com/watch?v=2WPH5pEHn2A&vq=hd1080"
            playing={playing}
            loop
            muted
            width="100%"
            height="100%"
            style={{ position: "absolute", top: 0, left: 0 }}
          />
          <TitleImage></TitleImage>
          <CharactersContainer>
            {isCharacterVisible && (
              <Character
                charactergif={characterGif}
                style={{
                  left: `${mousePosition.x}px`,
                  top: `${mousePosition.y}px`,
                }}
              />
            )}
          </CharactersContainer>
        </VideoBackground>
      </section>
      <section id="talkOak">
        <Background backgroundimage={secondback} height={95}>
          {showClick && <BlinkingImage />}
          {showTalk && (
            <BlackTalk onClick={handleSpeechBubbleClick} direction="flex-start" textdirection="left">
              <BubbleText>{message}</BubbleText>
            </BlackTalk>
          )}
          <CharactersContainer>
            {isCharacterVisible2 && (
              <Character
                charactergif={characterGif3}
                style={{
                  left: `${mousePosition.x}px`,
                  top: `${mousePosition.y}px`,
                }}
              />
            )}
            <TalkCharacter characterGif={characterGif3} position="right" onClick={changeShowTalk} />
          </CharactersContainer>
        </Background>
      </section>
      <section id="talkWizard">
        <Background backgroundimage={thirdback} height={95}>
          {showClick2 && <BlinkingImage2></BlinkingImage2>}
          {showTalk2 && (
            <BlackTalk onClick={handleSpeechBubbleClick2} direction="flex-end" textdirection="right">
              <BubbleText>{message2}</BubbleText>
            </BlackTalk>
          )}
          {showTalk3 && (
            <div>
              <BrownTalk height="42.5%">
                <BrownText onClick={handleSpeechBubbleClick3}>{message3}</BrownText>
              </BrownTalk>
              <BrownTalk height="30%">
                <BrownText onClick={handleSpeechBubbleClick4}>▶ 무시한다</BrownText>
              </BrownTalk>
            </div>
          )}
          <CharactersContainer>
            {isCharacterVisible3 && (
              <Character
                charactergif={characterGif2}
                style={{
                  left: `${mousePosition.x}px`,
                  top: `${mousePosition.y}px`,
                }}
              />
            )}
            <TalkCharacter characterGif={characterGif2} position="left" onClick={changeShowTalk2} />
          </CharactersContainer>
        </Background>
      </section>
      <section id="explain">
        <Background backgroundimage={weaponback} height={70}>
          <Woodback>
            <WoodText>
              새로운 전략적 게임 경험을 준비하세요! <br />
              <br /> TTD에서 당신은 단순한 지휘관이 아닙니다. <br />
              <br />
              지휘관이 되어 다양한 아이템과 포탑을 조합하고, <br /> <br />
              강력한 덱을 구성해보세요! <br /> <br />각 아이템 조합은 당신의 유닛을 더욱 강화하여 <br /> <br />
              적들을 압도할 수 있게 해줍니다. <br /> <br />
              강화된 포탑으로 전장을 지배하고, <br /> <br />
              마을을 지키세요!
            </WoodText>
          </Woodback>
          <CharactersContainer>
            {isCharacterVisible4 && (
              <Character
                charactergif={characterGif4}
                style={{
                  left: `${mousePosition.x}px`,
                  top: `${mousePosition.y}px`,
                }}
              />
            )}
            <TalkCharacter characterGif={characterGif4} position="left" onClick={changeShowTalk2} />
          </CharactersContainer>
        </Background>
      </section>
      <section id="download">
        <Background backgroundimage={starbackground} height={95}>
          <Starback>
            <StarText>
              <br /> <br />
              <br /> <br />
              별이 하나인 <br></br>
              <br></br>세 개의 유닛이 모여 <br></br>
              <br></br>두 별이 되었고 <br />
              <br /> 이 두 별이 또 세 개씩 모여 <br />
              <br />
              하나의 삼성을 만들었다. <br /> <br /> <br /> <br />- 제목: 5코 3성-<br></br>
            </StarText>
          </Starback>
          <Downloadback>
            <StarText1 onClick={DownloadforMobile}>
              모바일버전
              <br />
              <br /> 다운
            </StarText1>
          </Downloadback>
          <Downloadback2>
            <StarText2 onClick={DownloadforPC}>
              PC버전
              <br />
              <br /> 다운
            </StarText2>
          </Downloadback2>
          <CharactersContainer>
            {isCharacterVisible5 && (
              <Character
                charactergif={characterGif5}
                style={{
                  left: `${mousePosition.x}px`,
                  top: `${mousePosition.y}px`,
                }}
              />
            )}
          </CharactersContainer>
        </Background>
      </section>
    </>
  );
};

export default Main;
