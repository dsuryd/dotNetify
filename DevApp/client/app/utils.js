export const createEventEmitter = _ => {
   let subscribers = [];
   return {
      emit(...args) {
         subscribers.forEach(subscriber => subscriber(...args));
      },

      subscribe(subscriber) {
         !subscribers.includes(subscriber) && subscribers.push(subscriber);
         return () => (subscribers = subscribers.filter(x => x !== subscriber));
      }
   };
};
