---
title: Searchable SMS Joke Archive

organization: Personal Project

role: Fullstack Developer

environment: production

period:
  from: 2001-01
  to: 2003-12

status: archived

technologies:
  - perl
  - cgi
  - javascript
  - html
  - css

concepts:
  - cms
  - searchable-archive
  - content-moderation
  - user-generated-content
  - seo
  - database-design
  - form-handling
  - admin-workflow
  - backups
  - security
  - traffic-growth
  - user-engagement

dependencies:

links:
  portfolio: https://inge1980.github.io/portfolio/projects/need4sms/

---

# Overview

A public CMS and searchable archive of SMS jokes, pickup lines, and dynamic SMS messages, built in Perl while I was studying for my bachelor's degree.

The site stored more than 1,100 categorized items and let visitors search and browse the archive for free. Users could submit their own content through forms. An administrator reviewed submissions and published approved items with a single click.

The website attracted more than 10,000 unique visitors per month and became one of Norway's largest free SMS joke archives at the time. After a security breach, the site was taken offline.

---

# Context

Between 2001 and 2003, SMS jokes, pickup lines, and short messages were widely shared in Norway, but they were hard to keep organized and easy to find in one place.

I received submitted SMS jokes and needed a way to store, categorize, and publish them rather than keeping them as an unstructured collection. The goal was a free public archive that visitors could search and that I could grow through user submissions.

The work was a personal fullstack project during bachelor studies, not a commercial product. Traffic later became large enough that search visibility, navigation of a large text collection, and operational practices such as backups and security mattered in practice.

---

# Task

I designed, built, and operated the complete solution.

My responsibilities included:

- Building a CMS in Perl to organize and store submitted SMS jokes.
- Expanding the CMS into a searchable public archive of categorized jokes, pickup lines, and dynamic SMS messages.
- Implementing user submission forms.
- Implementing an administrator review workflow with one-click publish.
- Making a large amount of text data easy to find through search and categories.
- Optimizing content for search engines to attract visitors.
- Operating the live website until it was taken offline after a security breach.

---

# Challenge

## Challenge: Turning Submitted Jokes Into a Searchable Archive

### Problem

SMS jokes arrived as unstructured text. Without categories and search, a growing collection would be hard to browse, hard to maintain, and of little use to visitors looking for a specific kind of message.

### Solution

I built a Perl CMS that stored submitted content in a searchable database and organized it into categories. The public site exposed search and navigation over the archive instead of presenting a flat list of jokes.

### Result

The archive grew to more than 1,100 categorized jokes, pickup lines, and dynamic SMS messages. Visitors could find content in a large collection instead of scanning everything manually.

---

## Challenge: Publishing User Submissions Without Losing Editorial Control

### Problem

Open submissions were needed to grow the archive, but publishing every submission immediately would have mixed unreviewed content into the public site.

### Solution

Users submitted jokes through forms. Submissions waited for administrator review. Approved items were published with a single click rather than through a separate manual copy step.

### Result

The archive could grow from community submissions while remaining a reviewed, categorized public collection.

---

## Challenge: Making a Large Archive Easy to Find and Use

### Problem

A large joke archive only creates value if people can find it and then find content inside it. Poor search-engine visibility or poor in-site navigation would have limited both traffic and engagement.

### Solution

I treated the archive as searchable, categorized content and worked on search engine optimization so the site could be discovered. On the site, search and categories were the main way to move through more than a thousand items.

### Result

The site reached more than 10,000 unique visitors per month and became one of Norway's largest free SMS joke archives at the time. Easier navigation of the archive helped increase engagement.

---

## Challenge: Operating a Public Site After a Security Breach

### Problem

The live website was later compromised. Without adequate security measures and regular backups, the incident was severe enough that the site had to be taken offline.

### Solution

The immediate operational outcome was to take the website down. The lasting response was to treat security measures and regular backups as required practice on later projects, rather than as optional extras.

### Result

The SMS joke archive did not continue as a public service after the breach. The incident established backup and security habits that I have carried into later work.

---

# Action

## Architecture

### Frontend

The public website was HTML and CSS with JavaScript. Visitors searched and browsed categorized SMS jokes, pickup lines, and dynamic SMS messages. Submission forms collected user-generated content for later review.

### Backend

The CMS and request handling were implemented in Perl using CGI. The backend stored submissions, supported administrator review and one-click publish, and served the searchable archive to the public site.

### Database

Content lived in a searchable database used for categories, stored jokes, and lookup. The archive was designed around making a large amount of text easy to retrieve rather than around a small static page set.

### Infrastructure

The site ran as a public production website. Hosting details from 2001?2003 are not documented here. The service was later taken offline after a security breach.

---

## Technical Decisions

### Decision: Build a Custom Perl CGI CMS

#### Context

Submitted SMS jokes needed a dedicated system for storage, categories, search, and publishing. Off-the-shelf joke platforms were not the basis of this project.

#### Chosen Solution

I built a custom CMS in Perl with CGI, HTML, CSS, and JavaScript so I could own the full path from submission to searchable public archive.

#### Alternatives Considered

Not documented.

#### Trade-offs

A custom CMS matched the content workflow and gave fullstack control of forms, admin publish, search, and SEO. It also meant I owned database design, operations, security, and backups without a maintained platform handling those concerns.

---

### Decision: Searchable Categorized Archive Instead of a Flat List

#### Context

The collection was expected to grow past a size where a simple page of jokes would remain usable.

#### Chosen Solution

Jokes, pickup lines, and dynamic SMS messages were categorized and exposed through a searchable archive.

#### Alternatives Considered

Not documented.

#### Trade-offs

Search and categories made more than 1,100 items navigable and supported traffic growth. They required database design and ongoing content organization rather than static pages alone.

---

### Decision: Moderate Submissions Before Publish

#### Context

User-generated jokes were an important growth path, but the public archive needed to stay a reviewed collection.

#### Chosen Solution

Submissions went through forms into an administrator queue. Publishing an approved item required a single click.

#### Alternatives Considered

Not documented.

#### Trade-offs

Moderation protected the public archive and kept publishing fast for an administrator. Growth depended on someone reviewing the queue, so unpublished submissions did not appear automatically.

---

### Decision: Invest in Search Engine Optimization

#### Context

A free archive only reaches a large audience if people can discover it.

#### Chosen Solution

I optimized the content and site so search engines could drive traffic to the archive.

#### Alternatives Considered

Not documented.

#### Trade-offs

SEO contributed to more than 10,000 unique visitors per month. Higher visibility also increased the operational and security impact of running a popular public site.

---

## Implementation

### Features

- Perl CMS for organizing and storing SMS jokes
- Searchable public archive
- More than 1,100 categorized jokes, pickup lines, and dynamic SMS messages
- User submission forms
- Administrator review and one-click publish
- Category-based navigation
- Search engine optimization for public discovery
- Free public access to the archive

### APIs

The application used CGI request handling and HTML forms rather than a separate documented HTTP API.

### Data and Persistence

Jokes and related SMS content were stored in a searchable database with categories so the archive could be queried and browsed as it grew.

### Testing

Automated tests are not documented for this project.

---

# Result

The project delivered one of Norway's largest free SMS joke archives of its time: a searchable Perl CMS with more than 1,100 categorized items and more than 10,000 unique visitors per month.

It provided practical experience with fullstack web development, database design, user-generated content, search, SEO, and operating a high-traffic public site.

The website was later taken offline after a security breach. That ending is part of the project outcome: the archive was successful as a public service, then discontinued because security and backup practices were not sufficient.

---

# Lessons Learned

## Lesson: Large Text Collections Need Search and Structure

Storing more than a thousand jokes is not enough. Categories and a searchable database were what made the archive usable and helped engagement as the collection grew.

## Lesson: User Growth Needs a Review Path

Open forms can grow a catalog quickly, but publishing should stay an explicit administrator action. One-click publish kept review lightweight without putting unreviewed text on the public site.

## Lesson: Traffic Makes Operations and Security Non-Optional

SEO and a useful archive produced real traffic. A security breach then took the site offline. Popularity without backups and security measures is not a durable result.

## Lesson: Carry Backup and Security Practice Forward

The shutdown taught me to treat security measures and regular backups as standard practice. I have carried that into later projects rather than treating them as work to add after something goes wrong.

---

# Future Improvements

- Treat backups and restore drills as part of launching a public site, not as a reaction after an incident.
- Harden input handling, authentication, and hosting for user-submitted content.
- Keep the archive available as a read-only snapshot if the live write path is compromised.
- Replace ad-hoc CGI operations with clearer separation between public search, submission intake, and administrator publish.
