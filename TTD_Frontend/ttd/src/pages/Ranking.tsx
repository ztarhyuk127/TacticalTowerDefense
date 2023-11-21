import React, { useEffect, useState } from "react";
import styled from "styled-components";
import backgroundImage from "../asset/image/rankingbackground.png";
import characterGif from "../asset/image/unit2.gif";
import { Background, CharactersContainer, Character, Position } from "./Main";
import database from "../apis/firebase";
import { ref, child, get, DataSnapshot } from "firebase/database";
import rankingbutton from "../asset/image/rankingbutton.png";
interface userData {
  Nickname: string;
  Email: string;
  Difficulty: {
    Easy: { TimeScore: number };
    Normal: { TimeScore: number };
    Hard: { TimeScore: number };
    Infinite: {
      StageScore: number;
      TimeScore: number;
    };
  };
}

interface scoreData {
  nickname: string;
  score: number;
}

interface rankingData {
  Easy: scoreData[];
  Normal: scoreData[];
  Hard: scoreData[];
  Infinite: scoreData[];
}

const Container = styled.div`
  display: flex;
  justify-content: space-evenly;
  flex-wrap: wrap;
  padding: 5vh 3vh;
  margin: 0px 20vh 0px 20vh;

  /* 모든 브라우저에서 스크롤바 숨김 */
  &::-webkit-scrollbar {
    display: none;
  }

  /* 모든 브라우저에서 스크롤바 트랙 숨김 */
  &::-webkit-scrollbar-track {
    display: none;
  }

  /* 모든 브라우저에서 스크롤바 쓰기 영역 숨김 */
  &::-webkit-scrollbar-thumb {
    display: none;
  }
`;
interface rankinBtnProps {
  color: string;
}

const RankingBtn = styled.button<rankinBtnProps>`
  background-image: url(${rankingbutton});
  background-color: transparent;
  background-size: cover;
  background-position: center;
  color: DarkGray;
  font-family: "font_test", sans-serif;
  font-size: 2vh;
  font-weight: bold;
  padding: 1vh 2vh;
  width: 14vh;
  border: none;
  height: 14vh;

  span {
    color: ${(props) => props.color || "DarkGray"};
    display: block;
    margin-top: 1vh;
  }
`;

const RankingComponent = styled.div`
  background-color: rgba(0, 0, 0, 0.6);
  color: white;
  width: 50rem;
  height: 100px;
  margin-bottom: 10px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-left: 10px;
  padding-right: 10px;
  font-size: 2.5vh;
  text-shadow: black 3px 3px;
`;

const Ranking = () => {
  const [mousePosition, setMousePosition] = useState<Position>({ x: 0, y: 0 });
  const firebase = ref(database);
  const [rankingData, setRankingData] = useState<rankingData>({
    Easy: [],
    Normal: [],
    Hard: [],
    Infinite: [],
  });
  const [showData, setShowData] = useState<scoreData[]>([]);
  let cnt = 1;

  useEffect(() => {
    const handleMouseMove = (e: MouseEvent) => {
      setMousePosition({
        x: e.clientX - 100,
        y: e.clientY - 100,
      });
    };

    window.addEventListener("mousemove", handleMouseMove);
    getRankingInfo();
    return () => {
      window.removeEventListener("mousemove", handleMouseMove);
    };
  }, []);
  useEffect(() => {
    const handleMouseMove = (e: MouseEvent) => {
      setMousePosition({
        x: e.clientX - 100,
        y: e.clientY - 100,
      });
    };

    window.addEventListener("mousemove", handleMouseMove);
    getRankingInfo();
    handleClickInfo("Easy");
    return () => {
      window.removeEventListener("mousemove", handleMouseMove);
    };
  }, []);

  const getRankingInfo = () => {
    get(child(firebase, "/ClearData"))
      .then((snapshot) => {
        if (snapshot.exists()) {
          let newRankingData: rankingData = {
            Easy: [],
            Normal: [],
            Hard: [],
            Infinite: [],
          };
          snapshot.forEach((data: DataSnapshot) => {
            const userdata: userData = data.val();
            const userNickname = data.key || "";
            if (userdata.Difficulty.Easy.TimeScore > 0) {
              newRankingData.Easy.push({
                nickname: userNickname,
                score: userdata.Difficulty.Easy.TimeScore,
              });
            }
            if (userdata.Difficulty.Normal.TimeScore > 0) {
              newRankingData.Normal.push({
                nickname: userNickname,
                score: userdata.Difficulty.Normal.TimeScore,
              });
            }
            if (userdata.Difficulty.Hard.TimeScore > 0) {
              newRankingData.Hard.push({
                nickname: userNickname,
                score: userdata.Difficulty.Hard.TimeScore,
              });
            }
            if (userdata.Difficulty.Infinite.StageScore > 0) {
              newRankingData.Infinite.push({
                nickname: userNickname,
                score: userdata.Difficulty.Infinite.StageScore,
              });
            }
          });
          newRankingData.Easy.sort((a, b) => a.score - b.score);
          newRankingData.Normal.sort((a, b) => a.score - b.score);
          newRankingData.Hard.sort((a, b) => a.score - b.score);
          newRankingData.Infinite.sort((a, b) => b.score - a.score);
          setRankingData(newRankingData);
          setShowData(newRankingData.Easy);
        }
      })
      .catch((error) => {
        console.error(error);
      });
  };
  const [buttonColor, setButtonColor] = useState({
    Easy: "DarkGray",
    Normal: "DarkGray",
    Hard: "DarkGray",
    Infinity: "DarkGray",
  });
  const handleClickInfo = (RankName: string) => {
    setButtonColor({
      Easy: "DarkGray",
      Normal: "DarkGray",
      Hard: "DarkGray",
      Infinity: "DarkGray",
      [RankName]: "white",
    });
    if (RankName === "Easy") {
      setShowData(rankingData.Easy);
    } else if (RankName === "Normal") {
      setShowData(rankingData.Normal);
    } else if (RankName === "Hard") {
      setShowData(rankingData.Hard);
    } else if (RankName === "Infinity") {
      setShowData(rankingData.Infinite);
    }
  };

  return (
    <Background backgroundimage={backgroundImage} height={95}>
      <CharactersContainer>
        <Character charactergif={characterGif} style={{ top: `${mousePosition.y}px`, left: `${mousePosition.x}px` }} />
      </CharactersContainer>
      <Container>
        <RankingBtn color={buttonColor.Easy} onClick={() => handleClickInfo("Easy")}>
          <span>쉬움</span>
        </RankingBtn>
        <RankingBtn color={buttonColor.Normal} onClick={() => handleClickInfo("Normal")}>
          <span>보통</span>
        </RankingBtn>
        <RankingBtn color={buttonColor.Hard} onClick={() => handleClickInfo("Hard")}>
          <span>어려움</span>
        </RankingBtn>
        <RankingBtn color={buttonColor.Infinity} onClick={() => handleClickInfo("Infinity")}>
          <span>무한</span>
        </RankingBtn>
      </Container>
      <Container style={{ height: "50vh", overflowY: "auto" }}>
        {showData.length !== 0 &&
          showData.map((userdata: scoreData) => (
            <RankingComponent key={userdata.nickname}>
              <div style={{ marginLeft: "3vh" }}>{cnt++}</div>
              <div>{userdata.nickname}</div>
              <div style={{ marginRight: "3vh" }}>
                {userdata.score > 1000
                  ? `${Math.floor(userdata.score / 6000)
                      .toString()
                      .padStart(2, "0")}:${(Math.floor(userdata.score / 100) % 60).toString().padStart(2, "0")}:${(
                      userdata.score % 100
                    )
                      .toString()
                      .padStart(2, "0")}`
                  : userdata.score}
              </div>
            </RankingComponent>
          ))}
      </Container>
    </Background>
  );
};

export default Ranking;
