import React from "react";
import { Routes, Route } from "react-router-dom";
import Layout from "./pages/Layout";
import Main from "./pages/Main";
import Ranking from "./pages/Ranking";
import Patch from './pages/Patch'
import GlobalStyle from "./Styles/GlobalStyle";
import GlobalFont from "./Styles/GlobalFont";
import Tactics from "./pages/Tactics";

function App() {
  return (
    <>
      <GlobalFont />
      <GlobalStyle />
      <Routes>
        <Route path="/*" element={<Layout />}>
          <Route index element={<Main />} />
          <Route path="patch" element={<Patch />} />
          <Route path="tactics" element={<Tactics />} />
          <Route path="ranking" element={<Ranking />} />
        </Route>
      </Routes>
    </>
  );
}

export default App;
