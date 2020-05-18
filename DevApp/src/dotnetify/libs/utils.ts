/* 
Copyright 2018-2020 Dicky Suryadi

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
 */

class Utils {
  // Trim slashes from start and end of string.
  trim(iStr: string) {
    if (typeof iStr !== "string") return "";

    while (iStr.indexOf("/", iStr.length - 1) >= 0) iStr = iStr.substr(0, iStr.length - 1);
    while (iStr.indexOf("/") == 0) iStr = iStr.substr(1, iStr.length - 1);
    return iStr;
  }

  // Match two strings case-insensitive.
  equal(iStr1: string, iStr2: string) {
    return iStr1 != null && iStr2 != null && iStr1.toLowerCase() == iStr2.toLowerCase();
  }

  // Whether the string starts or ends with a value.
  startsWith(iStr: string, iValue: string) {
    return iStr.toLowerCase().slice(0, iValue.length) == iValue.toLowerCase();
  }

  endsWith(iStr: string, iValue: string) {
    return iValue == "" || iStr.toLowerCase().slice(-iValue.length) == iValue.toLowerCase();
  }

  // Dispatch event with IE polyfill.
  dispatchEvent(iEvent: string) {
    if (typeof Event === "function") dispatchEvent(new Event(iEvent));
    else {
      var event = document.createEvent("CustomEvent");
      event.initEvent(iEvent, true, true);
      dispatchEvent(event);
    }
  }

  grep(iArray: any[], iFilter: any) {
    return Array.isArray(iArray) ? iArray.filter(iFilter) : [];
  }
}

export interface IEventEmitter {
  emit: (...args: any) => void;
  subscribe: (subscriber: any) => () => void;
}

export const createEventEmitter = () => {
  let subscribers = [];
  return {
    emit(...args) {
      let handled = false;
      subscribers.forEach((subscriber) => {
        handled = subscriber(...args) || handled;
      });
      return handled;
    },

    subscribe(subscriber) {
      !subscribers.includes(subscriber) && subscribers.push(subscriber);
      return () => (subscribers = subscribers.filter((x) => x !== subscriber));
    },
  } as IEventEmitter;
};

export const fetch = (iMethod: string, iUrl: string, iData: string, iOptions: (request: XMLHttpRequest) => void) => {
  return new Promise((resolve, reject) => {
    let request = new XMLHttpRequest();
    request.open(iMethod, iUrl, true);
    if (typeof iOptions == "function") iOptions(request);

    request.onload = function() {
      if (request.status >= 200 && request.status < 400) {
        var response = request.responseText;
        resolve(response);
      } else reject(request);
    };
    request.onerror = function() {
      reject(request);
    };
    request.send(iData);
  });
};

export default new Utils();
