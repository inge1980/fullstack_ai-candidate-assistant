---
title: Responsive Bootstrap Modernization of Large-Scale School CMS

organization: Moava AS

role: Fullstack Developer

environment: production

period:
  from: 2013-11
  to: 2014-11

status: completed

technologies:
  - php
  - mysql
  - javascript
  - bootstrap-3
  - css
  - html

concepts:
  - frontend-modernization
  - legacy-modernization
  - responsive-design
  - mobile-first
  - bootstrap-grid
  - cms
  - saas
  - multi-tenant
  - backwards-compatibility
  - progressive-rollout
  - feature-flag
  - legacy-browser-support
  - data-migration
  - algorithm-design
  - user-experience
  - accessibility
  - universal-design
  - technical-debt
  - customer-retention
  - customer-satisfaction

links:
  github:
  live:

---

# Overview

Modernized the customer-facing frontend of Moava AS's school CMS from a fixed-width, desktop-oriented architecture to a responsive Bootstrap 3-based system.

Moava operated a SaaS CMS used by approximately 1,300 schools in Norway. Teachers and principals managed their own public school websites through the CMS, using more than 20 reusable modules such as navigation, news, articles, images, galleries, video, forms, tables, calendars, search, and content blocks.

The modernization was therefore not a redesign of a single website. It was a change to the reusable rendering architecture responsible for generating thousands of customer websites and dynamically generated pages.

The existing frontend had accumulated significant technical debt. It relied on fixed-width layouts, HTML tables for layout, pixel dimensions, custom CSS, inline styling, and module-specific assumptions that were not suitable for responsive web development.

The modernization introduced Bootstrap 3, responsive page scaffolding, mobile navigation, responsive modules, responsive images and tables, mobile column prioritization, touch-friendly interaction, and improved accessibility and usability.

A key requirement was to keep existing customer content unchanged. The old and new frontend therefore had to operate simultaneously against the same underlying customer data.

I implemented a per-school frontend-mode switch controlled through a restricted administration interface. This allowed the team to roll out the new frontend gradually, test it with individual schools, and immediately revert a school to the legacy frontend if required.

Another significant part of the project was converting approximately 1,000 existing customer column configurations. These configurations had accumulated over years and contained percentages, pixel values, mixed units, incomplete configurations, and technically invalid combinations. I analyzed the existing data, identified six recurring patterns, and implemented an algorithm to convert the historical configurations into Bootstrap-compatible 12-column layouts.

The modernization was introduced through pilot customers and a progressive rollout. Approximately three months after the new frontend was introduced, all schools had transitioned to the new design.

---

# Context

Moava AS provided a SaaS CMS used by approximately 1,300 schools in Norway.

Schools had substantial autonomy over their public websites. Teachers and principals could create content and configure pages themselves using reusable CMS modules.

The platform contained more than 20 modules, including:

- Navigation and menus
- News
- Articles
- Article lists
- Images
- Image galleries
- Video
- Slideshows
- Employees
- Email lists
- Forms
- Tables
- Calendars
- Search
- Content blocks

The public websites were generated dynamically from customer-specific content and configuration.

The existing frontend had been developed over many years and had accumulated significant technical debt. Its layout model was based primarily on:

- Fixed-width desktop layouts
- HTML tables used for layout
- Fixed pixel dimensions
- Custom CSS
- Inline CSS
- A fixed central content area
- Module-specific layout assumptions
- Limited responsive behavior
- Limited consideration for touch interaction
- Limited consideration for modern accessibility and universal-design requirements

Mobile access to school information such as schedules and weekly plans was becoming increasingly important. The platform therefore needed to support a much wider range of screen sizes and interaction methods without forcing customers to rebuild their existing websites.

The scale of the platform made a page-by-page migration impractical.

With approximately 1,300 schools and a conservative baseline of 30 generated pages per school, the platform represented at least approximately 39,000 dynamically generated pages. The actual number was higher because schools could create additional pages and combine modules in different configurations.

The practical migration boundary was therefore the reusable rendering architecture and CMS modules that generated those pages.

---

# Task

I took primary responsibility for the implementation of the responsive frontend modernization as part of the development team.

I was the product owner for the project and was responsible for driving the frontend modernization across the CMS while collaborating with the other developers when additional input or assistance was required.

My responsibility was to transform the existing presentation layer into a responsive system without requiring customers to recreate their content.

The main objectives were:

- Replace the fixed-width frontend with a responsive layout system.
- Modernize the reusable CMS modules.
- Preserve existing customer content and configuration where possible.
- Support old and new frontend versions simultaneously during migration.
- Provide a controlled rollout mechanism.
- Maintain compatibility with important legacy browsers.
- Improve mobile usability.
- Improve accessibility and usability.
- Convert incompatible legacy column configurations into the new grid model.
- Provide administrators with a safer way to configure responsive columns.

The project required changes across the frontend rendering layer, PHP presentation architecture, CMS modules, customer configuration, legacy-data conversion, and rollout process.

---

# Challenge

## Challenge: Modernizing a Large CMS Instead of a Single Website

### Problem

The project involved approximately 1,300 schools using a shared CMS platform.

Each school could combine more than 20 reusable modules to construct its public website. Those modules generated HTML, CSS, and JavaScript based on customer-specific content and configuration.

The existing frontend had been built around desktop-oriented assumptions, including fixed-width layouts, tables, pixel dimensions, custom CSS, and module-specific layout logic.

Simply adding responsive CSS to the outer page container would not have solved the problem because many individual modules generated markup and behavior that was themselves not responsive.

### Constraints

The modernization had to:

- Support approximately 1,300 existing schools.
- Preserve existing customer content.
- Avoid rebuilding individual pages.
- Support more than 20 reusable CMS modules.
- Allow old and new frontend versions to operate simultaneously.
- Allow individual schools to migrate independently.
- Provide an immediate rollback mechanism.
- Support desktop and mobile devices.
- Continue supporting important legacy browsers.
- Avoid creating a large spike in customer support issues.

### Solution

I treated the reusable rendering layer as the migration boundary.

Instead of migrating individual pages, I modernized the page scaffolding and the reusable modules responsible for generating those pages.

The underlying customer content remained unchanged.

The application was structured so that shared application logic could be reused while the presentation layer could generate either the legacy or responsive frontend.

A per-school frontend-mode setting determined which rendering mode was used.

### Result

The platform could modernize thousands of dynamically generated pages without individually rebuilding them.

The same customer content could be rendered using either frontend version, making the migration feasible at platform scale.

---

## Challenge: Making More Than 20 CMS Modules Responsive

### Problem

Each CMS module had its own HTML structure, CSS, layout assumptions, and sometimes JavaScript behavior.

The modules included navigation, news, articles, images, galleries, video, slideshows, forms, tables, calendars, search, and other content types.

A responsive page framework alone was therefore insufficient.

### Solution

I systematically reviewed and updated the 20+ CMS modules so their generated output worked within the new Bootstrap 3-based frontend.

The work included:

- Reworking module HTML structures.
- Updating CSS.
- Adjusting JavaScript behavior where required.
- Adapting module layouts to different viewport widths.
- Collapsing navigation on smaller screens.
- Implementing mobile navigation.
- Supporting browser back-button behavior for mobile navigation.
- Stacking columns on smaller screens.
- Introducing configurable mobile column priority.
- Making images responsive.
- Adapting tables for narrow screens.
- Adjusting typography.
- Redesigning headers and banners.
- Improving touch interaction.
- Adding responsive behavior to forms and controls.
- Adding legacy CSS fallbacks.

### Result

The modules became reusable responsive building blocks.

Because the public websites were generated from these modules, modernizing the modules automatically improved the responsive behavior of the large number of pages using them.

---

## Challenge: Converting Inconsistent Legacy Column Configurations

### Problem

Customers had historically configured page columns using free-form text fields.

The system accepted values such as:

- 30%
- 30% / 30%
- 20% / 1024 / 20%
- 100px / 400px / 400px / 100px
- 30% / 800px / 30%

The values could contain percentages, pixels, mixed units, incomplete configurations, and combinations that did not add up to 100%.

Some technically invalid configurations nevertheless appeared to work because of browser behavior, CSS quirks, or assumptions in the old rendering system.

Bootstrap 3 required a much more structured 12-column grid.

The legacy configurations therefore could not simply be copied into the new system.

### Solution

I analyzed approximately 1,000 existing customer configurations to determine how the configuration system had actually been used.

Rather than assuming the existing values followed a clean mathematical model, I looked for recurring patterns in the production data.

Six recurring patterns were identified.

The conversion algorithm considered factors such as:

- Number of columns.
- Percentage-based values.
- Pixel-based values.
- Relative numeric sizes.
- Relationships between multiple column values.
- Recurring combinations in the existing data.

The algorithm converted the historical configurations into Bootstrap-compatible grid widths while attempting to preserve the approximate visual proportions intended by the customer.

Where a configuration could not be reliably interpreted, a standard Bootstrap layout was used based on the number of columns.

The objective was to preserve the intended layout rather than reproduce technically invalid CSS literally.

### Result

Approximately 1,000 historical configurations could be converted automatically without manually correcting every customer configuration.

The new frontend no longer depended on the accidental browser and CSS behavior that had allowed many legacy configurations to work.

---

## Challenge: Preventing New Invalid Column Configurations

### Problem

The old administration interface allowed customers to enter arbitrary column-width values.

Continuing with free-form inputs would have allowed new configurations to violate the constraints of the Bootstrap grid.

### Solution

I replaced the free-form configuration approach with a custom multi-handle slider representing the Bootstrap 12-column grid.

The number of handles depended on the number of columns:

- One column: no handles and the slider was disabled.
- Two columns: one handle.
- Three columns: two handles.
- Four columns: three handles.

Handles snapped to the available Bootstrap grid positions.

This allowed administrators to configure column proportions visually while preventing configurations outside the supported grid model.

### Result

The administration interface became easier to use and prevented new invalid column configurations from being introduced through the normal UI.

The system intentionally traded arbitrary flexibility for predictable responsive layouts.

---

## Challenge: Running Old and New Frontends Simultaneously

### Problem

The entire customer base could not safely be migrated to the new frontend in a single operation.

At the same time, the old and new frontends needed to operate against the same customer content.

A school using the old frontend and a school using the new frontend had to coexist on the same platform.

### Solution

I implemented a per-school frontend-mode setting stored in the database.

The setting was exposed through the design section of the CMS administration interface, with access restricted to Moava employees.

The PHP rendering layer checked the setting and selected the corresponding frontend mode.

The old and new frontends therefore shared the same underlying customer content and application logic while using different presentation modes.

### Result

The team could migrate individual schools without affecting other customers.

A school could also be returned to the legacy frontend by changing the frontend-mode setting.

No content migration or page reconstruction was required.

---

## Challenge: Rolling Out the New Frontend Safely

### Problem

Enabling a new frontend across approximately 1,300 schools simultaneously would have created unnecessary operational risk.

Internal testing alone also could not reproduce every customer configuration.

### Solution

The rollout was performed progressively.

The new frontend was:

1. Tested locally.
2. Tested on remote test servers.
3. Tested with pilot customers.
4. Enabled for additional schools incrementally.
5. Supported alongside the old frontend during the transition.
6. Eventually enabled for the complete customer base.

The per-school frontend switch provided a practical rollback mechanism.

The team continued to maintain important fixes for the legacy frontend during the transition.

Moava also offered customers one hour of free telephone design consultation with a designer to help them adjust configuration and visual elements.

### Result

The rollout could be performed incrementally instead of as a high-risk big-bang migration.

Problems could be isolated to individual customers and corrected without affecting the entire platform.

Approximately three months after the new frontend was introduced, all schools had transitioned to the new design.

---

# Action

## Architecture

### Frontend

The frontend was rebuilt around Bootstrap 3 and responsive layout principles.

The fixed-width page scaffolding was replaced with a Bootstrap-based grid and responsive structure.

The 20+ CMS modules were updated to generate markup and behavior compatible with the new frontend.

The frontend supported:

- Bootstrap 3 grid layouts.
- Responsive navigation.
- Mobile navigation.
- Modal-style mobile interactions.
- Browser back-button behavior for mobile navigation.
- Responsive columns.
- Configurable mobile column priority.
- Responsive images.
- Responsive tables.
- Responsive typography.
- Responsive forms.
- Responsive module layouts.
- Responsive headers.
- Responsive banners.
- Touch-friendly controls.
- Legacy browser fallbacks.

The same customer content could be rendered using either the legacy or responsive frontend.

### Backend

The application was primarily PHP-based.

I refactored parts of the existing PHP rendering code toward a more object-oriented architecture.

The goal was to separate shared application logic from presentation-specific rendering so that the legacy and responsive frontends could share underlying functionality.

The PHP rendering layer read the per-school frontend-mode setting and selected the appropriate presentation mode.

### Database

Each school had its own database containing CMS content and configuration.

The modernization did not require duplicating or migrating the customer content.

A per-school frontend-mode variable controlled which presentation layer was used.

The same customer content could therefore be rendered through either frontend.

### Infrastructure

Development and testing were performed using local development environments and remote test servers.

The responsive frontend was tested internally before being introduced to pilot customers.

The production rollout was performed progressively so that individual schools could be migrated and, if necessary, reverted independently.

---

## Technical Decisions

### Decision: Bootstrap 3 as the Responsive Foundation

#### Context

The existing frontend lacked a consistent responsive grid and relied heavily on fixed-width and table-based layouts.

A common responsive layout system was required across many modules and customer configurations.

#### Chosen Solution

Bootstrap 3 was adopted as the foundation for the responsive frontend.

The page scaffolding and CMS modules were adapted to use Bootstrap's grid and responsive conventions.

#### Alternatives Considered

The existing custom CSS system could have been extended with additional responsive rules.

#### Trade-offs

Bootstrap provided a consistent grid and responsive model but imposed stricter layout constraints than the legacy system.

That required the existing column configurations to be normalized.

---

### Decision: Keep Customer Content Independent from Presentation

#### Context

Customers should not have to rebuild their websites because the presentation layer was being modernized.

The same content needed to work with both frontend versions.

#### Chosen Solution

The legacy and responsive frontends shared the same underlying customer data.

The PHP rendering layer selected the presentation mode using the per-school frontend setting.

#### Alternatives Considered

The team could have migrated customers to a new content model or created a separate version of the customer website.

#### Trade-offs

Maintaining two presentation modes temporarily increased implementation and maintenance complexity.

However, it avoided content migration, reduced rollout risk, and provided immediate rollback.

---

### Decision: Per-School Frontend Feature Switch

#### Context

A platform-wide frontend switch would have created a large blast radius.

The team needed to migrate schools independently.

#### Chosen Solution

A frontend-mode value was stored for each school.

Only authorized Moava employees could change the value through the administration interface.

The PHP rendering layer used the value to determine which frontend should be generated.

#### Alternatives Considered

A single global configuration could have switched the entire customer base at once.

#### Trade-offs

The per-school approach introduced additional configuration and required both rendering modes to remain functional during the transition.

In return, it provided controlled rollout and immediate rollback at customer level.

---

### Decision: Normalize Legacy Column Configurations

#### Context

The existing configuration system allowed arbitrary percentages, pixel values, mixed units, and invalid combinations.

Bootstrap required a 12-column grid.

#### Chosen Solution

Approximately 1,000 production configurations were analyzed.

Six recurring patterns were identified and used as the basis for an automated conversion algorithm.

The algorithm converted legacy values into Bootstrap-compatible widths while attempting to preserve their approximate visual proportions.

#### Alternatives Considered

Every customer configuration could have been manually reviewed and corrected.

A single generic mathematical conversion could also have been applied to all configurations.

#### Trade-offs

Some unusual configurations could not be reproduced perfectly.

However, analyzing actual production data produced a more reliable migration than either manual correction at scale or a generic conversion formula.

---

### Decision: Multi-Handle Bootstrap Grid Slider

#### Context

The old administration interface allowed arbitrary text input for column widths.

This flexibility had contributed to inconsistent historical configurations.

#### Chosen Solution

A custom multi-handle slider represented the Bootstrap 12-column grid.

The number of handles changed according to the number of columns:

- One column: no handles.
- Two columns: one handle.
- Three columns: two handles.
- Four columns: three handles.

Handles snapped to Bootstrap grid positions.

#### Alternatives Considered

The existing free-form inputs could have been retained with validation added.

#### Trade-offs

The slider was less flexible than arbitrary text input.

That was intentional. The new interface enforced the constraints of the responsive layout system and prevented customers from creating configurations the frontend could not reliably support.

---

### Decision: Progressive Customer Rollout

#### Context

Migrating approximately 1,300 schools simultaneously would have increased operational risk and potentially created a large support burden.

#### Chosen Solution

The new frontend was introduced through internal testing, remote test environments, pilot customers, and progressive customer migration.

The legacy frontend remained available during the transition.

#### Alternatives Considered

A single platform-wide migration could have been performed after internal testing.

#### Trade-offs

Maintaining two frontend modes temporarily increased maintenance overhead.

However, it reduced the impact of defects, enabled real-world feedback, and provided a practical rollback mechanism.

---

## Implementation

The implementation covered the frontend rendering architecture, PHP presentation layer, page scaffolding, more than 20 CMS modules, responsive behavior, legacy browser compatibility, customer configuration, legacy-data conversion, and rollout.

Key implementation work included:

- Rebuilding the page scaffolding around Bootstrap 3.
- Converting more than 20 CMS modules to responsive layouts.
- Refactoring PHP rendering code toward a more object-oriented structure.
- Separating shared application logic from frontend-specific presentation.
- Supporting simultaneous legacy and responsive rendering modes.
- Adding the per-school frontend-mode database setting.
- Restricting the frontend switch to authorized Moava employees.
- Implementing responsive navigation.
- Implementing mobile navigation using modal-style interaction.
- Supporting browser back-button behavior for mobile navigation.
- Making columns stack appropriately on smaller screens.
- Implementing configurable mobile column priority.
- Using the middle/main column as the default mobile priority.
- Making images responsive.
- Adapting tables for smaller screens.
- Adjusting typography for responsive layouts.
- Updating headers and banners.
- Improving touch interaction.
- Adding legacy CSS fallbacks.
- Supporting Internet Explorer and other important browsers of the period.
- Analyzing approximately 1,000 existing customer configurations.
- Identifying six recurring legacy column-width patterns.
- Implementing an algorithm for converting legacy values into Bootstrap grid widths.
- Designing a multi-handle custom grid slider.
- Dynamically changing the number of slider handles according to the number of columns.
- Snapping slider handles to Bootstrap's 12 grid positions.
- Preventing new invalid column configurations through the administration interface.
- Testing locally.
- Testing on remote test servers.
- Testing with pilot customers.
- Rolling the new frontend out progressively.
- Maintaining important legacy frontend fixes during the transition.
- Supporting customer configuration through design consultations.

---

# Result

The project transformed Moava's customer-facing CMS from a fixed-width, desktop-oriented frontend into a responsive Bootstrap 3-based system.

The main outcomes were:

- Approximately 1,300 schools were transitioned to the responsive frontend.
- More than 20 CMS modules were modernized.
- At least approximately 39,000 generated pages were covered by the responsive architecture based on the conservative baseline of 30 pages per school.
- Customer content did not need to be individually migrated or rebuilt.
- Existing customer databases remained usable.
- The old and new frontend modes could operate simultaneously.
- Individual schools could be switched between frontend modes.
- Individual schools could be reverted without content migration.
- The frontend could be rolled out progressively.
- Pilot customers could validate the new frontend before broader rollout.
- Responsive navigation was introduced.
- Mobile-specific navigation and interaction patterns were implemented.
- Columns could stack vertically on mobile.
- Customers could configure mobile column priority.
- Images became responsive.
- Tables were adapted for smaller screens.
- Typography and module layouts were adapted for different viewport sizes.
- Headers and banners were redesigned for the responsive frontend.
- Touch-friendly interactions were introduced.
- Older browsers, particularly Internet Explorer, remained supported through compatibility work and legacy CSS fallbacks.
- Approximately 1,000 existing column configurations were analyzed.
- Six recurring patterns were identified in the legacy configuration data.
- An automated conversion algorithm transformed inconsistent legacy configurations into Bootstrap-compatible layouts.
- The free-form column configuration interface was replaced with a constrained multi-handle grid slider.
- Slider handles snapped to Bootstrap's valid grid positions.
- New invalid column configurations could be prevented through the administration interface.
- The same underlying customer content could be used by both frontend versions.
- Only a small number of customers initially requested to revert to the old design.
- After configuration assistance and discussions with the team, those customers also chose to remain on the new version.
- Approximately three months after the new frontend was introduced, all schools had transitioned to the new design.

The project allowed Moava to modernize a large legacy SaaS CMS without requiring approximately 1,300 independent school customers to manually rebuild their websites.

---

# Lessons Learned

## Architecture

A large CMS frontend modernization is not primarily a CSS problem.

When thousands of pages are dynamically generated from reusable modules, the migration boundary should be the rendering architecture and the reusable components responsible for producing those pages.

Modernizing the scaffolding and modules made it possible to change the behavior of a very large number of generated pages without rebuilding the pages themselves.

Keeping customer content independent from presentation was also critical.

The same content could be rendered by either frontend mode, which significantly reduced the risk of the migration.

## Legacy Data

Legacy configuration data must be treated as production data, even when the data model is technically poor.

The existing column values were not clean or consistent, but they represented real customer configurations that had worked for years.

Analyzing the actual production data and identifying recurring patterns produced a more reliable migration strategy than assuming that the legacy data followed a mathematically consistent model.

## Responsive Design

Responsive design is not simply a matter of making a desktop layout narrower.

Each reusable module needs to be evaluated independently.

Navigation, tables, images, forms, calendars, slideshows, typography, headers, banners, and content blocks can all require different responsive behavior.

Mobile information hierarchy also matters.

The desktop position of a column does not necessarily indicate its importance on a mobile device, which made configurable column priority useful.

## Configuration Design

Free-form configuration appears flexible but can create significant technical debt.

The old column configuration system allowed arbitrary values, which eventually resulted in inconsistent and technically invalid combinations.

The new slider-based configuration deliberately reduced flexibility in exchange for predictable and valid layouts.

The configuration interface therefore became part of the technical architecture rather than simply being a UI replacement.

## Browser Compatibility

Modern responsive behavior had to be introduced while Internet Explorer and other older browsers were still important.

This required explicit compatibility work and legacy CSS fallbacks.

The project demonstrated that legacy-browser support can materially influence frontend architecture during modernization.

## Progressive Rollout

Progressive rollout was substantially safer than a big-bang migration.

Pilot customers exposed issues that internal testing could not necessarily reveal.

The per-school frontend switch made it possible to isolate problems and revert individual customers without affecting the rest of the platform.

Maintaining both frontend versions temporarily increased the maintenance burden, but the reduction in migration risk justified that cost.

## Product and Customer Impact

The project was not only a technical modernization.

Mobile access had become increasingly important for students, teachers, parents, and other visitors using school websites.

The modernization therefore addressed a real change in user behavior while also supporting broader product goals such as:

- Customer retention.
- Customer satisfaction.
- Product modernization.
- Competitiveness.
- Mobile usability.
- Accessibility.
- Universal design.

Customer support was also part of the migration strategy.

The design consultations gave customers practical assistance in adapting their configurations rather than leaving them to deal with the visual consequences of the migration themselves.

---

# Future Improvements

If continuing the project today, I would preserve the core architectural strategy but introduce stronger automated validation and observability.

Potential improvements include:

- Automated visual regression testing for CMS modules.
- Automated responsive testing.
- Automated accessibility testing.
- Automated browser compatibility testing.
- Component-level integration tests.
- Monitoring of module rendering failures.
- Automated detection of responsive layout regressions.
- Structured migration metrics.
- Better separation between content, application logic, and presentation.
- A formal migration test suite covering the historical column-configuration patterns.
- Automated reporting of schools still using the legacy frontend during a transition period.
- Removal of the legacy rendering path once the migration was complete.

---