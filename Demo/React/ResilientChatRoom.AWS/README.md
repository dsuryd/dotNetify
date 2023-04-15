## DotNetify - Resilient Chat Room on AWS

[Click here to download this project's source code](https://tinyurl.com/chatroomdemo).

This project demonstrates integration with Amazon API Gateway and the usage of Resiliency addon to allow the dotNetify app server to restore the view model instance state and resume communication with connected clients when the server is stopped and restarted.

To run this project, you will need:
- An AWS account and create your own Amazon WebSocket API as detailed [here](https://github.com/dsuryd/dotNetify/blob/master/DevApp.ViewModels/Docs/AWSIntegration.md).
- A Redis server to enable server resiliency as detailed [here](https://github.com/dsuryd/dotNetify/blob/master/DevApp.ViewModels/Docs/Premium/DotNetifyResiliencyAddon.md).
