import React from "react";
import { createRoot } from "react-dom/client";
import { HelloWorld } from "./HelloWorld";

createRoot(document.getElementById("App") as Element).render(<HelloWorld />);
