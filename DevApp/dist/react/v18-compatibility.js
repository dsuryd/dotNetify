import dotnetify from "./index";
import { createRoot, hydrateRoot } from "react-dom/client";

// Override dotnetify functions that invoke older React APIs with React 18 APIs.
dotnetify.react.router.render = (component, container) => {
  const root = createRoot(container);
  root.render(component);
  return () => root.unmount();
};
dotnetify.react.router.hydrate = (component, container) => hydrateRoot(container, component);
