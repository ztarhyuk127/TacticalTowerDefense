import React from "react";
import ReactDOM from "react-dom/client";
import App from "./App";
import { BrowserRouter } from "react-router-dom";
import GlobalStyle from "./Styles/GlobalStyle";

const root = ReactDOM.createRoot(document.getElementById("root") as HTMLElement);
const container = document.getElementById("root");
if (!container) throw new Error("Failed to find the root element");

root.render(
  <>
    <BrowserRouter>
      <App />
    </BrowserRouter>
  </>
);
