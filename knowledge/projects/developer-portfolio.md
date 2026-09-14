---
title: Developer Portfolio Website

organization: Personal Project

role: Frontend Developer

environment: production

period:
  from: 2022-10
  to: Present

status: active

technologies:
  - react
  - javascript
  - css
  - react-router
  - react-bootstrap
  - bootstrap
  - material-ui
  - react-scroll
  - react-lazyload
  - emailjs
  - recaptcha
  - jest
  - react-testing-library
  - github-pages

concepts:
  - frontend-architecture
  - component-design
  - responsive-design
  - user-experience
  - accessibility
  - testing
  - deployment
  - personal-branding
  - client-side-routing
  - lazy-loading

dependencies:
  - "@emailjs/browser"
  - "@emotion/react"
  - "@emotion/styled"
  - "@fontsource/roboto"
  - "@mui/icons-material"
  - "@mui/material"
  - "@testing-library/jest-dom"
  - "@testing-library/react"
  - bootstrap
  - react
  - react-bootstrap
  - react-dom
  - react-google-recaptcha
  - react-icons
  - react-lazyload
  - react-router-bootstrap
  - react-router-dom
  - react-scripts
  - react-scroll

links:
  github: https://github.com/inge1980/portfolio/
  live: https://inge1980.github.io/portfolio/

---

# Overview

A responsive developer portfolio website for presenting technical skills, projects, professional experience, and contact information.

The portfolio is built with React and provides dynamic navigation between portfolio sections and project-related content. The application combines reusable UI components, responsive layouts, and a structured presentation of technical work.

The project also includes automated frontend testing with Jest and React Testing Library and is deployed as a static website through GitHub Pages.

---

# Context

The portfolio was created as a personal platform for presenting software development experience and technical projects.

A portfolio also provides an opportunity to demonstrate frontend development practices through the application itself rather than only describing technical experience.

Important requirements included:

- Presenting projects and technical skills clearly.
- Providing dynamic navigation between portfolio sections.
- Supporting desktop and mobile screen sizes.
- Maintaining a consistent visual design.
- Providing a contact mechanism.
- Demonstrating frontend development practices through the portfolio itself.
- Deploying the application as a publicly accessible website.

---

# Task

My responsibility was designing and implementing the portfolio website from the frontend architecture through deployment.

I was responsible for:

- Designing the portfolio structure and user experience.
- Implementing the React application.
- Building reusable frontend components.
- Implementing dynamic navigation with React Router.
- Creating responsive layouts and UI components.
- Implementing project and skill presentation.
- Integrating the contact functionality.
- Adding automated frontend tests.
- Deploying the application through GitHub Pages.

The goal was to create a professional portfolio that could communicate technical experience while also demonstrating practical frontend engineering skills.

---

# Challenge

## Challenge: Structuring a Growing Portfolio Application

### Problem

A developer portfolio can quickly become difficult to maintain when projects, skills, experience, and other content are implemented directly inside individual UI components.

As the portfolio grows, tightly coupling content and presentation makes it harder to add projects, change navigation, or maintain a consistent user experience.

The application therefore needed a structure that allowed different portfolio sections to evolve without turning the frontend into a collection of tightly coupled components.

### Solution

The portfolio was structured as a React application with reusable components and client-side navigation.

React Router was used to provide dynamic navigation between the main portfolio sections and project-related content.

UI responsibilities were separated into reusable components so that common layout and presentation patterns could be maintained consistently.

Project content was separated from rendering logic through a structured data model (`ProjectsAll-in-oneData`), allowing categories, sections, and individual project pages to be rendered from shared data rather than hardcoded layouts.

Additional libraries were used where they provided specific UI or interaction functionality, including React-Bootstrap, Material UI, React Icons, React Scroll, and React Lazy Load.

### Result

The portfolio provides a consistent navigation experience while remaining maintainable as additional projects and content are added.

The component-based structure also keeps presentation logic separated into reusable frontend building blocks.

---

## Challenge: Responsive Portfolio Experience

### Problem

The portfolio needs to communicate technical information clearly across different screen sizes.

Project descriptions can contain substantially more information than typical marketing content, which makes responsive layout and information hierarchy important. A layout optimized only for desktop would provide a poor experience for users accessing the portfolio from mobile devices.

### Solution

The frontend was implemented using responsive UI components and layout patterns.

React-Bootstrap and Material UI were used for UI and layout components, while the portfolio's UI structure and content presentation were adapted around the information being presented.

The design prioritizes:

- Clear section hierarchy.
- Readable project information.
- Responsive layouts.
- Consistent navigation.
- Usable interaction patterns across screen sizes.

### Result

The portfolio provides a responsive presentation of projects, technical skills, and experience across desktop and mobile screen sizes.

Project images are also lazy-loaded to reduce unnecessary initial rendering while maintaining a smooth browsing experience.

---

## Challenge: Maintaining Frontend Quality Through Automated Testing

### Problem

Changes to navigation, reusable components, and portfolio content can unintentionally break existing UI behavior.

Because the portfolio itself is an actively developed application, manual verification alone would make regressions harder to detect as the codebase grows.

### Solution

Automated frontend tests were introduced using Jest and React Testing Library.

Tests focus on component behavior and rendered UI rather than implementation details.

React Testing Library allows components to be tested from the perspective of how users interact with and observe the application, while Jest provides the test execution and assertion infrastructure.

The test suite includes rendering tests, contact form interaction tests, mocked EmailJS integration, mocked reCAPTCHA behavior, and validation of both successful and failed submission flows.

### Result

The application has an automated test foundation that makes it easier to detect regressions when frontend components and navigation are changed.

---

## Challenge: Integrating Contact Functionality Into a Static Application

### Problem

The portfolio is deployed as a static application, but it still needs to provide a way for visitors to contact the developer.

A static frontend cannot rely on its own backend for processing contact requests.

### Solution

The contact functionality was integrated using EmailJS, allowing the frontend application to submit contact information without requiring a dedicated backend application.

reCAPTCHA was also included as part of the contact workflow to reduce automated abuse of the contact functionality.

### Result

The portfolio provides a contact mechanism while remaining deployable as a static frontend application.

---

# Action

## Architecture

### Frontend

The application is a React-based single-page frontend.

The frontend is organized around reusable components and client-side navigation.

Major frontend responsibilities include:

- Portfolio navigation.
- Project presentation.
- Technical skill presentation.
- Experience and contact information.
- Responsive layout.
- Reusable UI components.
- Contact form integration.

React Router provides navigation between portfolio sections without requiring full page reloads.

The project overview is generated from a centralized data structure that organizes projects by category, employer, and period, while React Scroll provides smooth in-page navigation and React Lazy Load defers loading of project images until they approach the viewport.

React-Bootstrap and Material UI provide UI and layout components, while supporting libraries handle icons, scrolling, lazy loading, and other frontend functionality.

---

### Backend

The portfolio does not require a dedicated backend application.

Contact functionality is handled through EmailJS from the frontend, while reCAPTCHA provides an additional protection mechanism for the contact workflow.

The application is therefore primarily a client-side React application with external services used where backend-like functionality is required.

---

### Database

No database is used by the portfolio application.

Project, skill, experience, and portfolio content are maintained as part of the frontend project rather than being retrieved from a dedicated application database.

---

### Infrastructure

The portfolio is deployed as a static website through GitHub Pages.

The deployment model keeps the infrastructure simple and avoids the operational overhead of running a dedicated backend or server.

The live application is available at:

https://inge1980.github.io/portfolio/

---

## Technical Decisions

### Decision: React for the Portfolio Application

#### Context

The portfolio needed dynamic navigation, reusable UI components, and enough flexibility to demonstrate frontend engineering practices rather than functioning as a collection of static HTML pages.

#### Chosen Solution

React was used as the primary frontend framework.

The application is structured as reusable components with React Router providing client-side navigation.

#### Alternatives Considered

- Static HTML pages.
- A server-rendered application.

A static HTML implementation would have reduced technical complexity but would provide less opportunity for reusable component architecture and dynamic navigation.

#### Trade-offs

Advantages:

- Component-based architecture.
- Reusable UI patterns.
- Dynamic navigation.
- Strong ecosystem.
- Suitable for demonstrating frontend development skills.

Disadvantages:

- More complexity than a purely static HTML website.
- Requires a JavaScript runtime in the browser.
- Introduces a build and deployment process.

---

### Decision: Client-Side Routing

#### Context

The portfolio contains multiple sections and project-oriented content that should be navigable without full page reloads.

#### Chosen Solution

React Router was used to manage client-side navigation.

This allows the application to treat different portfolio sections as navigable application views while maintaining the single-page application model.

#### Alternatives Considered

- Separate static HTML pages.
- Manual navigation using browser APIs.

React Router provides a more structured approach to navigation and route management.

#### Trade-offs

Advantages:

- Clear route structure.
- Reusable navigation logic.
- Smooth client-side transitions.
- Easier expansion of portfolio sections.

Disadvantages:

- Adds routing complexity to a relatively small application.
- Static hosting requires appropriate handling of client-side routes.

---

### Decision: Static Deployment Through GitHub Pages

#### Context

The portfolio primarily consists of frontend content and does not require a dedicated backend or database.

The deployment solution therefore needed to be simple and inexpensive while providing a publicly accessible website.

#### Chosen Solution

The React application is deployed as a static website through GitHub Pages.

#### Alternatives Considered

- Traditional web hosting.
- A dedicated application server.
- Cloud hosting with a backend runtime.

These alternatives would introduce infrastructure that is unnecessary for the current application requirements.

#### Trade-offs

Advantages:

- Simple deployment model.
- Low operational overhead.
- Suitable for static frontend applications.
- Integrated with the project's Git repository.

Disadvantages:

- Limited server-side functionality.
- External services are required for functionality such as contact handling.
- Client-side routing requires consideration when deploying to static hosting.

---

### Decision: Component-Based UI Architecture

#### Context

The portfolio contains repeated presentation patterns for projects, skills, navigation, and other content.

Implementing each section independently would increase duplication and make visual changes harder to maintain.

#### Chosen Solution

Reusable React components were used to separate presentation responsibilities and provide consistent UI patterns across the application.

A centralized project data structure was introduced so that navigation, project listings, and detail pages could reuse the same content model instead of duplicating information across components.

#### Alternatives Considered

- Large page-specific components.
- Static HTML templates.

#### Trade-offs

Advantages:

- Reduced duplication.
- Easier maintenance.
- Consistent presentation.
- Easier extension of the portfolio.

Disadvantages:

- Requires additional component structure.
- Over-abstraction can make a small portfolio unnecessarily complex if components are split too aggressively.

---

## Implementation

### Features

Implemented functionality includes:

- Developer profile and portfolio presentation.
- Technical skills presentation.
- Project overview and project information.
- Dynamic client-side navigation.
- Smooth in-page navigation between project entries.
- Lazy-loaded project images.
- Responsive desktop and mobile layouts.
- Reusable React UI components.
- Contact functionality.
- reCAPTCHA integration.
- Static deployment through GitHub Pages.

### APIs

The application does not expose its own API.

External services are used for selected functionality:

- EmailJS for contact form processing.
- reCAPTCHA for contact form protection.

### Data and Persistence

The portfolio does not use a database or persistent application storage.

Portfolio content is maintained as part of the frontend project and presented through a centralized data structure that organizes projects by category, section, and individual project entries.

### Automation

The application is deployed through GitHub Pages as a static frontend.

No backend processing or scheduled background jobs are required by the application.

### Testing

Frontend testing is implemented using:

- Jest.
- React Testing Library.
- Testing Library Jest DOM.

Tests verify component rendering, contact form interaction, EmailJS integration through mocks, reCAPTCHA behavior through mocks, and successful and failed submission scenarios.

---

# Result

The project resulted in a publicly accessible developer portfolio that combines professional presentation with a practical demonstration of frontend engineering.

The portfolio provides:

- A structured presentation of technical skills and projects.
- Dynamic navigation between portfolio content.
- Responsive frontend layouts.
- Reusable React components.
- Automated frontend testing.
- Integrated contact functionality.
- Static deployment through GitHub Pages.

The portfolio is available publicly at:

https://inge1980.github.io/portfolio/

---

# Lessons Learned

## Lesson: A Portfolio Is Also a Software Product

Building a portfolio as a real application changed the focus from simply presenting information to designing an actual user experience.

Navigation, information hierarchy, responsive behavior, accessibility, and maintainability all affect how effectively technical work is communicated.

---

## Lesson: Component Reuse Should Follow Actual UI Patterns

Reusable components are useful when there is a genuine shared responsibility or presentation pattern.

The project reinforced that componentization should reduce duplication and improve consistency rather than creating abstractions simply for the sake of abstraction.

---

## Lesson: Testing Frontend Behavior Matters

Even a relatively small frontend application benefits from automated tests when components and navigation are continuously modified.

Testing user-visible behavior with React Testing Library provides more useful confidence than relying exclusively on implementation-specific tests.

---

## Lesson: Keep Infrastructure Proportional to the Application

The portfolio does not require a backend, database, or dedicated application server.

Using static hosting keeps the operational model simple while external services can provide the limited functionality that cannot be handled purely by static files.

This reinforced the principle that infrastructure should match actual application requirements rather than adding complexity prematurely.

---

# Future Improvements

- Improve automated test coverage around navigation and contact workflows.
- Add stronger accessibility testing.
- Introduce automated visual regression testing for important UI components.
- Improve portfolio content management so new projects require less manual frontend modification.
- Add automated deployment validation before publishing changes.
- Improve performance monitoring and Core Web Vitals tracking.
- Consider a content-driven architecture if the number of projects grows significantly.

---