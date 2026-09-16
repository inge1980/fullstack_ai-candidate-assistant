---
title: Osedalen.org Local News Feed

organization: Personal Project

role: Fullstack Developer

environment: production

period:
  from: 2013-01
  to: 2018-12

status: archived

technologies:
  - wordpress
  - rss
  - twitter
  - php
  - javascript

concepts:
  - news-aggregation
  - workflow-automation
  - rss-ingestion
  - social-media-ingestion
  - content-moderation
  - scheduled-publishing
  - local-community
  - search
  - content-curation

dependencies:
  - WordPress

links:
  live: https://osedalen.org

---

# Overview

A WordPress-based local news site for Osedalen in Froland, Norway. Visitors could search news and find useful websites collected in one place, with the goal of making it easier to stay updated on the local community.

The system was an automated news aggregator. It collected and published local news from RSS feeds, weather and traffic alerts, and police updates from Twitter. An administrator could review and publish incoming items with a simple approval workflow rather than rewriting and posting each story by hand.

The site provided continuous local updates until around 2018, when it became less actively maintained.

---

# Context

In 2013, local news for a small place like Osedalen was spread across newspapers, public alerts, weather services, traffic notices, and social media. Residents had no single, searchable place that combined those sources.

The project was a personal production website, not a commercial newsroom product. The editorial need was to keep content fresh without spending the time a traditional local site would require for manual copy, paste, and publish.

WordPress was used as the public CMS and publishing surface. Incoming feeds were treated as candidates for the site, not as unreviewed copy that should always go live immediately.

After 2018 the site received less maintenance. The public service therefore declined even though the original aggregator and publishing workflow had already been built.

---

# Task

I designed, built, and operated Osedalen.org as a fullstack personal project.

My responsibilities included:

- Setting up WordPress as the public news site and CMS.
- Aggregating local news from multiple source types: RSS feeds, weather alerts, traffic alerts, and Twitter updates from the police.
- Automating collection and publishing so the site could stay current without manual rewriting of each item.
- Designing a simple review and publish path so incoming content could be approved rather than posted blindly.
- Providing search over news and a collection of useful local websites.
- Running the live site as a community information source through 2018.

Exact plugin names, hosting provider, and database schema from this period are not documented here.

---

# Challenge

## Challenge: Combining Heterogeneous Local Sources Into One Site

### Problem

Local information did not arrive in one format. RSS items, weather and traffic alerts, and police Twitter messages had different publishers, update rates, and content shapes. A site that only linked to one newspaper feed would have missed alerts that residents actually needed.

If each source had been copied by hand, the site would have been slow to update and easy to abandon.

### Solution

I used WordPress as the common publishing layer and automated ingestion from RSS feeds, weather and traffic alerts, and Twitter. Incoming items were normalized into the site's news workflow so visitors saw one searchable feed instead of several disconnected services.

### Result

Osedalen.org could present local news and alerts in one place. Residents did not have to follow every source separately to get a reasonably current picture of the area.

---

## Challenge: Keeping Automation Under Editorial Control

### Problem

Fully automatic publish would have saved time, but it would also have put unreviewed third-party text, alerts, and social posts on a community homepage. That is a poor fit for a trusted local news page.

Fully manual publish would have defeated the purpose: reducing the time spent on news updates.

### Solution

The aggregator collected candidate items automatically. Publishing was designed around simple review and approval so an administrator could accept relevant items without rewriting them.

### Result

The site could stay fresh with much less manual work while still treating publish as an explicit decision. Automation reduced the time spent on updates; it did not remove a human from the approval path.

---

## Challenge: Making a Small Community Site Discoverable and Useful Over Time

### Problem

A feed that is hard to search, or that only lists today's posts with no supporting local links, is easy to ignore. The site also needed to remain useful as a starting point for Osedalen, not only as a reverse-chronological dump of ingested items.

### Solution

The public site combined aggregated news with search and a collection of useful websites. The product pitch was that Osedalen.org made it easier to stay updated and to find local resources in one place.

### Result

The community had an active, easily accessible local news source with automatically updated news and alerts until maintenance dropped after 2018.

---

# Action

## Architecture

### Frontend

The public website was a WordPress site. Visitors searched news, read aggregated local updates, and used a collection of useful websites gathered in one place. JavaScript ran as part of the WordPress frontend rather than as a separate single-page application.

### Backend

WordPress (PHP) was the CMS, ingestion host, and publishing backend. Automation pulled items from RSS feeds, weather and traffic alerts, and Twitter, then placed them into a workflow where an administrator could approve and publish.

There was no separate custom REST API documented for this project. External sources were consumed as feeds and social updates, then stored and shown through WordPress.

### Database

WordPress persisted posts, search content, and site configuration in its standard database. The important content types were ingested news items, alerts, and curated local website links. A custom schema beyond WordPress is not documented here.

### Infrastructure

The site ran as a public production website at osedalen.org. Hosting and deployment details from 2013?2018 are not documented here. Operational attention declined after 2018.

---

## Technical Decisions

### Decision: Use WordPress as CMS and Publishing Surface

#### Context

The project needed a public site, search, and a simple approval path for ingested news. Building a custom CMS would have delayed a community site whose value was timely aggregation, not a novel content engine.

#### Chosen Solution

WordPress provided the public frontend, content storage, search, and publishing workflow. Aggregation fed WordPress rather than replacing it.

#### Alternatives Considered

A custom Perl or JavaScript CMS was not the implemented path for this site. Earlier personal projects used Perl; this project used WordPress so ingestion, review, and public pages could share one CMS.

#### Trade-offs

WordPress made publishing and search available quickly and matched a simple approval workflow. It also coupled the product to WordPress plugins, PHP hosting, and ongoing CMS maintenance. When that maintenance dropped after 2018, the public value of the aggregator declined with it.

---

### Decision: Aggregate RSS, Alerts, and Twitter Instead of Manual Copy

#### Context

Local news, weather, traffic, and police updates already existed elsewhere. The bottleneck was collecting them often enough for a small community site.

#### Chosen Solution

Automation ingested RSS feeds, weather and traffic alerts, and police Twitter messages into WordPress so the homepage could stay current without rewriting each item.

#### Alternatives Considered

Manual publishing of selected stories was the default for a small local site. That was rejected because it would not scale to continuous updates.

#### Trade-offs

Aggregation increased information flow and reduced administrator time. The site then depended on third-party feeds and Twitter remaining available, correctly formatted, and still relevant. Editorial quality depended on review remaining active.

---

### Decision: Approve Before Publish

#### Context

Ingested RSS, alerts, and social posts are not automatically suitable for a community homepage.

#### Chosen Solution

The system was designed so reviewing and publishing content stayed simple: collect automatically, approve explicitly.

#### Alternatives Considered

Unattended auto-publish would have been even cheaper operationally, but it would have mixed unreviewed third-party content into the public site.

#### Trade-offs

Approval protected the public feed and kept the administrator's job small. Freshness still required someone to review the queue. When that attention later faded, automation alone was not enough to keep the site a maintained local source.

---

### Decision: Combine News Search With Curated Local Links

#### Context

Residents needed more than a firehose of ingested posts. The site was also meant to collect useful websites for Osedalen in one place.

#### Chosen Solution

The public site offered search across news plus a set of useful local links, alongside the automated feed.

#### Alternatives Considered

A feed-only homepage was possible, but it would not have matched the goal of a single starting point for local information.

#### Trade-offs

Search and curated links made the site a small local portal rather than only a syndicator. Those pages also needed occasional curation, which is another form of maintenance.

---

## Implementation

### Features

- Public WordPress site for Osedalen in Froland
- Automated aggregation of local news from RSS feeds
- Ingestion of weather alerts
- Ingestion of traffic alerts
- Ingestion of police updates from Twitter
- Simple administrator review and publish
- Search across news
- Collection of useful local websites
- Continuous public updates without rewriting each story by hand

### APIs

The project consumed external RSS feeds and Twitter updates rather than exposing a documented custom HTTP API. Weather and traffic alerts were additional inbound sources. Provider-specific API contracts from 2013 are not documented here.

### Data and Persistence

Ingested items were stored as WordPress content so they could be searched, reviewed, and published. Curated local website links were part of the same public site. Exact table-level persistence details are not documented.

### Automation

Collection from RSS, weather, traffic, and Twitter ran as an automated pipeline into WordPress. The intended human step was approval and publish, not source-by-source manual posting.

### Testing

Automated tests are not documented for this project. Validation was operational: the live site ingested sources, presented local news, and remained usable for the community until maintenance declined after 2018.

---

# Result

Osedalen.org became an active, easily accessible local news source for residents of Osedalen in Froland. Users received continuous updates from RSS feeds, weather and traffic alerts, and police Twitter messages without requiring an administrator to rewrite each item.

Automation reduced the time spent on news updates and made it easier to keep content fresh and relevant for the local community. Search and a collection of useful websites supported the same goal: one place to stay updated.

The site was less maintained after 2018. The lasting outcome is therefore mixed: the aggregator worked as a community information channel, then faded when ongoing editorial and operational attention dropped.

---

# Lessons Learned

## Lesson: Aggregation Only Helps If Review Stays Cheap

Automatic collection is not the same as a living local site. The useful design was a short approval path, not a fully unattended firehose. When review stopped being cheap in practice, the public feed lost value even though the ingestion idea was sound.

## Lesson: Third-Party Feeds Are an Operational Dependency

RSS, weather, traffic, and Twitter were the product's supply chain. A community aggregator inherits outages, format changes, and platform policy from those sources. Treating feeds as reliable infrastructure without a fallback plan is a product risk.

## Lesson: Community Sites Fail Through Neglect, Not Only Through Bugs

The technical result through 2018 was a working local portal. The later decline came from reduced maintenance. A production personal site needs an honest plan for who will keep approving content and updating WordPress after the interesting build phase is over.

## Lesson: A Portal Is More Than a Reverse-Chronological Feed

Search and curated local links were part of why the site was useful. Syndication alone does not replace a small community starting page.

---

# Future Improvements

- Treat feed health, Twitter/API replacements, and WordPress updates as required operations, not optional follow-up.
- Keep a documented list of ingested sources, licenses, and attribution so third-party content remains defensible.
- Add explicit states for ingested, reviewed, published, and rejected items if the approval queue grows.
- Replace Twitter ingestion with a still-supported public-alert channel if that platform is no longer a reliable police source.
- Snapshot or archive the public site so the community collection remains readable if live ingestion stops.
- Separate curated local links from ingested news so the portal remains useful even when feeds go quiet.
