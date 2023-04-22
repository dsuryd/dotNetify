import { CognitoIdentityClient } from "@aws-sdk/client-cognito-identity";
import { fromCognitoIdentityPool } from "@aws-sdk/credential-provider-cognito-identity";
import { TranscribeStreamingClient } from "@aws-sdk/client-transcribe-streaming";
import MicrophoneStream from "microphone-stream";
import { StartStreamTranscriptionCommand } from "@aws-sdk/client-transcribe-streaming";
import { Buffer } from "buffer";

const SAMPLE_RATE = 44100; //16000;
let microphoneStream = undefined;
let transcribeClient = undefined;

export const startRecording = async (language, callback) => {
  if (!language) {
    return false;
  }
  if (microphoneStream || transcribeClient) {
    stopRecording();
  }
  createTranscribeClient();
  createMicrophoneStream();
  await startStreaming(language, callback);
};

export const stopRecording = function () {
  if (microphoneStream) {
    microphoneStream.stop();
    microphoneStream.destroy();
    microphoneStream = undefined;
  }
  if (transcribeClient) {
    transcribeClient.destroy();
    transcribeClient = undefined;
  }
};

const createTranscribeClient = () => {
  transcribeClient = new TranscribeStreamingClient({
    region: process.env.AWS_REGION,
    credentials: fromCognitoIdentityPool({
      client: new CognitoIdentityClient({ region: process.env.AWS_REGION }),
      identityPoolId: process.env.AWS_COGNITO_IDENTITY_POOL_ID || "",
    }),
  });
};

const createMicrophoneStream = async () => {
  microphoneStream = new MicrophoneStream.default();
  microphoneStream.setStream(
    await window.navigator.mediaDevices.getUserMedia({
      video: false,
      audio: true,
    })
  );
};

const startStreaming = async (language, callback) => {
  let sampleRate = SAMPLE_RATE;
  const urlParams = new URLSearchParams(window.location.search);
  if (urlParams.get("sample_rate")) {
    sampleRate = urlParams.get("sample_rate");
  }

  const command = new StartStreamTranscriptionCommand({
    LanguageCode: language,
    MediaEncoding: "pcm",
    MediaSampleRateHertz: SAMPLE_RATE,
    AudioStream: getAudioStream(),
  });
  const data = await transcribeClient.send(command);

  for await (const event of data.TranscriptResultStream) {
    for (const result of event.TranscriptEvent.Transcript.Results || []) {
      if (result.IsPartial === false) {
        const noOfResults = result.Alternatives[0].Items.length;
        for (let i = 0; i < noOfResults; i++) {
          callback(result.Alternatives[0].Items[i].Content.trim());
        }
      }
    }
  }
};

const getAudioStream = async function* () {
  for await (const chunk of microphoneStream) {
    if (chunk.length <= SAMPLE_RATE) {
      yield {
        AudioEvent: {
          AudioChunk: encodePCMChunk(chunk),
        },
      };
    }
  }
};

const encodePCMChunk = (chunk) => {
  const input = MicrophoneStream.default.toRaw(chunk);
  let offset = 0;
  const buffer = new ArrayBuffer(input.length * 2);
  const view = new DataView(buffer);
  for (let i = 0; i < input.length; i++, offset += 2) {
    let s = Math.max(-1, Math.min(1, input[i]));
    view.setInt16(offset, s < 0 ? s * 0x8000 : s * 0x7fff, true);
  }
  return Buffer.from(buffer);
};


import * as TranscribeClient from "./transcribeClient.js";

const header = document.getElementById("header");
const recordBtn = document.getElementById("record");
const transcribedText = document.getElementById("transcribedText");
let prompt = "";
let debounceId = 0;

const startTranscribing = async () => {
  try {
    await TranscribeClient.startRecording("en-US", onTranscriptionDataReceived);
  } catch (error) {
    alert("An error occurred while recording: " + error.message);
    stopRecording();
  }
};

const stopTranscribing = () => TranscribeClient.stopRecording();

const startRecording = async () => {
  clearTranscription();
  header.setAttribute("class", "header record-active");
  recordBtn.innerHTML = `<i class="fas fa-microphone"></i><span>Stop</span>`;

  await startTranscribing();
};

const stopRecording = function () {
  header.setAttribute("class", "header record-inactive");
  recordBtn.innerHTML = `<i class="fas fa-microphone"></i><span>Start</span>`;
  stopTranscribing();
};

window.onRecordPress = () => {
  if (header.getAttribute("class") === "header record-inactive") {
    startRecording();
  } else {
    stopRecording();
  }
};

const clearTranscription = () => {
  transcribedText.innerHTML = "";
};

const onTranscriptionDataReceived = (data) => {
  const isSentence = [".", "?", "!"].some((x) => data === x);
  prompt += isSentence || data === "," ? data : " " + data;

  console.log(prompt);

  if (isSentence) {
    if (debounceId) clearTimeout(debounceId);
    debounceId = setTimeout(() => {
      if (prompt.length > 10 || /\b(hello|hi)\b/i.test(prompt)) {
        processPrompt(prompt);
      }
      prompt = "";
    }, 500);
  } else {
    if (debounceId) clearTimeout(debounceId);
    debounceId = setTimeout(() => {
      if (prompt.length > 10 || /\b(hello|hi)\b/i.test(prompt)) {
        processPrompt(prompt);
      }
      prompt = "";
    }, 2000);
  }
};

const processPrompt = (text) => {
  transcribedText.insertAdjacentHTML(
    "beforeend",
    `<div class="chat-message left">${text}</div><div class="lds-ellipsis"><div></div><div></div><div></div><div></div></div>`
  );
  transcribedText.lastElementChild.scrollIntoView({ behavior: "smooth" });

  // Don't transcribe while the bot is speaking.
  stopTranscribing();

  sendPrompt(text).then((data) => {
    const error = /\bERROR\b/i.test(data.text);

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

    const audio = playAudio(data.audio);
    if (audio) {
      audio.addEventListener("ended", () => startTranscribing());
    } else {
      startTranscribing();
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

const sendPrompt = (prompt) => {
  return fetch("/prompt", {
    method: "POST",
    headers: {
      "Content-Type": "application/text",
    },
    body: prompt,
  })
    .then((response) => response.json())
    .catch((error) => console.error(error));
};
