import React, { useState, useEffect } from "react";
import SpinnerIcon from "../components/SpinnerIcon";

export const ProjectGenerator = () => {
  const urlParams = new URLSearchParams(window.location.search);

  const projectName = urlParams.get("project") || "MyProject";
  const sourceUrl = urlParams.get("url");
  const sourceDir = urlParams.get("dir");
  const [loading, setLoading] = useState();

  const gitUrl = `${sourceUrl}/tree/master/${sourceDir}`;

  const generate = async () => {
    const data = {
      templateSourceType: "git",
      templateSourceUrl: sourceUrl,
      templateSourceDirectory: sourceDir,
      templateType: "dotnet",
      tags: { projectName }
    };

    const response = await fetch("/api/generator", {
      method: "post",
      body: JSON.stringify(data),
      headers: { "Content-Type": "application/json" }
    });
    if (!response.ok) {
      console.error(response);
      alert(
        `Oops! This is unexpected, the generator appears to be off-line.  You'll have to manually download the template from ${gitUrl}.  Sorry about that!`
      );
      throw "[Error] unexpected";
    }
    return await response.blob();
  };

  const handleSubmit = async () => {
    if (!sourceUrl || !sourceDir) throw "[Error] Query string required: ?url=<git repo url>&dir=><git repo directory>";

    setLoading(true);
    const zip = await generate();
    setLoading(false);

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

  useEffect(() => {
    handleSubmit();
  }, []);

  return (
    <div style={{ padding: ".5rem" }}>
      {loading === true ? (
        <React.Fragment>
          <span>Generating project, please wait...</span>
          <SpinnerIcon style={{ marginLeft: "10px", transform: "scale(.2)" }} />{" "}
        </React.Fragment>
      ) : loading === false ? (
        <span>Project generated. You may close this window.</span>
      ) : (
        <span />
      )}
    </div>
  );
};

export default ProjectGenerator;
