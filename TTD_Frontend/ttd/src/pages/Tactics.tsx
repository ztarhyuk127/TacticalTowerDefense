import React from "react";
import { Background } from "./Main";
import backgroundImage from "../asset/image/rankingback.png";

const Tactics = () => {
  return (
    <Background backgroundimage={backgroundImage} height={95}>
      <p style={{color: "white", textAlign: "center", padding: "5px"}}>준비중입니다.</p>
    </Background>
  )
};

export default Tactics;
