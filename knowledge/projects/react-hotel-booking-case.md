---
title: Hotel Booking Interview Case 2024

organization: 24SevenOffice

role: Frontend Developer

environment: demo

period:
  from: 2024-10
  to: 2024-10

status: completed

technologies:
  - react
  - typescript
  - next.js
  - material-ui
  - emotion
  - dayjs

concepts:
  - form-validation
  - error-handling
  - user-experience
  - date-validation

dependencies:
  - "@emotion/react"
  - "@emotion/styled"
  - "@mui/icons-material"
  - "@mui/material"
  - "@mui/x-date-pickers"
  - dayjs
  - next
  - react
  - react-dom

links:
  github: https://github.com/inge1980/hotel_booking_case_2024_improved
  live: https://hotel-booking-case-2024-improved.vercel.app

---

# Overview

A simulated hotel booking application built with React and Next.js as part of an interview coding case for 24SevenOffice (now Finago).

The case focused on building a functional booking prototype within a limited timeframe, with particular emphasis on the booking UI, room selection, form validation, date validation, error handling, and user experience.

---

# Context

The project was developed as a 3?4 hour interview coding case for 24SevenOffice (now Finago).

The assignment was to implement the essential parts of a hotel booking system as a functional prototype. The proposed implementation included setting up the project structure, building the UI with MUI and Emotion, implementing room selection, form validation, date validation and error handling.

---

# Task

My responsibility was to implement the frontend of the hotel booking system according to the requirements of the coding case.

The main goals were to build the booking UI, implement room selection and reservation input, handle form validation and user feedback, and deliver a functional prototype within the 3?4 hour timeframe.

---

# Challenge

## Challenge: Form Validation and Error Handling

### Problem

A booking flow depends on users providing valid information. When required or invalid input is not handled clearly, users may not understand what needs to be corrected or why they cannot continue.

The challenge was therefore not only to validate the form, but to make validation feedback useful within the actual booking flow.

### Solution

I implemented form validation with a focus on clear and immediate user feedback.

Room selection is required before the form can be submitted. Validation state is maintained for the room type and date fields, and the submit button remains disabled until the required fields are valid.

Invalid or missing values are communicated through the form, with general validation feedback shown when date or room selection rules are not satisfied.

### Result

The booking flow provides clear feedback when required information is missing or invalid and prevents submission until the required booking data is valid.

---

## Challenge: Booking Date Validation

### Problem

The reservation form needs to prevent invalid booking periods. The check-out date cannot be the same as or earlier than the check-in date, and the booking dates need to remain within a defined future range.

### Solution

I implemented date validation using Day.js and MUI X Date Pickers.

The date picker prevents past dates and limits selectable dates to a maximum of 364 days from the current date.

The check-out date is constrained to at least one day after the selected check-in date. If the user changes the check-in date to a date that makes the existing check-out date invalid, the check-out date is reset.

Additional validation checks ensure that the selected dates are not equal and that the check-out date follows the check-in date.

### Result

The booking form prevents invalid date combinations and provides immediate feedback when the selected dates do not satisfy the booking rules.

---

# Action

## Architecture

### Frontend

The application is built with React and Next.js, using TypeScript as the primary development language.

The booking form is implemented as a React component with local state for room selection, dates, validation errors, and form validity.

Material UI provides the main UI component library, with Emotion used for styling. MUI X Date Pickers and Day.js are used for date-related input and validation.

The frontend is responsible for the booking interface, room selection, user input, validation, and feedback throughout the booking flow.

### Infrastructure

The application is built with Next.js and has a deployed version available as a live demo.

---

## Technical Decisions

### Decision: Material UI and Emotion for the User Interface

#### Context

The interview case explicitly required the use of MUI and Emotion to build the booking interface within a limited 3?4 hour timeframe.

#### Chosen Solution

Material UI was used as the main UI component library, with Emotion used for styling. MUI X Date Pickers was used for date selection.

This provided reusable UI components and allowed the booking interface to be implemented within the limited timeframe of the coding case.

#### Trade-offs

Using an established component library reduced the amount of UI code required and provided consistent components, but also introduced dependencies on the library's component APIs and styling approach.

---

## Implementation

### Features

- Hotel booking interface
- Room selection
- Reservation form
- Date selection
- Date validation
- Form validation
- Automatic feedback for validation errors
- Error handling
- Booking date range limited to 364 days ahead
- UX-focused booking flow

### Automation

The project includes development and build scripts through the Next.js setup.

---

# Result

The project resulted in a simulated hotel booking application developed within the context of a 3?4 hour interview coding case.

The implementation focuses on the essential booking flow, including room selection, reservation input, date validation, form validation, error handling, and user feedback.

The application is available as a deployed live demo.

---

# Lessons Learned

## Lesson: Validation Is Part of the User Experience

Form validation is not only about determining whether input is technically valid. How validation errors are communicated is equally important.

This project reinforced the importance of making feedback clear, immediate, and relevant to the user's current interaction with the form, particularly when working within a short implementation timeframe.

---