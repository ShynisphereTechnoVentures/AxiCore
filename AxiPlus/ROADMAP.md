# AxiPlus Academic Operating System Roadmap

This roadmap turns the full AxiPlus engine vision into a practical build order.
The goal is to complete a stable LMS core first, then add academic automation,
career workflows, and certification without rewriting earlier work.

## Current Status

| Engine / Feature | Status | Notes |
| --- | --- | --- |
| Authentication Engine | Mostly done | Login, JWT, role redirect exist. Password reset, email verification, lockout, and session/device management need completion. |
| User & Role Engine | Mostly done | Roles exist. Permission granularity and role hierarchy need expansion. |
| Track Engine | Mostly done | Track CRUD and track-module mapping exist. |
| Batch Allocation Engine | Partial | Batch creation and mentor assignment exist. Auto allocation/balancing needs completion. |
| Student Onboarding Engine | Partial | Registration exists. Document upload, fee verification, agreement acceptance, and onboarding workflow need completion. |
| Module System Engine | In progress | Modules, lessons, resources, live classes, and assignments exist. Needs admin polish and live-only cleanup. |
| Progression Engine | Partial | Student lesson/module progress exists. Eligibility rules and automatic unlocking need completion. |
| Attendance Engine | Mostly done | Student and mentor attendance flows exist. QR/live-class attendance, alerts, and reports need completion. |
| Assignment Engine | Mostly done | Publishing, submission, and mentor review exist. Marks and late tracking need completion. |
| Examination Engine | Not started | MCQ/coding/practical exams, retests, scoring, and pass rules need implementation. |
| Academic State Engine | Partial | Module status exists. Full student lifecycle state and history need implementation. |
| Review Engine | Partial | Assignment review exists. Risk detection, mentor notes, counselling records, and performance review need implementation. |
| Recovery Engine | Not started | Recovery plans, extra classes, recovery assignments, and reassessment need implementation. |
| Batch Reallocation Engine | Not started | Student transfer/repeat/shift workflow needs implementation. |
| Mentor Operations Engine | Partial | Mentor dashboard, attendance, assignments, live classes, and support exist. Needs role split and polish. |
| Internship Engine | Not started | Allocation, projects, timesheets, daily reports, evaluation, and internship certificate flow need implementation. |
| Placement Engine | Not started | Resume builder, mock interviews, AI interview integration, job applications, and placement tracking need implementation. |
| Certification Engine | Not started | Course/internship certificates, QR verification, and PDF download need implementation. |

## Phase 1: Stable LMS Core

Objective: deliver a reliable student, mentor, and admin product before adding
advanced academic automation.

- Complete Authentication Engine basics:
  - Password reset
  - Email verification placeholder or real flow
  - Account active/inactive enforcement
  - Safer frontend API error handling
- Complete User & Role Engine:
  - Confirm final roles: Student, Main Mentor, Assistant Mentor, Placement Officer, Trainer, Counsellor, College Coordinator, Admin, SuperAdmin
  - Add permission rules where role checks are too broad
- Complete Student Onboarding Engine:
  - Registration review
  - Profile completion fields
  - Document upload metadata
  - Fee verification state
  - Batch preference and specialization selection
- Complete Batch Allocation Engine:
  - Capacity check
  - Auto assign student to available batch
  - Mentor/assistant mentor assignment workflow
  - Batch transfer foundation
- Complete Module System Engine:
  - Track/module/lesson management polish
  - PDF notes/resources management
  - Remove or hide recorded-video features if the product is live-class-only
  - Ensure lesson next/previous navigation is stable
- Complete Student Portal:
  - Dashboard
  - My modules
  - Lesson details
  - Live classes
  - PDF notes
  - Assignments
  - Attendance
  - Notifications
  - Support
- Complete Mentor Portal:
  - Main mentor vs assistant mentor permissions
  - Batch view
  - Student progress view
  - Live class scheduling
  - Assignment publishing and review
  - Attendance marking
  - Support responses
  - Announcements
- Complete Admin Portal:
  - Users
  - Roles
  - Tracks
  - Batches
  - Students
  - Modules
  - Mentor assignment
  - Support overview

## Phase 2: Academic Automation

Objective: make AxiPlus operate like an academic workflow engine, not just a
content portal.

- Progression Engine:
  - Completion percentage calculation
  - Assignment completion rule
  - Attendance eligibility rule
  - Project submission rule
  - Assessment pass rule
  - Auto unlock next module
- Examination Engine:
  - Exam entity model
  - Question bank
  - MCQ exams
  - Practical/coding exam placeholder
  - Attempts/retests
  - Score and pass/fail calculation
- Academic State Engine:
  - Student academic states:
    - Applied
    - Enrolled
    - Learning
    - Assessment Pending
    - Internship Eligible
    - Internship Ongoing
    - Placement Ready
    - Placed
    - Completed
    - Dropped
    - Suspended
  - State history
  - Event-driven state transitions
- Review Engine:
  - Risk detection
  - Mentor review notes
  - Counselling records
  - Performance summary
- Recovery Engine:
  - Recovery plans
  - Extra classes
  - Recovery assignments
  - Reassessment
- Batch Reallocation Engine:
  - Transfer student
  - Repeat module
  - Shift batch
  - Reallocation history

## Phase 3: Career Workflow

Objective: move students from academic completion into internship and placement.

- Internship Engine:
  - Internship eligibility
  - Internship allocation
  - Project assignment
  - Timesheets
  - Daily reports
  - Evaluation
- Placement Engine:
  - Resume profile
  - Mock interview records
  - AI interview platform integration
  - Placement readiness score
  - Job applications
  - Placement status tracking

## Phase 4: Certification

Objective: issue verifiable documents at the right academic milestones.

- Certification Engine:
  - Course completion certificate
  - Internship certificate
  - Experience letter
  - Certificate PDF generation
  - QR verification
  - Certificate download

## Immediate Next Sprint

Start with Phase 1 mentor/admin/student stabilization.

1. Harden `MentorPortalApiService` against HTTP and JSON failures.
2. Add missing mentor UI actions for delete/edit where backend already supports it.
3. Define Main Mentor vs Assistant Mentor permission rules.
4. Remove/hide recording UI if AxiPlus is live-class-only.
5. Finish PDF notes/resource workflow.
6. Smoke test student, mentor, assistant mentor, admin, and super admin logins.
7. Rebuild and verify all role dashboards.

## Mentor and Assistant Mentor Operations Sprint

Objective: make Assistant Mentors and Main Mentors operationally complete for
daily batch handling, student follow-up, attendance, assignments, support, and
profile/payroll visibility.

### Existing Base

- AM/MM can view assigned batches.
- AM/MM can view class sessions/schedules.
- AM/MM can create batch live-class sessions.
- AM/MM can mark attendance.
- AM/MM can create assignments with deadlines.
- AM/MM can review assignment submissions.
- AM/MM can view support tickets raised by students.
- Students can raise support tickets.
- Students can receive notifications.
- Student phone number exists for escalation/contact.

### Required Features

- Assignment delegation:
  - Main Mentor can create assignments for each batch.
  - Assistant Mentor can create assignments for allocated batches.
  - Track who created each assignment.
  - Track whether assignment is from MM or AM.
  - Optional: MM assigns work to AM for preparation/review before publishing.
- Batch schedule:
  - AM and MM can see full class schedule per batch.
  - Weekly live class links can be uploaded per batch/day.
  - Schedule should show upcoming, completed, and attendance status.
- Attendance operations:
  - AM marks attendance after checking student activity in the live class.
  - Student can raise attendance discrepancy ticket.
  - AM reviews discrepancy ticket and updates/keeps attendance.
  - MM/Admin can audit attendance changes later.
- Student follow-up:
  - AM can identify students falling behind.
  - AM can send a meeting request to a student.
  - Student sees meeting request as a notification.
  - Student can accept/reject/request another time.
  - If student does not reply, AM can directly call the registered mobile number.
  - Meeting/request status should be tracked.
- Student review:
  - AM and MM can review each student per batch.
  - Attendance report.
  - Assignment report.
  - Project report.
  - Exam report.
  - Progress/risk status.
- Support tickets:
  - Student can raise ticket for app issue, class issue, attendance issue, assignment issue, or other.
  - AM reviews and responds.
  - MM/Admin can monitor unresolved tickets.
- Profile and salary:
  - Mentor/AM profile page with full details.
  - Salary slip list in profile.
  - Salary slip download/link.
  - Admin uploads salary slips for MM/AM.

### New Data Needed

- Mentor profile details:
  - UserId
  - Phone number
  - Address
  - Emergency contact
  - Joining date
  - Designation
  - Bank/payroll metadata if required
- Salary slip:
  - UserId
  - Month
  - Year
  - Amount summary
  - File URL/path
  - Uploaded by admin
  - Uploaded date
- Meeting request:
  - MentorUserId
  - StudentId
  - BatchId
  - RequestedAt
  - ScheduledAt
  - MeetingLink
  - Reason
  - Status: Pending, Accepted, Rejected, Rescheduled, Completed, NoResponse, Called
  - Student response note
  - Mentor follow-up note
- Assignment ownership/delegation:
  - CreatedByUserId
  - AssignedMentorUserId
  - SourceRole
  - Review owner if MM delegates to AM
- Attendance discrepancy:
  - StudentId
  - SessionId
  - ReportedStatus
  - RequestedStatus
  - Reason
  - ReviewedByUserId
  - Resolution note
  - Status

### Build Order

1. Add mentor profile and salary-slip data model/API/UI.
2. Add meeting-request data model/API/UI and student notification integration.
3. Add assignment ownership/delegation fields.
4. Add attendance discrepancy ticket flow.
5. Expand AM/MM dashboard tabs:
   - Schedule
   - Students
   - Attendance
   - Assignments
   - Projects
   - Exams
   - Meeting Requests
   - Tickets
   - Profile/Salary
6. Add role-specific visibility:
   - AM handles assigned batches and student operations.
   - MM can oversee AM work for assigned batches.
   - Admin/SuperAdmin can audit and configure.
