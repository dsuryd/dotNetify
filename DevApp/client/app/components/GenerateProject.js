import React, { useRef, useState, useLayoutEffect, Fragment } from "react";
import { Alert, Button, Modal, Panel, TextField } from "dotnetify-elements";
import SpinnerIcon from "./SpinnerIcon";
import styled from "styled-components";

export const GenerateProject = ({ caption, title, sourceUrl, sourceDir, useAnchor }) => {
  const [show, setShow] = useState(false);
  const [projectName, setProjectName] = useState();
  const [loading, setLoading] = useState();
  const textRef = useRef();

  caption = caption || "Generate";

  useLayoutEffect(() => {
    // Set focus to project name text field.
    if (textRef.current && textRef.current.inputRef) {
      textRef.current.inputRef.current.focus();
      textRef.current.inputRef.current.select();
    }
  });

  const generate = async () => {
    const data = {
      templateSourceType: "git",
      templateSourceUrl: sourceUrl,
      templateSourceDirectory: sourceDir,
      templateType: "dotnet",
      tags: { projectName: projectName || "MyProject", HelloWorld: projectName || "HelloWorld" }
    };

    const response = await fetch("/api/generator", {
      method: "post",
      body: JSON.stringify(data),
      headers: { "Content-Type": "application/json" }
    });
    if (!response.ok) {
      console.error(response);
      alert(
        "Oops! This is unexpected, the generator appears to be off-line.  You'll have to manually download the template from https://github.com/dsuryd/dotNetify-react-template.  Sorry about that!"
      );
      return;
    }
    return await response.blob();
  };

  const handleSubmit = async () => {
    setLoading(true);
    const zip = await generate();
    setLoading(false);
    setShow(false);

    const blob = new Blob([zip], { type: "application/zip" });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = projectName + ".zip";
    const clickHandler = () => {
      setTimeout(() => {
        URL.revokeObjectURL(url);
        a.removeEventListener("click", clickHandler);
      }, 150);
    };
    a.addEventListener("click", clickHandler, false);
    a.click();
  };

  const Anchor = styled.a`
    font-weight: 600;
    cursor: pointer;
    &:hover {
      opacity: 80%;
    }
  `;

  return (
    <Fragment>
      {useAnchor ? <Anchor onClick={() => setShow(true)}>{caption}</Anchor> : <Button onClick={() => setShow(true)}>{caption}</Button>}
      <Modal open={show} onSubmit={handleSubmit}>
        <header>{title || caption}</header>
        {loading && (
          <Alert warning>
            Generating the project, please wait...
            <SpinnerIcon style={{ marginLeft: "10px", transform: "scale(.2)" }} />
          </Alert>
        )}
        <TextField
          ref={textRef}
          horizontal
          id="ProjectName"
          label="Project name"
          css="div[class*=LabelContainer] { white-space: nowrap }"
          onDone={val => setProjectName(val)}
        />
        <footer>
          <Panel horizontal right>
            <Button label="Cancel" cancel secondary onClick={() => setShow(false)} />
            <Button id="Submit" label="Generate" submit onClick={handleSubmit} />
          </Panel>
        </footer>
      </Modal>
    </Fragment>
  );
};

export default GenerateProject;
