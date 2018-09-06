import * as utils from "../utils";

export const frameworkSelectEvent = utils.createEventEmitter();
export let currentFramework = window.localStorage["framework"] || "React";

frameworkSelectEvent.subscribe(framework => {
  currentFramework = framework; 
  window.localStorage["framework"] = currentFramework;
});