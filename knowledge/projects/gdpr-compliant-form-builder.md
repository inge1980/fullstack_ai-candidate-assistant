---
title: GDPR-Compliant Dynamic Form Builder

organization: Moava AS

role: Fullstack Developer

environment: production

period:
  from: 2013-01
  to: 2022-01

status: completed

technologies:
  - php
  - mysql
  - javascript
  - jquery
  - backbone.js
  - underscore.js
  - html
  - css
  - bootstrap
  - rest-api
  - json
  - aws-s3
  - aws-sdk
  - phpspreadsheet
  - zipstream
  - csv
  - xls
  - zip
  - subversion

concepts:
  - dynamic-ui
  - cms
  - frontend-architecture
  - backend-development
  - rest-api
  - json
  - data-security
  - gdpr
  - data-retention
  - automated-data-deletion
  - automation
  - user-experience
  - drag-and-drop
  - inline-editing
  - modular-design
  - data-management
  - file-storage
  - data-export
  - data-processing
  - spreadsheet-generation
  - spreadsheet-formatting
  - hyperlink-generation
  - streaming
  - cloud-storage
  - performance-optimization
  - memory-optimization

dependencies:
  - maennchen/zipstream-php
  - aws/aws-sdk-php
  - phpoffice/phpspreadsheet

links:
  github:
  live: Not available

---

# Overview

Developed a GDPR-compliant dynamic form builder for the administration system at Moava AS.

The system allowed administrators to create forms from predefined templates, edit and duplicate fields inline, reorder fields using drag-and-drop, manage submissions, configure data-retention periods, and export collected data.

The solution also handled uploaded files associated with submissions. Files were stored in AWS S3 and could be included in XLS and ZIP exports without first creating unnecessary local copies.

A major part of the project involved combining dynamic form management, privacy-oriented data retention, file lifecycle management, spreadsheet generation, and streaming exports into the existing administration platform.

---

# Context

Moava AS operated an administration system where customers could create and manage dynamic forms and questionnaires.

The form builder needed to be usable by non-technical administrators while supporting reusable templates, flexible field configuration, inline editing, duplication, and drag-and-drop ordering.

The system collected potentially sensitive or personal information. Data therefore needed a defined retention lifecycle, including warnings before deletion and automatic removal of expired submissions and associated uploaded files.

Customers also needed to export collected responses for further processing, analysis, consolidation, and sharing.

The export requirement became more complex when submissions contained uploaded files. Customers needed both the structured submission data and the actual files while maintaining a clear relationship between each submission and its attachments.

The project therefore combined:

- Dynamic form management.
- Submission management.
- GDPR-oriented data retention.
- Uploaded file lifecycle management.
- Spreadsheet generation.
- File-inclusive ZIP exports.
- Cloud-based file storage.
- Streaming and memory optimization.

---

# Task

The main task was to develop and integrate the dynamic form builder into the existing administration system.

My responsibilities included:

- Designing and implementing the dynamic form editing interface.
- Implementing form and field persistence through the REST API.
- Supporting inline editing, duplication, and drag-and-drop ordering.
- Implementing unsaved-change detection.
- Developing submission management functionality.
- Implementing configurable data-retention rules.
- Implementing automated deletion of expired submissions and uploaded files.
- Implementing CSV and XLS exports.
- Supporting multiple uploaded files per submission in XLS exports.
- Integrating AWS S3 file storage.
- Implementing streaming ZIP exports.
- Investigating and reducing memory consumption during spreadsheet generation.

The work covered both frontend and backend development as well as the integration between the administration interface, database, file storage, and export pipeline.

---

# Challenge

## Challenge: Dynamic Form Editing

### Problem

Administrators needed to modify form structures without directly editing database records or relying on developers.

Fields had to be individually editable and reorderable while remaining synchronized with the persisted form configuration.

### Solution

Each form field was stored as a separate database record and associated with its parent form through a form ID.

The frontend communicated with the PHP backend through a self-developed REST API using JSON.

Backbone.js represented the JSON data as client-side models and collections. This provided a structured way to manipulate individual fields and the overall form structure.

Administrators could:

- Select a form template.
- Edit individual fields inline.
- Duplicate fields.
- Reorder fields using drag-and-drop.
- Save field changes through the REST API.

### Result

The form structure could be maintained directly through the administration interface without requiring developers to modify the underlying configuration.

---

## Challenge: Unsaved Change Detection

### Problem

The editor allowed multiple fields to be modified independently. Administrators needed to know which fields had changed but had not yet been saved.

### Solution

The saved JSON representation of each field was retained as a reference state.

When a field was edited, its current JSON representation was compared with the stored reference.

If the current representation differed from the saved representation, the field was considered modified and received a visual indication using a faint red background.

### Result

Unsaved changes were immediately visible without interrupting the editing workflow.

---

## Challenge: GDPR Data Retention

### Problem

Form submissions could contain personal or sensitive information and could not simply be retained indefinitely.

Different forms could also have different legitimate retention requirements.

### Solution

Each form had a configurable retention period expressed as a number of days.

Each submission contained a date used to determine its retention deadline.

Before the deadline, the owner of the form received an email warning that the data was approaching deletion. This allowed the owner to review the information before it expired.

When the configured retention period was reached, the submission was deleted. Uploaded files associated with the submission followed the same retention period and were deleted as part of the same data lifecycle.

### Result

Data retention became part of the normal form and submission lifecycle rather than an unrelated cleanup process.

The system provided customers with a controlled mechanism for reviewing data before deletion while reducing the amount of personal data retained by the platform.

---

## Challenge: Exporting Submissions with Multiple Files

### Problem

Customers needed to export collected form results to CSV and XLS for further processing and analysis.

Some submissions contained multiple uploaded files. The exported spreadsheet needed to preserve the relationship between each file and its originating submission.

The spreadsheet representation also imposed a limitation: one cell could not provide multiple independent file hyperlinks in the required form.

A submission containing three uploaded files therefore needed three physical spreadsheet rows if each file was to receive its own hyperlink.

### Solution

PHPSpreadsheet was used for XLS generation.

For submissions containing multiple files:

1. The first file used the original submission row.
2. Additional files generated additional spreadsheet rows.
3. Each file received its own hyperlink in the file column.
4. The remaining submission fields were visually grouped across the physical rows.
5. The result appeared to the user as one logical submission containing multiple file links.

The XLS file was packaged together with the submitted files in a ZIP archive.

The spreadsheet contained relative links to the exported files so that the links remained usable after the ZIP archive was unpacked.

### Result

Customers could export submissions containing multiple attachments while retaining a clear relationship between each submission and its files.

---

## Challenge: Spreadsheet Export Performance

### Problem

The first implementation used PHPSpreadsheet's mergeCells() to vertically merge the non-file columns when one submission occupied multiple rows.

The approach produced the desired visual result but caused excessive memory consumption when many cells were merged.

This became problematic for larger exports containing many submissions and uploaded files.

### Solution

The implementation was changed so that actual spreadsheet cell merging was no longer required.

Gridlines were disabled and the appearance of merged cells was reproduced using borders and other PHPSpreadsheet styling APIs.

The spreadsheet therefore used independent cells structurally while making the relevant areas appear visually grouped to the user.

### Result

The export retained the intended visual layout without creating large numbers of native merged-cell ranges.

This reduced memory pressure and made the export more suitable for larger datasets.

---

## Challenge: Streaming Export Files from S3

### Problem

Exports could contain many and potentially large uploaded files.

Downloading every file from S3 to the application server before creating a ZIP archive would require temporary disk storage, additional I/O, and cleanup of both the downloaded files and the generated archive.

It would also introduce intermediate copies of data that did not need to exist for the actual export operation.

### Solution

Uploaded files were stored in AWS S3 and accessed through the AWS SDK for PHP.

During an export, the required S3 objects were streamed into maennchen/zipstream-php, which compressed them as part of the ZIP generation process.

The ZIP response was streamed directly to the customer instead of first creating a complete archive on the application server.

The effective data flow was:

AWS S3 -> AWS SDK for PHP -> ZipStream -> HTTP response -> browser

This avoided the unnecessary intermediate flow:

AWS S3 -> local filesystem -> ZIP file -> browser

### Result

The export process avoided unnecessary local copies of uploaded files and eliminated the need for a persistent intermediate ZIP archive.

The trade-off was that the HTTP request remained active while the archive was generated, making request duration and connection reliability relevant for very large exports.

---

# Action

## Architecture

The system followed a client-server architecture connecting a JavaScript administration frontend, PHP backend, MySQL database, AWS S3 file storage, and export services.

### Frontend

The frontend used:

- Backbone.js
- Underscore.js
- jQuery
- Bootstrap
- HTML
- CSS

Backbone.js models and collections represented forms and fields on the client side.

The frontend provided:

- Form template selection.
- Inline field editing.
- Field duplication.
- Drag-and-drop ordering.
- Unsaved-change detection.
- Submission listing and management.

### Backend

The backend was implemented in PHP 8.1.

A self-developed REST API provided JSON-based communication between the frontend and backend.

The backend handled:

- Form and field persistence.
- Submission management.
- Retention processing.
- Export generation.
- File relationships.
- AWS S3 access.
- CSV and XLS generation.

### Database

MySQL stored the structured application data.

Forms, fields, submissions, and file metadata were represented as relational data.

Form fields were stored individually and associated with their parent form, allowing fields to be modified, duplicated, and reordered independently.

Uploaded file contents were stored in AWS S3 rather than in MySQL.

### File Storage

AWS S3 was used for uploaded files.

The database maintained the relationship between uploaded files and their corresponding submissions, while the AWS SDK for PHP provided access to the stored objects.

Uploaded files participated in the same retention lifecycle as their associated submissions.

### Retention Processing

Retention processing evaluated submission age against the retention period configured for the corresponding form.

The lifecycle included:

- Retention-period configuration.
- Warning notifications.
- Submission deletion.
- Associated file deletion.

### Export Pipeline

The export system supported:

- CSV exports.
- XLS exports.
- Individual file hyperlinks.
- Multiple uploaded files per submission.
- ZIP packaging.
- Streaming access to S3 objects.
- Styling-based spreadsheet grouping.

For file-inclusive exports, S3 objects were streamed through the AWS SDK and ZipStream into the HTTP response without requiring all uploaded files to be stored locally first.

---

## Technical Decisions

### Decision: Backbone.js for Dynamic Form Management

#### Context

The form builder needed to manage dynamic collections of fields that could be edited, duplicated, reordered, and persisted.

#### Chosen Solution

Backbone.js models and collections were used to represent the form and its fields on the client.

The JSON returned by the REST API could be mapped into Backbone structures and manipulated before changes were sent back to the backend.

#### Alternatives Considered

A simpler jQuery-based implementation could have manipulated DOM elements and JSON objects directly.

#### Trade-offs

Backbone.js introduced additional client-side structure but provided a consistent model for dynamic form data and UI state.

For this type of editor, the model layer made operations such as reordering, duplication, and change detection easier to manage than relying exclusively on DOM state.

---

### Decision: JSON REST API

#### Context

The frontend needed to exchange dynamic form configuration with the PHP backend.

#### Chosen Solution

A self-developed REST API was used to exchange JSON between Backbone.js and PHP.

#### Alternatives Considered

The editor could have used traditional server-rendered HTML forms and full-page requests.

#### Trade-offs

The REST and JSON approach required more client-side implementation but enabled a more interactive editing experience and established a clear data contract between frontend and backend.

---

### Decision: Per-Form Retention Period

#### Context

Different forms could have different requirements for how long their submissions should be retained.

#### Chosen Solution

Each form defined its own retention period in days.

The submission date was evaluated against the configured period to determine when a submission should be reviewed and deleted.

#### Alternatives Considered

A single global retention period could have been applied to all forms.

#### Trade-offs

Per-form configuration added lifecycle logic but allowed retention to be defined according to the requirements of each individual form.

---

### Decision: AWS S3 for Uploaded Files

#### Context

Submissions could contain uploaded files that should not be stored directly in the relational database.

#### Chosen Solution

Files were stored in AWS S3 while the application maintained their relationship to the corresponding submission.

The AWS SDK for PHP provided access to the stored objects.

#### Alternatives Considered

Files could have been stored on the application server filesystem or as binary data in MySQL.

#### Trade-offs

S3 separated binary storage from relational application data and avoided storing potentially large files in MySQL.

It also introduced an external storage dependency and required the application to manage S3 objects as part of submission creation, export, and deletion.

---

### Decision: PHPSpreadsheet for XLS Export

#### Context

Customers needed spreadsheet exports that could be opened and processed using common spreadsheet applications.

The export also needed to support multiple uploaded files per submission.

#### Chosen Solution

PHPSpreadsheet was used to generate XLS files with formatting and hyperlinks.

Submissions containing multiple files were represented across multiple physical rows, with one hyperlink per file.

#### Trade-offs

PHPSpreadsheet provided the required spreadsheet generation and formatting capabilities.

However, native cell merging created excessive memory usage at scale, which led to the separate styling-based solution described below.

---

### Decision: Visual Grouping Instead of Native Cell Merging

#### Context

Multiple files belonging to one submission required multiple physical spreadsheet rows.

The non-file fields needed to appear visually as one logical result.

#### Chosen Solution

The first implementation used native cell merging but was replaced after memory profiling showed excessive consumption.

Gridlines were disabled and borders and styles were used to create the visual appearance of merged cells without creating native merged ranges.

#### Alternatives Considered

Continue using native cell merging for every additional file row.

#### Trade-offs

The styling-based solution did not create actual merged spreadsheet cells, so the structure differed from a true merged-cell representation.

However, the customer primarily needed the visual presentation. Reproducing that appearance through styling avoided unnecessary memory overhead and provided better scalability.

---

### Decision: Streaming ZIP Generation

#### Context

The export process needed to package files stored in S3 without unnecessarily consuming local disk space or creating an intermediate archive.

Exports could contain many and potentially large uploaded files, making local temporary storage undesirable.

#### Chosen Solution

maennchen/zipstream-php was used together with the AWS SDK for PHP to stream S3 objects directly into the ZIP response.

The spreadsheet and uploaded files could therefore be added to the ZIP as part of a streaming export process without first downloading all uploaded files to local storage or creating a complete intermediate ZIP archive.

#### Alternatives Considered

A conventional temporary-file approach would:

1. Download S3 files to local disk.
2. Create a ZIP archive from the downloaded files.
3. Serve the completed archive.
4. Delete the temporary files and archive.

#### Trade-offs

Streaming reduced local disk usage and avoided persistent intermediate files.

The trade-off was that the HTTP request remained active while the archive was generated, making request duration and connection reliability relevant for very large exports.

For extremely large exports, a background-job architecture would be more appropriate because it would remove the dependency on a single long-running HTTP request.

---

# Result

The resulting system provided a flexible administration interface for creating and managing dynamic forms without requiring developers to modify form definitions manually.

Administrators could create forms from templates, edit and duplicate fields, reorder fields using drag-and-drop, and identify unsaved changes.

The submission workflow supported list-based review, individual and bulk deletion, manual communication with submitters, and export of collected data.

The retention mechanism allowed each form to define its own data-retention period. Form owners received warnings before submissions reached their deletion deadline, and both submission data and associated uploaded files were deleted after the configured retention period.

The export functionality supported CSV and XLS formats and allowed customers to take collected data outside the administration system for further processing, analysis, consolidation, and sharing.

For submissions with uploaded files, the export produced a ZIP package containing the spreadsheet and corresponding files. Each uploaded file received its own hyperlink in the spreadsheet.

Multiple files belonging to one submission were represented using multiple spreadsheet rows while the remaining fields were visually grouped to preserve the appearance of one logical submission.

The initial use of native cell merging caused excessive memory consumption for larger exports. Replacing it with styling-based visual grouping reduced memory pressure while preserving the intended user-facing layout.

Uploaded files were stored in AWS S3 and streamed directly into ZIP generation. This avoided unnecessary local copies of uploaded files and removed the need to create a complete intermediate ZIP archive on the application server.

---

# Lessons Learned

## Dynamic UI State Should Be Explicit

A model-based frontend architecture was useful for an editor where individual fields could be edited, duplicated, reordered, and persisted independently.

Keeping saved and current representations separate also made unsaved changes explicit and easy to communicate visually.

## GDPR Affects the Entire Data Lifecycle

Retention cannot be treated as a single database cleanup operation.

In this system, the retention policy affected:

- Form configuration.
- Submission dates.
- Warning notifications.
- Submission deletion.
- Uploaded file deletion.
- Export workflows.
- Data processing responsibilities.

The submission and its associated files therefore needed to be treated as one logical data lifecycle even though they were stored in different systems.

## Spreadsheet Constraints Can Require a Different Data Representation

The requirement for multiple file hyperlinks exposed a limitation of the target spreadsheet representation.

Instead of trying to force multiple links into one cell, the export represented one logical submission across multiple physical rows.

This preserved the required relationships while remaining compatible with the spreadsheet format.

## Optimize for the Actual Requirement

Native cell merging was technically capable of producing the desired spreadsheet layout, but it introduced unnecessary memory overhead.

The customer did not require the cells to be structurally merged. They required them to appear grouped.

Replacing structural merging with styling was therefore a better solution because it implemented the actual user-facing requirement with lower resource usage.

## Streaming Can Remove Unnecessary Intermediate Storage

The S3-to-ZIP streaming architecture avoided downloading every uploaded file to the application server before compression.

The broader principle is that remote object storage can often be connected directly to a streaming transformation and response when the transformation can be performed incrementally.

## Export Is Part of the Data Lifecycle

The export feature was not only a reporting convenience.

It allowed customers to obtain collected information and continue processing it outside the administration system while the original stored data could be removed according to the configured retention policy.

This made export part of the overall privacy and data-management strategy rather than an isolated feature.

---

# Future Improvements

- Replace the legacy Backbone.js and jQuery frontend with a modern component-based architecture.
- Move very large export generation to background jobs instead of keeping a single HTTP request open for the entire operation.
- Add progress reporting for long-running exports.
- Profile PHPSpreadsheet memory usage across different dataset sizes and optimize generation further.
- Evaluate more modern spreadsheet-generation approaches if future export requirements exceed the capabilities or performance characteristics of PHPSpreadsheet.
- Add structured logging and operational metrics for retention processing and export generation.
- Add automated integration tests around retention, file deletion, and export workflows.
- Add automated performance tests for large spreadsheet and file-inclusive exports.

---