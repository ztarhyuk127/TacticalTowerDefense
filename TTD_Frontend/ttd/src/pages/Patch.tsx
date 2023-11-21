import React, { useEffect, useState } from "react";
import {Background} from './Main';
import backgroundImage from "../asset/image/patchback.png";
import styled from "styled-components";
import patchData from "../patchnotes/patchnotes.json"

interface BoxProps {
  width: string;
}

const Box = styled.div<BoxProps>`
  /* border: 1px solid white; */
  width: ${(props) => props.width};
  height: 35rem;
  display: flex;
  /* justify-content: center; */
  flex-direction: column;
  padding: 0.25rem;
  /* padding-top: 5rem; */
  margin-top: 5rem;
  margin-bottom: 1rem;
  overflow: scroll;

  &::-webkit-scrollbar {
    display: none;
  }

  &::-webkit-scrollbar-track {
    display: none;
  }

  &::-webkit-scrollbar-thumb {
    display: none;
  }
`

const PatchBtn = styled.button`
  width: 5rem;
  min-height: 60px;
  margin-bottom: 5px;
  background-color: #ffd500;
  border-radius: 15px;
  font-size: 16px;
`

const Patch = () => {
  const [versions, setVersions] = useState<any[]>([])
  const [contents, setContents] = useState([])
  let line = 1

  useEffect(() => {
    setVersions(patchData.patchs)
  }, [])

  const handleVersionView = (targetVersion:string) => {
    versions.forEach((version) => {
      if (version.version === targetVersion){
        setContents(version.content);
      }
    })
  }

  return (
    <Background 
    backgroundimage={backgroundImage} height={95} 
    style={{display: "flex", justifyContent: "center"}}>
      <div 
      style={{display: "flex", justifyContent: "center", alignItems: "center",
       width: "52rem", height: "38rem", color: "white", marginTop: "20px"}}>
        <Box width={"6rem"}>
          {
            versions.length !==0 && 
                  versions.map((version) => (
                     version.version[0] === "<" ?
                     <PatchBtn key={version.version.slice(1)} style={{backgroundColor: "white"}} 
                     onClick={() => handleVersionView(version.version)}
                     >
                     {version.version.slice(1)}</PatchBtn> 
                     :
                      <PatchBtn key={version.version} 
                      onClick={() => handleVersionView(version.version)}
                      >
                      {version.version}</PatchBtn>
                    
                  ))
          }
        </Box>
        <Box width={"46rem"} style={{justifyContent: "normal", backgroundColor: "#0000007e", padding: "1rem", paddingRight: "3rem"}}>
          {contents.length !==0 ? contents.map((content:string) => [
            (
              content[0] === "#"? <div key={line++} style={{fontSize: "36px", textAlign: "center"}}>{content.slice(1)}</div> :
              content[0] === ">"? <span key={line++} style={{textAlign: "end"}}>{content.slice(1)}</span> :
              content[0] === "!"? <div key={line++}>ㆍ{content.slice(1)}</div> :
              content !== "\n"?
              ( content[0] === "-"?
                <div key={line++} style={{fontSize: "20px", color: "yellow"}}>▶ {content.slice(1)}</div> : <div key={line++}>&nbsp;{content}</div>
              ) 
              : <br key={line++}></br>
            )
          ]): <div>왼쪽의 버전을 클릭해주세요.</div> }
        </Box>
      </div>
    </Background>
  );
};

export default Patch;
