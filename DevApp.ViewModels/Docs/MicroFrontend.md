## Micro-Frontend

Micro-frontend is a technique that decomposes an otherwise monolithic web application into smaller, isolated, and independently deployable sub-applications.  Each sub-application provides a subset of the UI, which will then be stitched together by a light orchestrator to present a unified, cohesive front-end to its users.

While this technique does increase development complexity and brings its own challenges, many large organizations find merits to this approach, as it allows them to better scale their development and deployment process across autonomous, geographically-dispersed teams.

There are several ways to implement micro-frontend. The one dotNetify offers is through an orchestration on the front-end.  Each sub-application is a self-contained dotNetify/ASP.NET Core application with its own build process and deployment, and can run and fulfill its functionality by itself.  

There will be a portal application that does the light orchestration: dynamically load the UI scripts from the running sub-applications, instantiate their root UI components and compose them as a single-page application with client-side routing.  
