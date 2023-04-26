const header = document.getElementById("header");
const recordBtn = document.getElementById("record");
const transcribedText = document.getElementById("transcribedText");
let text = "";

window.onRecordPress = () => {
  if (header.getAttribute("class") === "header record-inactive") {
    startRecording();
  } else {
    stopRecording();
  }
};

// Call ahead as getVoices is async so it will be ready when we need it.
if (speechSynthesis in window) speechSynthesis.getVoices();

const recognition = new webkitSpeechRecognition();
recognition.lang = "en-US";
recognition.continuous = true;
recognition.maxDuration = 10;

recognition.onresult = (event) => {
  text = event.results[0][0].transcript;
  onTranscriptionDataReceived(text);
};
recognition.onend = (event) => {
  if (!text) stopRecording();
  text = "";
};

const startRecording = () => {
  clearTranscription();
  header.setAttribute("class", "header record-active");
  recordBtn.innerHTML = `<i class="fas fa-microphone"></i><span>Stop</span>`;

  recognition.start();
};

const stopRecording = () => {
  header.setAttribute("class", "header record-inactive");
  recordBtn.innerHTML = `<i class="fas fa-microphone"></i><span>Start</span>`;

  recognition.stop();
  speechSynthesis.cancel();
};

const clearTranscription = () => {
  transcribedText.innerHTML = "";
};

const onTranscriptionDataReceived = (text) => {
  console.log(text);
  transcribedText.insertAdjacentHTML(
    "beforeend",
    `<div class="chat-message left">${text}</div><div class="lds-ellipsis"><div></div><div></div><div></div><div></div></div>`
  );
  transcribedText.lastElementChild.scrollIntoView({ behavior: "smooth" });

  recognition.stop();

  sendPrompt(text).then((data) => {
    const error = /\bERROR:\b/i.test(data.text);

    transcribedText.removeChild(transcribedText.lastElementChild);
    transcribedText.insertAdjacentHTML(
      "beforeend",
      `<div class="chat-message right ${error ? "error" : ""}">${
        data.text
      }</div>`
    );
    transcribedText.lastElementChild.scrollIntoView({
      behavior: "smooth",
    });

    if (error) {
      recognition.start();
      return;
    }

    const audio = playAudio(data.audio);
    if (audio) {
      audio.addEventListener("ended", () => recognition.start());
    } else {
      const speech = new SpeechSynthesisUtterance(data.text);
      speech.voice = speechSynthesis.getVoices()[110];
      speech.onend = () => recognition.start();
      speechSynthesis.speak(speech);
    }
  });
};

const playAudio = (base64Data) => {
  if (!base64Data) return;

  const decoder = new TextDecoder();
  const audioBlob = new Blob([base64ToArrayBuffer(base64Data)], {
    type: "audio/mp3",
  });
  const audioUrl = URL.createObjectURL(audioBlob);
  const audio = new Audio(audioUrl);
  audio.addEventListener("canplaythrough", () => audio.play());
  return audio;
};

const base64ToArrayBuffer = (base64) => {
  var binaryString = window.atob(base64);
  var binaryLen = binaryString.length;
  var bytes = new Uint8Array(binaryLen);
  for (var i = 0; i < binaryLen; i++) {
    var ascii = binaryString.charCodeAt(i);
    bytes[i] = ascii;
  }
  return bytes;
};

const sendPrompt = async (prompt) => {
  try {
    const response = await fetch("/prompt", {
      method: "POST",
      headers: {
        "Content-Type": "application/text",
      },
      body: prompt,
    });
    return await response.json();
  } catch (error) {
    return console.error(error);
  }
};
